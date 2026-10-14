# Compras con OCR

El OCR está reservado al rol **Administrador** y sólo se utiliza en Compras.
Al elegir una imagen o PDF se lee automáticamente. Se completan los datos reconocidos y se relacionan productos por nombre o código exacto. Los datos dudosos requieren revisión; no se crean productos ni proveedores inventados.

1. Selecciona la foto o PDF (PNG/JPG/BMP/TIFF/PDF; máximo 10 MB y 5 páginas).
2. Revisa proveedor, fecha, número, productos, cantidades e importes. Los productos desconocidos se relacionan mediante el selector. Los nuevos productos/proveedores se crean en sus pantallas de mantenimiento y se vuelve a extraer el documento.
3. Indica si los precios de las líneas incluyen IGV. Si el OCR no reconoció un importe, complétalo; un campo vacío no equivale a cero. Los importes se registran en soles.
4. Confirma la revisión y pulsa Guardar. Compra, detalles, entrada de stock y archivo se guardan en una sola transacción. El número y la fecha del proveedor se conservan.
5. En Compras realizadas, el enlace Comprobante descarga el archivo original. Las compras manuales o antiguas sin archivo no disponen de descarga.

La extracción utiliza reglas para formatos habituales (cantidad, descripción, precio unitario e importe, o descripción antes de la cantidad). No garantiza lectura de cualquier distribución, tablas sin líneas claras, descuentos por producto o fotos borrosas. Los errores de OCR nunca deben aceptarse sin revisión. No convierte monedas extranjeras automáticamente. El precio de compra guardado es el precio unitario confirmado en el formulario, con o sin IGV según el documento.

## Instalación

- ASP.NET Framework 4.8, SQL Server y Tesseract OCR.
- Ejecutar el esquema de `Data/use DB_keyove_Inventario.txt` **sólo para una base nueva**; no se ejecuta sobre una base existente.
- Ejecutar `Scripts/01_crear_comprobantes_ocr.sql` para habilitar el archivo digital.
- Ejecutar `Scripts/02_compras_comprobante_activo.sql` para permitir corregir una compra anulada usando su mismo comprobante, conservando el historial.
- Ejecutar `Scripts/03_registrar_compra_transaccion.sql` para crear el tipo de tabla `TipoDetalleCompra` y el procedimiento `sp_RegistrarCompra` que utiliza el formulario. Recibe todos los productos y guarda cabecera, detalles, stock, movimientos y archivo OCR en una sola transacción; cualquier error revierte todo. Puede ejecutarse nuevamente para actualizar el procedimiento.
- En `servicio_ocr`, ejecutar `setup.ps1` y después `start.ps1` o `start.bat`. Se puede usar `TESSERACT_CMD` para indicar otra ubicación del ejecutable.
- El servicio escucha en `127.0.0.1:5000` del **servidor web**. ASP.NET se comunica con él usando `OcrServiceUrl` de Web.config. El navegador no necesita instalar Python ni Tesseract.
- La conexión SQL acepta `connectionStrings/ConexionInventario`; si no se configura, usa `localhost\SQLEXPRESS`, base `DB_keyove_inventario`, con autenticación Windows. La identidad de IIS debe tener acceso.
- Los límites ASP.NET/IIS permiten el envío Base64 de un archivo de 10 MB. El archivo se conserva completo; los mayores se rechazan.

En este equipo se preparó el entorno `.venv` con acceso a las dependencias Python ya instaladas, se instaló pypdfium2 y se descargó el modelo español en `servicio_ocr/tessdata`. Estos archivos locales se excluyen de Git; en otro equipo hay que ejecutar la instalación.

## Inventario y correcciones

Registrar una compra aumenta el stock. Anularla revierte su movimiento una sola vez. Una compra no puede anularse si deja stock negativo. El guardado de compras rechaza comprobantes repetidos del mismo proveedor y valida que los productos coincidan con los importes.

En la pantalla de una compra registrada se modifica su observación. Para cambiar cantidades o importes se anula y registra correctamente la compra. La pantalla de detalles de compras queda como consulta y sus métodos de escritura rechazan cambios; así no se desajustan los movimientos. No deben editarse las tablas directamente.

El registro de compras mantiene el stock mediante sp_RegistrarCompra y su anulación mediante el código de compras. Ventas queda a cargo de su responsable; su código previo requiere sp_RegistrarVenta, cuya definición debe proporcionar esa responsable para instalaciones nuevas. Si se instalan triggers personalizados, revisar su interacción para no duplicar movimientos.

## Pruebas

- Compilación con MSBuild del proyecto VB.NET.
- `servicio_ocr/.venv/Scripts/python.exe -m unittest discover -s Tests -p test_ocr.py -v` (requiere `requirements-test.txt`). Incluye OCR real sobre una imagen y un PDF sintéticos.
- Tests JavaScript con Node y Playwright: `compras-ocr.test.cjs`, `compras-navigation.test.cjs`, `ventas-navigation.test.cjs`, `layout-responsive.test.cjs`, `detalles-ui.test.cjs`.
- `Tests/CompraIntegration.cs`: prueba el código compilado en una base SQL temporal de nombre aleatorio; no inserta operaciones en la base del negocio. Comprueba archivo mayor de 2 MB, duplicados, rollback, permisos, stock de compras y sus anulaciones. La base temporal se elimina al finalizar.

Las pruebas del navegador simulan las respuestas HTTP. La prueba SQL ejecuta los métodos del servidor con una sesión simulada. La prueba OCR usa documentos sintéticos; conviene validar fotografías reales de los proveedores antes del uso diario.
