"""Servicio privado: el navegador accede mediante la sesión de ASP.NET."""
import base64
import binascii
import io
import os
import sys
from pathlib import Path

# El proceso iniciado desde F5 no abre una consola; conserva su diagnóstico.
if __name__ == '__main__':
    runtime = Path(__file__).resolve().parent / '.runtime'
    runtime.mkdir(exist_ok=True)
    startup_log = (runtime / 'auto-start.log').open('a', encoding='utf-8', buffering=1)
    sys.stdout = startup_log
    sys.stderr = startup_log
import tempfile
from threading import Lock
from fastapi import FastAPI, HTTPException, Request, Response
from PIL import Image, ImageOps, ImageSequence, UnidentifiedImageError
from pydantic import BaseModel, Field
from ocr import OCRService
from extractor import ComprobanteExtractor
from resumen_compra import ResumenCompra, generar_resumen

MAX_BYTES = 10 * 1024 * 1024
app = FastAPI(title='OCR de compras KEYOVE', version='2.0.0')
ocr_service = OCRService()
extractor = ComprobanteExtractor()
processing = Lock()

@app.middleware('http')
async def solo_servidor_local(request: Request, call_next):
    from starlette.responses import JSONResponse
    if request.client is None or request.client.host not in ('127.0.0.1', '::1', 'testclient'):
        return JSONResponse(status_code=403, content={'detail': 'Servicio privado.'})
    if request.headers.get('origin'):
        return JSONResponse(status_code=403, content={'detail': 'Accede desde Compras.'})
    length = request.headers.get('content-length', '0')
    if not length.isdigit() or int(length) > 15 * 1024 * 1024:
        return JSONResponse(status_code=413, content={'detail': 'Archivo demasiado grande.'})
    return await call_next(request)

class Archivo(BaseModel):
    nombreArchivo: str = Field(min_length=1, max_length=255)
    contenidoBase64: str = Field(max_length=14_000_000)

def leer_archivo(file):
    try:
        content = base64.b64decode(file.contenidoBase64, validate=True)
    except (ValueError, binascii.Error):
        raise HTTPException(400, 'El archivo no es válido.')
    if not content or len(content) > MAX_BYTES:
        raise HTTPException(413, 'El archivo debe tener entre 1 byte y 10 MB.')
    return content

@app.post('/documento/pdf', response_class=Response)
def convertir_documento(file: Archivo):
    content = leer_archivo(file)
    if not processing.acquire(blocking=False):
        raise HTTPException(409, 'Hay otro comprobante en proceso. Reintenta en unos segundos.')
    pages = []
    try:
        pixels = 0
        with Image.open(io.BytesIO(content), formats=['JPEG', 'PNG', 'BMP', 'TIFF']) as image:
            for frame in ImageSequence.Iterator(image):
                pixels += frame.width * frame.height
                if pixels > Image.MAX_IMAGE_PIXELS or len(pages) >= 50:
                    raise HTTPException(413, 'La imagen supera el límite de 30 megapíxeles o 50 páginas.')
                with ImageOps.exif_transpose(frame).convert('RGBA') as oriented:
                    page = Image.new('RGB', oriented.size, 'white')
                    pages.append(page)
                    page.paste(oriented, mask=oriented.getchannel('A'))
        with io.BytesIO() as output:
            pages[0].save(output, format='PDF', save_all=True,
                          append_images=pages[1:], resolution=150, quality=85, title='Comprobante')
            if output.tell() > MAX_BYTES:
                raise HTTPException(413, 'El PDF generado supera los 10 MB.')
            return Response(output.getvalue(), media_type='application/pdf')
    except (UnidentifiedImageError, OSError, ValueError, Image.DecompressionBombError):
        raise HTTPException(422, 'No se pudo convertir la imagen a PDF.')
    finally:
        for page in pages:
            page.close()
        processing.release()

@app.get('/health')
def health():
    return {'status': 'ok', 'service': 'keyove-compras-ocr', 'pid': os.getpid()}

@app.post('/compra/resumen/pdf', response_class=Response)
def resumen_compra(compra: ResumenCompra):
    return Response(generar_resumen(compra), media_type='application/pdf')

@app.post('/ocr/extraer')
def extraer_comprobante(file: Archivo):
    extension = os.path.splitext(file.nombreArchivo)[1].lower()
    if extension not in ('.png', '.jpg', '.jpeg', '.bmp', '.tiff', '.tif', '.pdf'):
        raise HTTPException(400, 'Usa PNG, JPG, BMP, TIFF o PDF.')
    content = leer_archivo(file)
    if not processing.acquire(blocking=False):
        raise HTTPException(409, 'Hay otro comprobante en proceso. Reintenta en unos segundos.')
    path = None
    try:
        with tempfile.NamedTemporaryFile(delete=False, suffix=extension) as temp:
            temp.write(content)
            path = temp.name
        text = ocr_service.extract_text(path)
        if not text.strip():
            raise ValueError('No se encontró texto legible. Usa una foto más clara.')
        return {'success': True, 'texto_ocr': text, 'datos': extractor.extract(text)}
    except (ValueError, RuntimeError) as error:
        raise HTTPException(422, str(error))
    except Exception:
        raise HTTPException(422, 'No se pudo leer el archivo. Comprueba el formato y Tesseract con español.')
    finally:
        if path and os.path.exists(path):
            os.unlink(path)
        processing.release()

if __name__ == '__main__':
    import uvicorn
    uvicorn.run(app, host='127.0.0.1', port=5000)
