"""Extracción conservadora: los datos no reconocidos quedan pendientes."""
import re
from collections import Counter

def separar_producto(texto):
    """Separa campos y propone códigos claros; deja candidatos ambiguos pendientes."""
    limpio = re.sub(r"\b(?:S\s*/\s*N|SERIE|SERIAL)\s*[:#].*$", "", texto, flags=re.I).strip()
    limpio = re.sub(r"\b(?:UND|UNID|UNI|UNIDAD|PZA|PCS)\b(?:\s*X\s*\d+)?", "", limpio, flags=re.I)
    limpio = re.sub(r'\(?\b(?:UND|UNID)X\d+\b\)?', '', limpio, flags=re.I)
    limpio = re.sub(r'(\d)>V(?=-\d)', r'\1V', limpio, flags=re.I)
    limpio = re.sub(r'\b([4A]K)(?=STREAMING\b)', r'\1 ', limpio, flags=re.I)
    limpio = re.sub(r'\(\s*\)', '', limpio)
    limpio = re.sub(r"\s+", " ", limpio).strip()
    marca = ""
    for patron, valor in ((r"\bTP\s*-?\s*LINK\b", "TP-Link"), (r"\bLOGITECH\b", "Logitech"), (r"\bLENOVO\b", "Lenovo"), (r"\bEPSON\b", "Epson"), (r"\bSAMSUNG\b", "Samsung")):
        if re.search(patron, limpio, re.I):
            marca = valor
            limpio = re.sub(patron, valor, limpio, flags=re.I)
            break
    if not marca and re.search(r'\bFORZA\b', limpio, re.I):
        marca = 'Forza'
    if not marca and re.search(r'\bONN\b', limpio, re.I):
        marca = 'ONN'
    if not marca and re.search(r'\bUGREEN\b', limpio, re.I):
        marca = 'UGREEN'
    candidatos = re.findall(r"\b(?=[A-Z0-9-]{3,}\b)(?=[A-Z0-9-]*[A-Z])(?=[A-Z0-9-]*\d)[A-Z0-9-]+\b", limpio, re.I)
    candidatos = list(dict.fromkeys(c.upper() for c in candidatos if not re.fullmatch(r"(?:\d+(?:GB|TB|MB|MBPS|MHZ|GHZ|VA|V|W|MM|CM)|\d+V-\d+A)", c, re.I)))
    fragmento = re.match(r"^([A-Z0-9]{1,3}\s+\d+[A-Z][A-Z0-9-]*)\s+", limpio, re.I)
    if fragmento:
        candidatos = [fragmento[1]] + [c for c in candidatos if c not in fragmento[1].split()]
    # Un código estructurado único es una propuesta editable, nunca un ID de catálogo.
    estructurados = [c for c in candidatos if (re.fullmatch(r'[A-Z]{2,}(?:-[A-Z0-9]+)+', c) or re.fullmatch(r'[A-Z]{1,4}\d{2,}[A-Z0-9]*', c)) and re.search(r'\d', c)]
    modelo = estructurados[0] if len(estructurados) == 1 else ''
    nombre = re.split(r'\b(?:ESPECIFICACIONES|CARACTER[ÍI]STICAS)\b', limpio, maxsplit=1, flags=re.I)[0]
    if marca:
        nombre = re.sub(re.escape(marca), '', nombre, flags=re.I)
    for c in candidatos:
        nombre = re.sub(r"\b" + re.escape(c) + r"\b", "", nombre)
    nombre = re.sub(r'\(\s*\)', '', nombre)
    nombre = re.sub(r"\s+", " ", nombre).strip(" -|:;()")
    avisos = (["Modelo pendiente; confirma el candidato en el comprobante: " + ", ".join(candidatos)] if candidatos else ["Modelo no reconocido; completar si corresponde."]) if not modelo else ['Modelo propuesto por OCR; confirma sus letras y números en el comprobante.']
    if not marca:
        avisos.append("Marca no reconocida; completar si corresponde.")
    descripcion_detallada = limpio
    tipo = re.match(r'^(MOUSE|TECLADO|REGLETA|ESTABILIZADOR|CARGADOR|AUD[IÍ]FONOS?|ADAPTADOR|RECEPTOR|SWITCH|ROUTER|CABLE|MONITOR|IMPRESORA|LAPTOP)\b\s*(.*)', nombre, re.I)
    if tipo:
        nombre = tipo[1].capitalize()
        descripcion_detallada = tipo[2].strip(' -|:;()')
    streaming = re.search(r'\bSTREAMING\s+BOX\b', limpio, re.I)
    if streaming:
        nombre = 'Streaming box'
        descripcion_detallada = re.sub(r'\bSTREAMING\s+BOX\b', '', limpio, flags=re.I)
        if marca:
            descripcion_detallada = re.sub(r'\b' + re.escape(marca) + r'\b', '', descripcion_detallada, flags=re.I)
        descripcion_detallada = re.sub(r'\s+', ' ', descripcion_detallada).strip(' .-')
    codigo = re.search(r'\bC[OÓ]D(?:IGO)?\.?\s*[:#]\s*([A-Z0-9-]+)', texto, re.I)
    return dict(codigo=codigo[1] if codigo else '', nombre=nombre, marca=marca, modelo=modelo, descripcion_detallada=descripcion_detallada, descripcion_producto=limpio, texto_original=texto, modelo_candidato=" / ".join(candidatos), advertencias_producto=avisos)

