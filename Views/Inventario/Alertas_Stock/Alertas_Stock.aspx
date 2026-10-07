<%@ Page Title="Alertas de Stock"
    Language="vb"
    AutoEventWireup="false"
    CodeBehind="Alertas_Stock.aspx.vb"
    Inherits="WebApplication_keyove.Alertas_Stock"
    MasterPageFile="~/Views/Master/Site.Master" %>


<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">


    <!-- ====================================================== -->
    <!-- TITULO -->
    <!-- ====================================================== -->

    <div class="page-title">

        <h1>
            Alertas de Stock
        </h1>

        <p>
            Productos con stock bajo o agotado.
        </p>

    </div>


    <!-- ====================================================== -->
    <!-- TABLA DE ALERTAS -->
    <!-- ====================================================== -->

    <div class="card">

        <div class="card-title">
            Productos que necesitan atención
        </div>


        <div class="table-container">

            <asp:GridView
                ID="gvAlertasStock"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table"
                ShowHeaderWhenEmpty="True"
                EmptyDataText="No existen productos con alerta de stock.">

                <Columns>


                    <asp:BoundField
                        DataField="NombreProducto"
                        HeaderText="Producto" />


                    <asp:BoundField
                        DataField="iStockActual"
                        HeaderText="Stock Actual" />


                    <asp:BoundField
                        DataField="iStockMinimo"
                        HeaderText="Stock Mínimo" />


                    <asp:BoundField
                        DataField="EstadoStock"
                        HeaderText="Estado" />


                </Columns>

            </asp:GridView>

        </div>

    </div>


</asp:Content>