import pytesseract
from PIL import Image
import cv2
import numpy as np
import os

# Configurar ruta de tesseract
pytesseract.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR\tesseract.exe'

class OCRService:
    def __init__(self):
        self.config = '--oem 3 --psm 6'
    
    def preprocess_image(self, image_path):
        img = cv2.imread(image_path)
        if img is None:
            img = cv2.imdecode(np.fromfile(image_path, dtype=np.uint8), cv2.IMREAD_COLOR)
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        gray = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)[1]
        return gray
    
    def extract_text(self, image_path):
        try:
            processed = self.preprocess_image(image_path)
            text = pytesseract.image_to_string(processed, config=self.config, lang='spa')
            return text
        except Exception as e:
            raise Exception(f"Error en OCR: {str(e)}")