from datetime import datetime
from decimal import Decimal, InvalidOperation


def importe(texto, cantidad=False):
    value = re.sub(r"[^\d,.+-]", "", texto).strip('.,')
    if not value:
        return None
    if ',' in value and '.' in value:
        decimal = ',' if value.rfind(',') > value.rfind('.') else '.'
        value = value.replace('.' if decimal == ',' else ',', '').replace(decimal, '.')
    elif ',' in value or '.' in value:
        sep = ',' if ',' in value else '.'
        parts = value.split(sep)
        # En cantidades, 1.000 suele representar una unidad.
        if cantidad and len(parts) == 2:
            value = parts[0] + '.' + parts[1]
        else:
            value = ''.join(parts) if len(parts[-1]) == 3 else ''.join(parts[:-1]) + '.' + parts[-1]
    try:
        number = Decimal(value)
        return float(number) if number.is_finite() and number >= 0 else None
    except InvalidOperation:
        return None


class ComprobanteExtractor:
    TIPOS = (
        ('BOLETA', r'\bBOLETA(?:\s+DE\s+VENTA)?(?:\s+ELECTR[OÓ]NICA)?\b'),
        ('FACTURA', r'\bFACTURA(?:\s+ELECTR[OÓ]NICA)?\b'),
        ('PEDIDO', r'\b(?:PEDIDO|ORDEN\s+DE\s+COMPRA)\b'),
        ('TICKET', r'\b(?:TICKET|TIKET)\b'),
        ('NOTA_VENTA', r'\bNOTA\s+DE\s+VENTA\b'),
    )

    def tipo_documento(self, text):
        """Clasifica antes de extraer: cada documento tiene su propio formato."""
        for code, pattern in self.TIPOS:
            if re.search(pattern, text, re.I):
                return code
        # Una serie electrónica legible también identifica el documento.
        series = re.search(r'\b([FB])[A-Z0-9]{3,4}\s*[-–]', text, re.I)
        if series:
            return 'FACTURA' if series[1].upper() == 'F' else 'BOLETA'
        return 'OTRO'

    def numero_documento(self, text, tipo):
        """Busca el correlativo con reglas específicas del tipo clasificado."""
        patterns = {
            'FACTURA': (r'\b(F[A-Z0-9]{3,4})\s*[-–]\s*(\d{1,10})\b',
                         r'(?:FACTURA(?:\s+ELECTR[OÓ]NICA)?\s*)?(?:N(?:[°º.]|RO\.?)?|N[ÚU]MERO)\s*:?\s*(\d{3,4})\s*[-–]\s*(\d{1,10})\b'),
            'BOLETA': (r'\b(B[A-Z0-9]{3,5})\s*[-–]\s*(\d{1,10})\b',
                        r'(?:BOLETA(?:\s+DE\s+VENTA)?(?:\s+ELECTR[OÓ]NICA)?\s*)?(?:N(?:[°º.]|RO\.?)?|N[ÚU]MERO)\s*:?\s*(\d{3,4})\s*[-–]\s*(\d{1,10})\b'),
            'TICKET': (r'\b(TK\d{2})\s*[-–]\s*(\d{1,10})\b',
                       r'(?:TICKET|TIKET|N(?:[°º.]|RO\.?)?|N[ÚU]MERO)\s*:?\s*([A-Z]{0,5}\d{0,4})\s*[-–]\s*(\d{1,10})\b'),
            'PEDIDO': (r'(?:PEDIDO|ORDEN\s+DE\s+COMPRA)\s*(?:N(?:[°º.]|RO\.?)?|N[ÚU]MERO)?\s*:?\s*([A-Z]{0,6}\d{0,4})\s*[-–]\s*(\d{1,10})\b',
                        r'(?:PEDIDO|ORDEN\s+DE\s+COMPRA)\s*(?:N(?:[°º.]|RO\.?)?|N[ÚU]MERO)?\s*:?\s*(\d{3,10})\b'),
            'NOTA_VENTA': (r'(?:NOTA\s+DE\s+VENTA|N(?:[°º.]|RO\.?)?|N[ÚU]MERO)\s*:?\s*([A-Z]{0,6}\d{0,4})\s*[-–]\s*(\d{1,10})\b',
                            r'(?:NOTA\s+DE\s+VENTA)\s*(?:N(?:[°º.]|RO\.?)?|N[ÚU]MERO)?\s*:?\s*(\d{3,10})\b'),
            'OTRO': (r'(?:N(?:[°º.]|RO\.?)?|N[ÚU]MERO)\s*:?\s*(\d{3,4})\s*[-–]\s*(\d{1,10})\b',),
        }
        for pattern in patterns.get(tipo, ()):
            match = re.search(pattern, text, re.I)
            if not match:
                continue
            groups = match.groups()
            if len(groups) == 1:
                return groups[0]
            serie, correlativo = groups[-2], groups[-1]
            serie = serie.upper().replace(' ', '')
            if re.fullmatch(r'TK[O0-9]{2}', serie):
                serie = serie[:2] + serie[2:].replace('O', '0')
            if serie:
                if re.fullmatch(r'BB[O0]\d{2}', serie):
                    serie = 'BB' + serie[-2:]
                elif re.fullmatch(r'[FB][O0-9]{3}', serie):
                    serie = serie.replace('O', '0')
                return serie + '-' + correlativo
        return None

    def extract(self, text):
        variants = text.split('\n[LECTURA_ALTERNATIVA]\n')
        if len(variants) == 1:
            return self._extract_single(text)
        results = [self._extract_single(v) for v in variants]
        best = max(results, key=self._score)
        best['campos_por_confirmar'] = {}
        for key in ('numero_comprobante','fecha_emision','ruc_emisor','tipo_comprobante','subtotal','igv','total'):
            observaciones = [r[key] for r in results if r[key] is not None]
            values = {r[key] for r in results if r[key] is not None
                      and not (key == 'tipo_comprobante' and r[key] == 'OTRO')}
            if len(values) == 1:
                best[key] = values.pop()
                best['advertencias'] = [a for a in best['advertencias'] if a != 'Completa el dato no reconocido: ' + key.replace('_',' ') + '.']
            elif len(values) > 1:
                if key == 'total' and best['detalles'] and all(not d['requiere_revision'] for d in best['detalles']):
                    balanced = [v for v in values if abs(sum(d['importe'] for d in best['detalles']) - v) <= .02]
                    if len(balanced) == 1:
                        best[key] = balanced[0]
                        best['advertencias'] = [a for a in best['advertencias'] if a != 'Completa el dato no reconocido: total.']
                        best['advertencias'].append('Total contrastado con los importes de productos; confírmalo en el comprobante.')
                        continue
                if key == 'ruc_emisor':
                    counts = Counter(r[key] for r in results if r[key] is not None)
                    ranking = counts.most_common()
                    if ranking[0][1] >= 2 and ranking[0][1] > ranking[1][1]:
                        best[key] = ranking[0][0]
                        best['advertencias'].append('RUC propuesto por coincidencia entre lecturas; confírmalo en el comprobante.')
                        continue
                if key == 'igv' and best['subtotal'] is not None and best['total'] is not None:
                    balanced = [v for v in values if abs(best['subtotal'] + v - best['total']) <= .02]
                    if len(balanced) == 1:
                        best[key] = balanced[0]
                        best['advertencias'] = [a for a in best['advertencias'] if a != 'Completa el dato no reconocido: igv.']
                        continue
                best[key] = None
                best['advertencias'].append('Las lecturas difieren: revisa ' + key.replace('_',' ') + '.')
                if key == 'numero_comprobante':
                    best['advertencias'].append('Números leídos para comparar con el original: ' + ', '.join(sorted(values)) + '.')
                    propuesta = self._proponer_numero(observaciones)
                    if propuesta:
                        best[key] = propuesta
                        best['campos_por_confirmar'][key] = dict(valor=propuesta, alternativas=sorted(values))
                        best['advertencias'].append('Número propuesto por comparación de caracteres; debes confirmarlo con el original.')
        for detalle in best['detalles']:
            modelos = set()
            for result in results:
                filas = [d for d in result['detalles'] if d['importe'] == detalle['importe']]
                if len(filas) == 1 and filas[0]['modelo']:
                    modelos.add(filas[0]['modelo'])
            if len(modelos) > 1:
                detalle['modelo'] = ''
                detalle['modelo_candidato'] = ' / '.join(sorted(modelos))
                detalle['advertencias_producto'].append('Las lecturas difieren en el modelo; confirma cuál corresponde: ' + detalle['modelo_candidato'])
        best['advertencias'] = list(dict.fromkeys(best['advertencias']))
        if best['razon_social'] is None:
            candidates = [r['razon_social'] for r in results if r['razon_social']]
            if candidates:
                keys = {re.sub(r'[^A-Z0-9]', '', c.upper()) for c in candidates}
                if len(keys) == 1:
                    best['razon_social'] = max(candidates, key=len)
        if any(r.get('posible_anulado') for r in results):
            best['posible_anulado'] = True
            aviso = 'El comprobante contiene la palabra ANULADO. Verifica su validez antes de registrar la compra.'
            if aviso not in best['advertencias']:
                best['advertencias'].append(aviso)
        return best

    @staticmethod
    def _proponer_numero(numeros):
        """Propone únicamente un número observado; nunca fabrica un correlativo."""
        partes = [re.fullmatch(r'([A-Z0-9]+)-(\d+)', n) for n in numeros]
        if not partes or any(p is None for p in partes):
            return None
        if len({(p[1], len(p[2])) for p in partes}) != 1:
            return None
        valores = set(numeros)
        distancia = lambda a,b: sum(x != y for x,y in zip(a,b))
        if any(distancia(a,b) > 2 for a in valores for b in valores):
            return None
        puntajes = {n: sum(distancia(n,otro) for otro in numeros) for n in valores}
        minimo = min(puntajes.values())
        candidatos = [n for n,p in puntajes.items() if p == minimo]
        return candidatos[0] if len(candidatos) == 1 else None

    @staticmethod
    def _score(result):
        details = result['detalles']
        total = result['total']
        matches = bool(details and total is not None and
                       abs(sum(d['importe'] for d in details) - total) <= .02)
        # Una lectura con filas pendientes de revisión sigue siendo más útil
        # que un encabezado sin productos. No elimina sus marcas de revisión.
        return (matches, bool(details), sum(not d['requiere_revision'] for d in details),
                len(details),
                sum(result[k] is not None for k in ('ruc_emisor', 'numero_comprobante', 'total')))

    def _extract_single(self, text):
        # Lectura dispersa: el total monetario puede ocupar la línea siguiente.
        text = re.sub(r'(?im)^[ \t]*(TOTAL[ \t]+(?:S[ \t]*/\.?|PEN|US\$))[ \t]*\n(?:[ \t]*\n)*[ \t]*(\d[\d.,]*)[ \t]*$', r'\1 \2', text)
        result = dict(numero_comprobante=None, fecha_emision=None, ruc_emisor=None,
                      razon_social=None, igv=None, subtotal=None, total=None,
                      moneda='PEN', tipo_comprobante=None, detalles=[], advertencias=[])
        result['posible_anulado'] = bool(re.search(r'\bANULAD[OA]\b', text, re.I))
        if result['posible_anulado']:
            result['advertencias'].append('El comprobante contiene la palabra ANULADO. Verifica su validez antes de registrar la compra.')
        result['tipo_comprobante'] = self.tipo_documento(text)
        result['numero_comprobante'] = self.numero_documento(text, result['tipo_comprobante'])
        if result['tipo_comprobante'] == 'OTRO':
            result['tipo_comprobante'] = 'OTRO'
            result['advertencias'].append('No se reconoció el tipo del documento. Se marcó como OTRO; revísalo antes de guardar.')
        rucs = list(dict.fromkeys(re.findall(r'\bR\.?U\.?C\.?\s*[:\s]*(\d{11})\b', text, re.I)))
        if len(rucs) == 1:
            result['ruc_emisor'] = rucs[0]
            # Normalmente la razón social está en la línea previa al RUC.
            lines = [line.strip(' .:|-') for line in text.splitlines()]
            lines = [re.sub(r'\bE\.[T1]\.R\.L\.?', 'E.I.R.L.', line, flags=re.I) for line in lines]
            for index, line in enumerate(lines):
                if rucs[0] not in line:
                    continue
                candidates = reversed(lines[max(0, index - 6):index])
                result['razon_social'] = next((candidate for candidate in candidates
                                                if len(candidate) >= 4 and
                                                re.search(r'[A-Za-zÁÉÍÓÚÑáéíóúñ]', candidate) and
                                                re.search(r'\b(?:S\.?A\.?C\.?|S\.?A\.?|E\.?I\.?R\.?L\.?|S\.?R\.?L\.?)\s*$', candidate, re.I) and
                                                not re.search(r'\bR\.?U\.?C\b', candidate, re.I)), None)
                if result['razon_social']:
                    social_index = lines.index(result['razon_social'])
                    if social_index > 0 and re.search(r'\bIMPORTADORA\s*$', lines[social_index-1], re.I):
                        result['razon_social'] = lines[social_index-1] + ' ' + result['razon_social']
                    # En el logotipo de HDG, el símbolo & suele leerse como €.
                    result['razon_social'] = re.sub(r'(?<=\s)[€£$](?=\s)', '&', result['razon_social'])
                break
        elif rucs:
            result['advertencias'].append('Hay varios RUC: confirma el proveedor del comprobante.')
        # La fecha legal de autorización no identifica la emisión del comprobante.
        emission_lines = [line for line in text.splitlines() if not re.search(r'\b(?:RESOLUCI[OÓ]N|AUTORIZACI[OÓ]N|RETENEDOR)\b|\bDE\s+FECHA\b', line, re.I)]
        date_lines = re.findall(r'^.*?\bFECHA(?:\s+(?:DE\s+)?EMISI[OÓ]N)?\s*:?\s*([^\n]*)$', '\n'.join(emission_lines), re.I | re.M)
        dates = re.findall(r'\b(?:\d{4}[-/]\d{2}[-/]\d{2}|\d{2}[-/]\d{2}[-/]\d{4})\b', '\n'.join(date_lines) if date_lines else '\n'.join(emission_lines))
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
        labels_by_type = {
            # Comprobantes electrónicos: separan valor de venta e impuesto.
            'FACTURA': {'subtotal': r'(?:SUB\s*TOTAL|TOTAL\s+GRAVADO|OP\.?\s*GRAVADAS|OPERACIONES\s+GRAVADAS|VALOR\s+VENTA)',
                         'igv': r'[I1]\.?G\.?V\.?',
                         'total': r'(?:(?:IMPORTE|MONTO)\s+TOTAL|TOTAL(?:\s+A\s+PAGAR)?)'},
            'BOLETA': {'subtotal': r'(?:SUB\s*TOTAL|TOTAL\s+GRAVADO|OP\.?\s*GRAVADAS|OPERACIONES\s+GRAVADAS|VALOR\s+VENTA)',
                        'igv': r'[I1]\.?G\.?V\.?',
                        'total': r'(?:(?:IMPORTE|MONTO)\s+TOTAL|TOTAL(?:\s+A\s+PAGAR)?)'},
            # Pedidos y notas suelen mostrar importe cotizado, con IGV opcional.
            'PEDIDO': {'subtotal': r'(?:SUB\s*TOTAL|VALOR\s+VENTA|IMPORTE\s+DEL\s+PEDIDO)',
                       'igv': r'[I1]\.?G\.?V\.?',
                       'total': r'(?:(?:TOTAL\s+DEL\s+PEDIDO|IMPORTE\s+TOTAL|TOTAL))'},
            'NOTA_VENTA': {'subtotal': r'(?:SUB\s*TOTAL|VALOR\s+VENTA)',
                            'igv': r'[I1]\.?G\.?V\.?',
                            'total': r'(?:(?:IMPORTE\s+TOTAL|TOTAL(?:\s+A\s+PAGAR)?))'},
            # Ticket normalmente usa un único total pagado.
            'TICKET': {'subtotal': r'(?:SUB\s*TOTAL|CONSUMO)',
                       'igv': r'[I1]\.?G\.?V\.?',
                       'total': r'(?:(?:TOTAL\s+PAGADO|IMPORTE\s+PAGADO|TOTAL(?:\s+A\s+PAGAR)?))'},
        }
        labels = dict(labels_by_type.get(result['tipo_comprobante'], labels_by_type['NOTA_VENTA']))
        labels['subtotal'] = '(?:' + labels['subtotal'] + r'|GRAVADA)'
        for key, label in labels.items():
            # Acepta las variaciones frecuentes del OCR: $/105.08 y 5/124.00
            # corresponden visualmente a S/105.08 y S/124.00.
            pattern = r'\b' + label + r'\s*(?:\(?\d+(?:[.,]\d+)?\s*%\)?)?\s*[:=]?\s*(?:[S5$]\s*[/|íi\]]\.?\s*:?|PEN|US\$|\$)?\s*([\d][\d.,]*)(?:\s+[^\n]*)?$'
            # No atravesar saltos de línea ni tomar números del RUC o del pie.
            pattern = pattern.replace(r'\s', r'[ \t]')
            pattern = pattern.replace(r'(?:[ \t]+[^\n]*)?$', r'(?:[ \t]+[^\n]{0,12}|[|])?[ \t]*$')
            if key == 'total':
                pattern = r'(?<!SUB )(?<!SUB)' + pattern
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
        detalle_abierto = False
        for line in detail_text.splitlines():
            inferred = False
            repaired = False
            calculated = False
            # Ruido aislado al final de filas térmicas: no forma parte del importe.
            line = re.sub(r'(?<=\d)[.,]+(?=\s|$)', '', line)
            if re.match(r'^\s*\d+(?:[.,]\d+)?\s+[A-Za-zÁÉÍÓÚÑáéíóúñ]', line):
                line = re.sub(r'(\s\d+[.,]\d{2})\s+[^\d\s]{1,4}\s*$', r'\1', line)
            # Formato nombre / cantidad / unidad / precio sin columna importe.
            compacto = re.fullmatch(r'\s*(.+?[A-Za-zÁÉÍÓÚÑáéíóúñ].*?)\s+(\d+)\s+(?:UND|UNID|UNIDAD|PZA|PCS)\s+(\d+[.,]\d{2})\s*', line, re.I)
            if compacto:
                descripcion, unidades, unitario = compacto.groups()
                calculado = Decimal(unidades) * Decimal(unitario.replace(',', '.'))
                line = f'{unidades} {descripcion} {unitario} {calculado:.2f}'
                calculated = True
            # Sólo recomponer centavos separados cuando la operación de la fila coincide.
            split_price = re.fullmatch(
                r'\s*(\d+(?:[.,]\d+)?)\s+((?:UND|UNID|UNI|UNIDAD|PZA|PCS)\b.+?)\s+(\d+)\s+(\d{2})\s+(\d[\d.,]*)\s*', line, re.I)
            if split_price:
                q, label, whole, cents, row_total = split_price.groups()
                qn, tn = importe(q, cantidad=True), importe(row_total)
                pn = importe(whole + '.' + cents)
                if None not in (qn, tn, pn) and qn > 0 and abs(qn * pn - tn) <= .02:
                    line = f'{q} {label} {whole}.{cents} {row_total}'
                    repaired = True
            # El número de serie y el pie no forman parte de la descripción.
            if re.search(r'\b(?:SON|OPERADOR|VENDEDOR|GRACIAS|VUELVA|WWW)\b', line, re.I):
                detalle_abierto = False
                continue
            serial = re.search(r'\bS\s*/\s*N\s*:', line, re.I)
            if serial:
                if detalle_abierto:
                    prefix = line[:serial.start()].strip()
                    if prefix:
                        result['detalles'][-1]['descripcion'] += ' ' + prefix
                detalle_abierto = False
                continue
            if result['detalles'] and re.match(r'^\s*(?:UND|UNID|UNI|UNIDAD|PCS)\s+\S', line, re.I):
                result['detalles'][-1]['descripcion'] += ' ' + re.sub(r'^\s*(?:UND|UNID|UNI|UNIDAD|PCS)\s+', '', line, flags=re.I).strip()
                continue
            if re.search(r'\b(?:TOTAL|SUBTOTAL|IGV|RUC|FECHA|GRAVADAS?|BANCO|MONEDA|DESCUENTO|EXONERAD[AO])\b', line, re.I):
                detalle_abierto = False
                continue
            # Código de proveedor, cantidad, unidad, descripción, precio, importe.
            line = re.sub(r'^\s*[A-Z0-9-]+\s+(?=\d+(?:[.,]\d+)?\s+(?:UND|UNID|UNI|PZA|PCS)\b)', '', line, flags=re.I)
            match = re.fullmatch(r'\s*' + amount + r'\s+(.+?)\s+' + amount + r'\s+' + amount + r'\s*', line)
            if match:
                quantity, description, price, total = match.groups()
            else:
                match = re.fullmatch(r'\s*(.+?)\s+' + amount + r'\s+' + amount + r'\s+' + amount + r'\s*', line)
                if not match:
                    # Algunos comprobantes imprimen la cantidad antes de la columna
                    # y el OCR la pierde. Si precio y total son consistentes, se infiere.
                    match = re.search(r'(?:^|\s)(?:UND|UNID|UNI|UNIDAD|PZA|PCS)\.?\s+(.+?)\s+' + amount + r'\s+' + amount + r'(?:\s+\D.*)?\s*$', line, re.I)
                    if not match:
                        if detalle_abierto and re.search(r'[A-Za-zÁÉÍÓÚÑáéíóúñ]', line):
                            continuation = re.sub(r'^[^A-Za-zÁÉÍÓÚÑáéíóúñ]+', '', line).strip()
                            if continuation:
                                result['detalles'][-1]['descripcion'] += ' ' + continuation
                        continue
                    description, price, total = match.groups()
                    price, total = importe(price), importe(total)
                    if price is None or total is None or price <= 0:
                        continue
                    quantity = total / price
                    if abs(quantity - round(quantity)) > .001:
                        continue
                    quantity = str(round(quantity))
                    inferred = True
                    price, total = str(price), str(total)
                else:
                    description, quantity, price, total = match.groups()
            description = re.sub(r'^(?:UND|UNID|UNI|UNIDAD|UNIDADES|PZA|PCS)\.?\s+', '', description, flags=re.I)
            if not re.search(r'[A-Za-zÁÉÍÓÚÑáéíóúñ]', description):
                continue
            raw_price = price
            quantity, price, total = importe(quantity, cantidad=True), importe(price), importe(total)
            if None in (quantity, price, total) or quantity <= 0:
                continue
            # P/U suele imprimirse con tres decimales. Resolver la ambigüedad
            # con la cantidad y el importe, conservando miles cuando coinciden.
            decimal_price = importe(raw_price, cantidad=True)
            if (abs(quantity * price - total) > .02 and decimal_price is not None
                    and abs(quantity * decimal_price - total) <= .02):
                price = decimal_price
            # Retirar V/U (sin IGV) de la descripción cuando hay dos precios.
            description = re.sub(r'\s+\d+[.,]\d{3}$', '', description)
            valid = quantity.is_integer() and abs(quantity * price - total) <= .02
            result['detalles'].append(dict(descripcion=description.strip(), cantidad=quantity,
                                           precio_unitario=price, importe=total, requiere_revision=not valid or inferred or repaired or calculated))
            detalle_abierto = True
            if repaired:
                result['advertencias'].append('Precio con centavos separados recompuesto: confírmalo en el comprobante.')
            if inferred:
                result['advertencias'].append('Cantidad deducida del precio y el importe: confírmala en el comprobante.')
            if calculated:
                result['advertencias'].append('Importe de línea calculado a partir de cantidad y precio; compáralo con el comprobante.')
            if not valid:
                result['advertencias'].append('Revisa una línea con cantidad fraccionaria o importes inconsistentes.')
        for detalle in result['detalles']:
            detalle.update(separar_producto(detalle['descripcion']))
        if result['detalles'] and result['total'] is not None:
            if abs(sum(d['importe'] for d in result['detalles']) - result['total']) > .02:
                result['advertencias'].append('La suma de productos difiere del total; revisa productos omitidos, impuestos o descuentos.')
        if not result['detalles']:
            result['advertencias'].append('No se reconocieron productos. Usa una imagen más clara y vuelve a escanear el comprobante.')
        for key in ('numero_comprobante', 'fecha_emision', 'subtotal', 'igv', 'total'):
            if result['tipo_comprobante'] == 'TICKET' and key in ('subtotal', 'igv'):
                continue
            if result[key] is None:
                result['advertencias'].append('Completa el dato no reconocido: ' + key.replace('_', ' ') + '.')
        if result['tipo_comprobante'] == 'TICKET':
            result['advertencias'].append('El documento indica TICKET. Revisa subtotal e IGV si no están desglosados; no se deducen del total.')
        return result
