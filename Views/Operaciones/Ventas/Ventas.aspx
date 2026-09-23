<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Ventas.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Ventas.Venta1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Ventas.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Ventas</h1>

        <p>Registro de ventas</p>

    </div>

    <nav class="ventas-vistas" aria-label="Secciones de ventas">
        <button type="button" class="btn btn-primary" data-vista-ventas="registro" aria-controls="panelRegistroVenta" aria-pressed="true">Registrar venta</button>
        <button type="button" class="btn btn-secondary" data-vista-ventas="registros" aria-controls="panelVentasRegistradas" aria-pressed="false">Ventas registradas</button>
    </nav>
    <section id="panelRegistroVenta" aria-label="Registrar venta">
    <div class="card">

        <div class="card-title">
            Nueva venta
        </div>

        <input type="hidden" id="txtIdVenta" />
        <input type="hidden" id="txtSubTotal" />
        <input type="hidden" id="txtIgv" />
        <input type="hidden" id="txtTotal" />

        <div class="form-grid">

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

                <label for="txtDocumentoCliente">Documento del Cliente</label>

                <input type="text"
                       id="txtDocumentoCliente"
                       maxlength="11"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtNumeroCelular">Número de Celular</label>

                <input type="text"
                       id="txtNumeroCelular"
                       maxlength="9"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="cboMetodoPago">Método de Pago</label>

                <select id="cboMetodoPago"
                        class="input-control">
                    <option value="EFECTIVO">EFECTIVO</option>
                    <option value="TARJETA">TARJETA</option>
                    <option value="TRANSFERENCIA">TRANSFERENCIA</option>
                </select>

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
            Productos de la venta
        </div>

        <div class="form-grid">

            <div class="form-group">

                <label for="cboProducto">Producto</label>

                <select id="cboProducto"
                        class="input-control"></select>

            </div>

            <div class="form-group">

                <label for="txtCantidadVenta">Cantidad</label>

                <input type="number"
                       id="txtCantidadVenta"
                       class="input-control"
                       oninput="actualizarSubtotalLineaVenta()" />

            </div>

            <div class="form-group">

                <label for="txtPrecioVenta">Precio Venta</label>

                <input type="number"
                       step="0.01"
                       id="txtPrecioVenta"
                       class="input-control"
                       oninput="actualizarSubtotalLineaVenta()" />

            </div>

            <div class="form-group">

                <label for="txtSubtotalLineaVenta">Subtotal de línea</label>

                <input type="text"
                       id="txtSubtotalLineaVenta"
                       class="input-control"
                       readonly />

            </div>

        </div>

        <div class="form-actions">

            <button type="button"
                    class="btn btn-primary"
                    onclick="agregarProductoVenta()">
                Agregar producto
            </button>

        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaDetalleVenta">

                <thead>

                    <tr>
                        <th>Producto</th>
                        <th>Cantidad</th>
                        <th>Precio</th>
                        <th>Subtotal</th>
                        <th>Acción</th>
                    </tr>

                </thead>

                <tbody id="bodyDetalleVenta">

                </tbody>

            </table>

        </div>

        <div class="form-grid">

            <div class="form-group">

                <label for="txtSubtotalVentaGral">Subtotal general</label>

                <input type="text"
                       id="txtSubtotalVentaGral"
                       class="input-control"
                       readonly />

            </div>

            <div class="form-group">

                <label for="txtIGVVenta">IGV 18%</label>

                <input type="text"
                       id="txtIGVVenta"
                       class="input-control"
                       readonly />

            </div>

            <div class="form-group">

                <label for="txtTotalVenta">Total</label>

                <input type="text"
                       id="txtTotalVenta"
                       class="input-control"
                       readonly />

            </div>

        </div>

        <div class="form-actions">

            <button type="button"
                    class="btn btn-primary"
                    onclick="guardarVenta()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarVenta()">
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
    <section id="panelVentasRegistradas" aria-label="Ventas registradas" hidden>
    <div class="card">
        <div class="form-group">
            <label for="buscarVentas">Buscar ventas</label>
            <input type="search" id="buscarVentas" class="input-control" placeholder="Comprobante, documento, celular, usuario, fecha o estado" />
        </div>
        <div class="form-actions">
            <button type="button" class="btn btn-secondary" onclick="listarVentas()">Actualizar lista</button>
        </div>
        <p id="estadoBusquedaVentas" role="status" aria-live="polite"></p>
    </div>
    <div class="card">

        <div class="card-title">
            Ventas registradas
        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaVentas">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Usuario</th>
                        <th>Comprobante</th>
                        <th>N° Comprobante</th>
                        <th>Documento</th>
                        <th>Celular</th>
                        <th>Fecha</th>
                        <th>Sub Total</th>
                        <th>IGV</th>
                        <th>Total</th>
                        <th>Pago</th>
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyVentas">

                </tbody>

            </table>

        </div>

    </div>


    </section>
</asp:Content>
