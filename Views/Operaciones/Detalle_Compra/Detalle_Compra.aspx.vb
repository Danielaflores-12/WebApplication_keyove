Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloDetalleCompra = WebApplication_keyove.WebApplication_Keyove.Model.DetalleCompra
Imports ModeloCompra = WebApplication_keyove.WebApplication_Keyove.Model.Compras
Imports ModeloProducto = WebApplication_keyove.WebApplication_Keyove.Model.Productos

Namespace WebApplication_Keyove.Views.Operaciones.Detalle_Compra

    Partial Public Class DetalleCompra1
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
        ' LISTAR COMPRAS PARA COMBOBOX
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarComprasCombo() As List(Of OpcionCombo)

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
        ' LISTAR DETALLES DE COMPRA
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarDetallesCompra() As List(Of ModeloDetalleCompra)

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

        <WebMethod()>
        Public Shared Function GuardarDetalleCompra(detalle As ModeloDetalleCompra) As String

            Try

                detalle.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR DETALLE DE COMPRA
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarDetalleCompra(detalle As ModeloDetalleCompra) As String

            Try

                detalle.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' ELIMINAR DETALLE DE COMPRA
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarDetalleCompra(idDetalleCompra As Integer) As String

            Try

                Dim objDetalle As New ModeloDetalleCompra()

                objDetalle.iCodDetalleCompra = idDetalleCompra
                objDetalle.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER DETALLE DE COMPRA POR ID
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerDetalleCompra(idDetalleCompra As Integer) As ModeloDetalleCompra

            Dim objDetalle As New ModeloDetalleCompra()

            objDetalle.iCodDetalleCompra = idDetalleCompra
            objDetalle.getRecord()

            Return objDetalle

        End Function

    End Class

End Namespace