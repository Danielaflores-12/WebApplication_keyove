Imports System.Data.SqlClient
Imports WebApplication_keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Movimientos_Inventario

        '==================================================
        ' VARIABLES
        '==================================================

        Public iCodMovimiento As Integer
        Public iCodProducto As Integer
        Public cNombreProducto As String
        Public iCodUsuario As Integer
        Public cNombreUsuario As String

        Public iCodCompra As Integer?
        Public iCodVenta As Integer?

        Public cTipoMovimiento As String
        Public iCantidad As Integer
        Public iStockAnterior As Integer
        Public iStockNuevo As Integer
        Public cMotivo As String

        Public dFechaMovimiento As DateTime

        Public qSelect As String
        Public db As New ConexionBD()


        '==================================================
        ' CONVERTIR TEXTO VACÍO EN NULL
        '==================================================

        Private Function ValorONull(valor As String) As Object

            If String.IsNullOrWhiteSpace(valor) Then
                Return DBNull.Value
            End If

            Return valor.Trim()

        End Function


        '==================================================
        ' CONVERTIR INTEGER NULLABLE EN NULL SQL
        '==================================================

        Private Function EnteroONull(valor As Integer?) As Object

            If valor.HasValue Then
                Return valor.Value
            End If

            Return DBNull.Value

        End Function


        '==================================================
        ' CREAR PARÁMETROS DEL MOVIMIENTO
        '==================================================

        Private Function CrearParametros(
            incluirId As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodProducto", SqlDbType.Int) With {
                    .Value = Me.iCodProducto
                }
            )

            parametros.Add(
                New SqlParameter("@iCodUsuario", SqlDbType.Int) With {
                    .Value = Me.iCodUsuario
                }
            )

            parametros.Add(
                New SqlParameter("@iCodCompra", SqlDbType.Int) With {
                    .Value = EnteroONull(Me.iCodCompra)
                }
            )

            parametros.Add(
                New SqlParameter("@iCodVenta", SqlDbType.Int) With {
                    .Value = EnteroONull(Me.iCodVenta)
                }
            )

            parametros.Add(
                New SqlParameter("@cTipoMovimiento", SqlDbType.NVarChar, 30) With {
                    .Value = Me.cTipoMovimiento.Trim()
                }
            )

            parametros.Add(
                New SqlParameter("@iCantidad", SqlDbType.Int) With {
                    .Value = Me.iCantidad
                }
            )

            parametros.Add(
                New SqlParameter("@iStockAnterior", SqlDbType.Int) With {
                    .Value = Me.iStockAnterior
                }
            )

            parametros.Add(
                New SqlParameter("@iStockNuevo", SqlDbType.Int) With {
                    .Value = Me.iStockNuevo
                }
            )

            parametros.Add(
                New SqlParameter("@cMotivo", SqlDbType.NVarChar, 500) With {
                    .Value = ValorONull(Me.cMotivo)
                }
            )

            If incluirId Then

                parametros.Add(
                    New SqlParameter("@iCodMovimiento", SqlDbType.Int) With {
                        .Value = Me.iCodMovimiento
                    }
                )

            End If

            Return parametros

        End Function


        '==================================================
        ' CONSULTA PARA MODIFICAR
        '==================================================

        Private Function ConsultaModificar() As String

            Dim Query As String =
                "UPDATE Movimientos_Inventario SET " &
                "iCodProducto = @iCodProducto, " &
                "iCodUsuario = @iCodUsuario, " &
                "iCodCompra = @iCodCompra, " &
                "iCodVenta = @iCodVenta, " &
                "cTipoMovimiento = @cTipoMovimiento, " &
                "iCantidad = @iCantidad, " &
                "iStockAnterior = @iStockAnterior, " &
                "iStockNuevo = @iStockNuevo, " &
                "cMotivo = @cMotivo " &
                "WHERE iCodMovimiento = @iCodMovimiento"

            Return Query

        End Function


        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function


        '==================================================
        ' LISTAR TODOS LOS MOVIMIENTOS
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "M.iCodMovimiento, " &
                "M.iCodProducto, " &
                "M.iCodUsuario, " &
                "M.iCodCompra, " &
                "M.iCodVenta, " &
                "M.cTipoMovimiento, " &
                "M.iCantidad, " &
                "M.iStockAnterior, " &
                "M.iStockNuevo, " &
                "M.cMotivo, " &
                "M.dFechaMovimiento " &
                "FROM Movimientos_Inventario M " &
                "ORDER BY M.iCodMovimiento DESC"

            Return db.ExecuteDataTable(Query)

        End Function


        '==================================================
        ' LISTAR MOVIMIENTOS CON PRODUCTO Y USUARIO
        '==================================================

        Public Function ListaDatosDetalle() As DataTable

            Dim Query As String =
                "SELECT " &
                "M.iCodMovimiento, " &
                "M.iCodProducto, " &
                "P.cNombre AS Producto, " &
                "M.iCodUsuario, " &
                "U.cNombreUsuario AS Usuario, " &
                "M.iCodCompra, " &
                "M.iCodVenta, " &
                "M.cTipoMovimiento, " &
                "M.iCantidad, " &
                "M.iStockAnterior, " &
                "M.iStockNuevo, " &
                "M.cMotivo, " &
                "M.dFechaMovimiento " &
                "FROM Movimientos_Inventario M " &
                "INNER JOIN Productos P " &
                "ON M.iCodProducto = P.iCodProducto " &
                "INNER JOIN Usuarios U " &
                "ON M.iCodUsuario = U.iCodUsuario " &
                "ORDER BY M.iCodMovimiento DESC"

            Return db.ExecuteDataTable(Query)

        End Function


        '==================================================
        ' INSERTAR MOVIMIENTO
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Movimientos_Inventario " &
                "(iCodProducto, iCodUsuario, iCodCompra, iCodVenta, " &
                "cTipoMovimiento, iCantidad, iStockAnterior, " &
                "iStockNuevo, cMotivo) " &
                "VALUES " &
                "(@iCodProducto, @iCodUsuario, @iCodCompra, @iCodVenta, " &
                "@cTipoMovimiento, @iCantidad, @iStockAnterior, " &
                "@iStockNuevo, @cMotivo); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodMovimiento =
                Convert.ToInt32(
                    db.ExecuteScalar(Query, parametros)
                )

        End Sub


        '==================================================
        ' INSERTAR MOVIMIENTO EN TRANSACCIÓN
        '==================================================

        Public Sub InsertarTransact()

            Dim Query As String =
                "INSERT INTO Movimientos_Inventario " &
                "(iCodProducto, iCodUsuario, iCodCompra, iCodVenta, " &
                "cTipoMovimiento, iCantidad, iStockAnterior, " &
                "iStockNuevo, cMotivo) " &
                "VALUES " &
                "(@iCodProducto, @iCodUsuario, @iCodCompra, @iCodVenta, " &
                "@cTipoMovimiento, @iCantidad, @iStockAnterior, " &
                "@iStockNuevo, @cMotivo)"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' MODIFICAR MOVIMIENTO
        '==================================================

        Public Sub Modificar()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' MODIFICAR MOVIMIENTO EN TRANSACCIÓN
        '==================================================

        Public Sub ModificarTransact()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' OBTENER MOVIMIENTO POR ID
        '==================================================

        Public Sub getRecord()

            Dim Query As String =
                "SELECT " &
                "iCodMovimiento, " &
                "iCodProducto, " &
                "iCodUsuario, " &
                "iCodCompra, " &
                "iCodVenta, " &
                "cTipoMovimiento, " &
                "iCantidad, " &
                "iStockAnterior, " &
                "iStockNuevo, " &
                "cMotivo, " &
                "dFechaMovimiento " &
                "FROM Movimientos_Inventario " &
                "WHERE iCodMovimiento = @iCodMovimiento"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodMovimiento", SqlDbType.Int) With {
                    .Value = Me.iCodMovimiento
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(Query, parametros)

            Try

                If readers.Read() Then

                    Me.iCodMovimiento =
                        Convert.ToInt32(
                            readers("iCodMovimiento")
                        )

                    Me.iCodProducto =
                        Convert.ToInt32(
                            readers("iCodProducto")
                        )

                    Me.iCodUsuario =
                        Convert.ToInt32(
                            readers("iCodUsuario")
                        )


                    '==========================================
                    ' COMPRA PUEDE SER NULL
                    '==========================================

                    If IsDBNull(readers("iCodCompra")) Then

                        Me.iCodCompra = Nothing

                    Else

                        Me.iCodCompra =
                            Convert.ToInt32(
                                readers("iCodCompra")
                            )

                    End If


                    '==========================================
                    ' VENTA PUEDE SER NULL
                    '==========================================

                    If IsDBNull(readers("iCodVenta")) Then

                        Me.iCodVenta = Nothing

                    Else

                        Me.iCodVenta =
                            Convert.ToInt32(
                                readers("iCodVenta")
                            )

                    End If


                    Me.cTipoMovimiento =
                        Convert.ToString(
                            readers("cTipoMovimiento")
                        )

                    Me.iCantidad =
                        Convert.ToInt32(
                            readers("iCantidad")
                        )

                    Me.iStockAnterior =
                        Convert.ToInt32(
                            readers("iStockAnterior")
                        )

                    Me.iStockNuevo =
                        Convert.ToInt32(
                            readers("iStockNuevo")
                        )

                    Me.cMotivo =
                        Convert.ToString(
                            readers("cMotivo")
                        )

                    Me.dFechaMovimiento =
                        Convert.ToDateTime(
                            readers("dFechaMovimiento")
                        )

                End If

            Finally

                readers.Close()

            End Try

        End Sub


        '==================================================
        ' BUSCAR MOVIMIENTOS POR PRODUCTO
        '==================================================

        Public Function BuscarPorProducto(
            idProducto As Integer
        ) As DataTable

            Dim Query As String =
                "SELECT " &
                "M.iCodMovimiento, " &
                "M.iCodProducto, " &
                "M.iCodUsuario, " &
                "M.iCodCompra, " &
                "M.iCodVenta, " &
                "M.cTipoMovimiento, " &
                "M.iCantidad, " &
                "M.iStockAnterior, " &
                "M.iStockNuevo, " &
                "M.cMotivo, " &
                "M.dFechaMovimiento " &
                "FROM Movimientos_Inventario M " &
                "WHERE M.iCodProducto = @iCodProducto " &
                "ORDER BY M.iCodMovimiento DESC"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodProducto", SqlDbType.Int) With {
                    .Value = idProducto
                }
            )

            Return db.ExecuteDataTable(
                Query,
                parametros
            )

        End Function


        '==================================================
        ' BUSCAR POR TIPO DE MOVIMIENTO
        '==================================================

        Public Function BuscarPorTipo(
            tipoMovimiento As String
        ) As DataTable

            Dim Query As String =
                "SELECT " &
                "M.iCodMovimiento, " &
                "M.iCodProducto, " &
                "M.iCodUsuario, " &
                "M.iCodCompra, " &
                "M.iCodVenta, " &
                "M.cTipoMovimiento, " &
                "M.iCantidad, " &
                "M.iStockAnterior, " &
                "M.iStockNuevo, " &
                "M.cMotivo, " &
                "M.dFechaMovimiento " &
                "FROM Movimientos_Inventario M " &
                "WHERE M.cTipoMovimiento = @cTipoMovimiento " &
                "ORDER BY M.iCodMovimiento DESC"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@cTipoMovimiento", SqlDbType.NVarChar, 30) With {
                    .Value = tipoMovimiento.Trim()
                }
            )

            Return db.ExecuteDataTable(
                Query,
                parametros
            )

        End Function


        '==================================================
        ' LISTAR TIPOS DE MOVIMIENTO PARA COMBOBOX
        '==================================================

        Public Function ListaTiposMovimiento() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            miDataTable.Rows.Add(
                "ENTRADA",
                "Entrada"
            )

            miDataTable.Rows.Add(
                "SALIDA",
                "Salida"
            )

            miDataTable.Rows.Add(
                "AJUSTE_ENTRADA",
                "Ajuste de entrada"
            )

            miDataTable.Rows.Add(
                "AJUSTE_SALIDA",
                "Ajuste de salida"
            )

            Return miDataTable

        End Function

    End Class

End Namespace