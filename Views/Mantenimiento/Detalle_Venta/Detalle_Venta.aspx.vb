Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloDetalleVenta = WebApplication_keyove.WebApplication_Keyove.Model.DetalleVenta
Imports ModeloVenta = WebApplication_keyove.WebApplication_Keyove.Model.Ventas
Imports ModeloProducto = WebApplication_keyove.WebApplication_Keyove.Model.Productos

Namespace WebApplication_Keyove.Views.Mantenimiento.Detalle_Venta

    Partial Public Class DetalleVenta1
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

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

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarVentasCombo() As List(Of OpcionCombo)

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

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarProductosCombo() As List(Of OpcionCombo)

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

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarDetallesVenta() As List(Of ModeloDetalleVenta)

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

        <WebMethod()>
        Public Shared Function GuardarDetalleVenta(detalle As ModeloDetalleVenta) As String

            Try

                detalle.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR DETALLE DE VENTA
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarDetalleVenta(detalle As ModeloDetalleVenta) As String

            Try

                detalle.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' ELIMINAR DETALLE DE VENTA
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarDetalleVenta(idDetalleVenta As Integer) As String

            Try

                Dim objDetalle As New ModeloDetalleVenta()

                objDetalle.iCodDetalleVenta = idDetalleVenta
                objDetalle.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER DETALLE DE VENTA POR ID
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerDetalleVenta(idDetalleVenta As Integer) As ModeloDetalleVenta

            Dim objDetalle As New ModeloDetalleVenta()

            objDetalle.iCodDetalleVenta = idDetalleVenta
            objDetalle.getRecord()

            Return objDetalle

        End Function

    End Class

End Namespace