Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.Text

Partial Public Class Login
    Inherits Global.System.Web.UI.Page

    Private ReadOnly cadenaConexion As String =
        "Data Source=.\SQLEXPRESS;" &
        "Initial Catalog=DB_keyove_inventario;" &
        "Integrated Security=True;" &
        "TrustServerCertificate=True;"

    Protected Sub btnIngresar_Click(
        sender As Object,
        e As EventArgs
    )

        lblMensaje.Text = ""

        Dim usuario As String = txtUsuario.Text.Trim()
        Dim contrasena As String = txtContrasena.Text

        If String.IsNullOrWhiteSpace(usuario) OrElse
           String.IsNullOrWhiteSpace(contrasena) Then

            lblMensaje.Text = "Ingrese su usuario y contraseña."
            Return
        End If

        Dim hashIngresado As String = GenerarHash(contrasena)

        Dim consulta As String =
            "SELECT TOP 1 " &
            "u.iCodUsuario, u.cNombreUsuario, u.iCodRol, " &
            "r.cNombre AS Rol, p.cNombres, p.cApellidos " &
            "FROM Usuarios u " &
            "INNER JOIN Roles r ON r.iCodRol = u.iCodRol " &
            "LEFT JOIN Personas p ON p.iCodPersona = u.iCodPersona " &
            "WHERE u.cNombreUsuario = @Usuario " &
            "AND u.cContrasenaHash = @Hash " &
            "AND u.bEstado = 1 " &
            "AND r.bEstado = 1"

        Try
            Using conexion As New SqlConnection(cadenaConexion)
                Using comando As New SqlCommand(consulta, conexion)

                    comando.Parameters.AddWithValue("@Usuario", usuario)
                    comando.Parameters.AddWithValue("@Hash", hashIngresado)

                    conexion.Open()

                    Using lector As SqlDataReader = comando.ExecuteReader()

                        If lector.Read() Then

                            Session("iCodUsuario") =
                                Convert.ToInt32(lector("iCodUsuario"))

                            Session("Usuario") =
                                Convert.ToString(lector("cNombreUsuario"))

                            Session("Rol") =
                                Convert.ToString(lector("Rol"))

                            Session("NombreCompleto") =
                                Convert.ToString(lector("cNombres")) & " " &
                                Convert.ToString(lector("cApellidos"))

                            Response.Redirect(
                                "~/Views/Mantenimiento/Roles/Roles.aspx"
                            )

                        Else
                            lblMensaje.Text =
                                "Usuario o contraseña incorrectos."
                        End If

                    End Using
                End Using
            End Using

        Catch ex As Exception
            lblMensaje.Text = "No se pudo conectar con la base de datos."
        End Try

    End Sub

    Private Shared Function GenerarHash(texto As String) As String

        Using sha As SHA256 = SHA256.Create()

            Dim bytes As Byte() =
                sha.ComputeHash(Encoding.UTF8.GetBytes(texto))

            Dim resultado As New StringBuilder()

            For Each b As Byte In bytes
                resultado.Append(b.ToString("x2"))
            Next

            Return resultado.ToString()

        End Using

    End Function

End Class