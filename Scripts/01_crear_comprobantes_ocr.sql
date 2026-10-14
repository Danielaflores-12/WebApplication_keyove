--==================================================
-- KEYOVE - SCRIPT 01: TABLA COMPROBANTES_OCR
-- Almacena el archivo del comprobante (imagen/PDF)
-- junto con los datos extraidos por OCR y el vinculo
-- con la compra registrada. iCodVenta se conserva por compatibilidad.
-- Idempotente: puede ejecutarse varias veces.
--==================================================

USE DB_keyove_inventario;
GO

--==================================================
-- CREAR TABLA COMPROBANTES_OCR
--==================================================

IF OBJECT_ID('dbo.Comprobantes_OCR', 'U') IS NULL
BEGIN

    CREATE TABLE dbo.Comprobantes_OCR
    (
        iCodComprobante       INT IDENTITY(1, 1) NOT NULL,

        -- Vinculos opcionales: el comprobante pertenece
        -- a una compra, a una venta, o a ninguna.
        iCodCompra            INT NULL,
        iCodVenta             INT NULL,

        -- Datos del archivo subido
        cNombreArchivo        NVARCHAR(255) NOT NULL,
        cTipoMime             NVARCHAR(100) NULL,
        nTamanoBytes          INT NULL,

        -- Datos extraidos por el OCR
        cRucEmisor            NVARCHAR(20) NULL,
        cNumeroComprobante    NVARCHAR(50) NULL,
        cTipoComprobante      NVARCHAR(30) NULL,
        cFechaEmision         DATE NULL,
        nSubTotal             DECIMAL(18, 2) NULL,
        nIgv                  DECIMAL(18, 2) NULL,
        nTotal                DECIMAL(18, 2) NULL,
        cTextoOcr             NVARCHAR(MAX) NULL,

        -- Archivo binario (imagen/PDF).
        -- Se conserva completo. La aplicación admite archivos de hasta 10 MB.
        imgComprobante        VARBINARY(MAX) NULL,

        dFechaCarga           DATETIME NOT NULL
            CONSTRAINT DF_Comprobantes_OCR_dFechaCarga
            DEFAULT GETDATE(),

        CONSTRAINT PK_Comprobantes_OCR
            PRIMARY KEY CLUSTERED (iCodComprobante),

        -- Al menos uno de los dos vinculos debe existir
        CONSTRAINT CK_Comprobantes_OCR_Vinculo
            CHECK (iCodCompra IS NOT NULL OR iCodVenta IS NOT NULL),

        CONSTRAINT FK_Comprobantes_OCR_Compras
            FOREIGN KEY (iCodCompra)
            REFERENCES dbo.Compras (iCodCompra)
            ON DELETE CASCADE,

        CONSTRAINT FK_Comprobantes_OCR_Ventas
            FOREIGN KEY (iCodVenta)
            REFERENCES dbo.Ventas (iCodVenta)
            ON DELETE CASCADE
    );

    PRINT 'Tabla Comprobantes_OCR creada.';

END
ELSE
BEGIN

    PRINT 'La tabla Comprobantes_OCR ya existe. No se realizo ningun cambio.';

END
GO

--==================================================
-- INDICES
--==================================================

IF OBJECT_ID('dbo.Comprobantes_OCR', 'U') IS NOT NULL
BEGIN

    IF NOT EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = 'IX_Comprobantes_OCR_iCodCompra'
          AND object_id = OBJECT_ID('dbo.Comprobantes_OCR')
    )
    BEGIN

        CREATE NONCLUSTERED INDEX IX_Comprobantes_OCR_iCodCompra
            ON dbo.Comprobantes_OCR (iCodCompra);

        PRINT 'Indice IX_Comprobantes_OCR_iCodCompra creado.';

    END

    IF NOT EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = 'IX_Comprobantes_OCR_iCodVenta'
          AND object_id = OBJECT_ID('dbo.Comprobantes_OCR')
    )
    BEGIN

        CREATE NONCLUSTERED INDEX IX_Comprobantes_OCR_iCodVenta
            ON dbo.Comprobantes_OCR (iCodVenta);

        PRINT 'Indice IX_Comprobantes_OCR_iCodVenta creado.';

    END

END
GO
