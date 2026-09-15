Imports System.Data
Imports System.Data.SqlClient
Imports WebApplication_Keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class DetalleVenta

        Public iCodDetalleVenta As Integer
        Public iCodVenta As Integer
        Public iCodProducto As Integer
        Public cCodigo As String
        Public cNombreProducto As String
        Public iCantidad As Integer
        Public nPrecioVenta As Decimal
        Public nDescuento As Decimal = 0D
        Public nSubTotal As Decimal

        Public qSelect As String
        Public db As New ConexionBD()

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
        ' CREAR PARÁMETROS DEL DETALLE DE VENTA
        '==================================================

        Private Function CrearParametros(
            incluirCodigoDetalle As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodVenta",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodVenta
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@iCodProducto",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodProducto
                }
            )

            parametros.Add(
                New SqlParameter(
                    "@iCantidad",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCantidad
                }
            )

            parametros.Add(
                ParametroDecimal(
                    "@nPrecioVenta",
                    Me.nPrecioVenta
                )
            )

            parametros.Add(
                ParametroDecimal(
                    "@nDescuento",
                    Me.nDescuento
                )
            )

            If incluirCodigoDetalle Then

                parametros.Add(
                    New SqlParameter(
                        "@iCodDetalleVenta",
                        SqlDbType.Int
                    ) With {
                        .Value = Me.iCodDetalleVenta
                    }
                )

            End If

            Return parametros

        End Function

        '==================================================
        ' CONSULTA PARA MODIFICAR DETALLE DE VENTA
        '==================================================

        Private Function ConsultaModificar() As String

            Dim Query As String =
                "UPDATE Detalle_Venta SET " &
                "iCodVenta = @iCodVenta, " &
                "iCodProducto = @iCodProducto, " &
                "iCantidad = @iCantidad, " &
                "nPrecioVenta = @nPrecioVenta, " &
                "nDescuento = @nDescuento " &
                "WHERE iCodDetalleVenta = @iCodDetalleVenta"

            Return Query

        End Function

        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function

        '==================================================
        ' LISTAR TODOS LOS DETALLES DE VENTA
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "dv.iCodDetalleVenta, " &
                "dv.iCodVenta, " &
                "dv.iCodProducto, " &
                "p.cCodigo, " &
                "p.cNombre AS cNombreProducto, " &
                "dv.iCantidad, " &
                "dv.nPrecioVenta, " &
                "dv.nDescuento, " &
                "dv.nSubTotal " &
                "FROM Detalle_Venta dv " &
                "INNER JOIN Productos p " &
                "ON dv.iCodProducto = p.iCodProducto " &
                "ORDER BY dv.iCodDetalleVenta DESC"

            Return db.ExecuteDataTable(Query)

        End Function

        '==================================================
        ' INSERTAR DETALLE DE VENTA
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Detalle_Venta (" &
                "iCodVenta, " &
                "iCodProducto, " &
                "iCantidad, " &
                "nPrecioVenta, " &
                "nDescuento" &
                ") VALUES (" &
                "@iCodVenta, " &
                "@iCodProducto, " &
                "@iCantidad, " &
                "@nPrecioVenta, " &
                "@nDescuento" &
                "); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodDetalleVenta =
                Convert.ToInt32(
                    db.ExecuteScalar(
                        Query,
                        parametros
                    )
                )

        End Sub

        '==================================================
        ' MODIFICAR DETALLE DE VENTA
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
        ' MODIFICAR DETALLE DE VENTA EN TRANSACCIÓN
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
        ' ELIMINAR DETALLE DE VENTA
        '==================================================

        Public Sub Eliminar()

            Dim Query As String =
                "DELETE FROM Detalle_Venta " &
                "WHERE iCodDetalleVenta = @iCodDetalleVenta"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodDetalleVenta",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodDetalleVenta
                }
            )

            db.ExecuteQuery(
                Query,
                parametros
            )

        End Sub

        '==================================================
        ' ELIMINAR DETALLE DE VENTA EN TRANSACCIÓN
        '==================================================

        Public Sub EliminarTransact()

            Dim Query As String =
                "DELETE FROM Detalle_Venta " &
                "WHERE iCodDetalleVenta = @iCodDetalleVenta"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodDetalleVenta",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodDetalleVenta
                }
            )

            db.ExecuteQueryTransact(
                Query,
                parametros
            )

        End Sub

        '==================================================
        ' OBTENER UN DETALLE DE VENTA
        '==================================================

        Public Sub getRecord()

            Dim Query As String =
                "SELECT " &
                "iCodDetalleVenta, " &
                "iCodVenta, " &
                "iCodProducto, " &
                "iCantidad, " &
                "nPrecioVenta, " &
                "nDescuento, " &
                "nSubTotal " &
                "FROM Detalle_Venta " &
                "WHERE iCodDetalleVenta = @iCodDetalleVenta"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodDetalleVenta",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodDetalleVenta
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(
                    Query,
                    parametros
                )

            Try

                If readers.Read() Then

                    Me.iCodDetalleVenta =
                        Convert.ToInt32(
                            readers("iCodDetalleVenta")
                        )

                    Me.iCodVenta =
                        Convert.ToInt32(
                            readers("iCodVenta")
                        )

                    Me.iCodProducto =
                        Convert.ToInt32(
                            readers("iCodProducto")
                        )

                    Me.iCantidad =
                        Convert.ToInt32(
                            readers("iCantidad")
                        )

                    Me.nPrecioVenta =
                        Convert.ToDecimal(
                            readers("nPrecioVenta")
                        )

                    Me.nDescuento =
                        Convert.ToDecimal(
                            readers("nDescuento")
                        )

                    Me.nSubTotal =
                        Convert.ToDecimal(
                            readers("nSubTotal")
                        )

                End If

            Finally

                readers.Close()

            End Try

        End Sub

    End Class

End Namespace