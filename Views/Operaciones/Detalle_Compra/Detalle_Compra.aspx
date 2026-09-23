<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Detalle_Compra.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Detalle_Compra.DetalleCompra1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Detalle_Compra.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Detalle de Compras</h1>

        <p>Registro de detalle de compras</p>

    </div>

    <div class="card">

        <div class="card-title">
            Nuevo detalle
        </div>

        <input type="hidden" id="txtIdDetalleCompra" />

        <div class="form-grid">

            <div class="form-group">

                <label for="cboCompra">Compra</label>

                <select id="cboCompra"
                        class="input-control">
                </select>

            </div>

            <div class="form-group">

                <label for="cboProducto">Producto</label>

                <select id="cboProducto"
                        class="input-control">
                </select>

            </div>

            <div class="form-group">

                <label for="txtCantidad">Cantidad</label>

                <input type="number"
                       id="txtCantidad"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtPrecioCompra">Precio de Compra</label>

                <input type="number"
                       step="0.01"
                       id="txtPrecioCompra"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtSubTotal">Sub Total</label>

                <input type="number"
                       step="0.01"
                       id="txtSubTotal"
                       class="input-control"
                       readonly />

            </div>

        </div>

        <div class="form-actions">

            <button type="button"
                    class="btn btn-primary"
                    onclick="guardarDetalleCompra()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarDetalleCompra()">
                Modificar
            </button>

            <button type="button"
                    class="btn btn-secondary"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

    </div>

    <div class="card">

        <div class="card-title">
            Detalles de compra registrados
        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaDetalleCompra">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Compra</th>
                        <th>Producto</th>
                        <th>Cantidad</th>
                        <th>Precio</th>
                        <th>Sub Total</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyDetalleCompra">

                </tbody>

            </table>

        </div>

    </div>

</asp:Content>