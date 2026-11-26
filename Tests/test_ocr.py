import sys
import unittest
import tempfile
import base64
from pathlib import Path
from io import BytesIO
from contextlib import closing
from unittest.mock import patch
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'servicio_ocr'))
from extractor import ComprobanteExtractor, importe
from ocr import OCRService
from app import app
from fastapi.testclient import TestClient
from PIL import Image, ImageDraw, ImageFont
import pypdfium2

class ExtractorTests(unittest.TestCase):
    def test_pedido_con_descripcion_multilinea_y_total_sin_desglose(self):
        text = (
            'E « V IMPORTADORA\n\nEXPORTADORA E.!l.R.L.\n'
            'Cal. San Juan de Dios nro 639\nCercado\nRUC:20600855604\n'
            'PEDIDO\nPEO2 - 00017875\nCLIENTE: CLIENTE DE PRUEBA\n'
            'D.N.I.: 12345678\nFECHA: 21-05-2026 / 08:37 pm\n'
            'CANT. DESCRIPCION P.U. TOTAL\n10 4K STREAMING BOX 139.00 1,390.00\n'
            'UND WATCH ONN GOOGLE\nAZUL (UND X 1)\nTOTAL S/ 1,390.00\nOPERADOR: CAJA'
        )
        result = ComprobanteExtractor().extract(text)
        self.assertEqual((result['tipo_comprobante'], result['numero_comprobante'], result['fecha_emision']),
                         ('PEDIDO', 'PE02-00017875', '2026-05-21'))
        self.assertEqual((result['ruc_emisor'], result['razon_social']),
                         ('20600855604', 'E & V IMPORTADORA EXPORTADORA E.I.R.L.'))
        self.assertEqual((result['subtotal'], result['igv'], result['total']), (None, None, 1390))
        detalle, = result['detalles']
        self.assertEqual((detalle['cantidad'], detalle['precio_unitario'], detalle['importe']), (10, 139, 1390))
        self.assertEqual((detalle['nombre'], detalle['marca'], detalle['modelo']), ('Streaming box', 'ONN', ''))
        self.assertIn('GOOGLE AZUL', detalle['descripcion_detallada'])

    def test_pedido_sombreado_con_ruc_y_total_contrastados(self):
        text=('PEDIDO PE02-00017675\nRUC:20600895604\nFECHA:21-05-2026\n'
              '10 AKSTREAMING BOX 139.00 1,390.00\nUND WATCH ONN GOOGLE\nAZUL (UND X 1)\n'
              '[LECTURA_ALTERNATIVA]\nE & V IMPORTADORA\nEXPORTADORA E.T.R.L.\n'
              'RUC:20600855604\nPEDIDO PE02-00017875\nTOTAL S/ 4,390.00\n'
              '[LECTURA_ALTERNATIVA]\nRUC:20600855604\nPEDIDO PE02-00017875\nTOTAL S/\n\n1,390.00')
        r=ComprobanteExtractor().extract(text)
        self.assertEqual((r['ruc_emisor'],r['fecha_emision'],r['total']),('20600855604','2026-05-21',1390))
        self.assertEqual(r['razon_social'],'E & V IMPORTADORA EXPORTADORA E.I.R.L.')
        self.assertEqual((r['detalles'][0]['nombre'],r['detalles'][0]['marca']),('Streaming box','ONN'))
        self.assertEqual((r['detalles'][0]['cantidad'],r['detalles'][0]['precio_unitario']),(10,139))
        self.assertEqual(r['detalles'][0]['modelo'],'')
        self.assertEqual(r['numero_comprobante'],'PE02-00017875')
        self.assertIn('numero_comprobante',r['campos_por_confirmar'])
        self.assertIsNone(r['igv'])
        self.assertIsNone(r['subtotal'])

    def test_ticket_con_ruido_y_fecha_legal(self):
        text = ('TICKET TKO2 - 00025290\nTOTAL PEN 99.00.\n'
                'Resolucion 367-2025 de fecha 01/02/2026\n'
                '[LECTURA_ALTERNATIVA]\nTICKET TK02 - 00025290\n'
                'FECHA 04-09-2026 / 08:19 pm\n'
                '18 CARGADOR GAUCHO 5.50 99.00 z\nUND 5V-2A (UNDX1)\n')
        r=ComprobanteExtractor().extract(text)
        self.assertEqual(r['numero_comprobante'],'TK02-00025290')
        self.assertEqual(r['fecha_emision'],'2026-09-04')
        self.assertEqual(r['total'],99)
        self.assertEqual(len(r['detalles']),1)
        self.assertEqual((r['detalles'][0]['cantidad'],r['detalles'][0]['precio_unitario'],r['detalles'][0]['importe']),(18,5.5,99))
        self.assertIsNone(r['subtotal'])
        self.assertIsNone(r['igv'])
        self.assertFalse(any('Las lecturas difieren' in a for a in r['advertencias']))

    def test_sello_anulado_no_se_pierde_entre_lecturas(self):
        r=ComprobanteExtractor().extract('TICKET TK02-1\nANULADO\n[LECTURA_ALTERNATIVA]\n18 CARGADOR 5.50 99.00\nTOTAL 99.00')
        self.assertTrue(r['posible_anulado'])
        self.assertTrue(any('ANULADO' in a for a in r['advertencias']))

    def test_factura_tres_decimales_y_lecturas_alternativas(self):
        header = 'FACTURA F001-00002510\nRUC 20603890214\n13-10-2026\n'
        rows = ('6 UNIDAD FZ-FPS-001B REGLETA FORZA 11.864 14.000 84.00\n'
                '3 UNIDAD TL-WN725N ADAPTADOR TP-LINK 19.492 23,000 69.00\n'
                '10 UNIDAD FZ-FVR902 ESTABILIZADOR FORZA 30.085 35.500 355.00\n')
        totals = '\nGRAVADA S/. 430.51\nI.G.V. 18.00 % S/. {}\nTOTAL S/. 508.00'
        result = ComprobanteExtractor().extract(
            header + rows + totals.format('77.49') + '\n[LECTURA_ALTERNATIVA]\n'
            + header + rows + totals.format('71.43'))
        self.assertEqual([d['cantidad'] for d in result['detalles']], [6, 3, 10])
        self.assertEqual([d['precio_unitario'] for d in result['detalles']], [14, 23, 35.5])
        self.assertEqual((result['subtotal'], result['igv'], result['total']), (430.51, 77.49, 508))
        self.assertFalse(any(d['requiere_revision'] for d in result['detalles']))
        self.assertFalse(any('no reconocido: igv' in a for a in result['advertencias']))

    def test_precio_con_miles_se_conserva_si_cuadra(self):
        result = ComprobanteExtractor().extract('2 UNIDAD LAPTOP 1,250 2500.00')
        self.assertEqual(result['detalles'][0]['precio_unitario'], 1250)

    def test_ticket_sin_inventar_igv_ni_fecha_autorizacion(self):
        text = ('TICKET TK02 - 00025290\nRUC 20600855604\nFECHA: ilegible\n'
                '18 CARGADOR GAUCHO 5.50 99.00\nUND 5V-2A (UNDX1)\n'
                'TOTAL PEN 99.00\nResolucion de fecha 01/02/2026')
        result = ComprobanteExtractor().extract(text)
        self.assertEqual(result['numero_comprobante'],'TK02-00025290')
        self.assertEqual(result['total'],99)
        self.assertEqual(result['detalles'][0]['cantidad'],18)
        self.assertIn('5V-2A',result['detalles'][0]['descripcion'])
        self.assertIsNone(result['igv'])
        self.assertIsNone(result['subtotal'])
        self.assertIsNone(result['fecha_emision'])
        self.assertEqual(result['tipo_comprobante'], 'TICKET')

    def test_lecturas_alternativas_no_duplican_productos(self):
        result = ComprobanteExtractor().extract('18 CARGADOR 5.50 99.00\n'
                   '[LECTURA_ALTERNATIVA]\nTOTAL PEN 99.00\nRUC 20600855604')
        self.assertEqual(len(result['detalles']),1)
        self.assertEqual(result['total'],99)
        self.assertEqual(result['ruc_emisor'],'20600855604')

    def test_factura_columnas_codigo_y_totales(self):
        text = ('FACTURA FO11-0022817\nRUC: 20252011910\n30/09/2026\n'
                '0016269 3.00 UND TECLADO MK120 41.35 124.04\n'
                '0030442 2.00 UND TECLADO MK235 60.20 120.41\n'
                '0030567 1.00 UND TECLADO MK250 78.54 78.54\n'
                '0028993 1.00 UND TECLADO GENIUS 68.85 68.85\n'
                'SON: SOLES Total Gravado S/: 332.07|\nTotal No Gravado S/: 0.00\n'
                'Total Exonerado S/: 0.00\n1.G.V. S/: 59.78\nTOTAL S/: 391.85')
        result = ComprobanteExtractor().extract(text)
        self.assertEqual([d['cantidad'] for d in result['detalles']], [3, 2, 1, 1])
        self.assertEqual(result['numero_comprobante'], 'F011-0022817')
        self.assertEqual((result['subtotal'],result['igv'],result['total']), (332.07,59.78,391.85))

    def test_detalle_reconstruido_no_duplica_lectura_original(self):
        result = ComprobanteExtractor().extract('99999 1.00 UND TECLADO 10.00 10.00\n'
             '[DETALLE_RECONSTRUIDO]\n1.00 TECLADO 10.00 10.00\n[/DETALLE_RECONSTRUIDO]')
        self.assertEqual(len(result['detalles']), 1)
        self.assertEqual(result['detalles'][0]['cantidad'], 1)

    def test_codigo_antes_de_cantidad_no_se_convierte_en_stock(self):
        header = 'BOLETA DE VENTA B001-000123\nCOD [CANT.UM.] DESCRIPCIÓN P/U TOTAL\n'
        product = 'GOOGLE WATCH ONN 130.00 130.00\nSTICK FHD 2446A TV EXTREME DEVICE\nGAR 3 MESES'
        totals = '\nOP. GRAVADAS S/ 110.17\nIGV (18%) S/ 19.83\nTOTAL S/ 130.00'
        for leading in ('20233 [1 UND]', '20233 [1 ]', '20233[1UN]', '20233 1 UND]', '20233 1 ]'):
            with self.subTest(leading=leading):
                result = ComprobanteExtractor().extract(header + leading + ' ' + product + totals)
                line, = result['detalles']
                self.assertEqual((line['cantidad'], line['precio_unitario'], line['importe']), (1, 130, 130))
                self.assertEqual(line['descripcion'], 'GOOGLE WATCH ONN STICK FHD 2446A TV EXTREME DEVICE GAR 3 MESES')
                self.assertFalse(line['requiere_revision'])
                self.assertEqual((result['subtotal'], result['igv'], result['total']), (110.17, 19.83, 130))

        lectura_inconsistente = header + '20233 GOOGLE WATCH ONN 150.00 130.00' + totals
        alternativa = '20233 [1 UN] ' + product
        result = ComprobanteExtractor().extract(lectura_inconsistente + '\n[LECTURA_ALTERNATIVA]\n' + alternativa)
        self.assertEqual(result['detalles'][0]['cantidad'], 1)
        self.assertEqual(result['detalles'][0]['precio_unitario'], 130)
        self.assertEqual(result['total'], 130)

        result = ComprobanteExtractor().extract('20233 UND CABLE 1.00 20,233.00')
        self.assertEqual(result['detalles'][0]['cantidad'], 20233)

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

    def test_total_disperso_o_contrastado_con_igv(self):
        texto = ('BOLETA ELECTRÓNICA B001-154876\n'
                 '20469 [2 UND] MEMORIA MICROSD XC 123.00 246.00\n'
                 'IGV (18%) S/ 37.53')
        extractor = ComprobanteExtractor()
        lectura = extractor.extract(texto + '\nT0TAL\nS/\n246.00')
        self.assertEqual(lectura['total'], 246)
        self.assertFalse(any(a.startswith('Total propuesto') for a in lectura['advertencias']))
        lectura = extractor.extract(texto)
        self.assertEqual(lectura['total'], 246)
        self.assertTrue(any(a.startswith('Total propuesto') for a in lectura['advertencias']))
        self.assertIsNone(extractor.extract(texto.replace('37.53', '37.00'))['total'])

    def test_boleta_fisica_y_producto_con_numero(self):
        result = ComprobanteExtractor().extract('BOLETA DE VENTA\nN° 001-000345\n2 UND CABLE 14 5.00 10.00')
        self.assertEqual(result['numero_comprobante'], '001-000345')
        self.assertEqual(result['tipo_comprobante'], 'BOLETA')
        self.assertEqual(result['detalles'][0]['descripcion'], 'CABLE 14')

    def test_boleta_hdg_con_errores_tipicos_del_ocr(self):
        text = ('HDG IMPORT & TECNOLOGY S.A.C.\nRUC 20612773169\n'
                'Boleta De Venta Electronica\nBBO01-00014394\nF. Emision: 2026-10-17\n'
                'UND UGREEN 55545 Receptor de Audio BT Hi-Res LDAC Jack 62.00 124.00\n'
                'Op. Gravadas: $/105.08 q ES\nIGV: S/ 18.92 19\nTotal a pagar: 5/124.00 $e')
        result = ComprobanteExtractor().extract(text)
        self.assertEqual(result['numero_comprobante'], 'BB01-00014394')
        self.assertEqual(result['ruc_emisor'], '20612773169')
        self.assertEqual(result['razon_social'], 'HDG IMPORT & TECNOLOGY S.A.C')
        self.assertEqual((result['subtotal'], result['igv'], result['total']), (105.08, 18.92, 124))
        self.assertEqual(result['detalles'][0]['cantidad'], 2)
        self.assertEqual(result['detalles'][0]['precio_unitario'], 62)

    def test_tipos_de_documento_y_otro(self):
        extractor = ComprobanteExtractor()
        self.assertEqual(extractor.extract('FACTURA ELECTRONICA F001-0001')['tipo_comprobante'], 'FACTURA')
        self.assertEqual(extractor.extract('BOLETA DE VENTA ELECTRONICA B001-0001')['tipo_comprobante'], 'BOLETA')
        self.assertEqual(extractor.extract('PEDIDO 100')['tipo_comprobante'], 'PEDIDO')
        self.assertEqual(extractor.extract('NOTA DE VENTA 100')['tipo_comprobante'], 'NOTA_VENTA')
        other = extractor.extract('DOCUMENTO INTERNO 100')
        self.assertEqual(other['tipo_comprobante'], 'OTRO')
        self.assertTrue(any('No se reconoció el tipo' in warning for warning in other['advertencias']))

    def test_ruc_ambiguo_y_linea_inconsistente(self):
        result = ComprobanteExtractor().extract('RUC 20123456789\nRUC 20987654321\n3 MARTILLO 10.00 25.00')
        self.assertIsNone(result['ruc_emisor'])
        self.assertTrue(result['detalles'][0]['requiere_revision'])

