Imports System.Data
Imports System.Data.SqlClient
Imports WebApplication_Keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Productos

        Public iCodProducto As Integer
        Public iCodCategoria As Integer
        Public cNombreCategoria As String
        Public cCodigo As String
        Public cNombre As String
        Public cDescripcion As String
        Public cMarca As String
        Public cModelo As String
        Public nPrecioCompra As Decimal
        Public nPrecioVenta As Decimal
        Public iStockActual As Integer
        Public iStockMinimo As Integer = 5
        Public cUnidadMedida As String = "UNIDAD"
        Public bEstado As Boolean = True
        Public dFechaRegistro As DateTime

        Public qSelect As String
        Public db As New ConexionBD()

        '==================================================
        ' CONVERTIR CAMPOS VACÍOS EN NULL
        '==================================================

        Private Function ValorONull(valor As String) As Object

            If String.IsNullOrWhiteSpace(valor) Then
                Return DBNull.Value
            End If

            Return valor.Trim()

        End Function

        '==================================================
        ' CREAR PARÁMETRO DECIMAL
        '==================================================

        Private Function ParametroDecimal(
            nombre As String,
            valor As Decimal
        ) As SqlParameter

            Dim parametro As New SqlParameter(
                nombre,
                SqlDbType.Decimal
            )

            parametro.Precision = 18
            parametro.Scale = 2
            parametro.Value = valor

            Return parametro

        End Function

        '==================================================
        ' CREAR PARÁMETROS DEL PRODUCTO
        '==================================================

        Private Function CrearParametros(
            incluirCodigoProducto As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodCategoria",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodCategoria
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@cCodigo",
                    SqlDbType.NVarChar,
                    50
                ) With {
                    .Value = Me.cCodigo.Trim()
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@cNombre",
                    SqlDbType.NVarChar,
                    100
                ) With {
                    .Value = Me.cNombre.Trim()
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@cDescripcion",
                    SqlDbType.NVarChar,
                    500
                ) With {
                    .Value = ValorONull(Me.cDescripcion)
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@cMarca",
                    SqlDbType.NVarChar,
                    100
                ) With {
                    .Value = ValorONull(Me.cMarca)
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@cModelo",
                    SqlDbType.NVarChar,
                    100
                ) With {
                    .Value = ValorONull(Me.cModelo)
                }
            )

            parametros.Add(
                ParametroDecimal(
                    "@nPrecioCompra",
                    Me.nPrecioCompra
                )
            )

            parametros.Add(
                ParametroDecimal(
                    "@nPrecioVenta",
                    Me.nPrecioVenta
                )
            )

            parametros.Add(
                New SqlParameter(
                    "@iStockActual",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iStockActual
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@iStockMinimo",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iStockMinimo
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@cUnidadMedida",
                    SqlDbType.NVarChar,
                    30
                ) With {
                    .Value =
                        If(
                            String.IsNullOrWhiteSpace(Me.cUnidadMedida),
                            "UNIDAD",
                            Me.cUnidadMedida.Trim()
                        )
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@bEstado",
                    SqlDbType.Bit
                ) With {
                    .Value = Me.bEstado
                }
            )

            If incluirCodigoProducto Then

                parametros.Add(
                    New SqlParameter(
                        "@iCodProducto",
                        SqlDbType.Int
                    ) With {
                        .Value = Me.iCodProducto
                    }
                )

            End If

            Return parametros

        End Function

        '==================================================
        ' CONSULTA PARA MODIFICAR PRODUCTO
        '==================================================

        Private Function ConsultaModificar() As String

            Dim Query As String =
                "UPDATE Productos SET " &
                "iCodCategoria = @iCodCategoria, " &
                "cCodigo = @cCodigo, " &
                "cNombre = @cNombre, " &
                "cDescripcion = @cDescripcion, " &
                "cMarca = @cMarca, " &
                "cModelo = @cModelo, " &
                "nPrecioCompra = @nPrecioCompra, " &
                "nPrecioVenta = @nPrecioVenta, " &
                "iStockActual = @iStockActual, " &
                "iStockMinimo = @iStockMinimo, " &
                "cUnidadMedida = @cUnidadMedida, " &
                "bEstado = @bEstado " &
                "WHERE iCodProducto = @iCodProducto"

            Return Query

        End Function

        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function

        '==================================================
        ' LISTAR TODOS LOS PRODUCTOS
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "p.iCodProducto, " &
                "p.iCodCategoria, " &
                "c.cNombre AS cNombreCategoria, " &
                "p.cCodigo, " &
                "p.cNombre, " &
                "p.cDescripcion, " &
                "p.cMarca, " &
                "p.cModelo, " &
                "p.nPrecioCompra, " &
                "p.nPrecioVenta, " &
                "p.iStockActual, " &
                "p.iStockMinimo, " &
                "p.cUnidadMedida, " &
                "p.bEstado, " &
                "p.dFechaRegistro " &
                "FROM Productos p " &
                "INNER JOIN Categorias c " &
                "ON p.iCodCategoria = c.iCodCategoria " &
                "ORDER BY p.iCodProducto DESC"

            Return db.ExecuteDataTable(Query)

        End Function

        '==================================================
        ' INSERTAR PRODUCTO
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Productos (" &
                "iCodCategoria, " &
                "cCodigo, " &
                "cNombre, " &
                "cDescripcion, " &
                "cMarca, " &
                "cModelo, " &
                "nPrecioCompra, " &
                "nPrecioVenta, " &
                "iStockActual, " &
                "iStockMinimo, " &
                "cUnidadMedida, " &
                "bEstado" &
                ") VALUES (" &
                "@iCodCategoria, " &
                "@cCodigo, " &
                "@cNombre, " &
                "@cDescripcion, " &
                "@cMarca, " &
                "@cModelo, " &
                "@nPrecioCompra, " &
                "@nPrecioVenta, " &
                "@iStockActual, " &
                "@iStockMinimo, " &
                "@cUnidadMedida, " &
                "@bEstado" &
                "); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodProducto =
                Convert.ToInt32(
                    db.ExecuteScalar(
                        Query,
                        parametros
                    )
                )

        End Sub

        '==================================================
        ' MODIFICAR PRODUCTO
        '==================================================

        Public Sub Modificar()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQuery(
                Query,
                parametros
            )

        End Sub

        '==================================================
        ' MODIFICAR PRODUCTO EN TRANSACCIÓN
        '==================================================

        Public Sub ModificarTransact()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQueryTransact(
                Query,
                parametros
            )

        End Sub

        '==================================================
        ' DESACTIVAR PRODUCTO
        '==================================================

        Public Sub Eliminar()

            Dim Query As String =
                "UPDATE Productos " &
                "SET bEstado = 0 " &
                "WHERE iCodProducto = @iCodProducto"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodProducto",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodProducto
                }
            )

            db.ExecuteQuery(
                Query,
                parametros
            )

        End Sub

        '==================================================
        ' DESACTIVAR PRODUCTO EN TRANSACCIÓN
        '==================================================

        Public Sub EliminarTransact()

            Dim Query As String =
                "UPDATE Productos " &
                "SET bEstado = 0 " &
                "WHERE iCodProducto = @iCodProducto"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodProducto",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodProducto
                }
            )

            db.ExecuteQueryTransact(
                Query,
                parametros
            )

        End Sub

        '==================================================
        ' OBTENER UN PRODUCTO POR SU CÓDIGO
        '==================================================

        Public Sub getRecord()

            Dim Query As String =
                "SELECT " &
                "iCodProducto, " &
                "iCodCategoria, " &
                "cCodigo, " &
                "cNombre, " &
                "cDescripcion, " &
                "cMarca, " &
                "cModelo, " &
                "nPrecioCompra, " &
                "nPrecioVenta, " &
                "iStockActual, " &
                "iStockMinimo, " &
                "cUnidadMedida, " &
                "bEstado, " &
                "dFechaRegistro " &
                "FROM Productos " &
                "WHERE iCodProducto = @iCodProducto"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodProducto",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodProducto
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(
                    Query,
                    parametros
                )

            Try

                If readers.Read() Then

                    Me.iCodProducto =
                        Convert.ToInt32(
                            readers("iCodProducto")
                        )

                    Me.iCodCategoria =
                        Convert.ToInt32(
                            readers("iCodCategoria")
                        )

                    Me.cCodigo =
                        Convert.ToString(
                            readers("cCodigo")
                        )

                    Me.cNombre =
                        Convert.ToString(
                            readers("cNombre")
                        )

                    Me.cDescripcion =
                        Convert.ToString(
                            readers("cDescripcion")
                        )

                    Me.cMarca =
                        Convert.ToString(
                            readers("cMarca")
                        )

                    Me.cModelo =
                        Convert.ToString(
                            readers("cModelo")
                        )

                    Me.nPrecioCompra =
                        Convert.ToDecimal(
                            readers("nPrecioCompra")
                        )

                    Me.nPrecioVenta =
                        Convert.ToDecimal(
                            readers("nPrecioVenta")
                        )

                    Me.iStockActual =
                        Convert.ToInt32(
                            readers("iStockActual")
                        )

                    Me.iStockMinimo =
                        Convert.ToInt32(
                            readers("iStockMinimo")
                        )

                    Me.cUnidadMedida =
                        Convert.ToString(
                            readers("cUnidadMedida")
                        )

                    Me.bEstado =
                        Convert.ToBoolean(
                            readers("bEstado")
                        )

                    Me.dFechaRegistro =
                        Convert.ToDateTime(
                            readers("dFechaRegistro")
                        )

                End If

            Finally

                readers.Close()

            End Try

        End Sub

        '==================================================
        ' LISTAR PRODUCTOS ACTIVOS PARA COMBOBOX
        '==================================================

        Public Function ListaDatosCombo() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Dim Query As String =
                "SELECT " &
                "iCodProducto, " &
                "cCodigo, " &
                "cNombre " &
                "FROM Productos " &
                "WHERE bEstado = 1 " &
                "ORDER BY cNombre"

            Dim readers As IDataReader =
                db.ExecuteReader(Query)

            Try

                While readers.Read()

                    Dim nombreProducto As String =
                        Convert.ToString(
                            readers("cCodigo")
                        ) &
                        " - " &
                        Convert.ToString(
                            readers("cNombre")
                        )

                    miDataTable.Rows.Add(
                        Convert.ToString(
                            readers("iCodProducto")
                        ),
                        nombreProducto
                    )

                End While

            Finally

                readers.Close()

            End Try

            Return miDataTable

        End Function

    End Class

End Namespace