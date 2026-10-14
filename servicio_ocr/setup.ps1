$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
python -m venv .venv
if ($LASTEXITCODE -ne 0) { throw 'No se pudo crear el entorno de Python.' }
& .venv/Scripts/python.exe -m pip install -r requirements.txt
if ($LASTEXITCODE -ne 0) { throw 'No se pudieron instalar las dependencias.' }
New-Item -ItemType Directory -Path tessdata -Force | Out-Null
Invoke-WebRequest -Uri 'https://raw.githubusercontent.com/tesseract-ocr/tessdata_fast/main/spa.traineddata' -OutFile tessdata/spa.traineddata
Write-Host 'OCR preparado. Instala Tesseract si aún no está disponible y ejecuta start.ps1.'
