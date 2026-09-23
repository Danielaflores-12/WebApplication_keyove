<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Dashboard.aspx.vb"
    Inherits="WebApplication_keyove.Views.Dashboard.Dashboard" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">
</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">
        <h1>Dashboard</h1>
        <p>Resumen del sistema de inventario KEYOVE</p>
    </div>

    <div class="stat-grid">

        <div class="stat-card">
            <div class="stat-icono azul">S/</div>
            <div class="stat-info">
                <asp:Label ID="lblVentasHoy"
                    runat="server"
                    CssClass="stat-valor"
                    Text="0" />
                <span class="stat-label">Ventas de hoy</span>
            </div>
        </div>

        <div class="stat-card">
            <div class="stat-icono verde">&#9632;</div>
            <div class="stat-info">
                <asp:Label ID="lblProductosRegistrados"
                    runat="server"
                    CssClass="stat-valor"
                    Text="0" />
                <span class="stat-label">Productos registrados</span>
            </div>
        </div>

        <div class="stat-card">
            <div class="stat-icono rojo">&#9650;</div>
            <div class="stat-info">
                <asp:Label ID="lblStockBajo"
                    runat="server"
                    CssClass="stat-valor"
                    Text="0" />
                <span class="stat-label">Productos con stock bajo</span>
            </div>
        </div>

        <div class="stat-card">
            <div class="stat-icono violeta">&#128722;</div>
            <div class="stat-info">
                <asp:Label ID="lblComprasRealizadas"
                    runat="server"
                    CssClass="stat-valor"
                    Text="0" />
                <span class="stat-label">Compras realizadas</span>
            </div>
        </div>

    </div>

    <div class="card">
        <div class="card-title">Evolución de ventas</div>
        <p>Ventas reales por mes</p>

        <canvas id="graficoVentas"
                style="width: 100%; height: 300px;"
                aria-label="Gráfico de ventas mensuales">
        </canvas>

        <asp:Label ID="lblEstadoPrediccion"
            runat="server"
            Text="Aún no hay historial de ventas para generar una predicción." />
    </div>

    <div class="card">
        <div class="card-title">Productos más vendidos</div>

        <div class="table-container">
            <asp:GridView ID="gvMasVendidos"
                runat="server"
                CssClass="table"
                AutoGenerateColumns="False"
                ShowHeaderWhenEmpty="True"
                EmptyDataText="Aún no hay productos vendidos.">

                <Columns>
                    <asp:BoundField DataField="Producto"
                        HeaderText="Producto" />

                    <asp:BoundField DataField="CantidadVendida"
                        HeaderText="Cantidad vendida" />

                    <asp:BoundField DataField="TotalVendido"
                        HeaderText="Total (S/)"
                        DataFormatString="{0:N2}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>