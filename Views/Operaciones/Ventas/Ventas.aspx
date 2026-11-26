
<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Ventas.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Ventas.Venta1" %>

<asp:Content ID="ContentVentas"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <link href="<%= ResolveUrl("~/Assets/css/site.css") %>"
          rel="stylesheet" />

    <style>
        /* ==========================================
           PÁGINA DE VENTAS
        ========================================== */

        .ventas-page {
            padding: 24px;
        }

        .ventas-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 16px;
            flex-wrap: wrap;
            margin-bottom: 22px;
        }

        .ventas-header h2 {
            margin: 0;
            font-weight: 700;
        }

        .ventas-subtitulo {
            color: #6b7280;
            margin-top: 6px;
        }

        .ventas-acciones {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }

        /* ==========================================
           FILTROS
        ========================================== */

        .ventas-filtros {
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
            margin-bottom: 18px;
        }

        .ventas-filtros input,
        .ventas-filtros select {
            min-width: 220px;
            flex: 1;
        }

        #contadorVentas {
            color: #64748b;
            font-size: 13px;
            margin-bottom: 12px;
        }

        /* ==========================================
           TABLA PRINCIPAL
        ========================================== */

        .ventas-tabla-contenedor {
            overflow-x: auto;
            background: #fff;
            border-radius: 12px;
            border: 1px solid #e5e7eb;
        }

        #tablaVentas {
            width: 100%;
            border-collapse: collapse;
            min-width: 850px;
        }

        #tablaVentas th,
        #tablaVentas td {
            padding: 12px;
            border-bottom: 1px solid #e5e7eb;
            text-align: left;
            vertical-align: middle;
        }

        #tablaVentas th {
            background: #f8fafc;
            font-weight: 700;
            white-space: nowrap;
        }

        #tablaVentas tbody tr {
            cursor: pointer;
            transition: background .15s ease;
        }

        #tablaVentas tbody tr:hover {
            background: #f1f5f9;
        }

        #tablaVentas tbody tr.venta-seleccionada {
            background: #dbeafe;
        }

        /* ==========================================
           ESTADOS
        ========================================== */

        .venta-estado {
            display: inline-block;
            padding: 4px 9px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 700;
        }

        .venta-estado.registrada {
            background: #dcfce7;
            color: #166534;
        }

        .venta-estado.anulada {
            background: #fee2e2;
            color: #991b1b;
        }

        .venta-acciones-fila {
            display: flex;
            gap: 6px;
            flex-wrap: wrap;
        }

        .venta-acciones-fila button,
        .venta-acciones-fila a {
            font-size: 12px;
            white-space: nowrap;
        }

        /* ==========================================
           MODAL DE VENTAS
        ========================================== */

        .modal-venta[hidden] {
            display: none !important;
        }

        .modal-venta {
            position: fixed;
            inset: 0;
            z-index: 9999;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
            background: rgba(15, 23, 42, .65);
            overflow-y: auto;
        }

        .modal-venta-contenido {
            background: #fff;
            width: 100%;
            max-width: 1100px;
            max-height: 92vh;
            overflow-y: auto;
            border-radius: 14px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, .25);
        }

        .modal-venta-cabecera {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 12px;
            padding: 18px 22px;
            border-bottom: 1px solid #e5e7eb;
            position: sticky;
            top: 0;
            background: #fff;
            z-index: 2;
        }

        .modal-venta-cabecera h3 {
            margin: 0;
            font-weight: 700;
        }

        .modal-venta-cuerpo {
            padding: 22px;
        }

        /* ==========================================
           SECCIONES Y FORMULARIOS
        ========================================== */

        .venta-seccion {
            margin-bottom: 24px;
        }

        .venta-seccion h4 {
            margin: 0 0 14px;
            font-size: 16px;
            font-weight: 700;
        }

        .venta-form-grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 15px;
        }

        .venta-campo {
            display: flex;
            flex-direction: column;
            gap: 6px;
            min-width: 0;
        }

        .venta-campo label {
            font-weight: 600;
            font-size: 13px;
        }

        .venta-campo input,
        .venta-campo select,
        .venta-campo textarea {
            width: 100%;
            min-width: 0;
            padding: 9px 11px;
            border: 1px solid #d1d5db;
            border-radius: 7px;
            box-sizing: border-box;
        }

        .venta-campo textarea {
            resize: vertical;
        }

        .venta-campo-completo {
            grid-column: 1 / -1;
        }

        /* ==========================================
           PRODUCTOS
        ========================================== */

        .venta-productos-linea {
            display: grid;
            grid-template-columns:
                minmax(180px, 2fr)
                minmax(90px, .7fr)
                minmax(120px, 1fr)
                minmax(120px, 1fr)
                auto;
            align-items: end;
            gap: 10px;
        }

        .venta-detalle-contenedor {
            overflow-x: auto;
        }

        .venta-tabla-detalle {
            width: 100%;
            border-collapse: collapse;
            margin-top: 18px;
            min-width: 600px;
        }

        .venta-tabla-detalle th,
        .venta-tabla-detalle td {
            padding: 10px;
            border-bottom: 1px solid #e5e7eb;
            text-align: left;
        }

        .venta-tabla-detalle th {
            background: #f8fafc;
        }

        /* ==========================================
           TOTALES
        ========================================== */

        .venta-totales {
            display: flex;
            justify-content: flex-end;
            margin-top: 18px;
        }

        .venta-totales-contenido {
            width: 100%;
            max-width: 330px;
        }

        .venta-total-linea {
            display: flex;
            justify-content: space-between;
            gap: 16px;
            padding: 7px 0;
        }

        .venta-total-final {
            border-top: 2px solid #cbd5e1;
            margin-top: 6px;
            padding-top: 12px;
            font-size: 19px;
            font-weight: 800;
        }

        /* ==========================================
           BOTONES DEL MODAL
        ========================================== */

        .modal-venta-pie {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            flex-wrap: wrap;
            padding: 16px 22px;
            border-top: 1px solid #e5e7eb;
            position: sticky;
            bottom: 0;
            background: #fff;
        }

        .ventas-vacio {
            text-align: center !important;
            padding: 28px !important;
            color: #64748b;
        }

        /* ==========================================
           DISEÑO RESPONSIVO
        ========================================== */

        @media (max-width: 800px) {
            .ventas-page {
                padding: 14px;
            }

            .venta-form-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .venta-productos-linea {
                grid-template-columns: 1fr 1fr;
            }

            .venta-productos-linea .venta-campo:first-child {
                grid-column: 1 / -1;
            }
        }

        @media (max-width: 520px) {
            .venta-form-grid {
                grid-template-columns: 1fr;
            }

            .venta-campo-completo {
                grid-column: auto;
            }

            .modal-venta {
                padding: 8px;
            }

            .modal-venta-cuerpo {
                padding: 14px;
            }

            .modal-venta-cabecera,
            .modal-venta-pie {
                padding: 14px;
            }
        }
    </style>

    <!-- ==========================================
         CONTENIDO PRINCIPAL
    ========================================== -->

    <div class="ventas-page">

        <div class="ventas-header">
            <div>
                <h2>Gestión de ventas</h2>
                <div class="ventas-subtitulo">
                    Registra, consulta y administra las ventas
                    del sistema KEYOVE.
                </div>
            </div>

            <div class="ventas-acciones">

                <button type="button"
                        class="btn btn-primary"
                        onclick="abrirModalVenta('nuevo')">
                    + Agregar
                </button>

                <button type="button"
                        class="btn btn-warning"
                        onclick="abrirModalVenta('editar')">
                    Editar
                </button>

                <button type="button"
                        class="btn btn-danger"
                        onclick="anularVentaSeleccionada()">
                    Anular
                </button>

                <button type="button"
                        class="btn btn-secondary"
                        onclick="listarVentas()">
                    Actualizar
                </button>

            </div>
        </div>

        <!-- FILTROS -->

        <div class="ventas-filtros">

            <input type="search"
                   id="buscarVentas"
                   class="form-control"
                   placeholder="Buscar por número, usuario o estado..." />

            <select id="estadoBusquedaVentas"
                    class="form-control">
                <option value="">Todos los estados</option>
                <option value="REGISTRADA">Registrada</option>
                <option value="ANULADA">Anulada</option>
            </select>

        </div>

        <div id="contadorVentas"
             role="status"
             aria-live="polite">
            0 ventas encontradas
        </div>

        <!-- TABLA DE VENTAS -->

        <div class="ventas-tabla-contenedor">

            <table id="tablaVentas">
                <thead>
                    <tr>
                        <th>Código</th>
                        <th>Fecha</th>
                        <th>Comprobante</th>
                        <th>Número</th>
                        <th>Usuario</th>
                        <th>Total</th>
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>
                </thead>

                <tbody id="bodyVentas">
                    <tr>
                        <td colspan="8"
                            class="ventas-vacio">
                            Cargando ventas...
                        </td>
                    </tr>
                </tbody>
            </table>

        </div>
    </div>

    <!-- ==========================================
         MODAL DE REGISTRO Y EDICIÓN
    ========================================== -->

    <div id="modalVenta"
         class="modal-venta"
         role="dialog"
         aria-modal="true"
         aria-labelledby="tituloModalVenta"
         hidden="hidden">

        <div class="modal-venta-contenido">

            <!-- CABECERA -->

            <div class="modal-venta-cabecera">

                <h3 id="tituloModalVenta">
                    Registrar venta
                </h3>

                <button type="button"
                        class="btn btn-secondary"
                        aria-label="Cerrar"
                        onclick="cerrarModalVenta()">
                    X
                </button>

            </div>

            <!-- CUERPO -->

            <div class="modal-venta-cuerpo">

                <input type="hidden" id="txtIdVenta" />

                <!-- COMPROBANTE -->

                <section class="venta-seccion">
                    <h4>Comprobante electrónico</h4>

                    <div class="venta-form-grid">

                        <div class="venta-campo">
                            <label for="cboTipoComprobante">
                                Tipo de comprobante
                            </label>

                            <select id="cboTipoComprobante"
                                    class="form-control">
                                <option value="BOLETA">Boleta</option>
                                <option value="FACTURA">Factura</option>
                            </select>
                        </div>

                        <div class="venta-campo">
                            <label for="txtNumeroComprobante">
                                Número de comprobante
                            </label>

                            <input type="text"
                                   id="txtNumeroComprobante"
                                   class="form-control"
                                   placeholder="Se genera al guardar"
                                   readonly="readonly" />
                        </div>

                        <div class="venta-campo">
                            <label for="cboEstado">
                                Estado
                            </label>

                            <select id="cboEstado"
                                    class="form-control"
                                    disabled="disabled">
                                <option value="REGISTRADA">
                                    Registrada
                                </option>
                            </select>
                        </div>

                    </div>
                </section>

                <!-- DATOS DE VENTA -->

                <section class="venta-seccion">
                    <h4>Datos de la venta</h4>

                    <div class="venta-form-grid">

                        <div class="venta-campo">
                            <label for="cboUsuario">
                                Usuario responsable
                            </label>

                            <select id="cboUsuario"
                                    class="form-control">
                                <option value="">
                                    Seleccione un usuario
                                </option>
                            </select>
                        </div>

                        <div class="venta-campo">
                            <label for="txtDocumentoCliente">
                                Documento del cliente
                            </label>

                            <input type="text"
                                   id="txtDocumentoCliente"
                                   class="form-control"
                                   maxlength="11"
                                   inputmode="numeric"
                                   placeholder="DNI o RUC" />
                        </div>

                        <div class="venta-campo">
                            <label for="txtNumeroCelular">
                                Número de celular
                            </label>

                            <input type="tel"
                                   id="txtNumeroCelular"
                                   class="form-control"
                                   maxlength="9"
                                   inputmode="numeric"
                                   placeholder="Celular del cliente" />
                        </div>

                        <div class="venta-campo">
                            <label for="cboMetodoPago">
                                Método de pago
                            </label>

                            <select id="cboMetodoPago"
                                    class="form-control">
                                <option value="EFECTIVO">Efectivo</option>
                                <option value="TARJETA">Tarjeta</option>
                                <option value="TRANSFERENCIA">
                                    Transferencia
                                </option>
                            </select>
                        </div>

                        <div class="venta-campo venta-campo-completo">
                            <label for="txtObservacion">
                                Observaciones
                            </label>

                            <textarea id="txtObservacion"
                                      class="form-control"
                                      rows="2"
                                      maxlength="300"
                                      placeholder="Observaciones de la venta (opcional)"></textarea>
                        </div>

                    </div>
                </section>

                <!-- PRODUCTOS -->

                <section class="venta-seccion">
                    <h4>Productos de la venta</h4>

                    <div class="venta-productos-linea">

                        <div class="venta-campo">
                            <label for="cboProducto">
                                Producto
                            </label>

                            <select id="cboProducto"
                                    class="form-control">
                                <option value="">
                                    Seleccione un producto
                                </option>
                            </select>
                        </div>

                        <div class="venta-campo">
                            <label for="txtCantidadVenta">
                                Cantidad
                            </label>

                            <input type="number"
                                   id="txtCantidadVenta"
                                   class="form-control"
                                   min="1"
                                   step="1"
                                   value="1" />
                        </div>

                        <div class="venta-campo">
                            <label for="txtPrecioVenta">
                                Precio unitario
                            </label>

                            <input type="number"
                                   id="txtPrecioVenta"
                                   class="form-control"
                                   min="0.01"
                                   step="0.01"
                                   value="0.00" />
                        </div>

                        <div class="venta-campo">
                            <label for="txtSubtotalLineaVenta">
                                Subtotal
                            </label>

                            <input type="text"
                                   id="txtSubtotalLineaVenta"
                                   class="form-control"
                                   value="S/ 0.00"
                                   readonly="readonly" />
                        </div>

                        <div class="venta-campo">
                            <button type="button"
                                    id="btnAgregarProductoVenta"
                                    class="btn btn-success"
                                    onclick="agregarProductoVenta()">
                                Agregar
                            </button>
                        </div>

                    </div>

                    <!-- TABLA DE DETALLE -->

                    <div class="venta-detalle-contenedor">

                        <table id="tablaDetalleVenta"
                               class="venta-tabla-detalle">
                            <thead>
                                <tr>
                                    <th>Producto</th>
                                    <th>Cantidad</th>
                                    <th>Precio unitario</th>
                                    <th>Descuento</th>
                                    <th>Subtotal</th>
                                    <th>Acción</th>
                                </tr>
                            </thead>

                            <tbody id="bodyDetalleVenta">
                                <tr>
                                    <td colspan="6"
                                        class="ventas-vacio">
                                        Agrega productos para registrar la venta.
                                    </td>
                                </tr>
                            </tbody>
                        </table>

                    </div>

                    <!-- TOTALES -->

                    <div class="venta-totales">
                        <div class="venta-totales-contenido">

                            <div class="venta-total-linea">
                                <span>Subtotal:</span>
                                <strong id="txtSubtotalVentaGral">
                                    S/ 0.00
                                </strong>
                            </div>

                            <div class="venta-total-linea">
                                <span>IGV incluido (18 %):</span>
                                <strong id="txtIGVVenta">
                                    S/ 0.00
                                </strong>
                            </div>

                            <div class="venta-total-linea venta-total-final">
                                <span>Total:</span>
                                <strong id="txtTotalVenta">
                                    S/ 0.00
                                </strong>
                            </div>

                        </div>
                    </div>

                    <input type="hidden"
                           id="txtSubTotal"
                           value="0" />

                    <input type="hidden"
                           id="txtIgv"
                           value="0" />

                    <input type="hidden"
                           id="txtTotal"
                           value="0" />

                </section>

            </div>

            <!-- BOTONES DEL MODAL -->

            <div class="modal-venta-pie">

                <button type="button"
                        class="btn btn-light"
                        onclick="limpiarFormulario()">
                    Limpiar
                </button>

                <button type="button"
                        id="btnGuardarVentaModal"
                        class="btn btn-primary"
                        onclick="guardarVenta()">
                    Guardar venta
                </button>

                <button type="button"
                        id="btnModificarVentaModal"
                        class="btn btn-warning"
                        onclick="modificarVenta()"
                        hidden="hidden">
                    Guardar cambios
                </button>

                <button type="button"
                        class="btn btn-secondary"
                        onclick="cerrarModalVenta()">
                    Cerrar
                </button>

            </div>

        </div>
    </div>

    <!-- JAVASCRIPT -->

    <script src="<%= ResolveUrl("~/JScript/Ventas.js") %>?v=3"></script>

</asp:Content>
