<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="prueba conexion.aspx.vb" Inherits="WebApplication_keyove.prueba_conexion" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Prueba de conexión con SQL Server</h2>

        <asp:Button
            ID="btnProbarConexion"
            runat="server"
            Text="Probar conexión" />

        <br /><br />

        <asp:Label
            ID="lblResultado"
            runat="server">
        </asp:Label>
    </form>
</body>
</html>
