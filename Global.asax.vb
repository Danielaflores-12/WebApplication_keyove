Imports System.Web
Imports System.Web.SessionState

Public Class Global_asax
    Inherits HttpApplication

    Private Function EsPaginaAspx() As Boolean

        Return Request.AppRelativeCurrentExecutionFilePath.EndsWith(
            ".aspx",
            StringComparison.OrdinalIgnoreCase
        )

    End Function

    Sub Application_PostMapRequestHandler(
        ByVal sender As Object,
        ByVal e As EventArgs
    )

        If EsPaginaAspx() Then
            Context.SetSessionStateBehavior(
                SessionStateBehavior.Required
            )
        End If

    End Sub

    Sub Application_PostAcquireRequestState(
        ByVal sender As Object,
        ByVal e As EventArgs
    )

        If Not EsPaginaAspx() Then Return

        Dim pagina As String =
            Request.AppRelativeCurrentExecutionFilePath.ToLowerInvariant()

        Dim metodo As String =
            Request.PathInfo.Trim("/"c).ToLowerInvariant()

        ' El login es público, pero no sus posibles métodos adicionales.
        If pagina = "~/views/login.aspx" AndAlso
           metodo = "" Then
            Return
        End If

        If Context.Session Is Nothing OrElse
           Context.Session("iCodUsuario") Is Nothing Then

            If metodo <> "" OrElse Request.HttpMethod <> "GET" Then
                DenegarAcceso(401, "Debe iniciar sesión.")
            Else
                Response.Redirect(
                    VirtualPathUtility.ToAbsolute("~/Views/Login.aspx"),
                    False
                )
                CompleteRequest()
            End If

            Return
        End If

        Dim rol As String =
            Convert.ToString(Context.Session("Rol")).Trim()

        If String.Equals(
            rol,
            "Administrador",
            StringComparison.OrdinalIgnoreCase
        ) Then
            Return
        End If

        If Not String.Equals(
            rol,
            "Vendedor",
            StringComparison.OrdinalIgnoreCase
        ) Then
            DenegarAcceso(403, "Su rol no tiene acceso.")
            Return
        End If

        Select Case pagina

            Case "~/views/operaciones/ventas/ventas.aspx"

                If metodo = "" Then Return

                Select Case metodo
                    Case "listarusuarioscombo",
                         "listarproductoscombo",
                         "listarventas",
                         "guardarventa",
                         "obtenerventa"

                        If Request.HttpMethod = "POST" Then Return
                End Select

            Case "~/views/operaciones/ventas/ventasregistradas.aspx",
                 "~/views/operaciones/ventas/comprobante.aspx"

                If metodo = "" AndAlso
                   Request.HttpMethod = "GET" Then
                    Return
                End If

        End Select

        DenegarAcceso(
            403,
            "No tiene permiso para acceder a esta opción."
        )

    End Sub

    Private Sub DenegarAcceso(
        ByVal codigo As Integer,
        ByVal mensaje As String
    )

        Response.Clear()
        Response.StatusCode = codigo
        Response.TrySkipIisCustomErrors = True
        Response.SuppressFormsAuthenticationRedirect = True
        Response.ContentType = "application/json; charset=utf-8"
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()

        Response.Write(
            "{""Message"":""" & mensaje & """}"
        )

        CompleteRequest()

    End Sub

End Class