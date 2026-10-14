-- Dependencias que usa el registro normal de ventas. No incluye OCR.
USE DB_keyove_inventario;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
IF TYPE_ID(N'dbo.TipoDetalleVenta') IS NULL
    EXEC(N'CREATE TYPE dbo.TipoDetalleVenta AS TABLE(iCodProducto int, iCantidad int, nPrecioVenta decimal(18,2), nDescuento decimal(18,2))');
GO
CREATE OR ALTER PROCEDURE dbo.sp_RegistrarVenta
    @iCodUsuario int, @cTipoComprobante nvarchar(30), @cDocumentoCliente nvarchar(11),
    @cNumeroCelular varchar(9), @cNumeroComprobante nvarchar(50), @cMetodoPago nvarchar(50),
    @cObservacion nvarchar(300), @Detalles dbo.TipoDetalleVenta READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NOT EXISTS(SELECT 1 FROM @Detalles) THROW 50001, 'Agrega productos a la venta.', 1;
    IF EXISTS(SELECT 1 FROM @Detalles WHERE iCodProducto IS NULL OR iCantidad IS NULL OR iCantidad<=0 OR nPrecioVenta IS NULL OR nPrecioVenta<0 OR nDescuento IS NULL OR nDescuento<0 OR nDescuento>iCantidad*nPrecioVenta)
        THROW 50001, 'Revisa cantidades, precios y descuentos.', 1;
    IF EXISTS(SELECT iCodProducto FROM @Detalles GROUP BY iCodProducto HAVING COUNT(*)>1)
        THROW 50001, 'Agrupa los productos repetidos.', 1;
    IF @cTipoComprobante NOT IN('BOLETA','FACTURA') THROW 50001, 'Tipo de comprobante inválido.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS(SELECT 1 FROM Usuarios WHERE iCodUsuario=@iCodUsuario AND bEstado=1)
            THROW 50001, 'El usuario no está activo.', 1;
        IF EXISTS(SELECT 1 FROM @Detalles d LEFT JOIN Productos p WITH(UPDLOCK,HOLDLOCK) ON p.iCodProducto=d.iCodProducto WHERE p.iCodProducto IS NULL OR p.bEstado=0 OR p.iStockActual<d.iCantidad)
            THROW 50001, 'Producto no disponible o stock insuficiente.', 1;
        DECLARE @prefijo nvarchar(3)=CASE @cTipoComprobante WHEN 'BOLETA' THEN 'BO-' ELSE 'FC-' END, @ultimo bigint;
        SELECT @ultimo=COALESCE(MAX(TRY_CONVERT(bigint,SUBSTRING(cNumeroComprobante,4,47))),2899999)
        FROM Ventas WITH(TABLOCKX,HOLDLOCK) WHERE LEFT(cNumeroComprobante,3)=@prefijo;
        IF @ultimo<2899999 SET @ultimo=2899999;
        SET @cNumeroComprobante=@prefijo+CONVERT(nvarchar(47),@ultimo+1);
        DECLARE @subtotal decimal(18,2)=(SELECT SUM(iCantidad*nPrecioVenta-nDescuento) FROM @Detalles);
        DECLARE @igv decimal(18,2)=ROUND(@subtotal*0.18,2);
        INSERT Ventas(iCodUsuario,cTipoComprobante,cDocumentoCliente,cNumeroCelular,cNumeroComprobante,nSubTotal,nIgv,nTotal,cMetodoPago,cObservacion)
        VALUES(@iCodUsuario,@cTipoComprobante,NULLIF(@cDocumentoCliente,''),NULLIF(@cNumeroCelular,''),@cNumeroComprobante,@subtotal,@igv,@subtotal+@igv,@cMetodoPago,@cObservacion);
        DECLARE @id int=CONVERT(int,SCOPE_IDENTITY());
        INSERT Detalle_Venta(iCodVenta,iCodProducto,iCantidad,nPrecioVenta,nDescuento)
        SELECT @id,iCodProducto,iCantidad,nPrecioVenta,nDescuento FROM @Detalles;
        DECLARE @stocks TABLE(producto int, anterior int, nuevo int);
        UPDATE p SET iStockActual=p.iStockActual-d.iCantidad
        OUTPUT inserted.iCodProducto,deleted.iStockActual,inserted.iStockActual INTO @stocks
        FROM Productos p JOIN @Detalles d ON d.iCodProducto=p.iCodProducto;
        INSERT Movimientos_Inventario(iCodProducto,iCodUsuario,iCodVenta,cTipoMovimiento,iCantidad,iStockAnterior,iStockNuevo,cMotivo)
        SELECT producto,@iCodUsuario,@id,'SALIDA',anterior-nuevo,anterior,nuevo,'Registro de venta' FROM @stocks;
        COMMIT;
        SELECT @id AS iCodVenta;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_AnularVenta @id int, @usuario int
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @estado nvarchar(20);
        SELECT @estado=cEstado FROM Ventas WITH(UPDLOCK,HOLDLOCK) WHERE iCodVenta=@id;
        IF @estado IS NULL THROW 50001, 'La venta no existe.', 1;
        IF @estado='ANULADA' BEGIN COMMIT; RETURN; END;
        DECLARE @stocks TABLE(producto int, anterior int, nuevo int);
        UPDATE p SET iStockActual=p.iStockActual+m.cantidad
        OUTPUT inserted.iCodProducto,deleted.iStockActual,inserted.iStockActual INTO @stocks
        FROM Productos p JOIN (SELECT iCodProducto,SUM(iCantidad) AS cantidad FROM Movimientos_Inventario WHERE iCodVenta=@id AND cTipoMovimiento='SALIDA' GROUP BY iCodProducto) m ON m.iCodProducto=p.iCodProducto;
        INSERT Movimientos_Inventario(iCodProducto,iCodUsuario,cTipoMovimiento,iCantidad,iStockAnterior,iStockNuevo,cMotivo)
        SELECT producto,@usuario,'AJUSTE_ENTRADA',nuevo-anterior,anterior,nuevo,CONCAT('Anulación de venta ',@id) FROM @stocks;
        UPDATE Ventas SET cEstado='ANULADA' WHERE iCodVenta=@id;
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
