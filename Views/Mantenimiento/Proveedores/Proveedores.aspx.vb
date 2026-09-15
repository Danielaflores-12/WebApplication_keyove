Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloProveedor = WebApplication_keyove.WebApplication_Keyove.Model.Proveedores

Namespace WebApplication_Keyove.Views.Mantenimiento.Proveedores

    Partial Public Class Proveedor1
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

        End Sub


        '==================================================
        ' LISTAR PROVEEDORES
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarProveedores() As List(Of ModeloProveedor)

            Dim lista As New List(Of ModeloProveedor)()

            Dim tabla As DataTable =
                New ModeloProveedor().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloProveedor()

                obj.iCodProveedor =
                    Convert.ToInt32(fila("iCodProveedor"))

                obj.cRuc =
                    Convert.ToString(fila("cRuc"))

                obj.cRazonSocial =
                    Convert.ToString(fila("cRazonSocial"))

                If IsDBNull(fila("cRepresentante")) Then
                    obj.cRepresentante = Nothing
                Else
                    obj.cRepresentante =
                        Convert.ToString(fila("cRepresentante"))
                End If

                If IsDBNull(fila("cTelefono")) Then
                    obj.cTelefono = Nothing
                Else
                    obj.cTelefono =
                        Convert.ToString(fila("cTelefono"))
                End If

                If IsDBNull(fila("cCorreo")) Then
                    obj.cCorreo = Nothing
                Else
                    obj.cCorreo =
                        Convert.ToString(fila("cCorreo"))
                End If

                If IsDBNull(fila("cDireccion")) Then
                    obj.cDireccion = Nothing
                Else
                    obj.cDireccion =
                        Convert.ToString(fila("cDireccion"))
                End If

                obj.bEstado =
                    Convert.ToBoolean(fila("bEstado"))

                obj.dFechaRegistro =
                    Convert.ToDateTime(fila("dFechaRegistro"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR PROVEEDOR
        '==================================================

        <WebMethod()>
        Public Shared Function GuardarProveedor(proveedor As ModeloProveedor) As String

            Try

                proveedor.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR PROVEEDOR
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarProveedor(proveedor As ModeloProveedor) As String

            Try

                proveedor.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' DESACTIVAR PROVEEDOR
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarProveedor(idProveedor As Integer) As String

            Try

                Dim objProveedor As New ModeloProveedor()

                objProveedor.iCodProveedor = idProveedor
                objProveedor.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER PROVEEDOR POR ID
        '==================================================

        <WebMethod()>
        Public Shared Function ObtenerProveedor(idProveedor As Integer) As ModeloProveedor

            Dim objProveedor As New ModeloProveedor()

            objProveedor.iCodProveedor = idProveedor
            objProveedor.getRecord()

            Return objProveedor

        End Function

    End Class

End Namespace