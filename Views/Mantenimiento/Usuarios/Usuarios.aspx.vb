Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Security.Cryptography
Imports System.Text
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloUsuario = WebApplication_keyove.WebApplication_Keyove.Model.Usuarios
Imports ModeloRol = WebApplication_keyove.WebApplication_Keyove.Model.Roles
Imports ModeloPersona = WebApplication_keyove.WebApplication_Keyove.Model.Personas

Namespace WebApplication_Keyove.Views.Mantenimiento.Usuarios

    Partial Public Class Usuario1
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
        ' HASH DE CONTRASEÑA (SHA256)
        '==================================================

        Private Shared Function GenerarHash(
            texto As String
        ) As String

            Using sha As SHA256 = SHA256.Create()

                Dim bytes As Byte() =
                    sha.ComputeHash(
                        Encoding.UTF8.GetBytes(texto)
                    )

                Dim sb As New StringBuilder()

                For Each b As Byte In bytes

                    sb.Append(b.ToString("x2"))

                Next

                Return sb.ToString()

            End Using

        End Function


        '==================================================
        ' LISTAR ROLES PARA COMBOBOX
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarRolesCombo() As List(Of OpcionCombo)

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloRol().ListaDatosCombo()

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
        ' LISTAR PERSONAS PARA COMBOBOX
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarPersonasCombo() As List(Of OpcionCombo)

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloPersona().ListaDatosCombo()

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
        ' LISTAR USUARIOS
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarUsuarios() As List(Of ModeloUsuario)

            Dim lista As New List(Of ModeloUsuario)()

            Dim tabla As DataTable =
                New ModeloUsuario().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloUsuario()

                obj.iCodUsuario =
                    Convert.ToInt32(fila("iCodUsuario"))

                If IsDBNull(fila("iCodPersona")) Then
                    obj.iCodPersona = Nothing
                Else
                    obj.iCodPersona =
                        Convert.ToInt32(fila("iCodPersona"))
                End If

                obj.iCodRol =
                    Convert.ToInt32(fila("iCodRol"))

                obj.cNombreRol =
                    Convert.ToString(fila("Rol"))

                obj.cNombrePersona =
                    Convert.ToString(fila("cNombrePersona"))

                obj.cNombreUsuario =
                    Convert.ToString(fila("cNombreUsuario"))

                obj.cContrasenaHash = Nothing

                obj.dFechaRegistro =
                    Convert.ToDateTime(fila("dFechaRegistro"))

                obj.bEstado =
                    Convert.ToBoolean(fila("bEstado"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR USUARIO
        '==================================================

        <WebMethod()>
        Public Shared Function GuardarUsuario(usuario As ModeloUsuario) As String

            Try

                If String.IsNullOrWhiteSpace(
                    usuario.cContrasenaHash
                ) Then

                    Return "ERROR: Ingresa una contraseña."

                End If

                usuario.cContrasenaHash =
                    GenerarHash(
                        usuario.cContrasenaHash
                    )

                usuario.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR USUARIO
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarUsuario(usuario As ModeloUsuario) As String

            Try

                If String.IsNullOrWhiteSpace(
                    usuario.cContrasenaHash
                ) Then

                    ' Si no se ingresó nueva contraseña,
                    ' se conserva la actual.
                    Dim objExistente As New ModeloUsuario()

                    objExistente.iCodUsuario =
                        usuario.iCodUsuario

                    objExistente.getRecord()

                    usuario.cContrasenaHash =
                        objExistente.cContrasenaHash

                Else

                    usuario.cContrasenaHash =
                        GenerarHash(
                            usuario.cContrasenaHash
                        )

                End If

                usuario.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' DESACTIVAR USUARIO
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarUsuario(idUsuario As Integer) As String

            Try

                Dim objUsuario As New ModeloUsuario()

                objUsuario.iCodUsuario = idUsuario
                objUsuario.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER USUARIO POR ID
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerUsuario(idUsuario As Integer) As ModeloUsuario

            Dim objUsuario As New ModeloUsuario()

            objUsuario.iCodUsuario = idUsuario
            objUsuario.getRecord()

            Return objUsuario

        End Function

    End Class

End Namespace
