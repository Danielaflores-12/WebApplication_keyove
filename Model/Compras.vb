Imports System.Data.SqlClient
Imports WebApplication_Keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Compras

        '==================================================
        ' VARIABLES
        '==================================================

        Public iCodCompra As Integer
        Public iCodProveedor As Integer
        Public iCodUsuario As Integer
        Public cTipoComprobante As String
        Public cNumeroComprobante As String
        Public dFechaCompra As DateTime
        Public nSubTotal As Decimal
        Public nIgv As Decimal
        Public nTotal As Decimal
        Public cObservacion As String
        Public cEstado As String = "REGISTRADA"

        Public qSelect As String
        Public db As New ConexionBD()


        '==================================================
        ' CONVERTIR CAMPOS VACÍOS EN NULL PARA SQL SERVER
        '==================================================

        Private Function ValorONull(valor As String) As Object

            If String.IsNullOrWhiteSpace(valor) Then
                Return DBNull.Value
            End If

            Return valor.Trim()

        End Function


        '==================================================
        ' CREAR PARÁMETROS DE LA COMPRA
        '==================================================

        Private Function CrearParametros(
            incluirId As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodProveedor", SqlDbType.Int) With {
                    .Value = Me.iCodProveedor
                }
            )

            parametros.Add(
                New SqlParameter("@iCodUsuario", SqlDbType.Int) With {
                    .Value = Me.iCodUsuario
                }
            )

            parametros.Add(
                New SqlParameter("@cTipoComprobante", SqlDbType.NVarChar, 30) With {
                    .Value = Me.cTipoComprobante.Trim()
                }
            )

            parametros.Add(
                New SqlParameter("@cNumeroComprobante", SqlDbType.NVarChar, 50) With {
                    .Value = Me.cNumeroComprobante.Trim()
                }
            )

            parametros.Add(
                New SqlParameter("@nSubTotal", SqlDbType.Decimal) With {
                    .Precision = 18,
                    .Scale = 2,
                    .Value = Me.nSubTotal
                }
            )

            parametros.Add(
                New SqlParameter("@nIgv", SqlDbType.Decimal) With {
                    .Precision = 18,
                    .Scale = 2,
                    .Value = Me.nIgv
                }
            )

            parametros.Add(
                New SqlParameter("@nTotal", SqlDbType.Decimal) With {
                    .Precision = 18,
                    .Scale = 2,
                    .Value = Me.nTotal
                }
            )

            parametros.Add(
                New SqlParameter("@cObservacion", SqlDbType.NVarChar, 300) With {
                    .Value = ValorONull(Me.cObservacion)
                }
            )

            parametros.Add(
                New SqlParameter("@cEstado", SqlDbType.NVarChar, 20) With {
                    .Value = Me.cEstado.Trim()
                }
            )

            If incluirId Then

                parametros.Add(
                    New SqlParameter("@iCodCompra", SqlDbType.Int) With {
                        .Value = Me.iCodCompra
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
                "UPDATE Compras SET " &
                "iCodProveedor = @iCodProveedor, " &
                "iCodUsuario = @iCodUsuario, " &
                "cTipoComprobante = @cTipoComprobante, " &
                "cNumeroComprobante = @cNumeroComprobante, " &
                "nSubTotal = @nSubTotal, " &
                "nIgv = @nIgv, " &
                "nTotal = @nTotal, " &
                "cObservacion = @cObservacion, " &
                "cEstado = @cEstado " &
                "WHERE iCodCompra = @iCodCompra"

            Return Query

        End Function


        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function


        '==================================================
        ' LISTAR TODAS LAS COMPRAS
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "C.iCodCompra, " &
                "C.iCodProveedor, " &
                "C.iCodUsuario, " &
                "C.cTipoComprobante, " &
                "C.cNumeroComprobante, " &
                "C.dFechaCompra, " &
                "C.nSubTotal, " &
                "C.nIgv, " &
                "C.nTotal, " &
                "C.cObservacion, " &
                "C.cEstado " &
                "FROM Compras C " &
                "ORDER BY C.iCodCompra DESC"

            Return db.ExecuteDataTable(Query)

        End Function


        '==================================================
        ' INSERTAR COMPRA
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Compras " &
                "(iCodProveedor, iCodUsuario, " &
                "cTipoComprobante, cNumeroComprobante, " &
                "nSubTotal, nIgv, nTotal, " &
                "cObservacion, cEstado) " &
                "VALUES " &
                "(@iCodProveedor, @iCodUsuario, " &
                "@cTipoComprobante, @cNumeroComprobante, " &
                "@nSubTotal, @nIgv, @nTotal, " &
                "@cObservacion, @cEstado); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodCompra =
                Convert.ToInt32(
                    db.ExecuteScalar(Query, parametros)
                )

        End Sub


        '==================================================
        ' MODIFICAR COMPRA
        '==================================================

        Public Sub Modificar()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' MODIFICAR COMPRA EN TRANSACCIÓN
        '==================================================

        Public Sub ModificarTransact()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' ANULAR COMPRA
        '==================================================

        Public Sub Eliminar()

            Dim Query As String =
                "UPDATE Compras " &
                "SET cEstado = 'ANULADA' " &
                "WHERE iCodCompra = @iCodCompra"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodCompra", SqlDbType.Int) With {
                    .Value = Me.iCodCompra
                }
            )

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' ANULAR COMPRA EN TRANSACCIÓN
        '==================================================

        Public Sub EliminarTransact()

            Dim Query As String =
                "UPDATE Compras " &
                "SET cEstado = 'ANULADA' " &
                "WHERE iCodCompra = @iCodCompra"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodCompra", SqlDbType.Int) With {
                    .Value = Me.iCodCompra
                }
            )

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' OBTENER UNA COMPRA POR SU ID
        '==================================================

        Public Sub getRecord()

            Dim Query As String =
                "SELECT " &
                "iCodCompra, " &
                "iCodProveedor, " &
                "iCodUsuario, " &
                "cTipoComprobante, " &
                "cNumeroComprobante, " &
                "dFechaCompra, " &
                "nSubTotal, " &
                "nIgv, " &
                "nTotal, " &
                "cObservacion, " &
                "cEstado " &
                "FROM Compras " &
                "WHERE iCodCompra = @iCodCompra"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodCompra", SqlDbType.Int) With {
                    .Value = Me.iCodCompra
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(Query, parametros)

            Try

                If readers.Read() Then

                    Me.iCodCompra =
                        Convert.ToInt32(readers("iCodCompra"))

                    Me.iCodProveedor =
                        Convert.ToInt32(readers("iCodProveedor"))

                    Me.iCodUsuario =
                        Convert.ToInt32(readers("iCodUsuario"))

                    Me.cTipoComprobante =
                        Convert.ToString(readers("cTipoComprobante"))

                    Me.cNumeroComprobante =
                        Convert.ToString(readers("cNumeroComprobante"))

                    Me.dFechaCompra =
                        Convert.ToDateTime(readers("dFechaCompra"))

                    Me.nSubTotal =
                        Convert.ToDecimal(readers("nSubTotal"))

                    Me.nIgv =
                        Convert.ToDecimal(readers("nIgv"))

                    Me.nTotal =
                        Convert.ToDecimal(readers("nTotal"))

                    Me.cObservacion =
                        Convert.ToString(readers("cObservacion"))

                    Me.cEstado =
                        Convert.ToString(readers("cEstado"))

                End If

            Finally

                readers.Close()

            End Try

        End Sub


        '==================================================
        ' BUSCAR COMPRA POR NÚMERO DE COMPROBANTE
        '==================================================

        Public Function BuscarPorComprobante(
            numeroComprobante As String
        ) As DataTable

            Dim Query As String =
                "SELECT " &
                "iCodCompra, " &
                "iCodProveedor, " &
                "iCodUsuario, " &
                "cTipoComprobante, " &
                "cNumeroComprobante, " &
                "dFechaCompra, " &
                "nSubTotal, " &
                "nIgv, " &
                "nTotal, " &
                "cObservacion, " &
                "cEstado " &
                "FROM Compras " &
                "WHERE cNumeroComprobante LIKE @NumeroComprobante " &
                "ORDER BY iCodCompra DESC"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@NumeroComprobante",
                    SqlDbType.NVarChar,
                    50
                ) With {
                    .Value = "%" & numeroComprobante.Trim() & "%"
                }
            )

            Return db.ExecuteDataTable(
                Query,
                parametros
            )

        End Function


        '==================================================
        ' LISTAR COMPRAS REGISTRADAS PARA COMBOBOX
        '==================================================

        Public Function ListaDatosCombo() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Dim Query As String =
                "SELECT " &
                "iCodCompra, " &
                "cNumeroComprobante " &
                "FROM Compras " &
                "WHERE cEstado = 'REGISTRADA' " &
                "ORDER BY iCodCompra DESC"

            Dim readers As IDataReader =
                db.ExecuteReader(Query)

            Try

                While readers.Read()

                    miDataTable.Rows.Add(
                        Convert.ToString(
                            readers("iCodCompra")
                        ),
                        Convert.ToString(
                            readers("cNumeroComprobante")
                        )
                    )

                End While

            Finally

                readers.Close()

            End Try

            Return miDataTable

        End Function

    End Class

End Namespace