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

        Public Class ProveedorEdicionCompra
            Public id As Integer
            Public ruc As String
            Public razonSocial As String
            Public representante As String
            Public telefono As String
            Public correo As String
            Public direccion As String
        End Class

        <WebMethod(EnableSession:=True)>
        Public Shared Function ObtenerProveedorCompra(idProveedor As Integer) As ProveedorEdicionCompra
            ExigirAdministradora()
            Using db As New ConexionBD()
                Dim tabla = db.ExecuteDataTable("SELECT * FROM Proveedores WHERE iCodProveedor=@id AND bEstado=1", New List(Of SqlParameter) From {New SqlParameter("@id", idProveedor)})
                If tabla.Rows.Count <> 1 Then Throw New ArgumentException("El proveedor no está activo.")
                Dim r = tabla.Rows(0)
                Return New ProveedorEdicionCompra With {.id = idProveedor, .ruc = Convert.ToString(r("cRuc")), .razonSocial = Convert.ToString(r("cRazonSocial")), .representante = Convert.ToString(r("cRepresentante")), .telefono = Convert.ToString(r("cTelefono")), .correo = Convert.ToString(r("cCorreo")), .direccion = Convert.ToString(r("cDireccion"))}
            End Using
        End Function

        <WebMethod(EnableSession:=True)>
        Public Shared Function ActualizarProveedorCompra(proveedor As ProveedorEdicionCompra) As ResultadoCompra
            ExigirAdministradora()
            Try
                If proveedor Is Nothing OrElse proveedor.id <= 0 Then Throw New ArgumentException("Selecciona un proveedor.")
                proveedor.ruc = If(proveedor.ruc, "").Trim()
                proveedor.razonSocial = If(proveedor.razonSocial, "").Trim()
                proveedor.telefono = If(proveedor.telefono, "").Trim()
                proveedor.correo = If(proveedor.correo, "").Trim()
                proveedor.representante = If(proveedor.representante, "").Trim()
                proveedor.direccion = If(proveedor.direccion, "").Trim()
                If Not System.Text.RegularExpressions.Regex.IsMatch(proveedor.ruc, "^[0-9]{11}$") Then Throw New ArgumentException("El RUC debe tener 11 dígitos.")
                If proveedor.razonSocial.Length = 0 OrElse proveedor.razonSocial.Length > 150 OrElse proveedor.representante.Length > 100 OrElse proveedor.correo.Length > 100 OrElse proveedor.direccion.Length > 150 Then Throw New ArgumentException("Revisa la razón social y la longitud de los campos.")
                If proveedor.telefono <> "" AndAlso Not System.Text.RegularExpressions.Regex.IsMatch(proveedor.telefono, "^[0-9]{9}$") Then Throw New ArgumentException("El teléfono debe tener 9 dígitos.")
                If proveedor.correo <> "" Then
                    Dim direccionCorreo As New System.Net.Mail.MailAddress(proveedor.correo)
                    If direccionCorreo.Address <> proveedor.correo Then Throw New ArgumentException("Revisa el correo.")
                End If
                Using db As New ConexionBD()
                    Dim actualizado = Convert.ToInt32(db.ExecuteScalar(
                        "UPDATE Proveedores SET cRuc=@ruc,cRazonSocial=@razon,cRepresentante=@representante,cTelefono=NULLIF(@telefono,''),cCorreo=NULLIF(@correo,''),cDireccion=@direccion WHERE iCodProveedor=@id AND bEstado=1; SELECT @@ROWCOUNT;",
                        New List(Of SqlParameter) From {New SqlParameter("@id", proveedor.id), New SqlParameter("@ruc", proveedor.ruc), New SqlParameter("@razon", proveedor.razonSocial), New SqlParameter("@representante", proveedor.representante), New SqlParameter("@telefono", proveedor.telefono), New SqlParameter("@correo", proveedor.correo), New SqlParameter("@direccion", proveedor.direccion)}))
                    If actualizado <> 1 Then Throw New ArgumentException("El proveedor ya no está activo. Actualiza la página.")
                End Using
                Return New ResultadoCompra With {.ok = True, .mensaje = "Proveedor actualizado."}
            Catch ex As SqlException
                Return New ResultadoCompra With {.mensaje = If(ex.Number = 2601 OrElse ex.Number = 2627, "Ese RUC ya pertenece a otro proveedor.", "No se pudo actualizar el proveedor.")}
            Catch ex As Exception
                Return New ResultadoCompra With {.mensaje = ex.Message}
            End Try
        End Function

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
            Try
                OcrServiceHost.EnsureRunning(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, System.Web.HttpContext.Current.IsDebuggingEnabled)
            Catch ex As Exception
                Return New With {.success = False, .mensaje = ex.Message}
            End Try
            Dim serializer As New JavaScriptSerializer With {.MaxJsonLength = 16777216}
            Dim address As New Uri(System.Configuration.ConfigurationManager.AppSettings("OcrServiceUrl"))
            Try
                Using response = EnviarJsonOcr(address, serializer.Serialize(New With {archivo.nombreArchivo, archivo.contenidoBase64})),
                      reader As New StreamReader(response.GetResponseStream())
                    Dim resultado = serializer.Deserialize(Of Dictionary(Of String, Object))(reader.ReadToEnd())
                    Dim datosObjeto As Object = Nothing
                    If resultado.TryGetValue("datos", datosObjeto) Then
                        Dim datos = TryCast(datosObjeto, Dictionary(Of String, Object))
                        If datos IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Convert.ToString(datos("razon_social"))) Then
                            datos("razon_social") = System.Configuration.ConfigurationManager.AppSettings("OcrProveedor." & Convert.ToString(datos("ruc_emisor")))
                        End If
                    End If
                    Return resultado
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

        Private Shared Function EnviarJsonOcr(address As Uri, json As String, Optional timeout As Integer = 160000) As WebResponse
            Dim request = DirectCast(WebRequest.Create(address), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json; charset=utf-8"
            request.Timeout = timeout
            request.ReadWriteTimeout = timeout
            Dim body = Encoding.UTF8.GetBytes(json)
            request.ContentLength = body.Length
            Using stream = request.GetRequestStream()
                stream.Write(body, 0, body.Length)
            End Using
            Return request.GetResponse()
        End Function

        Private Shared Function DocumentoPdf(nombre As String, content As Byte(), Optional base64 As String = Nothing) As Byte()
            If String.Equals(Path.GetExtension(nombre), ".pdf", StringComparison.OrdinalIgnoreCase) Then
                Return ValidarPdf(content)
            End If
            Return SolicitarPdfOcr("../documento/pdf", New With {.nombreArchivo = nombre, .contenidoBase64 = If(base64, Convert.ToBase64String(content))})
        End Function

        Private Shared Function ValidarPdf(content As Byte()) As Byte()
            If content.Length < 5 OrElse Encoding.ASCII.GetString(content, 0, 5) <> "%PDF-" Then Throw New ArgumentException("El servicio no devolvió un PDF válido.")
            Return content
        End Function

        Private Shared Function SolicitarPdfOcr(ruta As String, datos As Object) As Byte()
            Dim context = System.Web.HttpContext.Current
            OcrServiceHost.EnsureRunning(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, context.IsDebuggingEnabled)
            Dim ocr As New Uri(System.Configuration.ConfigurationManager.AppSettings("OcrServiceUrl"))
            Dim address As New Uri(ocr, ruta)
            Dim serializer As New JavaScriptSerializer With {.MaxJsonLength = 16777216}
            Try
                Using response = EnviarJsonOcr(address, serializer.Serialize(datos), 15000),
                      stream = response.GetResponseStream(),
                      document As New MemoryStream()
                    If response.ContentType <> "application/pdf" Then Throw New ArgumentException("El servicio no devolvió un comprobante PDF.")
                    Dim buffer(8191) As Byte
                    Dim count = stream.Read(buffer, 0, buffer.Length)
                    While count > 0
                        If document.Length + count > ModeloComprobante.LIMITE_BYTES Then Throw New ArgumentException("El PDF generado supera los 10 MB.")
                        document.Write(buffer, 0, count)
                        count = stream.Read(buffer, 0, buffer.Length)
                    End While
                    Return ValidarPdf(document.ToArray())
                End Using
            Catch ex As WebException
                If ex.Response IsNot Nothing Then
                    Using response = DirectCast(ex.Response, HttpWebResponse)
                        If response.StatusCode = HttpStatusCode.NotFound Then
                            Throw New ArgumentException("El servicio OCR que está en ejecución no admite esta operación PDF. Detén el OCR anterior e inicia el de la carpeta actualizada (servicio_ocr/start.bat).", ex)
                        End If
                        If If(response.ContentType, "").StartsWith("application/json", StringComparison.OrdinalIgnoreCase) Then
                            Using reader As New StreamReader(response.GetResponseStream())
                                Dim message = TryCast(serializer.DeserializeObject(reader.ReadToEnd()), Dictionary(Of String, Object))
                                If message IsNot Nothing AndAlso message.ContainsKey("detail") AndAlso TypeOf message("detail") Is String Then
                                    Throw New ArgumentException(CStr(message("detail")), ex)
                                End If
                            End Using
                        End If
                    End Using
                End If
                Throw New ArgumentException("No se pudo generar el PDF. Verifica el servicio OCR y vuelve a intentar.", ex)
            End Try
        End Function

        <WebMethod(EnableSession:=True)>
        Public Shared Function GuardarCompra(compra As ModeloCompra, detalles As List(Of ModeloDetalleCompra), comprobante As ArchivoCompra, fechaCompra As String, preciosIncluyenIgv As Boolean) As ResultadoCompra
            If detalles IsNot Nothing AndAlso detalles.Any(Function(d) d IsNot Nothing AndAlso d.precioVenta.HasValue) Then
                Return GuardarCompraConAltas(compra, detalles, comprobante, fechaCompra, preciosIncluyenIgv, Nothing, Nothing)
            End If
            Return GuardarCompraInterna(compra, detalles, comprobante, fechaCompra, preciosIncluyenIgv, Nothing)
        End Function

        Private Shared Function GuardarCompraInterna(compra As ModeloCompra, detalles As List(Of ModeloDetalleCompra), comprobante As ArchivoCompra, fechaCompra As String, preciosIncluyenIgv As Boolean, transaccion As ConexionBD) As ResultadoCompra
            ExigirAdministradora()
            Try
                If transaccion Is Nothing Then
                    Using dbInfraestructura As New ConexionBD()
                        GarantizarInfraestructuraCompra(dbInfraestructura)
                    End Using
                End If
                If compra Is Nothing OrElse detalles Is Nothing OrElse detalles.Count = 0 Then Throw New ArgumentException("Agrega los productos de la compra.")
                If compra.iCodCompra <> 0 Then Throw New ArgumentException("Esta compra ya está registrada.")
                compra.iCodUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session("iCodUsuario"))
                compra.cEstado = "REGISTRADA"
                compra.cTipoComprobante = If(compra.cTipoComprobante, "").Trim().ToUpperInvariant()
                compra.cNumeroComprobante = If(compra.cNumeroComprobante, "").Trim().ToUpperInvariant()
                If compra.iCodProveedor <= 0 Then Throw New ArgumentException("Selecciona el proveedor.")
                If Not New String() {"BOLETA", "FACTURA", "PEDIDO", "TICKET", "NOTA_VENTA", "OTRO"}.Contains(compra.cTipoComprobante) Then Throw New ArgumentException("Selecciona un tipo de documento válido.")
                If compra.cNumeroComprobante.Length = 0 OrElse compra.cNumeroComprobante.Length > 50 Then Throw New ArgumentException("Completa el número del comprobante del proveedor.")
                Dim fecha As DateTime
                If Not DateTime.TryParseExact(fechaCompra, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, fecha) Then Throw New ArgumentException("Completa una fecha válida.")
                compra.dFechaCompra = fecha
                If compra.nSubTotal < 0 OrElse compra.nIgv < 0 OrElse compra.nTotal <= 0 OrElse compra.nTotal <> compra.nSubTotal + compra.nIgv Then Throw New ArgumentException("Subtotal más IGV debe ser igual al total.")
                If Decimal.Round(compra.nSubTotal, 2) <> compra.nSubTotal OrElse Decimal.Round(compra.nIgv, 2) <> compra.nIgv OrElse Decimal.Round(compra.nTotal, 2) <> compra.nTotal Then Throw New ArgumentException("Los importes del comprobante deben tener hasta dos decimales.")
                If If(compra.cObservacion, "").Length > 300 Then Throw New ArgumentException("La observación admite hasta 300 caracteres.")
                Dim suma As Decimal = 0
                Dim ids As New HashSet(Of Integer)
                For Each detalle In detalles
                    If detalle Is Nothing OrElse detalle.iCodProducto <= 0 OrElse detalle.iCantidad <= 0 OrElse detalle.nPrecioCompra < 0 Then Throw New ArgumentException("Revisa los productos, cantidades y precios.")
                    If Not ids.Add(detalle.iCodProducto) Then Throw New ArgumentException("Un producto está repetido. Agrupa sus cantidades en una sola línea.")
                    If Decimal.Round(detalle.nPrecioCompra, 2) <> detalle.nPrecioCompra Then Throw New ArgumentException("Los precios deben tener hasta dos decimales.")
                    If detalle.precioVenta.HasValue AndAlso (detalle.precioVenta.Value < 0 OrElse Decimal.Round(detalle.precioVenta.Value, 2) <> detalle.precioVenta.Value OrElse detalle.precioVenta.Value > 9999999999999999.99D) Then Throw New ArgumentException("El precio de venta debe ser no negativo y tener hasta dos decimales.")
                    suma += Decimal.Round(detalle.iCantidad * detalle.nPrecioCompra, 2, MidpointRounding.AwayFromZero)
                Next
                If Math.Abs(suma - If(preciosIncluyenIgv, compra.nTotal, compra.nSubTotal)) > 0.02D Then Throw New ArgumentException("La suma de los productos no coincide con el comprobante. Revisa las líneas y si los precios incluyen IGV.")
                Dim content As Byte() = Nothing
                Dim nombrePdf As String = Nothing
                If comprobante IsNot Nothing Then
                    content = DocumentoPdf(comprobante.nombreArchivo, LeerArchivo(comprobante), comprobante.contenidoBase64)
                    nombrePdf = Path.ChangeExtension(Path.GetFileName(comprobante.nombreArchivo), ".pdf")
                End If
                Dim tabla As New DataTable()
                tabla.Columns.Add("iCodProducto", GetType(Integer))
                tabla.Columns.Add("iCantidad", GetType(Integer))
                tabla.Columns.Add("nPrecioCompra", GetType(Decimal))
                For Each detalle In detalles
                    tabla.Rows.Add(detalle.iCodProducto, detalle.iCantidad, detalle.nPrecioCompra)
                Next
                Dim parametros As New List(Of SqlParameter) From {
                    New SqlParameter("@iCodProveedor", compra.iCodProveedor),
                    New SqlParameter("@iCodUsuario", compra.iCodUsuario),
                    New SqlParameter("@cTipoComprobante", SqlDbType.NVarChar, 30) With {.Value = compra.cTipoComprobante},
                    New SqlParameter("@cNumeroComprobante", SqlDbType.NVarChar, 50) With {.Value = compra.cNumeroComprobante},
                    New SqlParameter("@dFechaCompra", SqlDbType.Date) With {.Value = fecha},
                    New SqlParameter("@nSubTotal", SqlDbType.Decimal) With {.Precision = 18, .Scale = 2, .Value = compra.nSubTotal},
                    New SqlParameter("@nIgv", SqlDbType.Decimal) With {.Precision = 18, .Scale = 2, .Value = compra.nIgv},
                    New SqlParameter("@nTotal", SqlDbType.Decimal) With {.Precision = 18, .Scale = 2, .Value = compra.nTotal},
                    New SqlParameter("@bPreciosIncluyenIgv", SqlDbType.Bit) With {.Value = preciosIncluyenIgv},
                    New SqlParameter("@Detalles", SqlDbType.Structured) With {.TypeName = "dbo.TipoDetalleCompra", .Value = tabla},
                    New SqlParameter("@cObservacion", SqlDbType.NVarChar, 300) With {.Value = If(CType(compra.cObservacion, Object), DBNull.Value)},
                    New SqlParameter("@cNombreArchivo", SqlDbType.NVarChar, 255) With {.Value = If(CType(nombrePdf, Object), DBNull.Value)},
                    New SqlParameter("@cTipoMime", SqlDbType.NVarChar, 100) With {.Value = If(comprobante Is Nothing, DBNull.Value, CType("application/pdf", Object))},
                    New SqlParameter("@cTextoOcr", SqlDbType.NVarChar, -1) With {.Value = If(comprobante Is Nothing, DBNull.Value, If(CType(comprobante.textoOcr, Object), DBNull.Value))},
                    New SqlParameter("@cRucEmisor", SqlDbType.NVarChar, 20) With {.Value = If(comprobante Is Nothing, DBNull.Value, If(CType(comprobante.rucEmisor, Object), DBNull.Value))},
                    New SqlParameter("@imgComprobante", SqlDbType.VarBinary, -1) With {.Value = If(CType(content, Object), DBNull.Value)}}
                Dim sql = "EXEC dbo.sp_RegistrarCompra @iCodProveedor,@iCodUsuario,@cTipoComprobante,@cNumeroComprobante,@dFechaCompra,@nSubTotal,@nIgv,@nTotal,@bPreciosIncluyenIgv,@Detalles,@cObservacion,@cNombreArchivo,@cTipoMime,@cTextoOcr,@cRucEmisor,@imgComprobante"
                If transaccion Is Nothing Then
                    Using db As New ConexionBD()
                        compra.iCodCompra = Convert.ToInt32(db.ExecuteScalar(sql, parametros))
                    End Using
                Else
                    compra.iCodCompra = Convert.ToInt32(transaccion.ExecuteScalarTransact(sql, parametros))
                End If
                Return New ResultadoCompra With {.ok = True, .idCompra = compra.iCodCompra, .mensaje = "Compra, comprobante e inventario registrados."}
            Catch ex As SqlException
                Return New ResultadoCompra With {.mensaje = If(ex.Number = 2601 OrElse ex.Number = 2627, "Ese comprobante ya está registrado para el proveedor. No se duplicó el inventario.", "No se pudo guardar la compra. No se aplicaron cambios. " & ex.Message)}
            Catch ex As Exception
                Return New ResultadoCompra With {.mensaje = ex.Message}
            End Try
        End Function

        Private Sub MostrarDocumentoCompra()
            Dim id As Integer
            If Not Integer.TryParse(Request.QueryString("archivoCompra"), id) OrElse id <= 0 Then
                Throw New System.Web.HttpException(404, "La compra no existe.")
            End If
            Using db As New ConexionBD()
                Dim tabla = db.ExecuteDataTable("SELECT TOP (1) cNombreArchivo, imgComprobante FROM Comprobantes_OCR WHERE iCodCompra=@id ORDER BY iCodComprobante DESC", New List(Of SqlParameter) From {New SqlParameter("@id", id)})
                If tabla.Rows.Count = 0 OrElse IsDBNull(tabla.Rows(0)("imgComprobante")) Then
                    Throw New System.Web.HttpException(404, "Esta compra no tiene un archivo digital guardado.")
                End If
                Dim nombre = Path.GetFileName(Convert.ToString(tabla.Rows(0)("cNombreArchivo")))
                Dim content = DirectCast(tabla.Rows(0)("imgComprobante"), Byte())
                If content.Length = 0 Then Throw New System.Web.HttpException(404, "El archivo guardado está vacío.")
                Dim extension = Path.GetExtension(nombre).ToLowerInvariant()
                If content.Length >= 5 AndAlso Encoding.ASCII.GetString(content, 0, 5) = "%PDF-" Then
                    EnviarDocumentoAlNavegador(Path.ChangeExtension(If(nombre = "", "comprobante", nombre), ".pdf"), content)
                    Return
                End If
                Dim tipo As String
                Select Case extension
                    Case ".jpg", ".jpeg" : tipo = "image/jpeg"
                    Case ".png" : tipo = "image/png"
                    Case ".bmp" : tipo = "image/bmp"
                    Case ".tif", ".tiff" : tipo = "image/tiff"
                    Case Else : Throw New System.Web.HttpException(415, "El formato del comprobante guardado no es válido.")
                End Select
                EnviarDocumentoAlNavegador(nombre, content, tipo, tipo <> "image/tiff")
            End Using
        End Sub

        Private Sub MostrarResumenCompra()
            Dim id As Integer
            If Not Integer.TryParse(Request.QueryString("resumenCompra"), id) OrElse id <= 0 Then
                Throw New System.Web.HttpException(404, "La compra no existe.")
            End If
            Dim compra As Object
            Using db As New ConexionBD()
                Dim cabecera = db.ExecuteDataTable(
                    "SELECT c.cTipoComprobante, c.cNumeroComprobante, c.dFechaCompra, c.cEstado, c.cObservacion, " &
                    "c.nSubTotal, c.nIgv, c.nTotal, p.cRuc, p.cRazonSocial FROM Compras c " &
                    "INNER JOIN Proveedores p ON p.iCodProveedor=c.iCodProveedor WHERE c.iCodCompra=@id",
                    New List(Of SqlParameter) From {New SqlParameter("@id", id)})
                If cabecera.Rows.Count = 0 Then Throw New System.Web.HttpException(404, "La compra no existe.")
                Dim productos = db.ExecuteDataTable(
                    "SELECT p.cNombre, p.cCodigo, p.cMarca, p.cModelo, p.nPrecioVenta, d.iCantidad, d.nPrecioCompra, d.nSubTotal, " &
                    "m.iStockAnterior, m.iStockNuevo FROM Detalle_Compra d INNER JOIN Productos p ON p.iCodProducto=d.iCodProducto " &
                    "OUTER APPLY (SELECT TOP (1) mi.iStockAnterior, mi.iStockNuevo FROM Movimientos_Inventario mi " &
                    "WHERE mi.iCodCompra=d.iCodCompra AND mi.iCodProducto=d.iCodProducto AND mi.cTipoMovimiento='ENTRADA' " &
                    "AND mi.iCantidad=d.iCantidad ORDER BY mi.iCodMovimiento) m WHERE d.iCodCompra=@id ORDER BY d.iCodDetalleCompra",
                    New List(Of SqlParameter) From {New SqlParameter("@id", id)})
                Dim fila = cabecera.Rows(0)
                compra = New With {
                    .id = id,
                    .proveedor = Convert.ToString(fila("cRazonSocial")),
                    .ruc = Convert.ToString(fila("cRuc")),
                    .tipo = Convert.ToString(fila("cTipoComprobante")),
                    .numero = Convert.ToString(fila("cNumeroComprobante")),
                    .fecha = Convert.ToDateTime(fila("dFechaCompra")).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    .estado = Convert.ToString(fila("cEstado")),
                    .observacion = Convert.ToString(fila("cObservacion")),
                    .subtotal = Convert.ToDecimal(fila("nSubTotal")),
                    .igv = Convert.ToDecimal(fila("nIgv")),
                    .total = Convert.ToDecimal(fila("nTotal")),
                    .productos = productos.AsEnumerable().Select(Function(p) New With {
                        .nombre = Convert.ToString(p("cNombre")),
                        .referencia = String.Join(" · ", {Convert.ToString(p("cMarca")), Convert.ToString(p("cModelo")), Convert.ToString(p("cCodigo"))}.Where(Function(s) Not String.IsNullOrWhiteSpace(s))),
                        .cantidad = Convert.ToInt32(p("iCantidad")),
                        .precioCompra = Convert.ToDecimal(p("nPrecioCompra")),
                        .precioVentaActual = p.Field(Of Decimal?)("nPrecioVenta"),
                        .importe = Convert.ToDecimal(p("nSubTotal")),
                        .stockAnterior = p.Field(Of Integer?)("iStockAnterior"),
                        .stockNuevo = p.Field(Of Integer?)("iStockNuevo")
                    }).ToArray()
                }
            End Using
            EnviarDocumentoAlNavegador("compra-" & id.ToString(CultureInfo.InvariantCulture) & ".pdf", SolicitarPdfOcr("../compra/resumen/pdf", compra))
        End Sub

        Private Sub EnviarDocumentoAlNavegador(nombre As String, content As Byte(), Optional tipo As String = "application/pdf", Optional mostrar As Boolean = True)
            Response.Clear()
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache)
            Response.Cache.SetNoStore()
            Response.ContentType = tipo
            Response.AddHeader("X-Content-Type-Options", "nosniff")
            Response.AddHeader("Content-Disposition", If(mostrar, "inline", "attachment") & "; filename*=UTF-8''" & Uri.EscapeDataString(nombre))
            Response.BinaryWrite(content)
            Response.End()
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
