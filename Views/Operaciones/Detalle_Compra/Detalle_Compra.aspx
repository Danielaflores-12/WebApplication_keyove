<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Detalle_Compra.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Detalle_Compra.DetalleCompra1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Detalle de Compras</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Detalle_Compra.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Detalle de Compras</h2>

            <input type="hidden" id="txtIdDetalleCompra" />

            <div>
                <label>Compra:</label>
                <select id="cboCompra"></select>
            </div>

            <br />

            <div>
                <label>Producto:</label>
                <select id="cboProducto"></select>
            </div>

            <br />

            <div>
                <label>Cantidad:</label>
                <input type="number" id="txtCantidad" />
            </div>

            <br />

            <div>
                <label>Precio de Compra:</label>
                <input type="number" step="0.01" id="txtPrecioCompra" />
            </div>

            <br />

            <div>
                <label>Sub Total:</label>
                <input type="number"
                       step="0.01"
                       id="txtSubTotal"
                       readonly />
            </div>

            <br />

            <button type="button"
                    onclick="guardarDetalleCompra()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarDetalleCompra()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Detalles de Compra Registrados</h3>

            <table border="1"
                   id="tablaDetalleCompra">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Compra</th>
                        <th>Producto</th>
                        <th>Cantidad</th>
                        <th>Precio</th>
                        <th>Sub Total</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyDetalleCompra">

                </tbody>

            </table>

        </div>

    </form>

</body>
</html>