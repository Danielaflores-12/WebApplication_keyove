# Datos de prueba en la base existente

`insertar_30_en_base_existente.sql` conserva el lote insertado en
`DB_keyove_inventario`. Agrega 30 registros en Personas, Categorias,
Proveedores, Productos, Compras, Detalle_Compra, Ventas, Detalle_Venta y
Comprobantes_OCR, más 60 movimientos de inventario: 330 registros nuevos.

Utiliza la cuenta existente `ana_admin`. No crea usuarios ni roles, no cambia
la estructura de las tablas y no elimina ni actualiza registros anteriores.
Los comprobantes son imágenes ficticias sin valor comercial.

El catálogo DEMO está adaptado a periféricos y accesorios: teclados, mouse,
micro SD, memorias USB, smartwatch, TV Box, cables, tomacorrientes, mandos,
cargadores, audífonos y hubs. `actualizar_demo_perifericos.sql` actualiza el
lote anterior conservando IDs, precios, stock y relaciones, y actualiza los
PNG ficticios. Esa actualización ya se aplicó en este equipo.
También registra, si falta, el proveedor con RUC 20252011910 que figura en
la factura facilitada por la usuaria; no registra la compra de esa factura.

La carga ya está aplicada en este equipo. No es necesario ejecutarla otra vez.
El script rechaza códigos DEMO existentes para impedir duplicar la carga.
Para usarlo en otra instalación compatible, ejecutar desde la raíz del proyecto:

```powershell
sqlcmd -S 'localhost\SQLEXPRESS' -E -C -b -i Scripts/DatosPrueba/insertar_30_en_base_existente.sql
```

La base separada `DB_keyove_inventario_pruebas_20261015` y los archivos que
servían para crearla y verificarla fueron retirados por solicitud del usuario.
La base original y sus datos se conservan.
