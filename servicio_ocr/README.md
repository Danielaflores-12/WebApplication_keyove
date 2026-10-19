# Compras con OCR

El OCR está reservado al rol **Administrador** y sólo se utiliza en Compras.
Al elegir una imagen o PDF se lee automáticamente. Se completan los datos reconocidos y se relacionan productos por nombre o código exacto. Los datos dudosos requieren revisión; no se crean productos ni proveedores inventados.

1. Selecciona la foto o PDF (PNG/JPG/BMP/TIFF/PDF; máximo 10 MB y 5 páginas).
2. Revisa proveedor, fecha, número, productos, cantidades e importes. Los productos desconocidos se relacionan mediante el selector. Si faltan en el catálogo, se abre una ventana con los datos reconocidos para preparar su alta sin salir de Compras. Completa razón social y RUC del proveedor, y nombre, categoría y precio de venta del producto. Confirmar la ventana sólo prepara el borrador.
3. Indica si los precios de las líneas incluyen IGV. Si el OCR no reconoció un importe, complétalo; un campo vacío no equivale a cero. Los importes se registran en soles.
4. Confirma la revisión y pulsa Guardar. Proveedor y productos nuevos, compra, detalles, entrada de stock y archivo se guardan en una sola transacción. Si falla cualquiera de esos pasos, se revierte todo. Se reutilizan proveedores por RUC y productos por nombre exacto para evitar duplicados. El número y la fecha del proveedor se conservan.
5. En Compras realizadas, el enlace Comprobante descarga el archivo original. Las compras manuales o antiguas sin archivo no disponen de descarga.

En Nueva compra, **Revisar / modificar compra** abre una única ventana con todos los campos: proveedor, usuario, tipo y número de comprobante, fecha, observación, estado, productos, cantidades, precios, subtotal, IGV y total. Los botones Modificar de cada línea llevan a su fila dentro de esa misma ventana. Cerrar conserva el borrador; Guardar registra la compra.

**Modificar proveedor** muestra su ficha dentro de la ventana de compra. Guardar cambios actualiza el catálogo compartido; Cancelar descarta esa edición. Las fichas nuevas también se preparan dentro de la misma ventana y se crean junto con la compra al guardar. Los importes OCR se conservan para su conciliación.

Para tickets TK, el OCR intenta aislar el papel del fondo cuando falla la lectura
normal. Combina sólo campos reconocidos sin contradicciones entre lecturas.
Un ticket sin desglose no permite deducir automáticamente subtotal ni IGV, ni se
clasifica automáticamente como boleta. Esos campos y las fechas ilegibles quedan
pendientes de confirmación. Las pruebas no guardan el ticket real en compras.

La extracción utiliza reglas para formatos habituales (cantidad, descripción, precio unitario e importe, o descripción antes de la cantidad). No garantiza lectura de cualquier distribución, tablas sin líneas claras, descuentos por producto o fotos borrosas. Los errores de OCR nunca deben aceptarse sin revisión. No convierte monedas extranjeras automáticamente. El precio de compra guardado es el precio unitario confirmado en el formulario, con o sin IGV según el documento.

## Instalación

Las tablas con encabezados CANT., U.M., P.UNIT. e IMPORTE se reconstruyen
por posición de columna para separar códigos y cantidades incluso cuando
el papel curva las filas. Si las columnas no tienen el mismo número de
valores, se conserva la lectura normal y se requiere revisión.
Se admite Total Gravado como subtotal. Los precios que suman el total del
comprobante se marcan automáticamente como incluidos en IGV.

- ASP.NET Framework 4.8, SQL Server y Tesseract OCR.
- Ejecutar el esquema de `Data/use DB_keyove_Inventario.txt` **sólo para una base nueva**; no se ejecuta sobre una base existente.
- Ejecutar `Scripts/01_crear_comprobantes_ocr.sql` para habilitar el archivo digital.
- Ejecutar `Scripts/02_compras_comprobante_activo.sql` para permitir corregir una compra anulada usando su mismo comprobante, conservando el historial.
- Ejecutar `Scripts/03_registrar_compra_transaccion.sql` para crear el tipo de tabla `TipoDetalleCompra` y el procedimiento `sp_RegistrarCompra` que utiliza el formulario. Recibe todos los productos y guarda cabecera, detalles, stock, movimientos y archivo OCR en una sola transacción; cualquier error revierte todo. Puede ejecutarse nuevamente para actualizar el procedimiento.
- Preparación inicial por equipo: en `servicio_ocr`, ejecutar `setup.ps1` e instalar Tesseract. Se puede usar `TESSERACT_CMD` para indicar otra ubicación del ejecutable.
- Uso diario en Visual Studio: pulsar **F5**. Al arrancar ASP.NET en modo depuración, se inicia OCR en segundo plano sin abrir otra ventana. Compras espera a que esté listo y lo reinicia si se detuvo. No hace falta ejecutar `start.ps1` ni `start.bat` por separado.
- El servicio local permanece disponible al detener la depuración y se reutiliza en el próximo F5. El diagnóstico se guarda en `servicio_ocr/.runtime/auto-start.log`. `OcrAutoStart=false` desactiva este comportamiento. En publicación sin depuración, administrar el servicio OCR por separado. El arranque no reinstala dependencias ni ejecuta scripts de base de datos.
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
- `Tests/OcrStartupIntegration.cs`: comprueba arranque simultáneo, reutilización y recuperación del proceso OCR local; sólo detiene el proceso que inicia la propia prueba.
- `servicio_ocr/.venv/Scripts/python.exe -m unittest discover -s Tests -p test_ocr.py -v` (requiere `requirements-test.txt`). Incluye OCR real sobre una imagen y un PDF sintéticos.
- Tests JavaScript con Node y Playwright: `compras-ocr.test.cjs`, `compras-navigation.test.cjs`, `ventas-navigation.test.cjs`, `layout-responsive.test.cjs`, `detalles-ui.test.cjs`.
- `Tests/CompraIntegration.cs`: prueba el código compilado en una base SQL temporal de nombre aleatorio; no inserta operaciones en la base del negocio. Comprueba archivo mayor de 2 MB, duplicados, rollback, permisos, stock de compras y sus anulaciones. La base temporal se elimina al finalizar.

Las pruebas del navegador simulan las respuestas HTTP. La prueba SQL ejecuta los métodos del servidor con una sesión simulada. La prueba OCR usa documentos sintéticos; conviene validar fotografías reales de los proveedores antes del uso diario.

Las altas desde Compras utilizan la misma conexión y una transacción exterior al procedimiento existente; no requieren otro script SQL. Los productos nuevos reciben código interno automático, unidad UNIDAD y stock inicial cero; la compra añade la cantidad confirmada. Las pruebas SQL comprueban también la reversión de altas y la reutilización del catálogo.
