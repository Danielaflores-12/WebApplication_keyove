-- Permite corregir una compra anulada conservando el mismo comprobante.
-- Sólo puede existir una compra REGISTRADA por proveedor/tipo/número.
USE DB_keyove_inventario;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Compras') AND name='UQ_Compras_Comprobante' AND has_filter=0)
    DROP INDEX UQ_Compras_Comprobante ON dbo.Compras;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Compras') AND name='UQ_Compras_Comprobante')
    CREATE UNIQUE INDEX UQ_Compras_Comprobante
    ON dbo.Compras(iCodProveedor,cTipoComprobante,cNumeroComprobante)
    WHERE cEstado='REGISTRADA';
COMMIT;
GO
