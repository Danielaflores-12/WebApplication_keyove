Imports System.Web
Imports System.Web.Security

Public Class Site
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles Me.Load

        If Session("iCodUsuario") Is Nothing Then
            IrAlLogin()
            Return
        End If

        Dim rol As String =
            Convert.ToString(Session("Rol")).Trim()

        Dim esAdministrador As Boolean =
            String.Equals(
                rol,
                "Administrador",
                StringComparison.OrdinalIgnoreCase
            )

        Dim esVendedor As Boolean =
            String.Equals(
                rol,
                "Vendedor",
                StringComparison.OrdinalIgnoreCase
            )

        If Not esAdministrador AndAlso Not esVendedor Then
            Session.Clear()
            Session.Abandon()
            FormsAuthentication.SignOut()
            IrAlLogin()
            Return
        End If

        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()
        Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1))

        Dim nombre As String =
            Convert.ToString(Session("NombreCompleto")).Trim()

        If String.IsNullOrWhiteSpace(nombre) Then
            nombre = Convert.ToString(Session("Usuario")).Trim()
        End If

        litNombre.Text = nombre
        litRol.Text = rol

        If String.IsNullOrWhiteSpace(nombre) Then
            litInicial.Text = "U"
        Else
            litInicial.Text =
                nombre.Substring(0, 1).ToUpperInvariant()
        End If

        menuDashboard.Visible = esAdministrador
        menuMantenimiento.Visible = esAdministrador
        menuComprasAdmin.Visible = esAdministrador
        menuInventario.Visible = esAdministrador

        menuVentasPermitido.Visible =
            esAdministrador OrElse esVendedor

    End Sub

    Protected Sub CerrarSesion_Click(
        ByVal sender As Object,
        ByVal e As EventArgs
    )

        Session.Clear()
        Session.Abandon()
        FormsAuthentication.SignOut()

        IrAlLogin()

    End Sub

    Private Sub IrAlLogin()

        Response.Redirect("~/Views/Login.aspx", False)
        Context.ApplicationInstance.CompleteRequest()

    End Sub

End Class