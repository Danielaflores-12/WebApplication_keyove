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
    <style>
      .compra-dialog { margin:auto; border:0; padding:0; border-radius:10px; width:min(520px,calc(100vw - 32px)); max-height:90vh; box-shadow:0 16px 55px #0005; color:#263247; }
      .compra-dialog::backdrop { background:#17233399; }
      .compra-dialog header { background:#28538b; color:white; padding:16px 20px; display:flex; justify-content:space-between; align-items:center; }
      .compra-dialog h2 { margin:0; font-size:18px; }
      .compra-dialog header button { border:0; background:none; color:white; font-size:24px; cursor:pointer; }
      .compra-dialog .modal-cuerpo { padding:20px; display:grid; gap:12px; }
      .compra-dialog footer { padding:14px 20px; background:#f3f5f8; display:flex; justify-content:flex-end; gap:8px; }
      .compra-dialog .modal-error { color:#b42318; margin:0; }
      .compra-dialog .form-grid { grid-template-columns:repeat(2,minmax(0,1fr)); }
      .compra-completa { width:min(1120px,calc(100vw - 24px)); }
      .compra-completa > header { position:sticky; top:0; z-index:2; }
      .compra-completa .card { margin:0; box-shadow:none; min-width:0; }
      .aviso-revision { padding:12px 20px; margin:0; background:#eef4fc; }
      .compra-subseccion { border:1px solid #cbd5e1; border-radius:8px; margin:16px 0; }
      .compra-subseccion[hidden] { display:none; }
      .compra-subseccion header { padding:12px; background:#eef4fc; color:#263247; }
      .compra-subseccion header button { color:#263247; }
      .compra-subseccion .modal-cuerpo { padding:16px; }
      .compra-subseccion h2 { font-size:16px; }
      .compra-completa input, .compra-completa select, .compra-completa textarea { min-width:0; max-width:100%; }
      @media(max-width:480px) { .compra-dialog .form-grid { grid-template-columns:1fr; } }
    </style>

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
    <div class="card" id="formularioCompletoCompra">

        <div class="card-title">
            Nueva compra
        </div>

        <div class="form-group">
            <label for="txtArchivoComprobante">Cargar comprobante (imagen/PDF) para OCR</label>
            <input type="file" id="txtArchivoComprobante" class="input-control" accept=".jpg,.jpeg,.png,.bmp,.tif,.tiff,.pdf" />
            <div class="form-actions">
                <button type="button" class="btn btn-secondary" id="btnExtraerOCR" onclick="extraerOCRCompra()">Extraer datos con OCR</button>
                <span id="ocrEstado" role="status" aria-live="polite" style="margin-left:10px;"></span>
            </div>
        </div>

        <p>Sube una foto o un PDF (máximo 10 MB y 5 páginas). Revisa los productos y guarda la compra para actualizar el inventario.</p>
        <div id="revisionOCR" hidden>
            <p id="advertenciasOCR" role="status"></p>
            <button type="button" class="btn btn-secondary" onclick="prepararAltasCompra()">Revisar proveedor y productos nuevos</button>
            <details><summary>Ver texto leído del comprobante</summary><pre id="textoOCR" style="white-space:pre-wrap;overflow-wrap:anywhere"></pre></details>
        </div>
    <dialog id="modalProveedorCompra" class="compra-dialog" aria-labelledby="tituloProveedorCompra">
      <header><h2 id="tituloProveedorCompra">Modificar proveedor</h2><button type="button" aria-label="Cerrar" onclick="cerrarModalCompra('modalProveedorCompra')">×</button></header>
      <div class="modal-cuerpo">
        <p>Actualiza los datos del proveedor en el catálogo. No cambia el proveedor asociado a compras ya guardadas.</p>
        <label>RUC<input id="rucProveedorModal" class="input-control" maxlength="11" inputmode="numeric" /></label>
        <label>Razón social<input id="razonProveedorModal" class="input-control" maxlength="150" /></label>
        <label>Representante<input id="representanteProveedorModal" class="input-control" maxlength="100" /></label>
        <label>Teléfono<input id="telefonoProveedorModal" class="input-control" maxlength="9" inputmode="numeric" /></label>
        <label>Correo<input id="correoProveedorModal" class="input-control" type="email" maxlength="100" /></label>
        <label>Dirección<input id="direccionProveedorModal" class="input-control" maxlength="150" /></label>
        <p id="errorProveedorModal" class="modal-error" role="alert"></p>
      </div>
      <footer><button id="guardarProveedorModal" type="button" class="btn btn-primary" onclick="guardarProveedorDesdeCompra()">Guardar cambios</button><button type="button" class="btn btn-secondary" onclick="cerrarModalCompra('modalProveedorCompra')">Cancelar</button></footer>
    </dialog>
    <dialog id="modalAltasCompra" class="compra-dialog" aria-labelledby="tituloAltasCompra">
      <header><h2 id="tituloAltasCompra">Preparar nuevos registros</h2><button type="button" aria-label="Cerrar" onclick="cerrarModalCompra('modalAltasCompra')">×</button></header>
      <div class="modal-cuerpo">
        <p>Revisa los datos reconocidos. Se crearán junto con la compra cuando pulses Guardar; todavía no se modifica el inventario.</p>
        <div id="proveedorAltaCampos">
          <label>RUC del proveedor<input id="altaRuc" class="input-control" maxlength="11" inputmode="numeric" /></label>
          <label>Razón social del proveedor<input id="altaRazon" class="input-control" maxlength="150" placeholder="Completa el nombre si no se reconoció" /></label>
        </div>
        <div id="productosAltaCampos"></div>
        <p id="errorAltasCompra" class="modal-error" role="alert"></p>
      </div>
      <footer><button type="button" class="btn btn-primary" onclick="confirmarAltasCompra()">Usar estos datos</button><button type="button" class="btn btn-secondary" onclick="cerrarModalCompra('modalAltasCompra')">Cancelar</button></footer>
    </dialog>
        <input type="hidden" id="txtIdCompra" />
        <input type="hidden" id="txtSubTotal" />
        <input type="hidden" id="txtIgv" />
        <input type="hidden" id="txtTotal" />

        <div class="form-grid">

            <div class="form-group">

                <label for="cboProveedor">Proveedor</label>

                <select id="cboProveedor"
                        class="input-control"></select>
                <button type="button" class="btn btn-secondary" id="btnEditarProveedorCompra" onclick="abrirProveedorCompra()">Modificar proveedor</button>

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
                    <option value="BOLETA">Boleta de venta electrónica</option>
                    <option value="FACTURA">Factura electrónica</option>
                    <option value="PEDIDO">Pedido</option>
                    <option value="TICKET">Ticket</option>
                    <option value="NOTA_VENTA">Nota de venta</option>
                    <option value="OTRO">Otro documento / no identificado</option>
                </select>

            </div>

            <div class="form-group">

                <label for="txtNumeroComprobante">Número de Comprobante</label>

                <input type="text"
                       id="txtNumeroComprobante" maxlength="50" placeholder="Número del comprobante del proveedor"
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

                <select id="cboEstado" disabled
                        class="input-control">
                    <option value="REGISTRADA">REGISTRADA</option>
                    <option value="ANULADA">ANULADA</option>
                </select>

            </div>

        </div>

        <div class="form-grid">
            <div class="form-group"><label for="txtFechaCompra">Fecha del comprobante</label><input type="date" id="txtFechaCompra" class="input-control" /></div>
            <div class="form-group"><label for="preciosIncluyenIgv">Precios de los productos</label><select id="preciosIncluyenIgv" class="input-control" onchange="recalcTotalesCompra()"><option value="no">Sin IGV</option><option value="si">Incluyen IGV</option></select></div>
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

                <input type="number" step="0.01" min="0"
                       id="txtSubtotalCompraGral"
                       class="input-control"
                       onchange="actualizarMontosCompra()" />

            </div>

            <div class="form-group">

                <label for="txtIGVCompra">IGV del comprobante</label>

                <input type="number" step="0.01" min="0"
                       id="txtIGVCompra"
                       class="input-control"
                       onchange="actualizarMontosCompra()" />

            </div>

            <div class="form-group">

                <label for="txtTotalCompra">Total</label>

                <input type="number" step="0.01" min="0"
                       id="txtTotalCompra"
                       class="input-control"
                       onchange="actualizarMontosCompra()" />

            </div>

        </div>

        <div class="form-actions">

            <label id="confirmarOCRContenedor" class="form-check" hidden><input type="checkbox" id="confirmarOCR" /> Revisé el proveedor, los productos y los importes del documento.</label>

            <button type="button"
                    class="btn btn-primary"
                    id="btnGuardarCompra" onclick="guardarCompra()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    id="btnModificarCompra" onclick="modificarCompra()">
                Guardar observación
            </button>

            <button type="button"
                    class="btn btn-secondary"
                    id="btnLimpiarCompra" onclick="limpiarFormulario()">
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
