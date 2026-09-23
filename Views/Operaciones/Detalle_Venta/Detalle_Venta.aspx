<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Detalle_Venta.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Detalle_Venta.DetalleVenta1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Detalle_Venta.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Detalle de Ventas</h1>

        <p>Registro de detalle de ventas</p>

    </div>

    <div class="card">

        <div class="card-title">
            Nuevo detalle
        </div>

        <input type="hidden" id="txtIdDetalleVenta" />

        <div class="form-grid">

            <div class="form-group">

                <label for="cboVenta">Venta</label>

                <select id="cboVenta"
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

                <label for="txtPrecioVenta">Precio de Venta</label>

                <input type="number"
                       step="0.01"
                       id="txtPrecioVenta"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtDescuento">Descuento</label>

                <input type="number"
                       step="0.01"
                       id="txtDescuento"
                       class="input-control"
                       value="0" />

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
                    onclick="guardarDetalleVenta()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarDetalleVenta()">
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
            Detalles de venta registrados
        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaDetalleVenta">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Venta</th>
                        <th>Producto</th>
                        <th>Cantidad</th>
                        <th>Precio</th>
                        <th>Descuento</th>
                        <th>Sub Total</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyDetalleVenta">

                </tbody>

            </table>

        </div>

    </div>

</asp:Content>