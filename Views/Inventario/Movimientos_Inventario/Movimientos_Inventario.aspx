<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Movimientos_Inventario.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Inventario.Movimientos_Inventario.Movimiento1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Movimientos_Inventario.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Movimientos de Inventario</h1>

        <p>Registro de movimientos de inventario</p>

    </div>

    <div class="card">

        <div class="card-title">
            Nuevo movimiento
        </div>

        <input type="hidden" id="txtIdMovimiento" />

        <div class="form-grid">

            <div class="form-group">

                <label for="cboProducto">Producto</label>

                <select id="cboProducto"
                        class="input-control">
                </select>

            </div>

            <div class="form-group">

                <label for="cboUsuario">Usuario</label>

                <select id="cboUsuario"
                        class="input-control">
                </select>

            </div>

            <div class="form-group">

                <label for="cboTipoMovimiento">Tipo de Movimiento</label>

                <select id="cboTipoMovimiento"
                        class="input-control">

                    <option value="ENTRADA">ENTRADA</option>
                    <option value="SALIDA">SALIDA</option>

                </select>

            </div>

            <div class="form-group">

                <label for="txtCantidad">Cantidad</label>

                <input type="number"
                       id="txtCantidad"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtStockAnterior">Stock Anterior</label>

                <input type="number"
                       id="txtStockAnterior"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtStockNuevo">Stock Nuevo</label>

                <input type="number"
                       id="txtStockNuevo"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtMotivo">Motivo</label>

                <input type="text"
                       id="txtMotivo"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtIdCompra">ID Compra (opcional)</label>

                <input type="number"
                       id="txtIdCompra"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtIdVenta">ID Venta (opcional)</label>

                <input type="number"
                       id="txtIdVenta"
                       class="input-control" />

            </div>

        </div>

        <div class="form-actions">

            <button type="button"
                    class="btn btn-primary"
                    onclick="guardarMovimiento()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarMovimiento()">
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
            Movimientos registrados
        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaMovimientos">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Producto</th>
                        <th>Usuario</th>
                        <th>Compra</th>
                        <th>Venta</th>
                        <th>Tipo</th>
                        <th>Cantidad</th>
                        <th>Stock Ant.</th>
                        <th>Stock Nuevo</th>
                        <th>Motivo</th>
                        <th>Fecha</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyMovimientos">

                </tbody>

            </table>

        </div>

    </div>

</asp:Content>