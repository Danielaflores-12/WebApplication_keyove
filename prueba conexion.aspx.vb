Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Drawing
Imports WebApplication_keyove.WebApplication_Keyove.Data

Public Class prueba_conexion
    Inherits Global.System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

    End Sub


    Protected Sub btnProbarConexion_Click(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles btnProbarConexion.Click

        Try

            Using conexion As New ConexionBD()

                'OpenConnection ahora devuelve True o False
                Dim resultado As Boolean =
                    conexion.OpenConnection()

                If resultado Then

                    lblResultado.Text =
                        "Conexión exitosa con SQL Server."

                    lblResultado.ForeColor =
                        Color.Green

                Else

                    lblResultado.Text =
                        "No se pudo conectar con SQL Server."

                    lblResultado.ForeColor =
                        Color.Red

                End If

            End Using

        Catch ex As Exception

            lblResultado.Text =
                "Error de conexión: " & ex.Message

            lblResultado.ForeColor =
                Color.Red

        End Try

    End Sub


End Class