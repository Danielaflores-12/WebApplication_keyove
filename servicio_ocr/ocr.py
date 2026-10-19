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
        def header(prefix):
            return next((w for w in words if key(w['text']).startswith(prefix)), None)
        qty, unit, price, amount = (header(k) for k in ('CANT', 'UM', 'PUNIT', 'IMPORTE'))
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
            rows.append(f"{q['text']} {label} {prices[i]['text']} {amounts[i]['text']}")
        return rows

    def read_image(self, image):
        if image.width * image.height > Image.MAX_IMAGE_PIXELS:
            raise ValueError('La imagen supera los 30 megapíxeles.')
        image = ImageOps.exif_transpose(image).convert('RGB')
        gray = cv2.cvtColor(np.asarray(image), cv2.COLOR_RGB2GRAY)
        original = gray
        if gray.shape[1] < 1800 and gray.size * 4 <= Image.MAX_IMAGE_PIXELS:
            gray = cv2.resize(gray, None, fx=2, fy=2, interpolation=cv2.INTER_CUBIC)
        text = pytesseract.image_to_string(gray, config='--oem 3 --psm 6', lang='spa', timeout=25)
        if re.search(r'CANT|P\.?\s*UNIT', text, re.I):
            rows = self.table_rows(gray)
            if rows:
                text += '\n[DETALLE_RECONSTRUIDO]\n' + '\n'.join(rows) + '\n[/DETALLE_RECONSTRUIDO]\n'
        # Si el fondo domina la lectura, aislar la región clara del papel.
        from extractor import ComprobanteExtractor
        result = ComprobanteExtractor().extract(text)
        # Las facturas con tablas suelen leerse mejor con PSM 4. Sólo se usa como
        # segunda lectura cuando la lectura principal dejó datos esenciales vacíos.
        if (not result['detalles'] or
                any(result[key] is None for key in ('numero_comprobante', 'ruc_emisor', 'total'))):
            invoice_text = pytesseract.image_to_string(gray, config='--oem 3 --psm 4', lang='spa', timeout=25)
            if invoice_text.strip():
                text += '\n[LECTURA_ALTERNATIVA]\n' + invoice_text
                result = ComprobanteExtractor().extract(text)
        if not result['detalles']:
            mask = cv2.threshold(cv2.GaussianBlur(original, (31,31), 0), 170, 255, cv2.THRESH_BINARY)[1]
            contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
            if contours:
                x,y,w,h = cv2.boundingRect(max(contours, key=cv2.contourArea))
                if w*h > original.size*.25 and w < original.shape[1]*.9:
                    crop = original[y:y+h,x:x+w]
                    normalized = cv2.divide(crop, cv2.GaussianBlur(crop,(0,0),25),scale=255)
                    for candidate in (crop, normalized):
                        if candidate.shape[1] < 1800 and candidate.size*4 <= Image.MAX_IMAGE_PIXELS:
                            candidate = cv2.resize(candidate,None,fx=2,fy=2)
                        alternative = pytesseract.image_to_string(candidate,config='--oem 3 --psm 6',lang='spa',timeout=25)
                        text += '\n[LECTURA_ALTERNATIVA]\n' + alternative
        return text

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
