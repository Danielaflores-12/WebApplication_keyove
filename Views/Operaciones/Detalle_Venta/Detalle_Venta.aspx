<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Detalle_Venta.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Detalle_Venta.DetalleVenta1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../../JScript/Detalle_Venta.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title">
        <h1>Productos de la venta</h1>
        <p>Elige un comprobante para consultar o agregar sus productos.</p>
    </div>
    <div class="card">
        <div class="card-title">1. Selecciona el comprobante</div>
        <div class="form-group">
            <label for="cboVenta">Comprobante de venta</label>
            <select id="cboVenta" class="input-control"><option value="">Cargando comprobantes...</option></select>
        </div>
    </div>
    <div class="card">
        <div class="card-title" id="tituloEditor">2. Agrega un producto</div>
        <p id="mensajeDetalle" role="status" aria-live="polite"></p>
        <fieldset id="editorDetalle" class="detalle-editor" disabled>
            <input type="hidden" id="txtIdDetalleVenta" />
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
                    <label for="txtPrecioVenta">Precio unitario (S/)</label>
                    <input type="number" id="txtPrecioVenta" min="0" step="0.01" class="input-control" />
                </div>
            </div>
            <details id="opcionesDescuento" class="detalle-opcional"><summary>Aplicar descuento (opcional)</summary><div class="form-group"><label for="txtDescuento">Descuento total del producto (S/)</label><input type="number" id="txtDescuento" min="0" step="0.01" value="0" class="input-control" /></div></details>
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
            <table class="table" id="tablaDetalleVenta">
                <thead><tr><th>Producto</th><th>Cantidad</th><th>Precio unitario (S/)</th><th>Descuento (S/)</th><th>Importe (S/)</th><th>Acciones</th></tr></thead>
                <tbody id="bodyDetalleVenta"></tbody>
            </table>
        </div>
    </div>
</asp:Content>
