Imports System.Data.SqlClient
Imports WebApplication_Keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Ventas

        '==================================================
        ' VARIABLES
        '==================================================

        Public iCodVenta As Integer
        Public iCodUsuario As Integer
        Public cNombreUsuario As String
        Public cTipoComprobante As String = "BOLETA"
        Public cDocumentoCliente As String
        Public cNumeroCelular As String
        Public cNumeroComprobante As String
        Public dFechaVenta As DateTime
        Public nSubTotal As Decimal
        Public nIgv As Decimal
        Public nTotal As Decimal
        Public cMetodoPago As String = "EFECTIVO"
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
        ' CREAR PARÁMETROS DE LA VENTA
        '==================================================

        Private Function CrearParametros(
            incluirId As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

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
                New SqlParameter("@cDocumentoCliente", SqlDbType.NVarChar, 11) With {
                    .Value = ValorONull(Me.cDocumentoCliente)
                }
            )

            parametros.Add(
                New SqlParameter("@cNumeroCelular", SqlDbType.VarChar, 9) With {
                    .Value = ValorONull(Me.cNumeroCelular)
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
                New SqlParameter("@cMetodoPago", SqlDbType.NVarChar, 50) With {
                    .Value = Me.cMetodoPago.Trim()
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
                    New SqlParameter("@iCodVenta", SqlDbType.Int) With {
                        .Value = Me.iCodVenta
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
                "UPDATE Ventas SET " &
                "iCodUsuario = @iCodUsuario, " &
                "cTipoComprobante = @cTipoComprobante, " &
                "cDocumentoCliente = @cDocumentoCliente, " &
                "cNumeroCelular = @cNumeroCelular, " &
                "cNumeroComprobante = @cNumeroComprobante, " &
                "nSubTotal = @nSubTotal, " &
                "nIgv = @nIgv, " &
                "nTotal = @nTotal, " &
                "cMetodoPago = @cMetodoPago, " &
                "cObservacion = @cObservacion, " &
                "cEstado = @cEstado " &
                "WHERE iCodVenta = @iCodVenta"

            Return Query

        End Function


        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function


        '==================================================
        ' LISTAR TODAS LAS VENTAS
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "V.iCodVenta, " &
                "V.iCodUsuario, " &
                "U.cNombreUsuario AS Usuario, " &
                "V.cTipoComprobante, " &
                "V.cDocumentoCliente, " &
                "V.cNumeroCelular, " &
                "V.cNumeroComprobante, " &
                "V.dFechaVenta, " &
                "V.nSubTotal, " &
                "V.nIgv, " &
                "V.nTotal, " &
                "V.cMetodoPago, " &
                "V.cObservacion, " &
                "V.cEstado " &
                "FROM Ventas V " &
                "INNER JOIN Usuarios U " &
                "ON V.iCodUsuario = U.iCodUsuario " &
                "ORDER BY V.iCodVenta DESC"

            Return db.ExecuteDataTable(Query)

        End Function


        '==================================================
        ' INSERTAR VENTA
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Ventas " &
                "(iCodUsuario, cTipoComprobante, " &
                "cDocumentoCliente, cNumeroCelular, " &
                "cNumeroComprobante, nSubTotal, nIgv, nTotal, " &
                "cMetodoPago, cObservacion, cEstado) " &
                "VALUES " &
                "(@iCodUsuario, @cTipoComprobante, " &
                "@cDocumentoCliente, @cNumeroCelular, " &
                "@cNumeroComprobante, @nSubTotal, @nIgv, @nTotal, " &
                "@cMetodoPago, @cObservacion, @cEstado); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodVenta =
                Convert.ToInt32(
                    db.ExecuteScalar(Query, parametros)
                )

        End Sub


        '==================================================
        ' INSERTAR VENTA EN TRANSACCIÓN
        '==================================================

        Public Sub InsertarTransact()

            Dim Query As String =
                "INSERT INTO Ventas " &
                "(iCodUsuario, cTipoComprobante, " &
                "cDocumentoCliente, cNumeroCelular, " &
                "cNumeroComprobante, nSubTotal, nIgv, nTotal, " &
                "cMetodoPago, cObservacion, cEstado) " &
                "VALUES " &
                "(@iCodUsuario, @cTipoComprobante, " &
                "@cDocumentoCliente, @cNumeroCelular, " &
                "@cNumeroComprobante, @nSubTotal, @nIgv, @nTotal, " &
                "@cMetodoPago, @cObservacion, @cEstado); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodVenta =
                Convert.ToInt32(
                    db.ExecuteScalarTransact(Query, parametros)
                )

        End Sub


        '==================================================
        ' MODIFICAR VENTA
        '==================================================

        Public Sub Modificar()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' MODIFICAR VENTA EN TRANSACCIÓN
        '==================================================

        Public Sub ModificarTransact()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' ANULAR VENTA
        '==================================================

        Public Sub Eliminar()

            Dim Query As String =
                "UPDATE Ventas " &
                "SET cEstado = 'ANULADA' " &
                "WHERE iCodVenta = @iCodVenta"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodVenta", SqlDbType.Int) With {
                    .Value = Me.iCodVenta
                }
            )

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' ANULAR VENTA EN TRANSACCIÓN
        '==================================================

        Public Sub EliminarTransact()

            Dim Query As String =
                "UPDATE Ventas " &
                "SET cEstado = 'ANULADA' " &
                "WHERE iCodVenta = @iCodVenta"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodVenta", SqlDbType.Int) With {
                    .Value = Me.iCodVenta
                }
            )

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' OBTENER UNA VENTA POR SU ID
        '==================================================

        Public Sub getRecord()

            Dim Query As String =
                "SELECT " &
                "iCodVenta, " &
                "iCodUsuario, " &
                "cTipoComprobante, " &
                "cDocumentoCliente, " &
                "cNumeroCelular, " &
                "cNumeroComprobante, " &
                "dFechaVenta, " &
                "nSubTotal, " &
                "nIgv, " &
                "nTotal, " &
                "cMetodoPago, " &
                "cObservacion, " &
                "cEstado " &
                "FROM Ventas " &
                "WHERE iCodVenta = @iCodVenta"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodVenta", SqlDbType.Int) With {
                    .Value = Me.iCodVenta
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(Query, parametros)

            Try

                If readers.Read() Then

                    Me.iCodVenta =
                        Convert.ToInt32(readers("iCodVenta"))

                    Me.iCodUsuario =
                        Convert.ToInt32(readers("iCodUsuario"))

                    Me.cTipoComprobante =
                        Convert.ToString(readers("cTipoComprobante"))

                    Me.cDocumentoCliente =
                        Convert.ToString(readers("cDocumentoCliente"))

                    Me.cNumeroCelular =
                        Convert.ToString(readers("cNumeroCelular"))

                    Me.cNumeroComprobante =
                        Convert.ToString(readers("cNumeroComprobante"))

                    Me.dFechaVenta =
                        Convert.ToDateTime(readers("dFechaVenta"))

                    Me.nSubTotal =
                        Convert.ToDecimal(readers("nSubTotal"))

                    Me.nIgv =
                        Convert.ToDecimal(readers("nIgv"))

                    Me.nTotal =
                        Convert.ToDecimal(readers("nTotal"))

                    Me.cMetodoPago =
                        Convert.ToString(readers("cMetodoPago"))

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
        ' BUSCAR POR NÚMERO DE COMPROBANTE
        '==================================================

        Public Function BuscarPorComprobante(
            numeroComprobante As String
        ) As DataTable

            Dim Query As String =
                "SELECT " &
                "V.iCodVenta, " &
                "V.iCodUsuario, " &
                "U.cNombreUsuario AS Usuario, " &
                "V.cTipoComprobante, " &
                "V.cDocumentoCliente, " &
                "V.cNumeroCelular, " &
                "V.cNumeroComprobante, " &
                "V.dFechaVenta, " &
                "V.nSubTotal, " &
                "V.nIgv, " &
                "V.nTotal, " &
                "V.cMetodoPago, " &
                "V.cEstado " &
                "FROM Ventas V " &
                "INNER JOIN Usuarios U " &
                "ON V.iCodUsuario = U.iCodUsuario " &
                "WHERE V.cNumeroComprobante LIKE @NumeroComprobante " &
                "ORDER BY V.iCodVenta DESC"

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
        ' BUSCAR POR DOCUMENTO DEL CLIENTE
        '==================================================

        Public Function BuscarPorDocumento(
            documento As String
        ) As DataTable

            Dim Query As String =
                "SELECT " &
                "iCodVenta, " &
                "cDocumentoCliente, " &
                "cNumeroCelular, " &
                "cNumeroComprobante, " &
                "dFechaVenta, " &
                "nTotal, " &
                "cMetodoPago, " &
                "cEstado " &
                "FROM Ventas " &
                "WHERE cDocumentoCliente LIKE @Documento " &
                "ORDER BY iCodVenta DESC"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@Documento",
                    SqlDbType.NVarChar,
                    11
                ) With {
                    .Value = "%" & documento.Trim() & "%"
                }
            )

            Return db.ExecuteDataTable(
                Query,
                parametros
            )

        End Function


        '==================================================
        ' LISTAR VENTAS REGISTRADAS PARA COMBOBOX
        '==================================================

        Public Function ListaDatosCombo() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Dim Query As String =
                "SELECT " &
                "iCodVenta, " &
                "cNumeroComprobante " &
                "FROM Ventas " &
                "WHERE cEstado = 'REGISTRADA' " &
                "ORDER BY iCodVenta DESC"

            Dim readers As IDataReader =
                db.ExecuteReader(Query)

            Try

                While readers.Read()

                    miDataTable.Rows.Add(
                        Convert.ToString(
                            readers("iCodVenta")
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