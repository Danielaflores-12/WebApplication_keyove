<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Compras.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Compras.Compra1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Compras.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Compras</h1>

        <p>Registro de compras</p>

    </div>

    <nav class="ventas-vistas" aria-label="Secciones de compras">
        <button type="button" class="btn btn-primary" data-vista-compras="registro" aria-controls="panelRegistroCompra" aria-pressed="true">Registrar compra</button>
        <button type="button" class="btn btn-secondary" data-vista-compras="registros" aria-controls="panelComprasRegistradas" aria-pressed="false">Ver compras realizadas</button>
    </nav>
    <section id="panelRegistroCompra" aria-label="Registrar compra">
    <div class="card">

        <div class="card-title">
            Nueva compra
        </div>

        <input type="hidden" id="txtIdCompra" />
        <input type="hidden" id="txtSubTotal" />
        <input type="hidden" id="txtIgv" />
        <input type="hidden" id="txtTotal" />

        <div class="form-grid">

            <div class="form-group">

                <label for="cboProveedor">Proveedor</label>

                <select id="cboProveedor"
                        class="input-control"></select>

            </div>

            <div class="form-group">

                <label for="cboUsuario">Usuario</label>

                <select id="cboUsuario"
                        class="input-control"></select>

            </div>

            <div class="form-group">

                <label for="cboTipoComprobante">Tipo de Comprobante</label>

                <select id="cboTipoComprobante"
                        class="input-control">
                    <option value="BOLETA">BOLETA</option>
                    <option value="FACTURA">FACTURA</option>
                </select>

            </div>

            <div class="form-group">

                <label for="txtNumeroComprobante">Número de Comprobante</label>

                <input type="text"
                       id="txtNumeroComprobante" readonly placeholder="Se genera al guardar"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtObservacion">Observación</label>

                <input type="text"
                       id="txtObservacion"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="cboEstado">Estado</label>

                <select id="cboEstado"
                        class="input-control">
                    <option value="REGISTRADA">REGISTRADA</option>
                    <option value="ANULADA">ANULADA</option>
                </select>

            </div>

        </div>

        <div class="card-title">
            Productos de la compra
        </div>

        <div class="form-grid">

            <div class="form-group">

                <label for="cboProducto">Producto</label>

                <select id="cboProducto"
                        class="input-control"></select>

            </div>

            <div class="form-group">

                <label for="txtCantidadCompra">Cantidad</label>

                <input type="number"
                       id="txtCantidadCompra"
                       class="input-control"
                       oninput="actualizarSubtotalLineaCompra()" />

            </div>

            <div class="form-group">

                <label for="txtPrecioCompra">Precio Compra</label>

                <input type="number"
                       step="0.01"
                       id="txtPrecioCompra"
                       class="input-control"
                       oninput="actualizarSubtotalLineaCompra()" />

            </div>

            <div class="form-group">

                <label for="txtSubtotalLinea">Subtotal de línea</label>

                <input type="text"
                       id="txtSubtotalLinea"
                       class="input-control"
                       readonly />

            </div>

        </div>

        <div class="form-actions">

            <button type="button"
                    class="btn btn-primary"
                    onclick="agregarProductoCompra()">
                Agregar producto
            </button>

        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaDetalleCompra">

                <thead>

                    <tr>
                        <th>Producto</th>
                        <th>Cantidad</th>
                        <th>Precio</th>
                        <th>Subtotal</th>
                        <th>Acción</th>
                    </tr>

                </thead>

                <tbody id="bodyDetalleCompra">

                </tbody>

            </table>

        </div>

        <div class="form-grid">

            <div class="form-group">

                <label for="txtSubtotalCompraGral">Subtotal general</label>

                <input type="text"
                       id="txtSubtotalCompraGral"
                       class="input-control"
                       readonly />

            </div>

            <div class="form-group">

                <label for="txtIGVCompra">IGV 18%</label>

                <input type="text"
                       id="txtIGVCompra"
                       class="input-control"
                       readonly />

            </div>

            <div class="form-group">

                <label for="txtTotalCompra">Total</label>

                <input type="text"
                       id="txtTotalCompra"
                       class="input-control"
                       readonly />

            </div>

        </div>

        <div class="form-actions">

            <button type="button"
                    class="btn btn-primary"
                    onclick="guardarCompra()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarCompra()">
                Modificar
            </button>

            <button type="button"
                    class="btn btn-secondary"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

    </div>

    </section>
    <section id="panelComprasRegistradas" aria-label="Compras realizadas" hidden>
        <div class="card">
            <div class="form-group">
                <label for="buscarCompras">Buscar compras</label>
                <input type="search" id="buscarCompras" class="input-control" placeholder="Comprobante, proveedor, usuario, fecha o estado" />
            </div>
            <div class="form-actions">
                <button type="button" class="btn btn-secondary" onclick="listarCompras()">Actualizar lista</button>
            </div>
            <p id="estadoBusquedaCompras" role="status" aria-live="polite"></p>
        </div>
    <div class="card">

        <div class="card-title">
            Compras realizadas
        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaCompras">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Proveedor</th>
                        <th>Usuario</th>
                        <th>Comprobante</th>
                        <th>N° Comprobante</th>
                        <th>Fecha</th>
                        <th>Sub Total</th>
                        <th>IGV</th>
                        <th>Total</th>
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyCompras">

                </tbody>

            </table>

        </div>

    </div>

    </section>
</asp:Content>
