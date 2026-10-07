Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Data
Imports System.Data.SqlClient

Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports WebApplication_keyove.WebApplication_Keyove.Data
Imports ModeloComprobanteOCR = WebApplication_keyove.WebApplication_Keyove.Model.ComprobantesOCR
Imports ModeloDetalleVenta = WebApplication_keyove.WebApplication_Keyove.Model.DetalleVenta
Imports ModeloProducto = WebApplication_keyove.WebApplication_Keyove.Model.Productos
Imports ModeloUsuario = WebApplication_keyove.WebApplication_Keyove.Model.Usuarios
Imports ModeloVenta = WebApplication_keyove.WebApplication_Keyove.Model.Ventas

Namespace WebApplication_Keyove.Views.Operaciones.Ventas

    Partial Public Class Venta1
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(
            ByVal sender As Object,
            ByVal e As Global.System.EventArgs
        ) Handles Me.Load

        End Sub


        '==================================================
        ' OPCIÓN PARA COMBOBOX
        '==================================================

        Public Class OpcionCombo

            Public v As String
            Public t As String

        End Class


        '==================================================
        ' LISTAR USUARIOS PARA COMBOBOX
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarUsuariosCombo() As List(Of OpcionCombo)

            Dim sesionActual =
                System.Web.HttpContext.Current.Session

            If sesionActual Is Nothing OrElse
                sesionActual("iCodUsuario") Is Nothing Then

                Throw New System.Web.HttpException(
                    401,
                    "Debe iniciar sesión."
                )

            End If


            If String.Equals(
                Convert.ToString(
                    sesionActual("Rol")
                ).Trim(),
                "Vendedor",
                StringComparison.OrdinalIgnoreCase
            ) Then

                Return New List(Of OpcionCombo) From {
                    New OpcionCombo With {
                        .v = Convert.ToString(
                            sesionActual("iCodUsuario")
                        ),
                        .t = Convert.ToString(
                            sesionActual("Usuario")
                        )
                    }
                }

            End If


            Dim lista As New List(Of OpcionCombo)()


            Dim tabla As DataTable =
                New ModeloUsuario().ListaDatosCombo()


            For Each fila As DataRow In tabla.Rows

                Dim opcion As New OpcionCombo()


                opcion.v =
                    Convert.ToString(
                        fila("ValueMember")
                    )


                opcion.t =
                    Convert.ToString(
                        fila("DisplayMember")
                    )


                lista.Add(opcion)

            Next


            Return lista

        End Function


        '==================================================
        ' LISTAR PRODUCTOS PARA COMBOBOX
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarProductosCombo() As List(Of OpcionCombo)

            Dim lista As New List(Of OpcionCombo)()


            Dim tabla As DataTable =
                New ModeloProducto().ListaDatosShort()


            For Each fila As DataRow In tabla.Rows

                If Convert.ToBoolean(
                    fila("bEstado")
                ) Then

                    Dim opcion As New OpcionCombo()


                    opcion.v =
                        Convert.ToString(
                            fila("iCodProducto")
                        )


                    opcion.t =
                        Convert.ToString(
                            fila("cNombre")
                        )


                    lista.Add(opcion)

                End If

            Next


            Return lista

        End Function


        '==================================================
        ' LISTAR VENTAS
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarVentas() As List(Of ModeloVenta)

            Dim lista As New List(Of ModeloVenta)()


            Dim tabla As DataTable =
                New ModeloVenta().ListaDatosShort()


            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloVenta()


                obj.iCodVenta =
                    Convert.ToInt32(
                        fila("iCodVenta")
                    )


                obj.iCodUsuario =
                    Convert.ToInt32(
                        fila("iCodUsuario")
                    )


                obj.cNombreUsuario =
                    Convert.ToString(
                        fila("Usuario")
                    )


                obj.cTipoComprobante =
                    Convert.ToString(
                        fila("cTipoComprobante")
                    )


                If IsDBNull(
                    fila("cDocumentoCliente")
                ) Then

                    obj.cDocumentoCliente =
                        Nothing

                Else

                    obj.cDocumentoCliente =
                        Convert.ToString(
                            fila("cDocumentoCliente")
                        )

                End If


                If IsDBNull(
                    fila("cNumeroCelular")
                ) Then

                    obj.cNumeroCelular =
                        Nothing

                Else

                    obj.cNumeroCelular =
                        Convert.ToString(
                            fila("cNumeroCelular")
                        )

                End If


                obj.cNumeroComprobante =
                    Convert.ToString(
                        fila("cNumeroComprobante")
                    )


                obj.dFechaVenta =
                    Convert.ToDateTime(
                        fila("dFechaVenta")
                    )


                obj.nSubTotal =
                    Convert.ToDecimal(
                        fila("nSubTotal")
                    )


                obj.nIgv =
                    Convert.ToDecimal(
                        fila("nIgv")
                    )


                obj.nTotal =
                    Convert.ToDecimal(
                        fila("nTotal")
                    )


                obj.cMetodoPago =
                    Convert.ToString(
                        fila("cMetodoPago")
                    )


                If IsDBNull(
                    fila("cObservacion")
                ) Then

                    obj.cObservacion =
                        Nothing

                Else

                    obj.cObservacion =
                        Convert.ToString(
                            fila("cObservacion")
                        )

                End If


                obj.cEstado =
                    Convert.ToString(
                        fila("cEstado")
                    )


                lista.Add(obj)

            Next


            Return lista

        End Function


        '==================================================
        ' GUARDAR VENTA
        '
        ' AHORA UTILIZA:
        ' sp_RegistrarVenta
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function GuardarVenta(
            venta As ModeloVenta,
            detalles As List(Of ModeloDetalleVenta)
        ) As String

            Try

                '------------------------------------------
                ' VALIDAR SESIÓN
                '------------------------------------------

                Dim sesionActual =
                    System.Web.HttpContext.Current.Session


                If sesionActual Is Nothing OrElse
                    sesionActual("iCodUsuario") Is Nothing Then

                    Return "ERROR: Debe iniciar sesión."

                End If


                '------------------------------------------
                ' VALIDAR VENTA
                '------------------------------------------

                If venta Is Nothing Then

                    Return "ERROR: Los datos de la venta son obligatorios."

                End If


                '------------------------------------------
                ' VALIDAR DETALLES
                '------------------------------------------

                If detalles Is Nothing OrElse
                    detalles.Count = 0 Then

                    Return "ERROR: Agregue al menos un producto."

                End If


                '------------------------------------------
                ' OBTENER USUARIO DE LA SESIÓN
                '------------------------------------------

                venta.iCodUsuario =
                    Convert.ToInt32(
                        sesionActual("iCodUsuario")
                    )


                '------------------------------------------
                ' CREAR DATATABLE
                '
                ' DEBE COINCIDIR CON:
                '
                ' TipoDetalleVenta
                '------------------------------------------

                Dim tablaDetalles As New DataTable()


                tablaDetalles.Columns.Add(
                    "iCodProducto",
                    GetType(Integer)
                )


                tablaDetalles.Columns.Add(
                    "iCantidad",
                    GetType(Integer)
                )


                tablaDetalles.Columns.Add(
                    "nPrecioVenta",
                    GetType(Decimal)
                )


                tablaDetalles.Columns.Add(
                    "nDescuento",
                    GetType(Decimal)
                )


                '------------------------------------------
                ' CARGAR DETALLES
                '------------------------------------------

                For Each detalle As ModeloDetalleVenta In detalles

                    If detalle Is Nothing Then

                        Return "ERROR: Existe un detalle de venta inválido."

                    End If


                    Dim fila As DataRow =
                        tablaDetalles.NewRow()


                    fila("iCodProducto") =
                        detalle.iCodProducto


                    fila("iCantidad") =
                        detalle.iCantidad


                    fila("nPrecioVenta") =
                        detalle.nPrecioVenta


                    fila("nDescuento") =
                        detalle.nDescuento


                    tablaDetalles.Rows.Add(fila)

                Next


                '------------------------------------------
                ' CREAR PARÁMETROS
                '------------------------------------------

                Dim parametros As New List(Of SqlParameter)()


                '------------------------------------------
                ' iCodUsuario
                '------------------------------------------

                parametros.Add(
                    New SqlParameter(
                        "@iCodUsuario",
                        SqlDbType.Int
                    ) With {
                        .Value =
                            venta.iCodUsuario
                    }
                )


                '------------------------------------------
                ' cTipoComprobante
                '------------------------------------------

                parametros.Add(
                    New SqlParameter(
                        "@cTipoComprobante",
                        SqlDbType.NVarChar,
                        30
                    ) With {
                        .Value =
                            If(
                                String.IsNullOrWhiteSpace(
                                    venta.cTipoComprobante
                                ),
                                "BOLETA",
                                venta.cTipoComprobante
                            )
                    }
                )


                '------------------------------------------
                ' cDocumentoCliente
                '------------------------------------------

                Dim parametroDocumento As New SqlParameter(
                    "@cDocumentoCliente",
                    SqlDbType.NVarChar,
                    11
                )


                If String.IsNullOrWhiteSpace(
                    venta.cDocumentoCliente
                ) Then

                    parametroDocumento.Value =
                        DBNull.Value

                Else

                    parametroDocumento.Value =
                        venta.cDocumentoCliente

                End If


                parametros.Add(
                    parametroDocumento
                )


                '------------------------------------------
                ' cNumeroCelular
                '------------------------------------------

                Dim parametroCelular As New SqlParameter(
                    "@cNumeroCelular",
                    SqlDbType.VarChar,
                    9
                )


                If String.IsNullOrWhiteSpace(
                    venta.cNumeroCelular
                ) Then

                    parametroCelular.Value =
                        DBNull.Value

                Else

                    parametroCelular.Value =
                        venta.cNumeroCelular

                End If


                parametros.Add(
                    parametroCelular
                )


                '------------------------------------------
                ' cNumeroComprobante
                '------------------------------------------

                parametros.Add(
                    New SqlParameter(
                        "@cNumeroComprobante",
                        SqlDbType.NVarChar,
                        50
                    ) With {
                        .Value =
                            venta.cNumeroComprobante
                    }
                )


                '------------------------------------------
                ' cMetodoPago
                '------------------------------------------

                parametros.Add(
                    New SqlParameter(
                        "@cMetodoPago",
                        SqlDbType.NVarChar,
                        50
                    ) With {
                        .Value =
                            If(
                                String.IsNullOrWhiteSpace(
                                    venta.cMetodoPago
                                ),
                                "EFECTIVO",
                                venta.cMetodoPago
                            )
                    }
                )


                '------------------------------------------
                ' cObservacion
                '------------------------------------------

                Dim parametroObservacion As New SqlParameter(
                    "@cObservacion",
                    SqlDbType.NVarChar,
                    300
                )


                If String.IsNullOrWhiteSpace(
                    venta.cObservacion
                ) Then

                    parametroObservacion.Value =
                        DBNull.Value

                Else

                    parametroObservacion.Value =
                        venta.cObservacion

                End If


                parametros.Add(
                    parametroObservacion
                )


                '------------------------------------------
                ' @Detalles
                '
                ' TIPO TABLA:
                ' dbo.TipoDetalleVenta
                '------------------------------------------

                Dim parametroDetalles As New SqlParameter(
                    "@Detalles",
                    SqlDbType.Structured
                )


                parametroDetalles.TypeName =
                    "dbo.TipoDetalleVenta"


                parametroDetalles.Value =
                    tablaDetalles


                parametros.Add(
                    parametroDetalles
                )


                '------------------------------------------
                ' EJECUTAR PROCEDIMIENTO ALMACENADO
                '------------------------------------------

                Using db As New ConexionBD()


                    Dim resultado As DataTable =
                        db.ExecuteDataTable(
                            "EXEC dbo.sp_RegistrarVenta " &
                            "@iCodUsuario, " &
                            "@cTipoComprobante, " &
                            "@cDocumentoCliente, " &
                            "@cNumeroCelular, " &
                            "@cNumeroComprobante, " &
                            "@cMetodoPago, " &
                            "@cObservacion, " &
                            "@Detalles",
                            parametros
                        )


                    '--------------------------------------
                    ' COMPROBAR RESULTADO
                    '--------------------------------------

                    If resultado IsNot Nothing AndAlso
                        resultado.Rows.Count > 0 Then


                        Dim idVenta As Integer =
                            Convert.ToInt32(
                                resultado.Rows(0)(
                                    "iCodVenta"
                                )
                            )


                        Return "OK"

                    End If


                End Using


                Return "ERROR: No se pudo registrar la venta."


            Catch ex As SqlException

                Return "ERROR: " &
                       ex.Message


            Catch ex As Exception

                Return "ERROR: " &
                       ex.Message

            End Try

        End Function


        '==================================================
        ' GUARDAR ARCHIVO DE COMPROBANTE CON OCR
        ' Persiste la imagen/PDF del comprobante y los datos
        ' extraidos por el servicio OCR en Comprobantes_OCR,
        ' vinculandolo a la venta indicada.
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function GuardarComprobanteOCRVenta(
            nombreArchivo As String,
            tipoMime As String,
            contenidoBase64 As String,
            textoOcr As String,
            rucEmisor As String,
            numeroComprobante As String,
            tipoComprobante As String,
            fechaEmision As String,
            subtotal As Decimal,
            igv As Decimal,
            total As Decimal,
            idVenta As Integer
        ) As String

            Try

                Dim sesionActual = System.Web.HttpContext.Current.Session

                If sesionActual Is Nothing OrElse
                    sesionActual("iCodUsuario") Is Nothing Then

                    Return "ERROR: Debe iniciar sesión."

                End If

                Dim contenido As Byte() = Nothing

                If Not String.IsNullOrWhiteSpace(contenidoBase64) Then

                    contenido =
                        Convert.FromBase64String(
                            LimpiarBase64(contenidoBase64)
                        )

                End If

                Dim obj As New ModeloComprobanteOCR()

                obj.iCodCompra = 0
                obj.iCodVenta = idVenta
                obj.cNombreArchivo =
                    If(
                        String.IsNullOrWhiteSpace(nombreArchivo),
                        "comprobante_ocr",
                        nombreArchivo.Trim()
                    )
                obj.cTipoMime = tipoMime
                obj.nTamanoBytes =
                    If(contenido Is Nothing, 0, contenido.Length)
                obj.cRucEmisor = rucEmisor
                obj.cNumeroComprobante = numeroComprobante
                obj.cTipoComprobante = tipoComprobante
                obj.dFechaEmision = ConvertirFecha(fechaEmision)
                obj.nSubTotal = subtotal
                obj.nIgv = igv
                obj.nTotal = total
                obj.cTextoOcr = textoOcr
                obj.imgComprobante = contenido

                Using db As New ConexionBD()

                    db.BeginTransaction()

                    obj.db = db
                    obj.Insertar()

                    db.CommitTransaction()

                End Using

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER EL CODIGO DE LA ULTIMA VENTA REGISTRADA
        ' Permite asociar el comprobante OCR recien leido
        ' con la venta que se acaba de guardar.
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function ObtenerUltimaVentaId() As Integer

            Dim sesionActual = System.Web.HttpContext.Current.Session

            If sesionActual Is Nothing OrElse
                sesionActual("iCodUsuario") Is Nothing Then

                Return 0

            End If

            Dim parametros As New List(Of System.Data.SqlClient.SqlParameter)

            parametros.Add(
                New System.Data.SqlClient.SqlParameter(
                    "@iCodUsuario",
                    System.Data.SqlDbType.Int
                ) With {
                    .Value = Convert.ToInt32(sesionActual("iCodUsuario"))
                }
            )

            Dim obj As New ModeloVenta()

            Dim resultado As Object =
                obj.db.ExecuteScalar(
                    "SELECT MAX(iCodVenta) FROM Ventas WHERE iCodUsuario = @iCodUsuario",
                    parametros
                )

            If resultado Is Nothing OrElse IsDBNull(resultado) Then
                Return 0
            End If

            Return Convert.ToInt32(resultado)

        End Function


        '==================================================
        ' QUITAR EL PREFIJO DATA:...;BASE64,
        ' DEJANDO SOLO LA CADENA EN BASE64
        '==================================================

        Private Shared Function LimpiarBase64(contenido As String) As String

            Dim limpio As String = contenido.Trim()

            Dim posicion As Integer =
                limpio.IndexOf(","c)

            If posicion >= 0 Then
                limpio = limpio.Substring(posicion + 1)
            End If

            Return limpio.Trim()

        End Function


        '==================================================
        ' CONVERTIR TEXTO DE FECHA A DATE
        '==================================================

        Private Shared Function ConvertirFecha(valor As String) As Date?

            If String.IsNullOrWhiteSpace(valor) Then
                Return Nothing
            End If

            Dim fecha As Date

            If Date.TryParse(valor.Trim(), fecha) Then
                Return fecha.Date
            End If

            Return Nothing

        End Function


        '==================================================
        ' MODIFICAR VENTA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function ModificarVenta(
            venta As ModeloVenta
        ) As String

            Try

                venta.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " &
                       ex.Message

            End Try

        End Function


        '==================================================
        ' ANULAR VENTA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function EliminarVenta(
            idVenta As Integer
        ) As String

            Try

                Dim objVenta As New ModeloVenta()


                objVenta.iCodVenta =
                    idVenta


                objVenta.Eliminar()


                Return "OK"

            Catch ex As Exception

                Return "ERROR: " &
                       ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER VENTA POR ID
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerVenta(
            idVenta As Integer
        ) As ModeloVenta

            Dim objVenta As New ModeloVenta()


            objVenta.iCodVenta =
                idVenta


            objVenta.getRecord()


            Return objVenta

        End Function

    End Class

End Namespace