--==================================================
-- KEYOVE - SCRIPT 01: TABLA COMPROBANTES_OCR
-- Almacena el archivo del comprobante (imagen/PDF)
-- junto con los datos extraidos por OCR y el vinculo
-- con la compra o la venta registrada.
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
        -- Si el archivo supera el limite de 2 MB solo se
        -- guardan los metadatos y el texto OCR.
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

--==================================================
-- CONSULTAS DE VERIFICACION
--==================================================

-- 1) Verificar que la tabla existe y con que columnas
SELECT
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.CHARACTER_MAXIMUM_LENGTH,
    c.NUMERIC_PRECISION,
    c.NUMERIC_SCALE,
    c.IS_NULLABLE,
    c.COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'Comprobantes_OCR'
ORDER BY c.ORDINAL_POSITION;
GO

-- 2) Verificar restricciones (PK, CHECK, FK)
SELECT
    tc.CONSTRAINT_TYPE,
    tc.CONSTRAINT_NAME,
    kcu.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
    ON kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
WHERE tc.TABLE_NAME = 'Comprobantes_OCR'
ORDER BY tc.CONSTRAINT_TYPE, tc.CONSTRAINT_NAME;
GO

-- 3) Verificar indices
SELECT
    i.name AS Indice,
    i.type_desc AS Tipo,
    c.name AS Columna
FROM sys.indexes i
JOIN sys.index_columns ic
    ON ic.object_id = i.object_id AND ic.index_id = i.index_id
JOIN sys.columns c
    ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE i.object_id = OBJECT_ID('dbo.Comprobantes_OCR')
ORDER BY i.name, ic.key_ordinal;
GO

-- 4) Listar comprobantes almacenados (sin el binario)
SELECT
    iCodComprobante,
    iCodCompra,
    iCodVenta,
    cNombreArchivo,
    cTipoMime,
    nTamanoBytes,
    cRucEmisor,
    cNumeroComprobante,
    cTipoComprobante,
    cFechaEmision,
    nSubTotal,
    nIgv,
    nTotal,
    dFechaCarga,
    CASE
        WHEN imgComprobante IS NULL THEN 'SIN BINARIO'
        ELSE 'CON BINARIO'
    AS EstadoArchivo
FROM dbo.Comprobantes_OCR
ORDER BY iCodComprobante DESC;
GO

-- 5) Comprobantes asociados a una compra
SELECT
    co.iCodComprobante,
    co.cNombreArchivo,
    co.cNumeroComprobante,
    co.nTotal
FROM dbo.Comprobantes_OCR co
WHERE co.iCodCompra IS NOT NULL
ORDER BY co.iCodComprobante DESC;
GO

-- 6) Comprobantes asociados a una venta
SELECT
    co.iCodComprobante,
    co.cNombreArchivo,
    co.cNumeroComprobante,
    co.nTotal
FROM dbo.Comprobantes_OCR co
WHERE co.iCodVenta IS NOT NULL
ORDER BY co.iCodComprobante DESC;
GO