from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.middleware.cors import CORSMiddleware
import tempfile
import os
from ocr import OCRService
from extractor import ComprobanteExtractor

app = FastAPI(title="Servicio OCR - KEYOVE", version="1.0.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

ocr_service = OCRService()
extractor = ComprobanteExtractor()

@app.get("/health")
async def health():
    return {"status": "ok", "message": "Servicio OCR funcionando"}

@app.post("/ocr/extraer")
async def extraer_comprobante(file: UploadFile = File(...)):
    try:
        if not file.filename.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp', '.tiff', '.pdf')):
            raise HTTPException(status_code=400, detail="Formato de archivo no soportado")
        
        with tempfile.NamedTemporaryFile(delete=False, suffix=os.path.splitext(file.filename)[1]) as temp_file:
            content = await file.read()
            temp_file.write(content)
            temp_path = temp_file.name
        
        try:
            texto = ocr_service.extract_text(temp_path)
            datos = extractor.extract(texto)
            return {
                "success": True,
                "texto_ocr": texto,
                "datos": datos
            }
        finally:
            if os.path.exists(temp_path):
                os.unlink(temp_path)
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    import uvicorn
    uvicorn.run("app:app", host="0.0.0.0", port=5000, reload=True)