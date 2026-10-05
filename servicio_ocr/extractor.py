import re
from datetime import datetime

class ComprobanteExtractor:
    def __init__(self):
        pass
    
    def extract(self, text):
        resultado = {
            'numero_comprobante': '',
            'fecha_emision': '',
            'ruc_emisor': '',
            'razon_social': '',
            'igv': 0.0,
            'subtotal': 0.0,
            'total': 0.0,
            'moneda': 'PEN',
            'tipo_comprobante': '',
            'detalles': []
        }
        
        # Extraer número de comprobante (boleta/factura)
        patterns_comprobante = [
            r'(?:N°|Nº|SERIE|FACTURA|BOLETA)\s*[:\s]*([A-Z0-9\-]+)',
            r'([FB][A-Z]?\d{3}-?\d{6,8})',
            r'(B\d{3}-\d{6,8})',
            r'(F\d{3}-\d{6,8})'
        ]
        for pattern in patterns_comprobante:
            match = re.search(pattern, text, re.IGNORECASE)
            if match:
                resultado['numero_comprobante'] = match.group(1).strip()
                if 'F' in resultado['numero_comprobante'].upper():
                    resultado['tipo_comprobante'] = 'FACTURA'
                elif 'B' in resultado['numero_comprobante'].upper():
                    resultado['tipo_comprobante'] = 'BOLETA'
                break
        
        # Extraer fecha
        patterns_fecha = [
            r'(\d{2}/\d{2}/\d{4})',
            r'(\d{2}-\d{2}-\d{4})',
            r'(\d{4}/\d{2}/\d{2})'
        ]
        for pattern in patterns_fecha:
            match = re.search(pattern, text)
            if match:
                fecha_str = match.group(1)
                try:
                    if '/' in fecha_str:
                        parts = fecha_str.split('/')
                        if len(parts[0]) == 4:
                            fecha_obj = datetime.strptime(fecha_str, '%Y/%m/%d')
                        else:
                            fecha_obj = datetime.strptime(fecha_str, '%d/%m/%Y')
                    else:
                        fecha_obj = datetime.strptime(fecha_str, '%d-%m-%Y')
                    resultado['fecha_emision'] = fecha_obj.strftime('%Y-%m-%d')
                except:
                    resultado['fecha_emision'] = fecha_str
                break
        
        # Extraer RUC
        ruc_match = re.search(r'RUC\s*[:\s]*(\d{11})', text, re.IGNORECASE)
        if ruc_match:
            resultado['ruc_emisor'] = ruc_match.group(1)
        
        # Extraer montos
        # IGV
        igv_match = re.search(r'IGV\s*[:\s]*S/?\s*([\d,\.]+)', text, re.IGNORECASE)
        if igv_match:
            resultado['igv'] = float(igv_match.group(1).replace(',', '.'))
        
        # Subtotal
        subtotal_match = re.search(r'SUBTOTAL\s*[:\s]*S/?\s*([\d,\.]+)', text, re.IGNORECASE)
        if subtotal_match:
            resultado['subtotal'] = float(subtotal_match.group(1).replace(',', '.'))
        
        # Total
        total_match = re.search(r'TOTAL\s*[:\s]*S/?\s*([\d,\.]+)', text, re.IGNORECASE)
        if not total_match:
            total_match = re.search(r'IMPORTE\s*TOTAL\s*[:\s]*S/?\s*([\d,\.]+)', text, re.IGNORECASE)
        if total_match:
            resultado['total'] = float(total_match.group(1).replace(',', '.'))
        
        return resultado