Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports WebApplication_keyove.WebApplication_Keyove.Data
Imports ModeloCompra = WebApplication_keyove.WebApplication_Keyove.Model.Compras
Imports ModeloDetalleCompra = WebApplication_keyove.WebApplication_Keyove.Model.DetalleCompra
Imports ModeloProveedor = WebApplication_keyove.WebApplication_Keyove.Model.Proveedores
Imports ModeloProducto = WebApplication_keyove.WebApplication_Keyove.Model.Productos
Imports ModeloUsuario = WebApplication_keyove.WebApplication_Keyove.Model.Usuarios

Namespace WebApplication_Keyove.Views.Operaciones.Compras

    Partial Public Class Compra1
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
        ' LISTAR PROVEEDORES PARA COMBOBOX
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarProveedoresCombo() As List(Of OpcionCombo)

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloProveedor().ListaDatosCombo()

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
        ' LISTAR PRODUCTOS PARA COMBOBOX
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarProductosCombo() As List(Of OpcionCombo)

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloProducto().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                If Convert.ToBoolean(fila("bEstado")) Then

                    Dim opcion As New OpcionCombo()

                    opcion.v =
                        Convert.ToString(fila("iCodProducto"))

                    opcion.t =
                        Convert.ToString(fila("cNombre"))

                    lista.Add(opcion)

                End If

            Next

            Return lista

        End Function


        '==================================================
        ' LISTAR COMPRAS
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarCompras() As List(Of ModeloCompra)

            Dim lista As New List(Of ModeloCompra)()

            Dim tabla As DataTable =
                New ModeloCompra().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloCompra()

                obj.iCodCompra =
                    Convert.ToInt32(fila("iCodCompra"))

                obj.iCodProveedor =
                    Convert.ToInt32(fila("iCodProveedor"))

                obj.iCodUsuario =
                    Convert.ToInt32(fila("iCodUsuario"))

                obj.cTipoComprobante =
                    Convert.ToString(fila("cTipoComprobante"))

                obj.cNumeroComprobante =
                    Convert.ToString(fila("cNumeroComprobante"))

                obj.dFechaCompra =
                    Convert.ToDateTime(fila("dFechaCompra"))

                obj.nSubTotal =
                    Convert.ToDecimal(fila("nSubTotal"))

                obj.nIgv =
                    Convert.ToDecimal(fila("nIgv"))

                obj.nTotal =
                    Convert.ToDecimal(fila("nTotal"))

                If IsDBNull(fila("cObservacion")) Then
                    obj.cObservacion = Nothing
                Else
                    obj.cObservacion =
                        Convert.ToString(fila("cObservacion"))
                End If

                obj.cEstado =
                    Convert.ToString(fila("cEstado"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR COMPRA CON SUS DETALLES
        '==================================================

        <WebMethod()>
        Public Shared Function GuardarCompra(
            compra As ModeloCompra,
            detalles As List(Of ModeloDetalleCompra)
        ) As String

            Try

                Using db As New ConexionBD()

                    db.BeginTransaction()

                    compra.db = db
                    compra.InsertarTransact()

                    If detalles IsNot Nothing Then

                        For Each detalle As ModeloDetalleCompra In detalles

                            detalle.iCodCompra =
                                compra.iCodCompra

                            detalle.db = db
                            detalle.InsertarTransact()

                        Next

                    End If

                    db.CommitTransaction()

                End Using

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR COMPRA
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarCompra(compra As ModeloCompra) As String

            Try

                compra.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' ANULAR COMPRA
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarCompra(idCompra As Integer) As String

            Try

                Dim objCompra As New ModeloCompra()

                objCompra.iCodCompra = idCompra
                objCompra.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER COMPRA POR ID
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerCompra(idCompra As Integer) As ModeloCompra

            Dim objCompra As New ModeloCompra()

            objCompra.iCodCompra = idCompra
            objCompra.getRecord()

            Return objCompra

        End Function

    End Class

End Namespace