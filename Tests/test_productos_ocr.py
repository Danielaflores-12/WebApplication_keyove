import sys, unittest
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'servicio_ocr'))
from extractor import ComprobanteExtractor, separar_producto
class ProductosTests(unittest.TestCase):
    def test_modelo_dudoso(self):
        p=separar_producto('1S 1008G SWITCH TP- LINK LITE WAVE 8 PUERTOS UNI X 1')
        self.assertEqual(p['marca'], 'TP-Link')
        self.assertEqual(p['modelo'], '')
        self.assertIn('1S 1008G',p['modelo_candidato'])
        self.assertNotIn('1008G',p['nombre'])
    def test_multilinea_y_alternativa(self):
        p=ComprobanteExtractor().extract('1 UND SWITCH TP-LINK 58.00 58.00\nLITE WAVE 8 PUERTOS\n[LECTURA_ALTERNATIVA]\nTOTAL 58.00')
        self.assertEqual(len(p['detalles']),1)
        self.assertIn('LITE WAVE',p['detalles'][0]['descripcion_producto'])
        self.assertEqual(p['detalles'][0]['importe'],58)
    def test_sin_marca_serie(self):
        p=separar_producto('CABLE USB S/N: ABC123')
        self.assertEqual(p['marca'],'')
        self.assertEqual(p['modelo'],'')
        self.assertNotIn('ABC123',p['nombre'])
