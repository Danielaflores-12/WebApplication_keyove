Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports WebApplication_keyove.WebApplication_Keyove.Data
Imports ModeloCompra = WebApplication_keyove.WebApplication_Keyove.Model.Compras
Imports ModeloDetalleCompra = WebApplication_keyove.WebApplication_Keyove.Model.DetalleCompra
Imports ModeloComprobante = WebApplication_keyove.WebApplication_Keyove.Model.ComprobantesOCR

Namespace WebApplication_Keyove.Views.Operaciones.Compras
    Partial Public Class Compra1
        Public Class ArchivoCompra
            Public nombreArchivo As String
            Public contenidoBase64 As String
            Public tipoMime As String
            Public textoOcr As String
            Public rucEmisor As String
        End Class

        Public Class ResultadoCompra
            Public ok As Boolean
            Public idCompra As Integer
            Public mensaje As String
        End Class

        Private Shared Sub ExigirAdministradora()
            Dim context = System.Web.HttpContext.Current
            If context.Session Is Nothing OrElse context.Session("iCodUsuario") Is Nothing Then
                Throw New System.Web.HttpException(401, "Inicia sesión para acceder a Compras.")
            End If
            If Not String.Equals(Convert.ToString(context.Session("Rol")), "Administrador", StringComparison.OrdinalIgnoreCase) Then
                Throw New System.Web.HttpException(403, "Compras está disponible para la administradora.")
            End If
        End Sub

        Private Shared Function LeerArchivo(archivo As ArchivoCompra) As Byte()
            If archivo Is Nothing OrElse String.IsNullOrWhiteSpace(archivo.nombreArchivo) OrElse archivo.nombreArchivo.Length > 255 Then
                Throw New ArgumentException("Selecciona un comprobante válido.")
            End If
            Dim extension = Path.GetExtension(archivo.nombreArchivo).ToLowerInvariant()
            If Not New String() {".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif", ".pdf"}.Contains(extension) Then
                Throw New ArgumentException("Usa un archivo JPG, PNG, BMP, TIFF o PDF.")
            End If
            If String.IsNullOrEmpty(archivo.contenidoBase64) OrElse archivo.contenidoBase64.Length > 14000000 Then
                Throw New ArgumentException("El archivo debe tener un máximo de 10 MB.")
            End If
            Dim content = Convert.FromBase64String(archivo.contenidoBase64)
            If content.Length = 0 OrElse content.Length > ModeloComprobante.LIMITE_BYTES Then
                Throw New ArgumentException("El archivo debe tener entre 1 byte y 10 MB.")
            End If
            Return content
        End Function

        <WebMethod(EnableSession:=True)>
        Public Shared Function ExtraerComprobante(archivo As ArchivoCompra) As Object
            ExigirAdministradora()
            LeerArchivo(archivo)
            Dim serializer As New JavaScriptSerializer With {.MaxJsonLength = 16777216}
            Dim address = System.Configuration.ConfigurationManager.AppSettings("OcrServiceUrl")
            Dim request = DirectCast(WebRequest.Create(address), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json; charset=utf-8"
            request.Timeout = 160000
            request.ReadWriteTimeout = 160000
            Dim body = Encoding.UTF8.GetBytes(serializer.Serialize(New With {archivo.nombreArchivo, archivo.contenidoBase64}))
            request.ContentLength = body.Length
            Try
                Using stream = request.GetRequestStream()
                    stream.Write(body, 0, body.Length)
                End Using
                Using response = request.GetResponse(), reader As New StreamReader(response.GetResponseStream())
                    Return serializer.DeserializeObject(reader.ReadToEnd())
                End Using
            Catch ex As WebException
                Dim message = "No se pudo conectar al servicio OCR. Inícialo en el servidor y reintenta."
                If ex.Response IsNot Nothing Then
                    Using response = ex.Response, reader As New StreamReader(response.GetResponseStream())
                        Try
                            Dim data = serializer.Deserialize(Of Dictionary(Of String, Object))(reader.ReadToEnd())
                            If data.ContainsKey("detail") AndAlso TypeOf data("detail") Is String Then message = CStr(data("detail"))
                        Catch
                        End Try
                    End Using
                End If
                Return New With {.success = False, .mensaje = message}
            End Try
        End Function

        <WebMethod(EnableSession:=True)>
        Public Shared Function GuardarCompra(compra As ModeloCompra, detalles As List(Of ModeloDetalleCompra), comprobante As ArchivoCompra, fechaCompra As String, preciosIncluyenIgv As Boolean) As ResultadoCompra
            ExigirAdministradora()
            Try
                If compra Is Nothing OrElse detalles Is Nothing OrElse detalles.Count = 0 Then Throw New ArgumentException("Agrega los productos de la compra.")
                If compra.iCodCompra <> 0 Then Throw New ArgumentException("Esta compra ya está registrada.")
                compra.iCodUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session("iCodUsuario"))
                compra.cEstado = "REGISTRADA"
                compra.cTipoComprobante = If(compra.cTipoComprobante, "").Trim().ToUpperInvariant()
                compra.cNumeroComprobante = If(compra.cNumeroComprobante, "").Trim().ToUpperInvariant()
                If compra.iCodProveedor <= 0 Then Throw New ArgumentException("Selecciona el proveedor.")
                If Not New String() {"BOLETA", "FACTURA"}.Contains(compra.cTipoComprobante) Then Throw New ArgumentException("Selecciona boleta o factura.")
                If compra.cNumeroComprobante.Length = 0 OrElse compra.cNumeroComprobante.Length > 50 Then Throw New ArgumentException("Completa el número del comprobante del proveedor.")
                Dim fecha As DateTime
                If Not DateTime.TryParseExact(fechaCompra, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, fecha) Then Throw New ArgumentException("Completa una fecha válida.")
                compra.dFechaCompra = fecha
                If compra.nSubTotal < 0 OrElse compra.nIgv < 0 OrElse compra.nTotal <= 0 OrElse compra.nTotal <> compra.nSubTotal + compra.nIgv Then Throw New ArgumentException("Subtotal más IGV debe ser igual al total.")
                Dim suma As Decimal = 0
                Dim ids As New HashSet(Of Integer)
                For Each detalle In detalles
                    If detalle Is Nothing OrElse detalle.iCodProducto <= 0 OrElse detalle.iCantidad <= 0 OrElse detalle.nPrecioCompra < 0 Then Throw New ArgumentException("Revisa los productos, cantidades y precios.")
                    If Not ids.Add(detalle.iCodProducto) Then Throw New ArgumentException("Un producto está repetido. Agrupa sus cantidades en una sola línea.")
                    If Decimal.Round(detalle.nPrecioCompra, 2) <> detalle.nPrecioCompra Then Throw New ArgumentException("Los precios deben tener hasta dos decimales.")
                    suma += Decimal.Round(detalle.iCantidad * detalle.nPrecioCompra, 2, MidpointRounding.AwayFromZero)
                Next
                If Math.Abs(suma - If(preciosIncluyenIgv, compra.nTotal, compra.nSubTotal)) > 0.02D Then Throw New ArgumentException("La suma de los productos no coincide con el comprobante. Revisa las líneas y si los precios incluyen IGV.")
                Dim content As Byte() = Nothing
                If comprobante IsNot Nothing Then content = LeerArchivo(comprobante)
                Using db As New ConexionBD()
                    db.BeginTransaction()
                    Try
                        Dim proveedorActivo = Convert.ToInt32(db.ExecuteScalarTransact("SELECT COUNT(*) FROM Proveedores WHERE iCodProveedor=@id AND bEstado=1", New List(Of SqlParameter) From {New SqlParameter("@id", compra.iCodProveedor)}))
                        If proveedorActivo <> 1 Then Throw New ArgumentException("El proveedor no está activo.")
                        compra.db = db
                        compra.InsertarTransact()
                        ' Orden estable de bloqueos para compras simultáneas.
                        detalles.Sort(Function(a, b) a.iCodProducto.CompareTo(b.iCodProducto))
                        For Each detalle In detalles
                            detalle.iCodCompra = compra.iCodCompra
                            detalle.db = db
                            detalle.InsertarTransact()
                            db.ExecuteQueryTransact(
                                "DECLARE @anterior int; SELECT @anterior=iStockActual FROM Productos WITH (UPDLOCK,HOLDLOCK) WHERE iCodProducto=@producto AND bEstado=1; " &
                                "IF @anterior IS NULL THROW 50001, 'El producto no está activo.', 1; " &
                                "UPDATE Productos SET iStockActual=iStockActual+@cantidad, nPrecioCompra=@precio WHERE iCodProducto=@producto; " &
                                "INSERT INTO Movimientos_Inventario(iCodProducto,iCodUsuario,iCodCompra,cTipoMovimiento,iCantidad,iStockAnterior,iStockNuevo,cMotivo) " &
                                "VALUES(@producto,@usuario,@compra,'ENTRADA',@cantidad,@anterior,@anterior+@cantidad,'Registro de compra');",
                                New List(Of SqlParameter) From {
                                    New SqlParameter("@producto", detalle.iCodProducto), New SqlParameter("@cantidad", detalle.iCantidad),
                                    New SqlParameter("@precio", detalle.nPrecioCompra), New SqlParameter("@usuario", compra.iCodUsuario), New SqlParameter("@compra", compra.iCodCompra)})
                        Next
                        If comprobante IsNot Nothing Then
                            Dim saved As New ModeloComprobante With {
                                .db = db, .iCodCompra = compra.iCodCompra, .iCodVenta = 0,
                                .cNombreArchivo = Path.GetFileName(comprobante.nombreArchivo), .cTipoMime = MimeArchivo(comprobante.nombreArchivo),
                                .nTamanoBytes = content.Length, .imgComprobante = content,
                                .cTextoOcr = comprobante.textoOcr, .cRucEmisor = comprobante.rucEmisor,
                                .cNumeroComprobante = compra.cNumeroComprobante, .cTipoComprobante = compra.cTipoComprobante,
                                .dFechaEmision = fecha, .nSubTotal = compra.nSubTotal, .nIgv = compra.nIgv, .nTotal = compra.nTotal}
                            saved.Insertar()
                        End If
                        db.CommitTransaction()
                    Catch
                        db.RollbackTransaction()
                        Throw
                    End Try
                End Using
                Return New ResultadoCompra With {.ok = True, .idCompra = compra.iCodCompra, .mensaje = "Compra, comprobante e inventario registrados."}
            Catch ex As SqlException
                Return New ResultadoCompra With {.mensaje = If(ex.Number = 2601 OrElse ex.Number = 2627, "Ese comprobante ya está registrado para el proveedor. No se duplicó el inventario.", "No se pudo guardar la compra. No se aplicaron cambios. " & ex.Message)}
            Catch ex As Exception
                Return New ResultadoCompra With {.mensaje = ex.Message}
            End Try
        End Function

        Private Shared Function MimeArchivo(nombre As String) As String
            Select Case Path.GetExtension(nombre).ToLowerInvariant()
                Case ".pdf" : Return "application/pdf"
                Case ".png" : Return "image/png"
                Case ".jpg", ".jpeg" : Return "image/jpeg"
                Case ".bmp" : Return "image/bmp"
                Case Else : Return "image/tiff"
            End Select
        End Function

        Private Sub DescargarArchivoCompra()
            Dim id As Integer
            If Not Integer.TryParse(Request.QueryString("archivoCompra"), id) Then Return
            Using db As New ConexionBD()
                Dim tabla = db.ExecuteDataTable("SELECT TOP (1) cNombreArchivo, imgComprobante FROM Comprobantes_OCR WHERE iCodCompra=@id ORDER BY iCodComprobante DESC", New List(Of SqlParameter) From {New SqlParameter("@id", id)})
                If tabla.Rows.Count = 0 OrElse IsDBNull(tabla.Rows(0)("imgComprobante")) Then
                    Throw New System.Web.HttpException(404, "Esta compra no tiene un archivo digital guardado.")
                End If
                Response.Clear()
                Response.ContentType = "application/octet-stream"
                Response.AddHeader("X-Content-Type-Options", "nosniff")
                Response.AddHeader("Content-Disposition", "attachment; filename*=UTF-8''" & Uri.EscapeDataString(Path.GetFileName(CStr(tabla.Rows(0)("cNombreArchivo")))))
                Response.BinaryWrite(DirectCast(tabla.Rows(0)("imgComprobante"), Byte()))
                Response.End()
            End Using
        End Sub

        Private Shared Sub AnularCompraConInventario(id As Integer)
            Using db As New ConexionBD()
                db.BeginTransaction()
                Try
                    Dim estado = Convert.ToString(db.ExecuteScalarTransact("SELECT cEstado FROM Compras WITH(UPDLOCK,HOLDLOCK) WHERE iCodCompra=@id", New List(Of SqlParameter) From {New SqlParameter("@id", id)}))
                    If estado = "" Then Throw New ArgumentException("La compra no existe.")
                    If estado = "ANULADA" Then
                        db.CommitTransaction()
                        Return
                    End If
                    ' Sólo revertir entradas registradas: compras antiguas podrían no tener movimiento.
                    Dim entradas = db.ExecuteDataTableTransact("SELECT iCodProducto, SUM(iCantidad) AS cantidad FROM Movimientos_Inventario WHERE iCodCompra=@id AND cTipoMovimiento='ENTRADA' GROUP BY iCodProducto ORDER BY iCodProducto", New List(Of SqlParameter) From {New SqlParameter("@id", id)})
                    For Each fila As DataRow In entradas.Rows
                        db.ExecuteQueryTransact(
                            "DECLARE @anterior int; SELECT @anterior=iStockActual FROM Productos WITH(UPDLOCK,HOLDLOCK) WHERE iCodProducto=@producto; " &
                            "IF @anterior < @cantidad THROW 50002, 'No se puede anular: parte del stock ya se vendió o retiró.', 1; " &
                            "UPDATE Productos SET iStockActual=iStockActual-@cantidad WHERE iCodProducto=@producto; " &
                            "INSERT INTO Movimientos_Inventario(iCodProducto,iCodUsuario,cTipoMovimiento,iCantidad,iStockAnterior,iStockNuevo,cMotivo) " &
                            "VALUES(@producto,@usuario,'AJUSTE_SALIDA',@cantidad,@anterior,@anterior-@cantidad,@motivo);",
                            New List(Of SqlParameter) From {
                                New SqlParameter("@producto", fila("iCodProducto")), New SqlParameter("@cantidad", fila("cantidad")),
                                New SqlParameter("@usuario", Convert.ToInt32(System.Web.HttpContext.Current.Session("iCodUsuario"))),
                                New SqlParameter("@motivo", "Anulación de compra " & id.ToString())})
                    Next
                    db.ExecuteQueryTransact("UPDATE Compras SET cEstado='ANULADA' WHERE iCodCompra=@id", New List(Of SqlParameter) From {New SqlParameter("@id", id)})
                    db.CommitTransaction()
                Catch
                    db.RollbackTransaction()
                    Throw
                End Try
            End Using
        End Sub
    End Class
End Namespace
