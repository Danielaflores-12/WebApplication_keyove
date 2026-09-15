<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Productos.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Productos.Producto1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Productos</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Productos.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Productos</h2>

            <input type="hidden" id="txtIdProducto" />

            <div>
                <label>Categoría:</label>
                <select id="cboCategoria"></select>
            </div>

            <br />

            <div>
                <label>Código:</label>
                <input type="text" id="txtCodigo" />
            </div>

            <br />

            <div>
                <label>Nombre:</label>
                <input type="text" id="txtNombre" />
            </div>

            <br />

            <div>
                <label>Descripción:</label>
                <input type="text" id="txtDescripcion" />
            </div>

            <br />

            <div>
                <label>Marca:</label>
                <input type="text" id="txtMarca" />
            </div>

            <br />

            <div>
                <label>Modelo:</label>
                <input type="text" id="txtModelo" />
            </div>

            <br />

            <div>
                <label>Precio Compra:</label>
                <input type="number" step="0.01" id="txtPrecioCompra" />
            </div>

            <br />

            <div>
                <label>Precio Venta:</label>
                <input type="number" step="0.01" id="txtPrecioVenta" />
            </div>

            <br />

            <div>
                <label>Stock Actual:</label>
                <input type="number" id="txtStockActual" />
            </div>

            <br />

            <div>
                <label>Stock Mínimo:</label>
                <input type="number" id="txtStockMinimo" value="5" />
            </div>

            <br />

            <div>
                <label>Unidad de Medida:</label>
                <select id="cboUnidadMedida">

                    <option value="UNIDAD">UNIDAD</option>
                    <option value="CAJA">CAJA</option>
                    <option value="PAQUETE">PAQUETE</option>
                    <option value="KILOGRAMO">KILOGRAMO</option>
                    <option value="LITRO">LITRO</option>

                </select>
            </div>

            <br />

            <div>
                <label>Estado:</label>
                <input type="checkbox" id="chkEstado" checked />
            </div>

            <br />

            <button type="button"
                    onclick="guardarProducto()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarProducto()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Productos Registrados</h3>

            <table border="1"
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

    </form>

</body>
</html>