USE DB_keyove_inventario;
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
BEGIN TRY
BEGIN TRANSACTION;
-- Usuario exclusivo de esta carga; no modifica cuentas existentes.
DECLARE @duena int, @rolDemo int, @personaDemo int;
IF EXISTS (SELECT 1 FROM dbo.Usuarios WITH(UPDLOCK,HOLDLOCK)
           WHERE cNombreUsuario=N'keyove_demo')
    THROW 51000, 'El usuario keyove_demo ya existe. No se modificó la cuenta ni se insertó el lote.', 1;
SELECT @rolDemo=iCodRol FROM dbo.Roles WITH(UPDLOCK,HOLDLOCK)
WHERE cNombre=N'Administrador' AND bEstado=1;
IF @rolDemo IS NULL
    THROW 51000, 'Falta un rol Administrador activo. No se insertó el lote.', 1;
IF EXISTS(SELECT 1 FROM dbo.Productos WITH(UPDLOCK,HOLDLOCK) WHERE cCodigo LIKE 'DEMO-P-%')
 THROW 51000, 'Ya existen productos DEMO-P-. No se insertó el lote.', 1;
IF EXISTS (SELECT 1 FROM dbo.Personas WITH(UPDLOCK,HOLDLOCK)
           WHERE cCorreo=N'keyove.demo@example.test')
    THROW 51000, 'El correo reservado keyove.demo@example.test ya existe. No se insertó el lote.', 1;
INSERT dbo.Personas(cNombres,cApellidos,cGenero,cCorreo,cTelefono,dFechaNacimiento)
VALUES(N'Usuario',N'DEMO KEYOVE','M',N'keyove.demo@example.test',NULL,'19900101');
SET @personaDemo=CONVERT(int,SCOPE_IDENTITY());
-- Contraseña inicial de prueba: KeyoveDemo2026!
-- SHA-256 hexadecimal en minúsculas, compatible con el login del proyecto revisado.
INSERT dbo.Usuarios(iCodPersona,iCodRol,cNombreUsuario,cContrasenaHash,bEstado)
VALUES(@personaDemo,@rolDemo,N'keyove_demo',
       LOWER(CONVERT(varchar(64),HASHBYTES('SHA2_256',CAST('KeyoveDemo2026!' AS varchar(100))),2)),1);
