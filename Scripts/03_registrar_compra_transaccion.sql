-- Requiere las tablas del esquema base y Scripts/01_crear_comprobantes_ocr.sql.
-- Recibe todos los productos de una compra, no un producto por llamada.
USE DB_keyove_inventario;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
IF TYPE_ID(N'dbo.TipoDetalleCompra') IS NULL
    EXEC(N'CREATE TYPE dbo.TipoDetalleCompra AS TABLE (
        iCodProducto int NOT NULL,
        iCantidad int NOT NULL,
        nPrecioCompra decimal(18,2) NOT NULL
    )');
GO
CREATE OR ALTER PROCEDURE dbo.sp_RegistrarCompra
    @iCodProveedor int,
    @iCodUsuario int,
    @cTipoComprobante nvarchar(30),
    @cNumeroComprobante nvarchar(50),
    @dFechaCompra date,
    @nSubTotal decimal(18,2),
    @nIgv decimal(18,2),
    @nTotal decimal(18,2),
    @bPreciosIncluyenIgv bit,
    @Detalles dbo.TipoDetalleCompra READONLY,
    @cObservacion nvarchar(300) = NULL,
    @cNombreArchivo nvarchar(255) = NULL,
    @cTipoMime nvarchar(100) = NULL,
    @cTextoOcr nvarchar(max) = NULL,
    @cRucEmisor nvarchar(20) = NULL,
    @imgComprobante varbinary(max) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    -- Opciones necesarias para el índice filtrado de comprobantes activos.
    SET ANSI_PADDING ON;
    SET ANSI_WARNINGS ON;
    SET CONCAT_NULL_YIELDS_NULL ON;
    SET ARITHABORT ON;
    SET NUMERIC_ROUNDABORT OFF;

    BEGIN TRY
        BEGIN TRANSACTION;

        SET @cTipoComprobante = UPPER(LTRIM(RTRIM(@cTipoComprobante)));
        SET @cNumeroComprobante = UPPER(LTRIM(RTRIM(@cNumeroComprobante)));
        IF @cTipoComprobante IS NULL OR @cTipoComprobante NOT IN ('BOLETA','FACTURA','PEDIDO','TICKET','NOTA_VENTA','OTRO')
            THROW 50001, 'Selecciona un tipo de documento válido.', 1;
        IF NULLIF(@cNumeroComprobante,'') IS NULL OR @dFechaCompra IS NULL
            THROW 50001, 'Completa el número y la fecha del comprobante del proveedor.', 1;
        IF @nSubTotal IS NULL OR @nIgv IS NULL OR @nTotal IS NULL OR @bPreciosIncluyenIgv IS NULL
            OR @nSubTotal<0 OR @nIgv<0 OR @nTotal<=0 OR @nTotal<>@nSubTotal+@nIgv
            THROW 50001, 'Subtotal más IGV debe ser igual al total y los importes no pueden ser negativos.', 1;
        IF NOT EXISTS (SELECT 1 FROM @Detalles)
            THROW 50001, 'Agrega los productos de la compra.', 1;
        IF EXISTS (SELECT 1 FROM @Detalles WHERE iCodProducto<=0 OR iCantidad<=0 OR nPrecioCompra<0)
            THROW 50001, 'Revisa los productos, cantidades y precios.', 1;
        IF EXISTS (SELECT iCodProducto FROM @Detalles GROUP BY iCodProducto HAVING COUNT(*)>1)
            THROW 50001, 'Agrupa las cantidades de los productos repetidos.', 1;
        DECLARE @suma decimal(38,2) = (SELECT SUM(CONVERT(decimal(10,0),iCantidad)*nPrecioCompra) FROM @Detalles);
        IF ABS(@suma-CASE WHEN @bPreciosIncluyenIgv=1 THEN @nTotal ELSE @nSubTotal END)>0.02
            THROW 50001, 'La suma de los productos no coincide con el comprobante.', 1;
        IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores WITH(HOLDLOCK) WHERE iCodProveedor=@iCodProveedor AND bEstado=1)
            THROW 50001, 'El proveedor no está activo.', 1;
        IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios u WITH(HOLDLOCK) JOIN dbo.Roles r ON r.iCodRol=u.iCodRol
                       WHERE u.iCodUsuario=@iCodUsuario AND u.bEstado=1 AND r.bEstado=1 AND r.cNombre='Administrador')
            THROW 50001, 'La compra requiere un usuario administrador activo.', 1;
        IF @imgComprobante IS NOT NULL AND
            (DATALENGTH(@imgComprobante)=0 OR DATALENGTH(@imgComprobante)>10485760 OR NULLIF(LTRIM(RTRIM(@cNombreArchivo)),'') IS NULL)
            THROW 50001, 'El comprobante debe tener nombre y un tamaño entre 1 byte y 10 MB.', 1;
        IF @imgComprobante IS NULL AND (@cNombreArchivo IS NOT NULL OR @cTextoOcr IS NOT NULL)
            THROW 50001, 'Falta el archivo del comprobante.', 1;

        INSERT dbo.Compras(iCodProveedor,iCodUsuario,cTipoComprobante,cNumeroComprobante,dFechaCompra,nSubTotal,nIgv,nTotal,cObservacion,cEstado)
        VALUES(@iCodProveedor,@iCodUsuario,@cTipoComprobante,@cNumeroComprobante,@dFechaCompra,@nSubTotal,@nIgv,@nTotal,@cObservacion,'REGISTRADA');
        DECLARE @iCodCompra int=CONVERT(int,SCOPE_IDENTITY());

        INSERT dbo.Detalle_Compra(iCodCompra,iCodProducto,iCantidad,nPrecioCompra)
        SELECT @iCodCompra,iCodProducto,iCantidad,nPrecioCompra FROM @Detalles;

        -- Se bloquean los productos en orden para serializar sus cambios de stock.
        DECLARE @producto int, @cantidad int, @precio decimal(18,2), @anterior int;
        DECLARE productos CURSOR LOCAL FAST_FORWARD FOR
            SELECT iCodProducto,iCantidad,nPrecioCompra FROM @Detalles ORDER BY iCodProducto;
        OPEN productos;
        FETCH NEXT FROM productos INTO @producto,@cantidad,@precio;
        WHILE @@FETCH_STATUS=0
        BEGIN
            SET @anterior=NULL;
            SELECT @anterior=iStockActual FROM dbo.Productos WITH(UPDLOCK,HOLDLOCK)
            WHERE iCodProducto=@producto AND bEstado=1;
            IF @anterior IS NULL THROW 50001, 'El producto no existe o no está activo.', 1;
            UPDATE dbo.Productos SET iStockActual=iStockActual+@cantidad,nPrecioCompra=@precio
            WHERE iCodProducto=@producto;
            INSERT dbo.Movimientos_Inventario(iCodProducto,iCodUsuario,iCodCompra,cTipoMovimiento,iCantidad,iStockAnterior,iStockNuevo,cMotivo)
            VALUES(@producto,@iCodUsuario,@iCodCompra,'ENTRADA',@cantidad,@anterior,@anterior+@cantidad,'Registro de compra');
            FETCH NEXT FROM productos INTO @producto,@cantidad,@precio;
        END;
        CLOSE productos;
        DEALLOCATE productos;

        IF @imgComprobante IS NOT NULL
            INSERT dbo.Comprobantes_OCR(iCodCompra,iCodVenta,cNombreArchivo,cTipoMime,nTamanoBytes,cRucEmisor,cNumeroComprobante,cTipoComprobante,cFechaEmision,nSubTotal,nIgv,nTotal,cTextoOcr,imgComprobante)
            VALUES(@iCodCompra,NULL,@cNombreArchivo,@cTipoMime,DATALENGTH(@imgComprobante),@cRucEmisor,@cNumeroComprobante,@cTipoComprobante,@dFechaCompra,@nSubTotal,@nIgv,@nTotal,@cTextoOcr,@imgComprobante);

        COMMIT TRANSACTION;
        SELECT @iCodCompra AS iCodCompra;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO
