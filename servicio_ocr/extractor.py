"""Extracción conservadora: los datos no reconocidos quedan pendientes."""
import re
from datetime import datetime
from decimal import Decimal, InvalidOperation


def importe(texto):
    value = re.sub(r"[^\d,.+-]", "", texto)
    if not value:
        return None
    if ',' in value and '.' in value:
        decimal = ',' if value.rfind(',') > value.rfind('.') else '.'
        value = value.replace('.' if decimal == ',' else ',', '').replace(decimal, '.')
    elif ',' in value or '.' in value:
        sep = ',' if ',' in value else '.'
        parts = value.split(sep)
        value = ''.join(parts) if len(parts[-1]) == 3 else ''.join(parts[:-1]) + '.' + parts[-1]
    try:
        number = Decimal(value)
        return float(number) if number.is_finite() and number >= 0 else None
    except InvalidOperation:
        return None


class ComprobanteExtractor:
    def extract(self, text):
        variants = text.split('\n[LECTURA_ALTERNATIVA]\n')
        if len(variants) == 1:
            return self._extract_single(text)
        results = [self._extract_single(v) for v in variants]
        best = max(results, key=lambda r: (sum(not d['requiere_revision'] for d in r['detalles']),
                                           sum(r[k] is not None for k in ('ruc_emisor','numero_comprobante','total'))))
        for key in ('numero_comprobante','fecha_emision','ruc_emisor','tipo_comprobante','subtotal','igv','total'):
            values = {r[key] for r in results if r[key] is not None}
            if len(values) == 1:
                best[key] = values.pop()
                best['advertencias'] = [a for a in best['advertencias'] if a != 'Completa el dato no reconocido: ' + key.replace('_',' ') + '.']
            elif len(values) > 1:
                best[key] = None
                best['advertencias'].append('Las lecturas difieren: revisa ' + key.replace('_',' ') + '.')
        return best

    def _extract_single(self, text):
        result = dict(numero_comprobante=None, fecha_emision=None, ruc_emisor=None,
                      razon_social=None, igv=None, subtotal=None, total=None,
                      moneda='PEN', tipo_comprobante=None, detalles=[], advertencias=[])
        number = re.search(r'\b([FB][A-Z0-9]{3})\s*[-–]\s*(\d{1,10})\b', text, re.I)
        if not number:
            number = re.search(r'\b(TK\d{2})\s*[-–]\s*(\d{1,10})\b', text, re.I)
        if not number:
            number = re.search(r'(?:N[°º.]|NRO\.?|N[ÚU]MERO|BOLETA(?:\s+DE\s+VENTA)?|FACTURA)\s*:?\s*(\d{3,4})\s*[-–]\s*(\d{1,10})\b', text, re.I)
        if number:
            result['numero_comprobante'] = number[1].upper() + '-' + number[2]
            if re.fullmatch(r'[FB][O0-9]{3}', number[1], re.I):
                result['numero_comprobante'] = number[1].upper().replace('O', '0') + '-' + number[2]
        kind = re.search(r'\b(FACTURA|BOLETA)\b', text, re.I)
        if kind:
            result['tipo_comprobante'] = kind[1].upper()
        elif number and number[1].upper().startswith(('F', 'B')):
            result['tipo_comprobante'] = 'FACTURA' if number[1].upper().startswith('F') else 'BOLETA'
        rucs = list(dict.fromkeys(re.findall(r'\bR\.?U\.?C\.?\s*[:\s]*(\d{11})\b', text, re.I)))
        if len(rucs) == 1:
            result['ruc_emisor'] = rucs[0]
        elif rucs:
            result['advertencias'].append('Hay varios RUC: confirma el proveedor del comprobante.')
        date_lines = re.findall(r'^.*?\bFECHA\s*:\s*(.*)$', text, re.I | re.M)
        dates = re.findall(r'\b(?:\d{4}[-/]\d{2}[-/]\d{2}|\d{2}[-/]\d{2}[-/]\d{4})\b', '\n'.join(date_lines) if date_lines else text)
        for candidate in dates:
            for fmt in ('%d/%m/%Y', '%d-%m-%Y', '%Y/%m/%d', '%Y-%m-%d'):
                try:
                    result['fecha_emision'] = datetime.strptime(candidate, fmt).date().isoformat()
                    break
                except ValueError:
                    pass
            if result['fecha_emision']:
                break
        if re.search(r'\b(?:USD|DOLARES|DÓLARES)\b|US\$', text, re.I):
            result['moneda'] = 'USD'
            result['advertencias'].append('Este formulario registra soles; convierte los importes antes de guardar.')
        labels = {'subtotal': r'(?:SUB\s*TOTAL|TOTAL\s+GRAVADO|OP\.?\s*GRAVADAS|OPERACIONES\s+GRAVADAS|VALOR\s+VENTA)',
                  'igv': r'[I1]\.?G\.?V\.?',
                  'total': r'(?:(?:IMPORTE|MONTO)\s+TOTAL|TOTAL(?:\s+A\s+PAGAR)?)'}
        for key, label in labels.items():
            pattern = r'\b' + label + r'\s*(?:\(?18\s*%\)?)?\s*[:=]?\s*(?:S\s*[/|íi\]]\.?\s*:?|PEN|US\$|\$)?\s*([\d][\d.,]*)\s*[|]?\s*$'
            values = [importe(m) for m in re.findall(pattern, text, re.I | re.M)]
            values = list(dict.fromkeys(v for v in values if v is not None))
            if len(values) == 1:
                result[key] = values[0]
        if all(result[k] is not None for k in ('subtotal', 'igv', 'total')):
            if abs(result['subtotal'] + result['igv'] - result['total']) > .02:
                result['advertencias'].append('Subtotal más IGV no coincide con el total.')
        amount = r'(\d[\d.,]*)'
        reconstructed = re.findall(r'\[DETALLE_RECONSTRUIDO\](.*?)\[/DETALLE_RECONSTRUIDO\]', text, re.S)
        detail_text = '\n'.join(reconstructed) if reconstructed else text
        for line in detail_text.splitlines():
            if result['detalles'] and re.match(r'^\s*(?:UND|UNID|UNIDAD|PCS)\s+\S', line, re.I):
                result['detalles'][-1]['descripcion'] += ' ' + re.sub(r'^\s*(?:UND|UNID|UNIDAD|PCS)\s+', '', line, flags=re.I).strip()
                continue
            if re.search(r'\b(?:TOTAL|SUBTOTAL|IGV|RUC|FECHA|GRAVADAS|DESCUENTO|EXONERAD[AO])\b', line, re.I):
                continue
            # Código de proveedor, cantidad, unidad, descripción, precio, importe.
            line = re.sub(r'^\s*[A-Z0-9-]+\s+(?=\d+(?:[.,]\d+)?\s+(?:UND|UNID|PZA|PCS)\b)', '', line, flags=re.I)
            match = re.fullmatch(r'\s*' + amount + r'\s+(.+?)\s+' + amount + r'\s+' + amount + r'\s*', line)
            if match:
                quantity, description, price, total = match.groups()
            else:
                match = re.fullmatch(r'\s*(.+?)\s+' + amount + r'\s+' + amount + r'\s+' + amount + r'\s*', line)
                if not match:
                    continue
                description, quantity, price, total = match.groups()
            description = re.sub(r'^(?:UND|UNID|UNIDAD|UNIDADES|PZA|PCS)\.?\s+', '', description, flags=re.I)
            if not re.search(r'[A-Za-zÁÉÍÓÚÑáéíóúñ]', description):
                continue
            quantity, price, total = importe(quantity), importe(price), importe(total)
            if None in (quantity, price, total) or quantity <= 0:
                continue
            valid = quantity.is_integer() and abs(quantity * price - total) <= .02
            result['detalles'].append(dict(descripcion=description.strip(), cantidad=quantity,
                                           precio_unitario=price, importe=total, requiere_revision=not valid))
            if not valid:
                result['advertencias'].append('Revisa una línea con cantidad fraccionaria o importes inconsistentes.')
        if not result['detalles']:
            result['advertencias'].append('No se reconocieron productos. Agrégalos o usa una imagen más clara.')
        for key in ('numero_comprobante', 'fecha_emision', 'subtotal', 'igv', 'total'):
            if result[key] is None:
                result['advertencias'].append('Completa el dato no reconocido: ' + key.replace('_', ' ') + '.')
        if number and number[1].upper().startswith('TK'):
            result['advertencias'].append('El documento indica TICKET. Confirma el tipo de comprobante y los impuestos antes de guardar; no se deducen del total.')
        return result
