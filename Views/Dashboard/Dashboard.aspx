<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Dashboard.aspx.vb"
    Inherits="WebApplication_keyove.Views.Dashboard.Dashboard" %>


<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style>

        .dashboard-grafico {
            width: 100%;
            margin-top: 20px;
        }

        .grafico-barras {
            display: flex;
            align-items: flex-end;
            justify-content: space-around;
            gap: 12px;
            height: 300px;
            padding: 20px 10px 0 10px;
            border-bottom: 1px solid #e5e7eb;
        }

        .barra-item {
            flex: 1;
            min-width: 45px;
            height: 100%;
            display: flex;
            flex-direction: column;
            justify-content: flex-end;
            align-items: center;
            gap: 7px;
        }

        .barra {
            width: 65%;
            height: 100px;
            min-height: 4px;
            background: #2563eb;
            border-radius: 6px 6px 0 0;
        }

        .barra-valor {
            font-size: 11px;
            font-weight: 600;
            color: #374151;
            text-align: center;
            white-space: nowrap;
        }

        .barra-mes {
            font-size: 12px;
            color: #6b7280;
            text-align: center;
        }

        .sin-ventas {
            display: flex;
            align-items: center;
            justify-content: center;
            height: 280px;
            color: #6b7280;
        }

    </style>

</asp:Content>


<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">


    <!-- ====================================================== -->
    <!-- TITULO -->
    <!-- ====================================================== -->

    <div class="page-title">

        <h1>Dashboard</h1>

        <p>
            Resumen del sistema de inventario KEYOVE
        </p>

    </div>


    <!-- ====================================================== -->
    <!-- TARJETAS -->
    <!-- ====================================================== -->

    <div class="stat-grid">


        <!-- VENTAS DE HOY -->

        <div class="stat-card">

            <div class="stat-icono azul">
                S/
            </div>

            <div class="stat-info">

                <asp:Label
                    ID="lblVentasHoy"
                    runat="server"
                    CssClass="stat-valor"
                    Text="0" />

                <span class="stat-label">
                    Ventas de hoy
                </span>

            </div>

        </div>


        <!-- PRODUCTOS REGISTRADOS -->

        <div class="stat-card">

            <div class="stat-icono verde">
                &#9632;
            </div>

            <div class="stat-info">

                <asp:Label
                    ID="lblProductosRegistrados"
                    runat="server"
                    CssClass="stat-valor"
                    Text="0" />

                <span class="stat-label">
                    Productos registrados
                </span>

            </div>

        </div>


        <!-- STOCK BAJO -->

        <div class="stat-card">

            <div class="stat-icono rojo">
                &#9650;
            </div>

            <div class="stat-info">

                <asp:Label
                    ID="lblStockBajo"
                    runat="server"
                    CssClass="stat-valor"
                    Text="0" />

                <span class="stat-label">
                    Productos con stock bajo
                </span>

            </div>

        </div>


        <!-- COMPRAS -->

        <div class="stat-card">

            <div class="stat-icono violeta">
                &#128722;
            </div>

            <div class="stat-info">

                <asp:Label
                    ID="lblComprasRealizadas"
                    runat="server"
                    CssClass="stat-valor"
                    Text="0" />

                <span class="stat-label">
                    Compras realizadas
                </span>

            </div>

        </div>

    </div>


    <!-- ====================================================== -->
    <!-- GRAFICO DE VENTAS -->
    <!-- ====================================================== -->

    <div class="card">

        <div class="card-title">
            Evolución de ventas
        </div>

        <p>
            Ventas reales registradas durante los últimos 12 meses.
        </p>


        <asp:Panel
            ID="pnlGraficoVentas"
            runat="server"
            CssClass="dashboard-grafico">


            <asp:Repeater
                ID="rptGraficoVentas"
                runat="server">

                <HeaderTemplate>

                    <div class="grafico-barras">

                </HeaderTemplate>


                <ItemTemplate>

                    <div class="barra-item">

                        <div class="barra-valor">
                            S/ <%# Eval("TotalVentas", "{0:N2}") %>
                        </div>


                        <div class="barra">
                        </div>


                        <div class="barra-mes">
                            <%# Eval("MesNombre") %>
                        </div>

                    </div>

                </ItemTemplate>


                <FooterTemplate>

                    </div>

                </FooterTemplate>

            </asp:Repeater>


        </asp:Panel>


        <asp:Panel
            ID="pnlSinVentas"
            runat="server"
            CssClass="sin-ventas"
            Visible="False">

            No existen ventas registradas durante los últimos 12 meses.

        </asp:Panel>

    </div>


    <!-- ====================================================== -->
    <!-- PRODUCTOS MAS VENDIDOS -->
    <!-- ====================================================== -->

    <div class="card">

        <div class="card-title">
            Productos más vendidos
        </div>


        <div class="table-container">

            <asp:GridView
                ID="gvMasVendidos"
                runat="server"
                CssClass="table"
                AutoGenerateColumns="False"
                ShowHeaderWhenEmpty="True"
                EmptyDataText="Aún no hay productos vendidos.">

                <Columns>

                    <asp:BoundField
                        DataField="Producto"
                        HeaderText="Producto" />


                    <asp:BoundField
                        DataField="CantidadVendida"
                        HeaderText="Cantidad vendida" />


                    <asp:BoundField
                        DataField="TotalVendido"
                        HeaderText="Total (S/)"
                        DataFormatString="{0:N2}" />

                </Columns>

            </asp:GridView>

        </div>

    </div>


</asp:Content>