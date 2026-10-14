Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloDetalleVenta = WebApplication_keyove.WebApplication_Keyove.Model.DetalleVenta
Imports ModeloVenta = WebApplication_keyove.WebApplication_Keyove.Model.Ventas
Imports ModeloProducto = WebApplication_keyove.WebApplication_Keyove.Model.Productos

Namespace WebApplication_Keyove.Views.Operaciones.Detalle_Venta

    Partial Public Class DetalleVenta1
        Inherits Global.System.Web.UI.Page

        Private Shared Sub ExigirAccesoDetalle()
            Dim sesion = System.Web.HttpContext.Current.Session
            If sesion Is Nothing OrElse sesion("iCodUsuario") Is Nothing Then Throw New System.Web.HttpException(401, "Inicia sesión.")
            Dim rol = Convert.ToString(sesion("Rol"))
            If rol <> "Administrador" AndAlso rol <> "Vendedor" Then Throw New System.Web.HttpException(403, "No tienes permiso para consultar estos detalles.")
        End Sub


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load
            ExigirAccesoDetalle()

        End Sub


        '==================================================
        ' OPCIÓN PARA COMBOBOX
        '==================================================

        Public Class OpcionCombo

            Public v As String
            Public t As String

        End Class


        '==================================================
        ' LISTAR VENTAS PARA COMBOBOX
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarVentasCombo() As List(Of OpcionCombo)
            ExigirAccesoDetalle()

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloVenta().ListaDatosCombo()

            For Each fila As DataRow In tabla.Rows

                Dim opcion As New OpcionCombo()

                opcion.v =
                    Convert.ToString(fila("ValueMember"))

                opcion.t =
                    Convert.ToString(fila("DisplayMember"))

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
            ExigirAccesoDetalle()

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloProducto().ListaDatosCombo()

            For Each fila As DataRow In tabla.Rows

                Dim opcion As New OpcionCombo()

                opcion.v =
                    Convert.ToString(fila("ValueMember"))

                opcion.t =
                    Convert.ToString(fila("DisplayMember"))

                lista.Add(opcion)

            Next

            Return lista

        End Function


        '==================================================
        ' LISTAR DETALLES DE VENTA
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarDetallesVenta() As List(Of ModeloDetalleVenta)
            ExigirAccesoDetalle()

            Dim lista As New List(Of ModeloDetalleVenta)()

            Dim tabla As DataTable =
                New ModeloDetalleVenta().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloDetalleVenta()

                obj.iCodDetalleVenta =
                    Convert.ToInt32(fila("iCodDetalleVenta"))

                obj.iCodVenta =
                    Convert.ToInt32(fila("iCodVenta"))

                obj.iCodProducto =
                    Convert.ToInt32(fila("iCodProducto"))

                obj.cCodigo =
                    Convert.ToString(fila("cCodigo"))

                obj.cNombreProducto =
                    Convert.ToString(fila("cNombreProducto"))

                obj.iCantidad =
                    Convert.ToInt32(fila("iCantidad"))

                obj.nPrecioVenta =
                    Convert.ToDecimal(fila("nPrecioVenta"))

                obj.nDescuento =
                    Convert.ToDecimal(fila("nDescuento"))

                obj.nSubTotal =
                    Convert.ToDecimal(fila("nSubTotal"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR DETALLE DE VENTA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function GuardarDetalleVenta(detalle As ModeloDetalleVenta) As String
            ExigirAccesoDetalle()
            Return "ERROR: Los productos de una operación registrada son de consulta. Para corregir cantidades, anula la operación y regístrala nuevamente."
        End Function


        '==================================================
        ' MODIFICAR DETALLE DE VENTA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function ModificarDetalleVenta(detalle As ModeloDetalleVenta) As String
            ExigirAccesoDetalle()
            Return "ERROR: Los productos de una operación registrada son de consulta. Para corregir cantidades, anula la operación y regístrala nuevamente."
        End Function


        '==================================================
        ' ELIMINAR DETALLE DE VENTA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function EliminarDetalleVenta(idDetalleVenta As Integer) As String
            ExigirAccesoDetalle()
            Return "ERROR: Los productos de una operación registrada son de consulta. Para corregir cantidades, anula la operación y regístrala nuevamente."
        End Function


        '==================================================
        ' OBTENER DETALLE DE VENTA POR ID
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerDetalleVenta(idDetalleVenta As Integer) As ModeloDetalleVenta
            ExigirAccesoDetalle()

            Dim objDetalle As New ModeloDetalleVenta()

            objDetalle.iCodDetalleVenta = idDetalleVenta
            objDetalle.getRecord()

            Return objDetalle

        End Function

    End Class

End Namespace