import os
import shutil
import re
import unicodedata
import numpy as np
import cv2
import pytesseract
from PIL import Image, ImageOps

command = os.getenv('TESSERACT_CMD') or shutil.which('tesseract')
if not command and os.name == 'nt':
    command = r'C:\Program Files\Tesseract-OCR\tesseract.exe'
if command:
    pytesseract.pytesseract.tesseract_cmd = command
local_data = os.path.join(os.path.dirname(__file__), 'tessdata')
if os.path.isfile(os.path.join(local_data, 'spa.traineddata')):
    os.environ.setdefault('TESSDATA_PREFIX', local_data)
Image.MAX_IMAGE_PIXELS = 30_000_000


class OCRService:
    def table_rows(self, gray):
        """Reconstruye columnas por posición cuando el papel curva las filas."""
        data = pytesseract.image_to_data(gray, config='--oem 3 --psm 11', lang='spa',
                                         timeout=25, output_type=pytesseract.Output.DICT)
        words = [dict(text=t.strip(), x=data['left'][i], y=data['top'][i],
                      w=data['width'][i], h=data['height'][i])
                 for i, t in enumerate(data['text']) if t.strip()]
        def key(t):
            return re.sub('[^A-Z]', '', unicodedata.normalize('NFKD', t.upper()))
        qty = next((w for w in words if key(w['text']).startswith('CANT')), None)
        if not qty:
            return []
        def header(*labels):
            return next((w for w in words if w['x'] > qty['x']
                         and abs(w['y'] - qty['y']) <= qty['h'] * 2
                         and key(w['text']) in labels), None)
        unit = header('UM')
        description_header = header('DESCRIPCION')
        price = header('PUNIT', 'PU', 'PUNITARIO')
        amount = header('IMPORTE', 'TOTAL')
        if not unit and description_header and price and amount:
            return self._description_rows(words, qty, description_header, price, amount)
        if not all((qty, unit, price, amount)) or not qty['x'] < unit['x'] < price['x'] < amount['x']:
            return []
        if max(w['y'] for w in (qty, unit, price, amount))-min(w['y'] for w in (qty, unit, price, amount)) > qty['h']*4:
            return []
        def column(left, right, top):
            candidates = sorted([w for w in words if left <= w['x']+w['w']/2 < right
                                 and w['y'] > top and re.fullmatch(r'\d+(?:[.,]\d+)?', w['text'])], key=lambda w:w['y'])
            contiguous = []
            for w in candidates:
                previous = contiguous[-1]['y'] if contiguous else top
                if w['y']-previous > qty['h']*4:
                    break
                contiguous.append(w)
            return contiguous
        quantities = column(qty['x'], unit['x'], qty['y']+qty['h'])
        prices = column(price['x'], amount['x'], price['y']+price['h'])
        amounts = column(amount['x'], gray.shape[1], amount['y']+amount['h'])
        if not quantities or not len(quantities) == len(prices) == len(amounts):
            return []
        rows = []
        code_header = next((w for w in words if key(w['text']) == 'CODIGO'
                            and w['x'] < qty['x'] and abs(w['y'] - qty['y']) <= 2 * qty['h']), None)
        codes = column(0, qty['x'], code_header['y'] + code_header['h']) if code_header else []
        if len(codes) != len(quantities) or any(not w['text'].isdigit() or abs(w['y'] - q['y']) > 2 * q['h']
                                                 for w, q in zip(codes, quantities)):
            codes = []
        for i, q in enumerate(quantities):
            description = []
            for w in words:
                if not unit['x']+unit['w'] < w['x'] < price['x'] or key(w['text']) == 'DESCRIPCION':
                    continue
                nearest = min(range(len(quantities)), key=lambda j:abs(w['y']+w['h']/2-quantities[j]['y']-quantities[j]['h']/2))
                if nearest == i and abs(w['y']+w['h']/2-q['y']-q['h']/2) <= q['h']*1.3:
                    description.append(w)
            label = ' '.join(w['text'] for w in sorted(description, key=lambda w:w['x']))
            if not re.search('[a-zA-Z]', label):
                return []
            code = f" COD: {codes[i]['text']}" if codes else ''
            rows.append(f"{q['text']} {label}{code} {prices[i]['text']} {amounts[i]['text']}")
        return rows

    @staticmethod
    def _description_rows(words, qty, description, price, amount):
        """Lee tablas CANT. / DESCRIPCIÓN / P.U. / TOTAL con descripciones de varias líneas."""
        from extractor import importe

        if not qty['x'] < description['x'] < price['x'] < amount['x']:
            return []
        height = qty['h']
        top = max(w['y'] + w['h'] / 2 for w in (qty, description, price, amount)) + height / 2
        def x(w):
            return w['x'] + w['w'] / 2
        def y(w):
            return w['y'] + w['h'] / 2
        footer = min((y(w) for w in words if y(w) > top and
                      re.fullmatch(r'(?:TOTAL|SUBTOTAL|IGV|OPERADOR)[:.]?', w['text'], re.I)), default=float('inf'))
        left = description['x'] - height / 2
        middle = (x(price) + x(amount)) / 2
        amounts = [w for w in words if top < y(w) < footer and re.fullmatch(r'\d[\d.,]*', w['text'])]
        quantities = sorted((w for w in amounts if qty['x'] - height <= x(w) < left), key=y)
        rows = []
        for i, q in enumerate(quantities):
            prices = [w for w in amounts if price['x'] - height <= x(w) < middle and abs(y(w) - y(q)) <= height]
            totals = [w for w in amounts if x(w) >= middle and abs(y(w) - y(q)) <= height]
            if len(prices) != 1 or len(totals) != 1:
                return []
            unit_price, total = prices[0]['text'], totals[0]['text']
            units, value, line_total = importe(q['text'], cantidad=True), importe(unit_price), importe(total)
            if None in (units, value, line_total) or units <= 0 or abs(units * value - line_total) > .02:
                return []
            next_row = y(quantities[i + 1]) - height / 2 if i + 1 < len(quantities) else footer
            label_words = sorted((w for w in words if left <= x(w) < price['x'] - height
                                  and y(q) - height / 2 <= y(w) < next_row
                                  and y(w) <= y(q) + 4 * height
                                  and re.search(r'\w', w['text'])), key=y)
            lines = []
            for w in label_words:
                if not lines or y(w) - y(lines[-1][0]) > height / 2:
                    lines.append([])
                lines[-1].append(w)
            label = ' '.join(w['text'] for line in lines for w in sorted(line, key=x))
            if not re.search(r'[A-Za-zÁÉÍÓÚÑáéíóúñ]', label):
                return []
            rows.append(f"{q['text']} {label} {unit_price} {total}")
        return rows

    def _read(self, gray, psm):
        return pytesseract.image_to_string(
            gray, config=f'--oem 3 --psm {psm}', lang='spa', timeout=25)

    def _orientation(self, gray):
        """OSD corrige giros de 90 grados; no depende del EXIF de la foto."""
        sample = gray
        if max(gray.shape) > 1800:
            scale = 1800 / max(gray.shape)
            sample = cv2.resize(gray, None, fx=scale, fy=scale)
        try:
            info = pytesseract.image_to_osd(
                sample, output_type=pytesseract.Output.DICT, timeout=8)
            angle = int(info.get('rotate', 0)) % 360
            if float(info.get('orientation_conf', 0)) >= 2 and angle:
                return np.ascontiguousarray(np.rot90(gray, -(angle // 90))), True
        except (RuntimeError, pytesseract.TesseractError):
            pass
        return gray, False

    def _prepare(self, gray):
        if gray.shape[1] < 1800 and gray.size * 4 <= Image.MAX_IMAGE_PIXELS:
            return cv2.resize(gray, None, fx=2, fy=2, interpolation=cv2.INTER_CUBIC)
        return gray

    def read_image(self, image):
        if image.width * image.height > Image.MAX_IMAGE_PIXELS:
            raise ValueError('La imagen supera los 30 megapíxeles.')
        from extractor import ComprobanteExtractor
        parser = ComprobanteExtractor()
        image = ImageOps.exif_transpose(image).convert('RGB')
        original = cv2.cvtColor(np.asarray(image), cv2.COLOR_RGB2GRAY)
        original, oriented = self._orientation(original)
        variants = []

        def read(candidate, psm=6, reconstruct=False):
            try:
                text = self._read(self._prepare(candidate), psm)
            except (RuntimeError, pytesseract.TesseractError):
                # Una alternativa fallida no elimina las lecturas anteriores.
                return ''
            if reconstruct and re.search(r'CANT|P\.?\s*UNIT', text, re.I):
                try:
                    rows = self.table_rows(self._prepare(candidate))
                except (RuntimeError, pytesseract.TesseractError):
                    rows = []
                if rows:
                    # Conservar también la lectura original si la tabla se reconstruyó incompleta.
                    if text.strip() and text not in variants:
                        variants.append(text)
                    text += '\n[DETALLE_RECONSTRUIDO]\n' + '\n'.join(rows) + '\n[/DETALLE_RECONSTRUIDO]\n'
            if text.strip() and text not in variants:
                variants.append(text)
            return text

        initial = read(original, reconstruct=True)
        # Si OSD no pudo decidir y la foto es horizontal, contrastar ambos giros.
        if not oriented and original.shape[1] > original.shape[0]:
            options = [(original, initial)]
            for turn in (1, 3):
                rotated = np.ascontiguousarray(np.rot90(original, turn))
                options.append((rotated, read(rotated)))
            original, _ = max(options, key=lambda pair: (
                parser._score(parser.extract(pair[1])),
                len(re.findall(r'[A-Za-zÁÉÍÓÚÑáéíóúñ]{4,}', pair[1]))))
            # Las orientaciones descartadas no deben generar conflictos de campos.
            variants.clear()
            read(original, reconstruct=True)
        result = parser.extract('\n[LECTURA_ALTERNATIVA]\n'.join(variants))
        incomplete = (not result['detalles'] or any(
            result[key] is None for key in ('numero_comprobante', 'ruc_emisor', 'fecha_emision', 'total')))
        inconsistent = any(d['requiere_revision'] for d in result['detalles'])
        if result['detalles'] and result['total'] is not None:
            inconsistent |= abs(sum(d['importe'] for d in result['detalles']) - result['total']) > .02
        if incomplete or inconsistent:
            read(original, psm=4)
            # Comparar iluminación normalizada aun si ya se reconoció un producto.
            background = cv2.GaussianBlur(original, (0, 0), 25)
            normalized = cv2.divide(original, background, scale=255)
            read(normalized)
            # La luz desigual de los tickets puede ocultar la mitad inferior.
            # Se conserva cada lectura para contrastar campos y aritmética.
            adaptive = cv2.adaptiveThreshold(original, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
                                             cv2.THRESH_BINARY, 41, 12)
            read(adaptive)
            read(adaptive, psm=11)
            # Recorte conservador: sólo si una región clara ocupa gran parte de la foto.
            mask = cv2.threshold(cv2.GaussianBlur(original, (31, 31), 0), 170, 255, cv2.THRESH_BINARY)[1]
            contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
            if contours:
                x, y, w, h = cv2.boundingRect(max(contours, key=cv2.contourArea))
                if w * h > original.size * .25 and w < original.shape[1] * .9:
                    read(normalized[y:y+h, x:x+w])
        if not variants:
            raise RuntimeError('No se pudo obtener texto. Revisa la imagen y Tesseract con español.')
        return '\n[LECTURA_ALTERNATIVA]\n'.join(variants)

    def extract_text(self, path):
        if path.lower().endswith('.pdf'):
            import pypdfium2 as pdfium
            document = pdfium.PdfDocument(path)
            try:
                if not 1 <= len(document) <= 5:
                    raise ValueError('El PDF debe tener entre 1 y 5 páginas.')
                texts = []
                for index in range(len(document)):
                    page = document[index]
                    try:
                        width, height = page.get_size()
                        if width * height * 4 > Image.MAX_IMAGE_PIXELS:
                            raise ValueError('La página del PDF es demasiado grande.')
                        bitmap = page.render(scale=2)
                        try:
                            texts.append(self.read_image(bitmap.to_pil()))
                        finally:
                            bitmap.close()
                    finally:
                        page.close()
                return '\n'.join(texts)
            finally:
                document.close()
        with Image.open(path) as image:
            return self.read_image(image)
