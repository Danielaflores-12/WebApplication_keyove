<%@ Page Language="vb" %>
<script runat="server">
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Response.Redirect("Ventas.aspx?vista=registros", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub
</script>