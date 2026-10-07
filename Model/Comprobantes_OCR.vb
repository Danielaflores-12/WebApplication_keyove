Imports System.Data
Imports System.Data.SqlClient
Imports WebApplication_Keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class ComprobantesOCR

        '==================================================
        ' LIMITE DE TAMANO PARA GUARDAR EL BINARIO
        ' Si el archivo supera este limite se guardan solo
        ' los metadatos (nombre y tamano) y el texto OCR,
        ' evitando enviar megabytes por la red en cada operacion.
        '==================================================

        Public Const LIMITE_BYTES As Integer = 2 * 1024 * 1024

        '==================================================
        ' VARIABLES
        '==================================================

        Public iCodComprobante As Integer
        Public iCodCompra As Integer
        Public iCodVenta As Integer
        Public cNombreArchivo As String
        Public cTipoMime As String
        Public nTamanoBytes As Integer
        Public cRucEmisor As String
        Public cNumeroComprobante As String
        Public cTipoComprobante As String
        Public dFechaEmision As Date?
        Public nSubTotal As Decimal
        Public nIgv As Decimal
        Public nTotal As Decimal
        Public cTextoOcr As String
        Public imgComprobante As Byte()
        Public dFechaCarga As DateTime

        Public qSelect As String
        Public db As New ConexionBD()


        '==================================================
        ' CONVERTIR CAMPOS VACIOS EN NULL PARA SQL SERVER
        '==================================================

        Private Function ValorONull(valor As String) As Object

            If String.IsNullOrWhiteSpace(valor) Then
                Return DBNull.Value
            End If

            Return valor.Trim()

        End Function


        '==================================================
        ' CONVERTIR ENTEROS EN NULL (0 SIGNIFICA SIN VINCULO)
        '==================================================

        Private Function EnteroONull(valor As Integer) As Object

            If valor <= 0 Then
                Return DBNull.Value
            End If

            Return valor

        End Function


        '==================================================
        ' CREAR PARAMETRO DECIMAL
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
        ' CREAR PARAMETROS DEL COMPROBANTE
        '==================================================

        Private Function CrearParametros() As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

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
                New SqlParameter("@cNombreArchivo", SqlDbType.NVarChar, 255) With {
                    .Value = Me.cNombreArchivo.Trim()
                }
            )

            parametros.Add(
                New SqlParameter("@cTipoMime", SqlDbType.NVarChar, 100) With {
                    .Value = ValorONull(Me.cTipoMime)
                }
            )

            parametros.Add(
                New SqlParameter("@nTamanoBytes", SqlDbType.Int) With {
                    .Value = Me.nTamanoBytes
                }
            )

            parametros.Add(
                New SqlParameter("@cRucEmisor", SqlDbType.NVarChar, 20) With {
                    .Value = ValorONull(Me.cRucEmisor)
                }
            )

            parametros.Add(
                New SqlParameter("@cNumeroComprobante", SqlDbType.NVarChar, 50) With {
                    .Value = ValorONull(Me.cNumeroComprobante)
                }
            )

            parametros.Add(
                New SqlParameter("@cTipoComprobante", SqlDbType.NVarChar, 30) With {
                    .Value = ValorONull(Me.cTipoComprobante)
                }
            )

            parametros.Add(
                New SqlParameter("@cFechaEmision", SqlDbType.Date) With {
                    .Value = If(
                        Me.dFechaEmision.HasValue,
                        Me.dFechaEmision.Value,
                        DBNull.Value
                    )
                }
            )

            parametros.Add(
                ParametroDecimal("@nSubTotal", Me.nSubTotal)
            )

            parametros.Add(
                ParametroDecimal("@nIgv", Me.nIgv)
            )

            parametros.Add(
                ParametroDecimal("@nTotal", Me.nTotal)
            )

            parametros.Add(
                New SqlParameter("@cTextoOcr", SqlDbType.NVarChar, -1) With {
                    .Value = ValorONull(Me.cTextoOcr)
                }
            )

            parametros.Add(
                New SqlParameter("@imgComprobante", SqlDbType.VarBinary, -1) With {
                    .Value = If(
                        Me.imgComprobante,
                        CType(DBNull.Value, Object)
                    )
                }
            )

            Return parametros

        End Function


        '==================================================
        ' LISTAR DATOS SEGUN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function


        '==================================================
        ' LISTAR COMPROBANTES SIN EL BINARIO
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "iCodComprobante, " &
                "iCodCompra, " &
                "iCodVenta, " &
                "cNombreArchivo, " &
                "cTipoMime, " &
                "nTamanoBytes, " &
                "cRucEmisor, " &
                "cNumeroComprobante, " &
                "cTipoComprobante, " &
                "cFechaEmision, " &
                "nSubTotal, " &
                "nIgv, " &
                "nTotal, " &
                "CASE WHEN imgComprobante IS NULL THEN 0 ELSE 1 END AS bTieneArchivo, " &
                "LEN(cTextoOcr) AS nLargoTextoOcr, " &
                "dFechaCarga " &
                "FROM Comprobantes_OCR " &
                "ORDER BY iCodComprobante DESC"

            Return db.ExecuteDataTable(Query)

        End Function


        '==================================================
        ' INSERTAR COMPROBANTE
        ' Si el binario supera LIMITE_BYTES se guarda solo
        ' la metadata (cNombreArchivo y nTamanoBytes) y el
        ' texto del OCR, no el archivo completo.
        '==================================================

        Public Sub Insertar()

            If Me.imgComprobante IsNot Nothing AndAlso
               Me.imgComprobante.Length > LIMITE_BYTES Then

                Me.imgComprobante = Nothing

            End If

            Dim Query As String =
                "INSERT INTO Comprobantes_OCR (" &
                "iCodCompra, " &
                "iCodVenta, " &
                "cNombreArchivo, " &
                "cTipoMime, " &
                "nTamanoBytes, " &
                "cRucEmisor, " &
                "cNumeroComprobante, " &
                "cTipoComprobante, " &
                "cFechaEmision, " &
                "nSubTotal, " &
                "nIgv, " &
                "nTotal, " &
                "cTextoOcr, " &
                "imgComprobante" &
                ") VALUES (" &
                "@iCodCompra, " &
                "@iCodVenta, " &
                "@cNombreArchivo, " &
                "@cTipoMime, " &
                "@nTamanoBytes, " &
                "@cRucEmisor, " &
                "@cNumeroComprobante, " &
                "@cTipoComprobante, " &
                "@cFechaEmision, " &
                "@nSubTotal, " &
                "@nIgv, " &
                "@nTotal, " &
                "@cTextoOcr, " &
                "@imgComprobante" &
                "); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros()

            Me.iCodComprobante =
                Convert.ToInt32(
                    db.ExecuteScalar(Query, parametros)
                )

        End Sub


        '==================================================
        ' OBTENER UN COMPROBANTE CON SU BINARIO
        '==================================================

        Public Sub getRecord()

            Dim Query As String =
                "SELECT " &
                "iCodComprobante, " &
                "iCodCompra, " &
                "iCodVenta, " &
                "cNombreArchivo, " &
                "cTipoMime, " &
                "nTamanoBytes, " &
                "cRucEmisor, " &
                "cNumeroComprobante, " &
                "cTipoComprobante, " &
                "cFechaEmision, " &
                "nSubTotal, " &
                "nIgv, " &
                "nTotal, " &
                "cTextoOcr, " &
                "imgComprobante, " &
                "dFechaCarga " &
                "FROM Comprobantes_OCR " &
                "WHERE iCodComprobante = @iCodComprobante"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodComprobante", SqlDbType.Int) With {
                    .Value = Me.iCodComprobante
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(Query, parametros)

            Try

                If readers.Read() Then

                    Me.iCodComprobante =
                        Convert.ToInt32(readers("iCodComprobante"))

                    If Not IsDBNull(readers("iCodCompra")) Then
                        Me.iCodCompra =
                            Convert.ToInt32(readers("iCodCompra"))
                    End If

                    If Not IsDBNull(readers("iCodVenta")) Then
                        Me.iCodVenta =
                            Convert.ToInt32(readers("iCodVenta"))
                    End If

                    Me.cNombreArchivo =
                        Convert.ToString(readers("cNombreArchivo"))

                    If IsDBNull(readers("cTipoMime")) Then
                        Me.cTipoMime = Nothing
                    Else
                        Me.cTipoMime =
                            Convert.ToString(readers("cTipoMime"))
                    End If

                    If IsDBNull(readers("nTamanoBytes")) Then
                        Me.nTamanoBytes = 0
                    Else
                        Me.nTamanoBytes =
                            Convert.ToInt32(readers("nTamanoBytes"))
                    End If

                    If IsDBNull(readers("cRucEmisor")) Then
                        Me.cRucEmisor = Nothing
                    Else
                        Me.cRucEmisor =
                            Convert.ToString(readers("cRucEmisor"))
                    End If

                    If IsDBNull(readers("cNumeroComprobante")) Then
                        Me.cNumeroComprobante = Nothing
                    Else
                        Me.cNumeroComprobante =
                            Convert.ToString(readers("cNumeroComprobante"))
                    End If

                    If IsDBNull(readers("cTipoComprobante")) Then
                        Me.cTipoComprobante = Nothing
                    Else
                        Me.cTipoComprobante =
                            Convert.ToString(readers("cTipoComprobante"))
                    End If

                    If IsDBNull(readers("cFechaEmision")) Then
                        Me.dFechaEmision = Nothing
                    Else
                        Me.dFechaEmision =
                            Convert.ToDateTime(readers("cFechaEmision"))
                    End If

                    If IsDBNull(readers("nSubTotal")) Then
                        Me.nSubTotal = 0
                    Else
                        Me.nSubTotal =
                            Convert.ToDecimal(readers("nSubTotal"))
                    End If

                    If IsDBNull(readers("nIgv")) Then
                        Me.nIgv = 0
                    Else
                        Me.nIgv =
                            Convert.ToDecimal(readers("nIgv"))
                    End If

                    If IsDBNull(readers("nTotal")) Then
                        Me.nTotal = 0
                    Else
                        Me.nTotal =
                            Convert.ToDecimal(readers("nTotal"))
                    End If

                    If IsDBNull(readers("cTextoOcr")) Then
                        Me.cTextoOcr = Nothing
                    Else
                        Me.cTextoOcr =
                            Convert.ToString(readers("cTextoOcr"))
                    End If

                    If IsDBNull(readers("imgComprobante")) Then
                        Me.imgComprobante = Nothing
                    Else
                        Me.imgComprobante =
                            CType(readers("imgComprobante"), Byte())
                    End If

                    If Not IsDBNull(readers("dFechaCarga")) Then
                        Me.dFechaCarga =
                            Convert.ToDateTime(readers("dFechaCarga"))
                    End If

                End If

            Finally

                readers.Close()

            End Try

        End Sub


        '==================================================
        ' ACTUALIZAR LOS VINCLOS DE COMPRA Y VENTA
        '==================================================

        Public Sub ActualizarVinculos(
            compra As Integer?,
            venta As Integer?
        )

            Dim Query As String =
                "UPDATE Comprobantes_OCR SET " &
                "iCodCompra = @iCodCompra, " &
                "iCodVenta = @iCodVenta " &
                "WHERE iCodComprobante = @iCodComprobante"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodCompra", SqlDbType.Int) With {
                    .Value = If(compra.HasValue, compra.Value, CType(DBNull.Value, Object))
                }
            )

            parametros.Add(
                New SqlParameter("@iCodVenta", SqlDbType.Int) With {
                    .Value = If(venta.HasValue, venta.Value, CType(DBNull.Value, Object))
                }
            )

            parametros.Add(
                New SqlParameter("@iCodComprobante", SqlDbType.Int) With {
                    .Value = Me.iCodComprobante
                }
            )

            db.ExecuteQuery(Query, parametros)

            If compra.HasValue Then
                Me.iCodCompra = compra.Value
            End If

            If venta.HasValue Then
                Me.iCodVenta = venta.Value
            End If

        End Sub

    End Class

End Namespace