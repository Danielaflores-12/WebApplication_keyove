<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Dashboard.aspx.vb"
    Inherits="WebApplication_keyove.Views.Dashboard.Dashboard" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">
</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Dashboard</h1>

        <p>Resumen del sistema de inventario KEYOVE</p>

    </div>

    <div class="stat-grid">

        <div class="stat-card">

            <div class="stat-icono azul">&#8364;</div>

            <div class="stat-info">

                <span class="stat-valor"
                      id="statVentasHoy">
                    12
                </span>

                <span class="stat-label">
                    Ventas de hoy
                </span>

            </div>

        </div>

        <div class="stat-card">

            <div class="stat-icono verde">&#9632;</div>

            <div class="stat-info">

                <span class="stat-valor"
                      id="statProductosRegistrados">
                    128
                </span>

                <span class="stat-label">
                    Productos registrados
                </span>

            </div>

        </div>

        <div class="stat-card">

            <div class="stat-icono rojo">&#9650;</div>

            <div class="stat-info">

                <span class="stat-valor"
                      id="statStockBajo">
                    5
                </span>

                <span class="stat-label">
                    Productos con stock bajo
                </span>

            </div>

        </div>

        <div class="stat-card">

            <div class="stat-icono violeta">&#128722;</div>

            <div class="stat-info">

                <span class="stat-valor"
                      id="statComprasRealizadas">
                    8
                </span>

                <span class="stat-label">
                    Compras realizadas
                </span>

            </div>

        </div>

    </div>

    <div class="card">

        <div class="card-title">
            Productos más vendidos
        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaMasVendidos">

                <thead>

                    <tr>
                        <th>Producto</th>
                        <th>Cantidad vendida</th>
                        <th>Total</th>
                    </tr>

                </thead>

                <tbody id="bodyMasVendidos">

                    <tr>
                        <td class="fila-ejemplo">
                            Pendiente de conexión con SQL Server
                        </td>
                        <td class="fila-ejemplo">-</td>
                        <td class="fila-ejemplo">-</td>
                    </tr>

                </tbody>

            </table>

        </div>

    </div>

</asp:Content>