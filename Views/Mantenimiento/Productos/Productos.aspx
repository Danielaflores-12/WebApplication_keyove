<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Productos.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Productos.Producto1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Productos.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Productos</h1>

        <p>Gestión de productos del inventario</p>

    </div>

    <div class="card">

        <div class="card-title">
            Nuevo producto
        </div>

        <input type="hidden" id="txtIdProducto" />

        <div class="form-grid">

            <div class="form-group">

                <label for="cboCategoria">Categoría</label>

                <select id="cboCategoria"
                        class="input-control">
                </select>

            </div>

            <div class="form-group">

                <label for="txtCodigo">Código</label>

                <input type="text"
                       id="txtCodigo"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtNombre">Nombre</label>

                <input type="text"
                       id="txtNombre"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtDescripcion">Descripción</label>

                <input type="text"
                       id="txtDescripcion"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtMarca">Marca</label>

                <input type="text"
                       id="txtMarca"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtModelo">Modelo</label>

                <input type="text"
                       id="txtModelo"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtPrecioCompra">Precio Compra</label>

                <input type="number"
                       step="0.01"
                       id="txtPrecioCompra"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtPrecioVenta">Precio Venta</label>

                <input type="number"
                       step="0.01"
                       id="txtPrecioVenta"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtStockActual">Stock Actual</label>

                <input type="number"
                       id="txtStockActual"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtStockMinimo">Stock Mínimo</label>

                <input type="number"
                       id="txtStockMinimo"
                       value="5"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="cboUnidadMedida">Unidad de Medida</label>

                <select id="cboUnidadMedida"
                        class="input-control">

                    <option value="UNIDAD">UNIDAD</option>
                    <option value="CAJA">CAJA</option>
                    <option value="PAQUETE">PAQUETE</option>
                    <option value="KILOGRAMO">KILOGRAMO</option>
                    <option value="LITRO">LITRO</option>

                </select>

            </div>

            <div class="form-group form-group-check">

                <label class="form-check" for="chkEstado">

                    <input type="checkbox"
                           id="chkEstado"
                           class="input-control"
                           checked />

                    Activo

                </label>

            </div>

        </div>

        <div class="form-actions">

            <button type="button"
                    class="btn btn-primary"
                    onclick="guardarProducto()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarProducto()">
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
            Productos registrados
        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaProductos">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Código</th>
                        <th>Nombre</th>
                        <th>Categoría</th>
                        <th>Marca</th>
                        <th>Modelo</th>
                        <th>Precio Compra</th>
                        <th>Precio Venta</th>
                        <th>Stock</th>
                        <th>Stock Mín.</th>
                        <th>Unidad</th>
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyProductos">

                </tbody>

            </table>

        </div>

    </div>

</asp:Content>