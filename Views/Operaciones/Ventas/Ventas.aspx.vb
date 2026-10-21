Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Data
Imports System.Data.SqlClient

Imports WebApplication_Keyove.WebApplication_Keyove.Model
Imports WebApplication_Keyove.WebApplication_Keyove.Data
Imports ModeloDetalleVenta = WebApplication_Keyove.WebApplication_Keyove.Model.DetalleVenta
Imports ModeloProducto = WebApplication_Keyove.WebApplication_Keyove.Model.Productos
Imports ModeloUsuario = WebApplication_Keyove.WebApplication_Keyove.Model.Usuarios
Imports ModeloVenta = WebApplication_Keyove.WebApplication_Keyove.Model.Ventas

Namespace WebApplication_Keyove.Views.Operaciones.Ventas

    Partial Public Class Venta1
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(
            ByVal sender As Object,
            ByVal e As Global.System.EventArgs
        ) Handles Me.Load
        End Sub


        ' OPCIÃ“N PARA COMBOBOX

        Public Class OpcionCombo
            Public v As String
            Public t As String
        End Class

        Public Class DetalleVentaEdicion
            Public Property iCodProducto As Integer
            Public Property cNombreProducto As String
            Public Property iCantidad As Integer
            Public Property nPrecioVenta As Decimal
            Public Property nDescuento As Decimal
            Public Property nSubTotal As Decimal
        End Class


        ' LISTAR USUARIOS PARA COMBOBOX

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarUsuariosCombo() As List(Of OpcionCombo)

            Dim sesionActual = System.Web.HttpContext.Current.Session

            If sesionActual Is Nothing OrElse
                sesionActual("iCodUsuario") Is Nothing Then

                Throw New System.Web.HttpException(
                    401,
                    "Debe iniciar sesión."
                )
            End If

            If String.Equals(
                Convert.ToString(sesionActual("Rol")).Trim(),
                "Vendedor",
                StringComparison.OrdinalIgnoreCase
            ) Then

                Return New List(Of OpcionCombo) From {
                    New OpcionCombo With {
                        .v = Convert.ToString(sesionActual("iCodUsuario")),
                        .t = Convert.ToString(sesionActual("Usuario"))
                    }
                }

            End If

            Dim lista As New List(Of OpcionCombo)()
            Dim tabla As DataTable = New ModeloUsuario().ListaDatosCombo()

            For Each fila As DataRow In tabla.Rows
                lista.Add(
                    New OpcionCombo With {
                        .v = Convert.ToString(fila("ValueMember")),
                        .t = Convert.ToString(fila("DisplayMember"))
                    }
                )
            Next

            Return lista

        End Function


        ' LISTAR PRODUCTOS PARA COMBOBOX

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarProductosCombo() As List(Of OpcionCombo)

            Dim lista As New List(Of OpcionCombo)()
            Dim tabla As DataTable = New ModeloProducto().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows
                If Not IsDBNull(fila("bEstado")) AndAlso
                   Convert.ToBoolean(fila("bEstado")) Then

                    lista.Add(
                        New OpcionCombo With {
                            .v = Convert.ToString(fila("iCodProducto")),
                            .t = Convert.ToString(fila("cNombre"))
                        }
                    )
                End If
            Next

            Return lista

        End Function


        ' LISTAR VENTAS

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarVentas() As List(Of ModeloVenta)

            Dim lista As New List(Of ModeloVenta)()
            Dim tabla As DataTable = New ModeloVenta().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloVenta()

                obj.iCodVenta = Convert.ToInt32(fila("iCodVenta"))
                obj.iCodUsuario = Convert.ToInt32(fila("iCodUsuario"))
                obj.cNombreUsuario = Convert.ToString(fila("Usuario"))
                obj.cTipoComprobante = Convert.ToString(fila("cTipoComprobante"))

                obj.cDocumentoCliente =
                    If(IsDBNull(fila("cDocumentoCliente")),
                       Nothing,
                       Convert.ToString(fila("cDocumentoCliente")))

                obj.cNumeroCelular =
                    If(IsDBNull(fila("cNumeroCelular")),
                       Nothing,
                       Convert.ToString(fila("cNumeroCelular")))

                obj.cNumeroComprobante = Convert.ToString(fila("cNumeroComprobante"))
                obj.dFechaVenta = Convert.ToDateTime(fila("dFechaVenta"))
                obj.nSubTotal = Convert.ToDecimal(fila("nSubTotal"))
                obj.nIgv = Convert.ToDecimal(fila("nIgv"))
                obj.nTotal = Convert.ToDecimal(fila("nTotal"))
                obj.cMetodoPago = Convert.ToString(fila("cMetodoPago"))

                obj.cObservacion =
                    If(IsDBNull(fila("cObservacion")),
                       Nothing,
                       Convert.ToString(fila("cObservacion")))

                obj.cEstado = Convert.ToString(fila("cEstado"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        ' GUARDAR VENTA
        '
        ' AHORA UTILIZA:
        ' sp_RegistrarVenta

        <WebMethod(EnableSession:=True)>
        Public Shared Function GuardarVenta(
            venta As ModeloVenta,
            detalles As List(Of ModeloDetalleVenta)
        ) As String

            Try
                Dim sesionActual = System.Web.HttpContext.Current.Session

                If sesionActual Is Nothing OrElse
                    sesionActual("iCodUsuario") Is Nothing Then

                End If

                If venta Is Nothing Then
                    Return "ERROR: Los datos de la venta son obligatorios."
                End If

                If detalles Is Nothing OrElse detalles.Count = 0 Then
                    Return "ERROR: Agregue al menos un producto."
                End If

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
                tablaDetalles.Columns.Add("iCodProducto", GetType(Integer))
                tablaDetalles.Columns.Add("iCantidad", GetType(Integer))
                tablaDetalles.Columns.Add("nPrecioVenta", GetType(Decimal))
                tablaDetalles.Columns.Add("nDescuento", GetType(Decimal))

                For Each detalle As ModeloDetalleVenta In detalles

                    If detalle Is Nothing Then
                        Return "ERROR: Existe un detalle de venta inválido."
                    End If

                    Dim fila As DataRow = tablaDetalles.NewRow()
                    fila("iCodProducto") = detalle.iCodProducto
                    fila("iCantidad") = detalle.iCantidad
                    fila("nPrecioVenta") = detalle.nPrecioVenta
                    fila("nDescuento") = detalle.nDescuento

                    tablaDetalles.Rows.Add(fila)

                Next

                Dim parametros As New List(Of SqlParameter)()

                parametros.Add(
                    New SqlParameter("@iCodUsuario", SqlDbType.Int) With {
                        .Value = venta.iCodUsuario
                    }
                )

                parametros.Add(
                    New SqlParameter("@cTipoComprobante", SqlDbType.NVarChar, 30) With {
                        .Value = If(
                            String.IsNullOrWhiteSpace(venta.cTipoComprobante),
                            "BOLETA",
                            venta.cTipoComprobante
                        )
                    }
                )

                Dim parametroDocumento As New SqlParameter(
                    "@cDocumentoCliente", SqlDbType.NVarChar, 11
                )
                parametroDocumento.Value =
                    If(String.IsNullOrWhiteSpace(venta.cDocumentoCliente),
                       CType(DBNull.Value, Object),
                       venta.cDocumentoCliente)
                parametros.Add(parametroDocumento)

                Dim parametroCelular As New SqlParameter(
                    "@cNumeroCelular", SqlDbType.VarChar, 9
                )
                parametroCelular.Value =
                    If(String.IsNullOrWhiteSpace(venta.cNumeroCelular),
                       CType(DBNull.Value, Object),
                       venta.cNumeroCelular)
                parametros.Add(parametroCelular)

                parametros.Add(
                    New SqlParameter("@cNumeroComprobante", SqlDbType.NVarChar, 50) With {
                        .Value = venta.cNumeroComprobante
                    }
                )

                parametros.Add(
                    New SqlParameter("@cMetodoPago", SqlDbType.NVarChar, 50) With {
                        .Value = If(
                            String.IsNullOrWhiteSpace(venta.cMetodoPago),
                            "EFECTIVO",
                            venta.cMetodoPago
                        )
                    }
                )

                Dim parametroObservacion As New SqlParameter(
                    "@cObservacion", SqlDbType.NVarChar, 300
                )
                parametroObservacion.Value =
                    If(String.IsNullOrWhiteSpace(venta.cObservacion),
                       CType(DBNull.Value, Object),
                       venta.cObservacion)
                parametros.Add(parametroObservacion)

                Dim parametroDetalles As New SqlParameter(
                    "@Detalles", SqlDbType.Structured
                )
                parametroDetalles.TypeName = "dbo.TipoDetalleVenta"
                parametroDetalles.Value = tablaDetalles
                parametros.Add(parametroDetalles)

                Using db As New ConexionBD()

                    Dim resultado As DataTable = db.ExecuteDataTable(
                        "EXEC dbo.sp_RegistrarVenta " &
                        "@iCodUsuario, @cTipoComprobante, " &
                        "@cDocumentoCliente, @cNumeroCelular, " &
                        "@cNumeroComprobante, @cMetodoPago, " &
                        "@cObservacion, @Detalles",
                        parametros
                    )

                    If resultado IsNot Nothing AndAlso
                       resultado.Rows.Count > 0 Then
                        Return "OK"
                    End If

                End Using

                Return "ERROR: No se pudo registrar la venta."

            Catch ex As SqlException
                Return "ERROR: " & ex.Message
            Catch ex As Exception

                Return "ERROR: " &
                       ex.Message

            End Try

        End Function








        ' CONVERTIR TEXTO DE FECHA A DATE

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
        ' OBTENER DETALLES DE UNA VENTA PARA EDITAR
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerDetallesVenta(
            idVenta As Integer
        ) As List(Of DetalleVentaEdicion)

            Dim sesionActual = System.Web.HttpContext.Current.Session

            If sesionActual Is Nothing OrElse
               sesionActual("iCodUsuario") Is Nothing Then
                Throw New System.Web.HttpException(401, "Debe iniciar sesión.")
            End If

            If Not String.Equals(
                Convert.ToString(sesionActual("Rol")).Trim(),
                "Administrador",
                StringComparison.OrdinalIgnoreCase
            ) Then
                Throw New System.Web.HttpException(
                    403, "Solo el administrador puede editar ventas."
                )
            End If

            If idVenta <= 0 Then
                Throw New System.Web.HttpException(
                    400, "El código de venta no es válido."
                )
            End If

            Dim lista As New List(Of DetalleVentaEdicion)()

            Dim parametros As New List(Of SqlParameter) From {
                New SqlParameter("@iCodVenta", SqlDbType.Int) With {
                    .Value = idVenta
                }
            }

            Using db As New ConexionBD()

                Dim consulta As String =
                    "SELECT d.iCodProducto, " &
                    "p.cNombre AS cNombreProducto, " &
                    "d.iCantidad, d.nPrecioVenta, d.nDescuento, " &
                    "d.nSubTotal " &
                    "FROM dbo.Detalle_Venta AS d " &
                    "INNER JOIN dbo.Productos AS p " &
                    "ON p.iCodProducto = d.iCodProducto " &
                    "WHERE d.iCodVenta = @iCodVenta " &
                    "ORDER BY d.iCodProducto"

                Dim tabla As DataTable =
                    db.ExecuteDataTable(consulta, parametros)

                For Each fila As DataRow In tabla.Rows

                    Dim detalle As New DetalleVentaEdicion With {
                        .iCodProducto = Convert.ToInt32(fila("iCodProducto")),
                        .cNombreProducto = Convert.ToString(fila("cNombreProducto")),
                        .iCantidad = Convert.ToInt32(fila("iCantidad")),
                        .nPrecioVenta = Convert.ToDecimal(fila("nPrecioVenta")),
                        .nDescuento = Convert.ToDecimal(fila("nDescuento")),
                        .nSubTotal = Convert.ToDecimal(fila("nSubTotal"))
                    }

                    lista.Add(detalle)

                Next

            End Using

            Return lista

        End Function

        ' MODIFICAR VENTA

        <WebMethod(EnableSession:=True)>
        Public Shared Function ModificarVenta(
            venta As ModeloVenta,
            detalles As List(Of ModeloDetalleVenta)
        ) As String

            Try
                Dim sesionActual = System.Web.HttpContext.Current.Session

                If sesionActual Is Nothing OrElse
                   sesionActual("iCodUsuario") Is Nothing Then
                    Return "ERROR: Debe iniciar sesión."
                End If

                If Not String.Equals(
                    Convert.ToString(sesionActual("Rol")).Trim(),
                    "Administrador",
                    StringComparison.OrdinalIgnoreCase
                ) Then
                    Return "ERROR: Solo el administrador puede modificar ventas."
                End If

                If venta Is Nothing OrElse venta.iCodVenta <= 0 Then
                    Return "ERROR: Selecciona una venta válida."
                End If

                If Not String.Equals(
                    Convert.ToString(venta.cEstado).Trim(),
                    "REGISTRADA",
                    StringComparison.OrdinalIgnoreCase
                ) Then
                    Return "ERROR: Solo se pueden modificar ventas registradas."
                End If

                If detalles Is Nothing OrElse detalles.Count = 0 Then
                    Return "ERROR: La venta debe contener al menos un producto."
                End If

                If venta.iCodUsuario <= 0 Then
                    Return "ERROR: Selecciona un usuario válido."
                End If

                If String.IsNullOrWhiteSpace(venta.cTipoComprobante) OrElse
                   String.IsNullOrWhiteSpace(venta.cNumeroComprobante) OrElse
                   String.IsNullOrWhiteSpace(venta.cMetodoPago) Then
                    Return "ERROR: Completa el comprobante, su número y el método de pago."
                End If

                Dim tablaDetalles As New DataTable()
                tablaDetalles.Columns.Add("iCodProducto", GetType(Integer))
                tablaDetalles.Columns.Add("iCantidad", GetType(Integer))
                tablaDetalles.Columns.Add("nPrecioVenta", GetType(Decimal))
                tablaDetalles.Columns.Add("nDescuento", GetType(Decimal))

                For Each detalle As ModeloDetalleVenta In detalles

                    If detalle Is Nothing Then
                        Return "ERROR: Existe un detalle de venta inválido."
                    End If

                    If detalle.iCodProducto <= 0 OrElse
                       detalle.iCantidad <= 0 OrElse
                       detalle.nPrecioVenta < 0D OrElse
                       detalle.nDescuento < 0D OrElse
                       detalle.nDescuento > (detalle.iCantidad * detalle.nPrecioVenta) Then

                        Return "ERROR: Revisa el producto, la cantidad, el precio y el descuento."
                    End If

                    Dim fila As DataRow = tablaDetalles.NewRow()
                    fila("iCodProducto") = detalle.iCodProducto
                    fila("iCantidad") = detalle.iCantidad
                    fila("nPrecioVenta") = detalle.nPrecioVenta
                    fila("nDescuento") = detalle.nDescuento

                    tablaDetalles.Rows.Add(fila)

                Next

                Dim parametros As New List(Of SqlParameter)()

                parametros.Add(
                    New SqlParameter("@iCodVenta", SqlDbType.Int) With {
                        .Value = venta.iCodVenta
                    }
                )

                parametros.Add(
                    New SqlParameter("@iCodUsuario", SqlDbType.Int) With {
                        .Value = venta.iCodUsuario
                    }
                )

                parametros.Add(
                    New SqlParameter("@cTipoComprobante", SqlDbType.NVarChar, 30) With {
                        .Value = venta.cTipoComprobante.Trim()
                    }
                )

                parametros.Add(
                    New SqlParameter("@cNumeroComprobante", SqlDbType.NVarChar, 50) With {
                        .Value = venta.cNumeroComprobante.Trim()
                    }
                )

                parametros.Add(
                    New SqlParameter("@cMetodoPago", SqlDbType.NVarChar, 50) With {
                        .Value = venta.cMetodoPago.Trim()
                    }
                )

                Dim parametroDetalles As New SqlParameter(
                    "@Detalles", SqlDbType.Structured
                )
                parametroDetalles.TypeName = "dbo.TipoDetalleVenta"
                parametroDetalles.Value = tablaDetalles
                parametros.Add(parametroDetalles)

                Dim parametroDocumento As New SqlParameter(
                    "@cDocumentoCliente", SqlDbType.NVarChar, 11
                )
                If String.IsNullOrWhiteSpace(venta.cDocumentoCliente) Then
                    parametroDocumento.Value = DBNull.Value
                Else
                    parametroDocumento.Value = venta.cDocumentoCliente.Trim()
                End If
                parametros.Add(parametroDocumento)

                Dim parametroCelular As New SqlParameter(
                    "@cNumeroCelular", SqlDbType.VarChar, 9
                )
                If String.IsNullOrWhiteSpace(venta.cNumeroCelular) Then
                    parametroCelular.Value = DBNull.Value
                Else
                    parametroCelular.Value = venta.cNumeroCelular.Trim()
                End If
                parametros.Add(parametroCelular)

                Dim parametroObservacion As New SqlParameter(
                    "@cObservacion", SqlDbType.NVarChar, 300
                )
                If String.IsNullOrWhiteSpace(venta.cObservacion) Then
                    parametroObservacion.Value = DBNull.Value
                Else
                    parametroObservacion.Value = venta.cObservacion.Trim()
                End If
                parametros.Add(parametroObservacion)

                Using db As New ConexionBD()

                    Dim resultado As DataTable = db.ExecuteDataTable(
                        "EXEC dbo.sp_ModificarVenta " &
                        "@iCodVenta, @iCodUsuario, @cTipoComprobante, " &
                        "@cNumeroComprobante, @cMetodoPago, @Detalles, " &
                        "@cDocumentoCliente, @cNumeroCelular, @cObservacion",
                        parametros
                    )

                    If resultado IsNot Nothing AndAlso resultado.Rows.Count > 0 Then
                        Return "OK"
                    End If

                End Using

                Return "ERROR: No se recibió confirmación de la modificación."

            Catch ex As SqlException
                Return "ERROR: " & ex.Message

            Catch ex As Exception
                Return "ERROR: " & ex.Message
            End Try

        End Function


        ' ANULAR VENTA

        <WebMethod(EnableSession:=True)>
        Public Shared Function EliminarVenta(
            idVenta As Integer
        ) As String

            Try
                Dim sesionActual = System.Web.HttpContext.Current.Session

                If sesionActual Is Nothing OrElse
                   sesionActual("iCodUsuario") Is Nothing Then
                    Return "ERROR: Debe iniciar sesión."
                End If

                If Not String.Equals(
                    Convert.ToString(sesionActual("Rol")).Trim(),
                    "Administrador",
                    StringComparison.OrdinalIgnoreCase
                ) Then
                    Return "ERROR: Solo el administrador puede anular ventas."
                End If

                If idVenta <= 0 Then
                    Return "ERROR: El código de venta no es válido."
                End If

                Dim objVenta As New ModeloVenta()
                objVenta.iCodVenta = idVenta
                objVenta.Eliminar()

                Return "OK"

            Catch ex As Exception
                Return "ERROR: " & ex.Message
            End Try

        End Function


        ' OBTENER VENTA POR ID

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerVenta(
            idVenta As Integer
        ) As ModeloVenta

            Dim sesionActual = System.Web.HttpContext.Current.Session

            If sesionActual Is Nothing OrElse
               sesionActual("iCodUsuario") Is Nothing Then
                Throw New System.Web.HttpException(401, "Debe iniciar sesión.")
            End If

            If idVenta <= 0 Then
                Throw New System.Web.HttpException(400, "El código de venta no es válido.")
            End If

            Dim objVenta As New ModeloVenta()
            objVenta.iCodVenta = idVenta
            objVenta.getRecord()

            Return objVenta

        End Function

    End Class

End Namespace