SET @duena=CONVERT(int,SCOPE_IDENTITY());
DECLARE @categoria int;

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Combos USB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Combos USB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-001',N'TECLADO LOGITECH + MOUSE MK120 USB SP BLACK (PN 920-004428)','Producto de prueba','DEMO',7.50,12.50,4);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Combos inalámbricos';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Combos inalámbricos','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-002',N'TECLADO LOGITECH + MOUSE MK235 WIRELESS USB SP BLACK (PN 920-007','Producto de prueba','DEMO',10.00,15.00,4);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Combos Bluetooth';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Combos Bluetooth','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-003',N'TECLADO LOGITECH + MOUSE MK250 BLUETOOTH SP BLACK (920-013513)','Producto de prueba','DEMO',12.50,17.50,4);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Combos Genius';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Combos Genius','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-004',N'TECLADO GENIUS + MOUSE DUAL WIRELESS/BT SLIMSTAR 8230 AI COPILOT','Producto de prueba','DEMO',15.00,20.00,8);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Micro SD 32 GB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Micro SD 32 GB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-005',N'Memoria micro SD 32 GB','Producto de prueba','DEMO',17.50,22.50,8);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Micro SD 64 GB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Micro SD 64 GB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-006',N'Memoria micro SD 64 GB','Producto de prueba','DEMO',20.00,25.00,8);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Micro SD 128 GB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Micro SD 128 GB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-007',N'Memoria micro SD 128 GB','Producto de prueba','DEMO',22.50,27.50,8);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Memorias USB 32 GB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Memorias USB 32 GB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-008',N'Memoria USB 32 GB','Producto de prueba','DEMO',25.00,30.00,12);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Memorias USB 64 GB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Memorias USB 64 GB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-009',N'Memoria USB 64 GB','Producto de prueba','DEMO',27.50,32.50,12);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Teclados USB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Teclados USB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-010',N'Teclado USB multimedia','Producto de prueba','DEMO',30.00,35.00,12);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Teclados mecánicos';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Teclados mecánicos','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-011',N'Teclado mecánico RGB','Producto de prueba','DEMO',32.50,37.50,12);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Mouse USB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Mouse USB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-012',N'Mouse óptico USB','Producto de prueba','DEMO',35.00,40.00,16);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Mouse inalámbrico';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Mouse inalámbrico','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-013',N'Mouse inalámbrico 2.4 GHz','Producto de prueba','DEMO',37.50,42.50,16);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Smartwatch';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Smartwatch','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-014',N'Smartwatch deportivo Bluetooth','Producto de prueba','DEMO',40.00,45.00,16);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'TV Box';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'TV Box','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-015',N'TV Box Android HDMI','Producto de prueba','DEMO',42.50,47.50,16);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Streaming TV';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Streaming TV','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-016',N'Adaptador streaming TV HDMI','Producto de prueba','DEMO',45.00,50.00,20);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Cables HDMI';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Cables HDMI','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-017',N'Cable HDMI 2 metros','Producto de prueba','DEMO',47.50,52.50,20);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Cables USB C';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Cables USB C','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-018',N'Cable USB tipo C','Producto de prueba','DEMO',50.00,55.00,20);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Cables micro USB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Cables micro USB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-019',N'Cable micro USB','Producto de prueba','DEMO',52.50,57.50,20);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Cables Lightning';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Cables Lightning','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-020',N'Cable Lightning USB','Producto de prueba','DEMO',55.00,60.00,4);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Cables de red';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Cables de red','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-021',N'Cable de red CAT6 3 metros','Producto de prueba','DEMO',57.50,62.50,4);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Tomacorrientes';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Tomacorrientes','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-022',N'Tomacorriente doble','Producto de prueba','DEMO',60.00,65.00,4);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Supresores de picos';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Supresores de picos','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-023',N'Supresor de picos 6 tomas','Producto de prueba','DEMO',62.50,67.50,4);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Mandos para PC';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Mandos para PC','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-024',N'Mando USB para PC','Producto de prueba','DEMO',65.00,70.00,8);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Mandos Bluetooth';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Mandos Bluetooth','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-025',N'Mando Bluetooth para PC','Producto de prueba','DEMO',67.50,72.50,8);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Cargadores USB C';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Cargadores USB C','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-026',N'Cargador USB C 20 W','Producto de prueba','DEMO',70.00,75.00,8);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Cargadores USB';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Cargadores USB','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-027',N'Cargador USB doble puerto','Producto de prueba','DEMO',72.50,77.50,8);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Cargadores de laptop';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Cargadores de laptop','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-028',N'Cargador universal para laptop','Producto de prueba','DEMO',75.00,80.00,12);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Audífonos';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Audífonos','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-029',N'Audífonos con micrófono USB','Producto de prueba','DEMO',77.50,82.50,12);

SET @categoria=NULL;
SELECT @categoria=iCodCategoria FROM dbo.Categorias WITH(UPDLOCK,HOLDLOCK) WHERE cNombre=N'Adaptadores y hubs';
IF @categoria IS NULL
BEGIN
 INSERT Categorias(cNombre,cDescripcion) VALUES(N'Adaptadores y hubs','Categoría ficticia de accesorios electrónicos');
 SET @categoria=CONVERT(int,SCOPE_IDENTITY());
END;
INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,nPrecioCompra,nPrecioVenta,iStockActual)
VALUES(@categoria,'DEMO-P-030',N'Hub USB de 4 puertos','Producto de prueba','DEMO',80.00,85.00,12);

IF (SELECT COUNT(*) FROM dbo.Productos WHERE cCodigo LIKE 'DEMO-P-%')<>30
 THROW 51001, 'La carga de productos no quedó completa.', 1;
COMMIT TRANSACTION;
PRINT 'Carga completada: 1 persona, 1 usuario, 30 productos y las categorías faltantes (hasta 30).';
PRINT 'Usuario de aplicación: keyove_demo. Cambia su contraseña después de las pruebas.';
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
GO
