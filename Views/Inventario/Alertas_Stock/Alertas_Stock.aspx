<%@ Page Title="Alertas de Stock" 
    Language="vb" 
    AutoEventWireup="false" 
    CodeBehind="Alertas_Stock.aspx.vb" 
    Inherits="WebApplication_keyove.Alertas_Stock" 
    MasterPageFile="~/Views/Master/Site.Master" %>


<asp:Content ID="Content1" 
    ContentPlaceHolderID="head" 
    runat="server">

</asp:Content>


<asp:Content ID="Content2" 
    ContentPlaceHolderID="ContentPlaceHolder1" 
    runat="server">


    <div class="container-fluid">

        <h1 class="mb-3">
            Alertas de Stock
        </h1>


        <p>
            Productos con stock bajo o agotado.
        </p>


        <div class="card">

            <div class="card-body">

                <asp:GridView 
                    ID="gvAlertasStock"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-bordered table-striped"
                    EmptyDataText="No existen productos con alerta de stock">

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


    </div>


</asp:Content>