CREATE TABLE Movimientos_Inventario (
    iCodMovimiento INT IDENTITY(1,1) Not NULL,
    iCodProducto INT Not NULL,
    iCodUsuario INT Not NULL,
    iCodCompra INT NULL,
    iCodVenta INT NULL,
    cTipoMovimiento NVARCHAR(30) Not NULL,
    iCantidad INT Not NULL,
    iStockAnterior INT Not NULL,
    iStockNuevo INT Not NULL,
    cMotivo NVARCHAR(500) NULL,
    dFechaMovimiento DATETIME2 Not NULL
        Default SYSDATETIME(),

    CONSTRAINT PK_Movimientos_Inventario
        PRIMARY KEY(iCodMovimiento),

    CONSTRAINT FK_Movimientos_Productos
        FOREIGN KEY(iCodProducto)
        REFERENCES Productos(iCodProducto),

    CONSTRAINT FK_Movimientos_Usuarios
        FOREIGN KEY(iCodUsuario)
        REFERENCES Usuarios(iCodUsuario),

    CONSTRAINT FK_Movimientos_Compras
        FOREIGN KEY(iCodCompra)
        REFERENCES Compras(iCodCompra),

    CONSTRAINT FK_Movimientos_Ventas
        FOREIGN KEY(iCodVenta)
        REFERENCES Ventas(iCodVenta),

    CONSTRAINT CK_Movimientos_Tipo
CHECK(
    cTipoMovimiento IN (
                'ENTRADA',
                'SALIDA',
                'AJUSTE_ENTRADA',
                'AJUSTE_SALIDA'
            )
        ),

    CONSTRAINT CK_Movimientos_Cantidad
CHECK(iCantidad > 0),

    CONSTRAINT CK_Movimientos_Stock
CHECK(
    iStockAnterior >= 0
            And iStockNuevo >= 0
        ),

    CONSTRAINT CK_Movimientos_Origen
CHECK(
    (
        cTipoMovimiento = 'ENTRADA'
                And iCodCompra Is Not NULL
                And iCodVenta Is NULL
            )
            Or
            (
                cTipoMovimiento = 'SALIDA'
                And iCodVenta Is Not NULL
                And iCodCompra Is NULL
            )
            Or
            (
                cTipoMovimiento = 'AJUSTE_ENTRADA'
                And iCodCompra Is NULL
                And iCodVenta Is NULL
            )
            Or
            (
                cTipoMovimiento = 'AJUSTE_SALIDA'
                And iCodCompra Is NULL
                And iCodVenta Is NULL
            )
        )
);
GO