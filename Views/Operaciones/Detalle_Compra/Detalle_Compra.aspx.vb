Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloDetalleCompra = WebApplication_keyove.WebApplication_Keyove.Model.DetalleCompra
Imports ModeloCompra = WebApplication_keyove.WebApplication_Keyove.Model.Compras
Imports ModeloProducto = WebApplication_keyove.WebApplication_Keyove.Model.Productos

Namespace WebApplication_Keyove.Views.Operaciones.Detalle_Compra

    Partial Public Class DetalleCompra1
        Inherits Global.System.Web.UI.Page

        Private Shared Sub ExigirAccesoDetalle()
            Dim sesion = System.Web.HttpContext.Current.Session
            If sesion Is Nothing OrElse sesion("iCodUsuario") Is Nothing Then Throw New System.Web.HttpException(401, "Inicia sesión.")
            Dim rol = Convert.ToString(sesion("Rol"))
            If rol <> "Administrador" Then Throw New System.Web.HttpException(403, "No tienes permiso para consultar estos detalles.")
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
        ' LISTAR COMPRAS PARA COMBOBOX
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarComprasCombo() As List(Of OpcionCombo)
            ExigirAccesoDetalle()

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloCompra().ListaDatosCombo()

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
        ' LISTAR DETALLES DE COMPRA
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarDetallesCompra() As List(Of ModeloDetalleCompra)
            ExigirAccesoDetalle()

            Dim lista As New List(Of ModeloDetalleCompra)()

            Dim tabla As DataTable =
                New ModeloDetalleCompra().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloDetalleCompra()

                obj.iCodDetalleCompra =
                    Convert.ToInt32(fila("iCodDetalleCompra"))

                obj.iCodCompra =
                    Convert.ToInt32(fila("iCodCompra"))

                obj.iCodProducto =
                    Convert.ToInt32(fila("iCodProducto"))

                obj.cCodigo =
                    Convert.ToString(fila("cCodigo"))

                obj.cNombreProducto =
                    Convert.ToString(fila("cNombreProducto"))

                obj.iCantidad =
                    Convert.ToInt32(fila("iCantidad"))

                obj.nPrecioCompra =
                    Convert.ToDecimal(fila("nPrecioCompra"))

                obj.nSubTotal =
                    Convert.ToDecimal(fila("nSubTotal"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR DETALLE DE COMPRA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function GuardarDetalleCompra(detalle As ModeloDetalleCompra) As String
            ExigirAccesoDetalle()
            Return "ERROR: Los productos de una operación registrada son de consulta. Para corregir cantidades, anula la operación y regístrala nuevamente."
        End Function


        '==================================================
        ' MODIFICAR DETALLE DE COMPRA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function ModificarDetalleCompra(detalle As ModeloDetalleCompra) As String
            ExigirAccesoDetalle()
            Return "ERROR: Los productos de una operación registrada son de consulta. Para corregir cantidades, anula la operación y regístrala nuevamente."
        End Function


        '==================================================
        ' ELIMINAR DETALLE DE COMPRA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function EliminarDetalleCompra(idDetalleCompra As Integer) As String
            ExigirAccesoDetalle()
            Return "ERROR: Los productos de una operación registrada son de consulta. Para corregir cantidades, anula la operación y regístrala nuevamente."
        End Function


        '==================================================
        ' OBTENER DETALLE DE COMPRA POR ID
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerDetalleCompra(idDetalleCompra As Integer) As ModeloDetalleCompra
            ExigirAccesoDetalle()

            Dim objDetalle As New ModeloDetalleCompra()

            objDetalle.iCodDetalleCompra = idDetalleCompra
            objDetalle.getRecord()

            Return objDetalle

        End Function

    End Class

End Namespace