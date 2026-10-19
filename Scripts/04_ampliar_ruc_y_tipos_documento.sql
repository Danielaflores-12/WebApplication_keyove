-- Corrección compatible con bases creadas con una versión anterior del sistema.
-- Amplía el RUC sin alterar proveedores existentes y actualiza la validación del registro de compras.
USE DB_keyove_inventario;
GO

IF COL_LENGTH('dbo.Proveedores', 'cRuc') < 11
    ALTER TABLE dbo.Proveedores ALTER COLUMN cRuc VARCHAR(11) NOT NULL;
GO

IF OBJECT_ID('dbo.CK_Proveedores_Ruc', 'C') IS NOT NULL
    ALTER TABLE dbo.Proveedores DROP CONSTRAINT CK_Proveedores_Ruc;
GO

ALTER TABLE dbo.Proveedores
    ADD CONSTRAINT CK_Proveedores_Ruc
    CHECK (LEN(cRuc) IN (10, 11) AND cRuc NOT LIKE '%[^0-9]%');
GO

-- Ejecuta después Scripts/03_registrar_compra_transaccion.sql
-- para aplicar los tipos BOLETA, FACTURA, PEDIDO, TICKET, NOTA_VENTA y OTRO.
