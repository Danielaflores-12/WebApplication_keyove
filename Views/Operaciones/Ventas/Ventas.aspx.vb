Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloVenta = WebApplication_keyove.WebApplication_Keyove.Model.Ventas
Imports ModeloUsuario = WebApplication_keyove.WebApplication_Keyove.Model.Usuarios

Namespace WebApplication_Keyove.Views.Operaciones.Ventas

    Partial Public Class Venta1
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
        ' LISTAR VENTAS
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarVentas() As List(Of ModeloVenta)

            Dim lista As New List(Of ModeloVenta)()

            Dim tabla As DataTable =
                New ModeloVenta().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloVenta()

                obj.iCodVenta =
                    Convert.ToInt32(fila("iCodVenta"))

                obj.iCodUsuario =
                    Convert.ToInt32(fila("iCodUsuario"))

                obj.cNombreUsuario =
                    Convert.ToString(fila("Usuario"))

                obj.cTipoComprobante =
                    Convert.ToString(fila("cTipoComprobante"))

                If IsDBNull(fila("cDocumentoCliente")) Then
                    obj.cDocumentoCliente = Nothing
                Else
                    obj.cDocumentoCliente =
                        Convert.ToString(fila("cDocumentoCliente"))
                End If

                If IsDBNull(fila("cNumeroCelular")) Then
                    obj.cNumeroCelular = Nothing
                Else
                    obj.cNumeroCelular =
                        Convert.ToString(fila("cNumeroCelular"))
                End If

                obj.cNumeroComprobante =
                    Convert.ToString(fila("cNumeroComprobante"))

                obj.dFechaVenta =
                    Convert.ToDateTime(fila("dFechaVenta"))

                obj.nSubTotal =
                    Convert.ToDecimal(fila("nSubTotal"))

                obj.nIgv =
                    Convert.ToDecimal(fila("nIgv"))

                obj.nTotal =
                    Convert.ToDecimal(fila("nTotal"))

                obj.cMetodoPago =
                    Convert.ToString(fila("cMetodoPago"))

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
        ' GUARDAR VENTA
        '==================================================

        <WebMethod()>
        Public Shared Function GuardarVenta(venta As ModeloVenta) As String

            Try

                venta.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR VENTA
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarVenta(venta As ModeloVenta) As String

            Try

                venta.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' ANULAR VENTA
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarVenta(idVenta As Integer) As String

            Try

                Dim objVenta As New ModeloVenta()

                objVenta.iCodVenta = idVenta
                objVenta.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER VENTA POR ID
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerVenta(idVenta As Integer) As ModeloVenta

            Dim objVenta As New ModeloVenta()

            objVenta.iCodVenta = idVenta
            objVenta.getRecord()

            Return objVenta

        End Function

    End Class

End Namespace