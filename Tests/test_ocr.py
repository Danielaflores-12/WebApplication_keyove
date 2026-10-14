import sys
import unittest
import tempfile
import base64
from pathlib import Path
from unittest.mock import patch
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'servicio_ocr'))
from extractor import ComprobanteExtractor, importe
from ocr import OCRService
from app import app
from fastapi.testclient import TestClient
from PIL import Image, ImageDraw, ImageFont

class ExtractorTests(unittest.TestCase):
    def test_separadores(self):
        for value in ['1,250.50', '1.250,50', '1250,50', '1250.50']:
            self.assertEqual(importe(value), 1250.5)
        self.assertEqual(importe('1,250'), 1250)

    def test_totales_y_productos(self):
        result = ComprobanteExtractor().extract('FACTURA ELECTRONICA F001-000123\nRUC: 20123456789\n13/10/2026\n2 MARTILLO 10.00 20.00\nSUBTOTAL S/ 20.00\nIGV 18% S/ 3.60\nTOTAL S/ 23.60')
        self.assertEqual(result['total'], 23.6)
        self.assertEqual(result['subtotal'], 20)
        self.assertEqual(result['numero_comprobante'], 'F001-000123')
        self.assertEqual(result['fecha_emision'], '2026-10-13')
        self.assertEqual(result['detalles'][0]['cantidad'], 2)
        self.assertEqual(result['detalles'][0]['descripcion'], 'MARTILLO')

    def test_no_inventar_datos(self):
        result = ComprobanteExtractor().extract('SUBTOTAL S/ 100.00')
        self.assertIsNone(result['total'])
        self.assertIsNone(result['igv'])
        self.assertIsNone(result['numero_comprobante'])

    def test_boleta_fisica_y_producto_con_numero(self):
        result = ComprobanteExtractor().extract('BOLETA DE VENTA\nN° 001-000345\n2 UND CABLE 14 5.00 10.00')
        self.assertEqual(result['numero_comprobante'], '001-000345')
        self.assertEqual(result['tipo_comprobante'], 'BOLETA')
        self.assertEqual(result['detalles'][0]['descripcion'], 'CABLE 14')

    def test_ruc_ambiguo_y_linea_inconsistente(self):
        result = ComprobanteExtractor().extract('RUC 20123456789\nRUC 20987654321\n3 MARTILLO 10.00 25.00')
        self.assertIsNone(result['ruc_emisor'])
        self.assertTrue(result['detalles'][0]['requiere_revision'])

class ServiceTests(unittest.TestCase):
    def setUp(self):
        self.client = TestClient(app)

    def test_rechaza_archivo_invalido_y_origen_web(self):
        self.assertEqual(self.client.post('/ocr/extraer', json={'nombreArchivo': 'x.exe', 'contenidoBase64': 'YQ=='}).status_code, 400)
        self.assertEqual(self.client.post('/ocr/extraer', json={'nombreArchivo': 'x.png', 'contenidoBase64': '??'}).status_code, 400)
        self.assertEqual(self.client.get('/health', headers={'origin': 'https://otro.test'}).status_code, 403)

    def test_no_publicar_success_si_no_hay_texto(self):
        with patch('app.ocr_service.extract_text', return_value=''):
            response = self.client.post('/ocr/extraer', json={'nombreArchivo': 'x.png', 'contenidoBase64': 'YQ=='})
        self.assertEqual(response.status_code, 422)

    def test_foto_pdf_reales(self):
        with tempfile.TemporaryDirectory() as folder:
            img = Image.new('RGB', (1600, 900), 'white')
            draw = ImageDraw.Draw(img)
            font = ImageFont.truetype('C:/Windows/Fonts/arial.ttf', 42)
            lines = ['FACTURA F001-000123', 'RUC: 20123456789', '13/10/2026', '2 MARTILLO 10.00 20.00', 'SUBTOTAL S/ 20.00', 'IGV S/ 3.60', 'TOTAL S/ 23.60']
            for index, line in enumerate(lines):
                draw.text((60, 40 + index * 95), line, fill='black', font=font)
            for extension in ('png', 'pdf'):
                path = Path(folder) / ('comprobante.' + extension)
                img.save(path)
                text = OCRService().extract_text(str(path))
                result = ComprobanteExtractor().extract(text)
                self.assertEqual(result['numero_comprobante'], 'F001-000123', text)
                self.assertEqual(result['total'], 23.6, text)
                self.assertEqual(result['detalles'][0]['cantidad'], 2, text)
                response = self.client.post('/ocr/extraer', json={'nombreArchivo': path.name, 'contenidoBase64': base64.b64encode(path.read_bytes()).decode()})
                self.assertEqual(response.status_code, 200, response.text)
                self.assertEqual(response.json()['datos']['total'], 23.6)

if __name__ == '__main__':
    unittest.main()