class ServiceTests(unittest.TestCase):
    def test_resumen_pdf_usa_datos_de_compra_y_repitiendo_cabecera(self):
        producto = {'nombre': 'Streaming Box <4K>', 'referencia': 'ONN & Google',
                    'cantidad': 10, 'precioCompra': 139, 'precioVentaActual': 140,
                    'importe': 1390, 'stockAnterior': 0, 'stockNuevo': 10}
        compra = {'id': 54, 'proveedor': 'E & V IMPORTADORA EXPORTADORA E.I.R.L.',
                  'ruc': '20600855604', 'tipo': 'PEDIDO', 'numero': 'PE02-00017875',
                  'fecha': '2026-05-21', 'estado': 'REGISTRADA', 'subtotal': 1390, 'igv': 0,
                  'total': 1390, 'productos': [producto]}

        def texto_pdf():
            response = self.client.post('/compra/resumen/pdf', json=compra)
            self.assertEqual(response.status_code, 200)
            self.assertEqual(response.headers['content-type'], 'application/pdf')
            with closing(pypdfium2.PdfDocument(response.content)) as pdf:
                paginas = []
                for page_index in range(len(pdf)):
                    with closing(pdf[page_index]) as page, closing(page.get_textpage()) as text:
                        paginas.append(text.get_text_bounded())
                return paginas

        primera, = texto_pdf()
        for texto in ('Resumen de compra', compra['proveedor'], compra['numero'], '21/05/2026',
                      producto['nombre'], producto['referencia'], '0 + 10 = 10', 'S/ 1,390.00'):
            self.assertIn(texto, primera)
        self.assertNotIn('IGV', primera)
        compra.update(tipo='FACTURA', estado='ANULADA', igv='250.20', total='1640.20',
                      productos=[{**producto, 'nombre': f'Producto {i}'} for i in range(1, 51)])
        paginas = texto_pdf()
        self.assertGreater(len(paginas), 1)
        self.assertIn('Anulada', paginas[0])
        self.assertIn('Precio venta actual', paginas[1])
        self.assertIn('Producto 50', paginas[-1])
        self.assertIn('IGV', paginas[-1])
        self.assertIn('S/ 1,640.20', paginas[-1])

    def test_reconstruye_tabla_de_pedido_con_descripcion_multilinea(self):
        import numpy as np
        words = [('CANT.', 70, 100, 90, 25), ('DESCRIPCION', 183, 100, 190, 25),
                 ('P.U.', 615, 100, 55, 25), ('TOTAL', 730, 101, 95, 25),
                 ('10', 104, 157, 34, 23), ('4K', 183, 153, 34, 23),
                 ('STREAMING', 228, 154, 130, 23), ('BOX', 370, 153, 45, 23),
                 ('139.00', 580, 158, 95, 23), ('1,390.00', 704, 160, 124, 23),
                 ('UND', 90, 191, 65, 23), ('WATCH', 183, 190, 75, 23), ('ONN', 268, 191, 54, 23),
                 ('AZUL', 183, 225, 65, 23), ('TOTAL', 495, 283, 85, 23),
                 ('1,390.00', 704, 283, 124, 23)]
        data = {key: [word[i] for word in words] for i, key in enumerate(['text', 'left', 'top', 'width', 'height'])}
        with patch('ocr.pytesseract.image_to_data', return_value=data):
            self.assertEqual(OCRService().table_rows(np.zeros((400, 900), dtype=np.uint8)),
                             ['10 4K STREAMING BOX WATCH ONN AZUL 139.00 1,390.00'])

    def test_convierte_comprobante_a_pdf_con_todas_sus_paginas(self):
        with BytesIO() as original, Image.new('RGB', (100, 200), 'white') as first, Image.new('RGB', (200, 100), 'blue') as second:
            first.save(original, 'TIFF', save_all=True, append_images=[second])
            payload = {'nombreArchivo': 'pedido.tiff', 'contenidoBase64': base64.b64encode(original.getvalue()).decode()}
        response = self.client.post('/documento/pdf', json=payload)
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.headers['content-type'], 'application/pdf')
        with closing(pypdfium2.PdfDocument(response.content)) as pdf:
            self.assertEqual(len(pdf), 2)
            with closing(pdf[0]) as page:
                self.assertGreater(page.get_height(), page.get_width())
            with closing(pdf[1]) as page:
                self.assertGreater(page.get_width(), page.get_height())
        payload['contenidoBase64'] = 'YQ=='
        self.assertEqual(self.client.post('/documento/pdf', json=payload).status_code, 422)

    def test_columnas_curvadas_y_columna_incompleta(self):
        import numpy as np
        words = [('CANT.',100,50,40,20), ('U.M.',160,50,40,20),
                 ('P.UNIT.',600,45,70,20), ('IMPORTE',750,45,80,20),
                 ('3.00',110,100,40,20), ('2.00',110,140,40,20),
                 ('TECLADO',210,105,100,20), ('MK120',320,95,80,20),
                 ('TECLADO',210,145,100,20), ('MK235',320,135,80,20),
                 ('41.35',620,90,60,20), ('60.20',620,130,60,20),
                 ('124.04',770,85,70,20), ('120.41',770,125,70,20)]
        def data(rows):
            return {k:[w[i] for w in rows] for i,k in enumerate(['text','left','top','width','height'])}
        with patch('ocr.pytesseract.image_to_data', return_value=data(words)):
            result = OCRService().table_rows(np.zeros((1000,1000),dtype=np.uint8))
            self.assertEqual(result, ['3.00 TECLADO MK120 41.35 124.04', '2.00 TECLADO MK235 60.20 120.41'])
        with patch('ocr.pytesseract.image_to_data', return_value=data(words[:-1])):
            self.assertEqual(OCRService().table_rows(np.zeros((1000,1000),dtype=np.uint8)), [])

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
            lines = ['FACTURA F001-000123', 'RUC: 20123456789', '13/10/2026', '2 MARTILLO 10.00 20.00', '4 ALICATE 5.00 20.00', 'SUBTOTAL S/ 40.00', 'IGV S/ 7.20', 'TOTAL S/ 47.20']
            for index, line in enumerate(lines):
                draw.text((60, 40 + index * 95), line, fill='black', font=font)
            for extension in ('png', 'pdf'):
                path = Path(folder) / ('comprobante.' + extension)
                img.save(path)
                text = OCRService().extract_text(str(path))
                result = ComprobanteExtractor().extract(text)
                self.assertEqual(result['numero_comprobante'], 'F001-000123', text)
                self.assertEqual(result['total'], 47.2, text)
                self.assertEqual(result['detalles'][0]['cantidad'], 2, text)
                response = self.client.post('/ocr/extraer', json={'nombreArchivo': path.name, 'contenidoBase64': base64.b64encode(path.read_bytes()).decode()})
                self.assertEqual(response.status_code, 200, response.text)
                datos = response.json()['datos']
                self.assertEqual([datos[k] for k in ('tipo_comprobante', 'numero_comprobante', 'ruc_emisor', 'fecha_emision', 'subtotal', 'igv', 'total')],
                                 ['FACTURA', 'F001-000123', '20123456789', '2026-10-13', 40, 7.2, 47.2])
                self.assertEqual([(d['descripcion'].upper(), d['cantidad'], d['precio_unitario'], d['importe']) for d in datos['detalles']],
                                 [('MARTILLO', 2, 10, 20), ('ALICATE', 4, 5, 20)])

if __name__ == '__main__':
    unittest.main()
