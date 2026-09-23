<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Detalle_Compra.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Detalle_Compra.DetalleCompra1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../../JScript/Detalle_Compra.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title">
        <h1>Productos de la compra</h1>
        <p>Elige un comprobante para consultar o agregar sus productos.</p>
    </div>
    <nav class="ventas-vistas" aria-label="Ir a compras">
        <a class="btn btn-secondary" href="../Compras/Compras.aspx">Registrar compra</a>
        <a class="btn btn-secondary" href="../Compras/Compras.aspx?vista=registros">Ver compras realizadas</a>
    </nav>
    <div class="card">
        <div class="card-title">1. Selecciona el comprobante</div>
        <div class="form-group">
            <label for="cboCompra">Comprobante de compra</label>
            <select id="cboCompra" class="input-control"><option value="">Cargando comprobantes...</option></select>
        </div>
    </div>
    <div class="card">
        <div class="card-title" id="tituloEditor">2. Agrega un producto</div>
        <p id="mensajeDetalle" role="status" aria-live="polite"></p>
        <fieldset id="editorDetalle" class="detalle-editor" disabled>
            <input type="hidden" id="txtIdDetalleCompra" />
            <div class="form-grid">
                <div class="form-group">
                    <label for="cboProducto">Producto</label>
                    <select id="cboProducto" class="input-control"><option value="">Cargando productos...</option></select>
                </div>
                <div class="form-group">
                    <label for="txtCantidad">Cantidad</label>
                    <input type="number" id="txtCantidad" min="1" step="1" value="1" class="input-control" />
                </div>
                <div class="form-group">
                    <label for="txtPrecioCompra">Precio unitario (S/)</label>
                    <input type="number" id="txtPrecioCompra" min="0" step="0.01" class="input-control" />
                </div>
            </div>

            <input type="hidden" id="txtSubTotal" value="0.00" />
            <p class="detalle-importe">Importe del producto: <strong id="importeDetalle" aria-live="polite">S/ 0.00</strong></p>
            <div class="form-actions">
                <button type="button" id="btnGuardarDetalle" class="btn btn-primary" onclick="enviarDetalle()">Agregar producto</button>
                <button type="button" id="btnCancelarDetalle" class="btn btn-secondary" onclick="limpiarFormulario()">Limpiar producto</button>
            </div>
        </fieldset>
    </div>
    <div class="card">
        <div class="card-title">Productos del comprobante</div>
        <div class="table-container">
            <table class="table" id="tablaDetalleCompra">
                <thead><tr><th>Producto</th><th>Cantidad</th><th>Precio unitario (S/)</th><th>Importe (S/)</th><th>Acciones</th></tr></thead>
                <tbody id="bodyDetalleCompra"></tbody>
            </table>
        </div>
    </div>
</asp:Content>
