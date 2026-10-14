import os
import shutil
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
    def read_image(self, image):
        if image.width * image.height > Image.MAX_IMAGE_PIXELS:
            raise ValueError('La imagen supera los 30 megapíxeles.')
        image = ImageOps.exif_transpose(image).convert('RGB')
        gray = cv2.cvtColor(np.asarray(image), cv2.COLOR_RGB2GRAY)
        gray = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)[1]
        return pytesseract.image_to_string(gray, config='--oem 3 --psm 6', lang='spa', timeout=25)

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
