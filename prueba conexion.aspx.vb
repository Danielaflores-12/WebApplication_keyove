Public Class prueba_conexion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnProbarConexion_Click(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles btnProbarConexion.Click

        Try
            Using conexion As New Data.ConexionBD()

                Dim resultado As String =
                    conexion.OpenConnection()

                If resultado = "OK" Then

                    lblResultado.Text =
                        "Conexión exitosa con SQL Server."

                    lblResultado.ForeColor =
                        System.Drawing.Color.Green

                Else

                    lblResultado.Text =
                        "Error de conexión: " & resultado

                    lblResultado.ForeColor =
                        System.Drawing.Color.Red

                End If

            End Using

        Catch ex As Exception

            lblResultado.Text =
                "Error: " & ex.Message

            lblResultado.ForeColor =
                System.Drawing.Color.Red

        End Try

    End Sub
End Class