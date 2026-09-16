Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloMovimiento = WebApplication_keyove.WebApplication_Keyove.Model.Movimientos_Inventario
Imports ModeloProducto = WebApplication_keyove.WebApplication_Keyove.Model.Productos
Imports ModeloUsuario = WebApplication_keyove.WebApplication_Keyove.Model.Usuarios

Namespace WebApplication_Keyove.Views.Inventario.Movimientos_Inventario

    Partial Public Class Movimiento1
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
        ' LISTAR USUARIOS PARA COMBOBOX
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarUsuariosCombo() As List(Of OpcionCombo)

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloUsuario().ListaDatosCombo()

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
        ' LISTAR MOVIMIENTOS
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarMovimientos() As List(Of ModeloMovimiento)

            Dim lista As New List(Of ModeloMovimiento)()

            Dim tabla As DataTable =
                New ModeloMovimiento().ListaDatosDetalle()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloMovimiento()

                obj.iCodMovimiento =
                    Convert.ToInt32(fila("iCodMovimiento"))

                obj.iCodProducto =
                    Convert.ToInt32(fila("iCodProducto"))

                obj.cNombreProducto =
                    Convert.ToString(fila("Producto"))

                obj.iCodUsuario =
                    Convert.ToInt32(fila("iCodUsuario"))

                obj.cNombreUsuario =
                    Convert.ToString(fila("Usuario"))

                If IsDBNull(fila("iCodCompra")) Then
                    obj.iCodCompra = Nothing
                Else
                    obj.iCodCompra =
                        Convert.ToInt32(fila("iCodCompra"))
                End If

                If IsDBNull(fila("iCodVenta")) Then
                    obj.iCodVenta = Nothing
                Else
                    obj.iCodVenta =
                        Convert.ToInt32(fila("iCodVenta"))
                End If

                obj.cTipoMovimiento =
                    Convert.ToString(fila("cTipoMovimiento"))

                obj.iCantidad =
                    Convert.ToInt32(fila("iCantidad"))

                obj.iStockAnterior =
                    Convert.ToInt32(fila("iStockAnterior"))

                obj.iStockNuevo =
                    Convert.ToInt32(fila("iStockNuevo"))

                If IsDBNull(fila("cMotivo")) Then
                    obj.cMotivo = Nothing
                Else
                    obj.cMotivo =
                        Convert.ToString(fila("cMotivo"))
                End If

                obj.dFechaMovimiento =
                    Convert.ToDateTime(fila("dFechaMovimiento"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR MOVIMIENTO
        '==================================================

        <WebMethod()>
        Public Shared Function GuardarMovimiento(movimiento As ModeloMovimiento) As String

            Try

                movimiento.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR MOVIMIENTO
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarMovimiento(movimiento As ModeloMovimiento) As String

            Try

                movimiento.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER MOVIMIENTO POR ID
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerMovimiento(idMovimiento As Integer) As ModeloMovimiento

            Dim objMovimiento As New ModeloMovimiento()

            objMovimiento.iCodMovimiento = idMovimiento
            objMovimiento.getRecord()

            Return objMovimiento

        End Function

    End Class

End Namespace