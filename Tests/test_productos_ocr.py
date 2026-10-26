import sys, unittest
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'servicio_ocr'))
from extractor import ComprobanteExtractor, separar_producto
class ProductosTests(unittest.TestCase):
    def test_mouse_cantidad_antes_de_unidad(self):
        p=ComprobanteExtractor().extract('MOUSE LOGITECH G203 NEGRO 5 UND 65.00')['detalles'][0]
        self.assertEqual((p['nombre'],p['marca'],p['modelo'],p['descripcion_detallada']),('Mouse','Logitech','G203','NEGRO'))
        self.assertEqual((p['cantidad'],p['precio_unitario'],p['importe']),(5,65,325))
        self.assertIn('MOUSE LOGITECH G203 NEGRO',p['texto_original'])
    def test_modelos_discrepantes_quedan_pendientes(self):
        result=ComprobanteExtractor().extract('10 UNIDAD FZ-FVR-902 ESTABILIZADOR FORZA 35.50 355.00\n[LECTURA_ALTERNATIVA]\n10 UNIDAD FZ-FVA-902 ESTABILIZADOR FORZA 35.50 355.00')
        self.assertEqual(result['detalles'][0]['modelo'],'')
        self.assertIn('FZ-FVR-902',result['detalles'][0]['modelo_candidato'])
        self.assertIn('FZ-FVA-902',result['detalles'][0]['modelo_candidato'])
    def test_campos_separados_y_modelo_estructurado(self):
        p=separar_producto('FZ-FPS-001B (FZ-FPS-001B) REGLETA NEGRA FORZA - FORZA')
        self.assertEqual(p['marca'],'Forza')
        self.assertEqual(p['modelo'],'FZ-FPS-001B')
        self.assertEqual(p['nombre'],'Regleta')
        self.assertEqual(p['descripcion_detallada'],'NEGRA')
        self.assertIn('FZ-FPS-001B',p['texto_original'])
        p=separar_producto('TL-WN725N ADAPTADOR TP-LINK 150MBPS')
        self.assertEqual(p['modelo'],'TL-WN725N')
        self.assertNotIn('150MBPS',p['modelo_candidato'])
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
