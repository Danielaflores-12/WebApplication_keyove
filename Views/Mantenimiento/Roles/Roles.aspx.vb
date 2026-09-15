Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloRoles = WebApplication_keyove.WebApplication_Keyove.Model.Roles

Namespace WebApplication_Keyove.Views.Mantenimiento.Roles

    Partial Public Class Rol1
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

        End Sub


        '==================================================
        ' LISTAR ROLES
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarRoles() As List(Of ModeloRoles)

            Dim lista As New List(Of ModeloRoles)()

            Dim tabla As DataTable =
                New ModeloRoles().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloRoles()

                obj.iCodRol =
                    Convert.ToInt32(fila("iCodRol"))

                obj.cNombre =
                    Convert.ToString(fila("cNombre"))

                If IsDBNull(fila("cDescripcion")) Then
                    obj.cDescripcion = Nothing
                Else
                    obj.cDescripcion =
                        Convert.ToString(fila("cDescripcion"))
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
        ' GUARDAR ROL
        '==================================================

        <WebMethod()>
        Public Shared Function GuardarRol(rol As ModeloRoles) As String

            Try

                rol.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR ROL
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarRol(rol As ModeloRoles) As String

            Try

                rol.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' DESACTIVAR ROL
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarRol(idRol As Integer) As String

            Try

                Dim objRol As New ModeloRoles()

                objRol.iCodRol = idRol
                objRol.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER ROL POR ID
        '==================================================

        <WebMethod()>
        Public Shared Function ObtenerRol(idRol As Integer) As ModeloRoles

            Dim objRol As New ModeloRoles()

            objRol.iCodRol = idRol
            objRol.getRecord()

            Return objRol

        End Function

    End Class

End Namespace