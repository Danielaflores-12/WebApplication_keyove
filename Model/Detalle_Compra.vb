Imports System.Data
Imports System.Data.SqlClient
Imports WebApplication_Keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class DetalleCompra

        Public iCodDetalleCompra As Integer
        Public iCodCompra As Integer
        Public iCodProducto As Integer
        Public iCantidad As Integer
        Public nPrecioCompra As Decimal
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
        ' CREAR PARÁMETROS DEL DETALLE DE COMPRA
        '==================================================

        Private Function CrearParametros(
            incluirCodigoDetalle As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodCompra",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodCompra
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
                    "@nPrecioCompra",
                    Me.nPrecioCompra
                )
            )

            If incluirCodigoDetalle Then

                parametros.Add(
                    New SqlParameter(
                        "@iCodDetalleCompra",
                        SqlDbType.Int
                    ) With {
                        .Value = Me.iCodDetalleCompra
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
                "UPDATE Detalle_Compra SET " &
                "iCodCompra = @iCodCompra, " &
                "iCodProducto = @iCodProducto, " &
                "iCantidad = @iCantidad, " &
                "nPrecioCompra = @nPrecioCompra " &
                "WHERE iCodDetalleCompra = @iCodDetalleCompra"

            Return Query

        End Function

        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function

        '==================================================
        ' LISTAR TODOS LOS DETALLES DE COMPRA
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "dc.iCodDetalleCompra, " &
                "dc.iCodCompra, " &
                "dc.iCodProducto, " &
                "p.cCodigo, " &
                "p.cNombre AS cNombreProducto, " &
                "dc.iCantidad, " &
                "dc.nPrecioCompra, " &
                "dc.nSubTotal " &
                "FROM Detalle_Compra dc " &
                "INNER JOIN Productos p " &
                "ON dc.iCodProducto = p.iCodProducto " &
                "ORDER BY dc.iCodDetalleCompra DESC"

            Return db.ExecuteDataTable(Query)

        End Function

        '==================================================
        ' INSERTAR DETALLE DE COMPRA
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Detalle_Compra (" &
                "iCodCompra, " &
                "iCodProducto, " &
                "iCantidad, " &
                "nPrecioCompra" &
                ") VALUES (" &
                "@iCodCompra, " &
                "@iCodProducto, " &
                "@iCantidad, " &
                "@nPrecioCompra" &
                "); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodDetalleCompra =
                Convert.ToInt32(
                    db.ExecuteScalar(
                        Query,
                        parametros
                    )
                )

        End Sub

        '==================================================
        ' MODIFICAR DETALLE DE COMPRA
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
        ' MODIFICAR EN TRANSACCIÓN
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
        ' ELIMINAR DETALLE DE COMPRA
        '==================================================

        Public Sub Eliminar()

            Dim Query As String =
                "DELETE FROM Detalle_Compra " &
                "WHERE iCodDetalleCompra = @iCodDetalleCompra"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodDetalleCompra",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodDetalleCompra
                }
            )

            db.ExecuteQuery(
                Query,
                parametros
            )

        End Sub

        '==================================================
        ' ELIMINAR EN TRANSACCIÓN
        '==================================================

        Public Sub EliminarTransact()

            Dim Query As String =
                "DELETE FROM Detalle_Compra " &
                "WHERE iCodDetalleCompra = @iCodDetalleCompra"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodDetalleCompra",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodDetalleCompra
                }
            )

            db.ExecuteQueryTransact(
                Query,
                parametros
            )

        End Sub

        '==================================================
        ' OBTENER UN DETALLE DE COMPRA
        '==================================================

        Public Sub getRecord()

            Dim Query As String =
                "SELECT " &
                "iCodDetalleCompra, " &
                "iCodCompra, " &
                "iCodProducto, " &
                "iCantidad, " &
                "nPrecioCompra, " &
                "nSubTotal " &
                "FROM Detalle_Compra " &
                "WHERE iCodDetalleCompra = @iCodDetalleCompra"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@iCodDetalleCompra",
                    SqlDbType.Int
                ) With {
                    .Value = Me.iCodDetalleCompra
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(
                    Query,
                    parametros
                )

            Try

                If readers.Read() Then

                    Me.iCodDetalleCompra =
                        Convert.ToInt32(
                            readers("iCodDetalleCompra")
                        )

                    Me.iCodCompra =
                        Convert.ToInt32(
                            readers("iCodCompra")
                        )

                    Me.iCodProducto =
                        Convert.ToInt32(
                            readers("iCodProducto")
                        )

                    Me.iCantidad =
                        Convert.ToInt32(
                            readers("iCantidad")
                        )

                    Me.nPrecioCompra =
                        Convert.ToDecimal(
                            readers("nPrecioCompra")
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