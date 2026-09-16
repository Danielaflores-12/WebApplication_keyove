<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Detalle_Venta.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Detalle_Venta.DetalleVenta1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Detalle de Ventas</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Detalle_Venta.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Detalle de Ventas</h2>

            <input type="hidden" id="txtIdDetalleVenta" />

            <div>
                <label>Venta:</label>
                <select id="cboVenta"></select>
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
                <label>Precio de Venta:</label>
                <input type="number" step="0.01" id="txtPrecioVenta" />
            </div>

            <br />

            <div>
                <label>Descuento:</label>
                <input type="number" step="0.01" id="txtDescuento" value="0" />
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
                    onclick="guardarDetalleVenta()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarDetalleVenta()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Detalles de Venta Registrados</h3>

            <table border="1"
                   id="tablaDetalleVenta">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Venta</th>
                        <th>Producto</th>
                        <th>Cantidad</th>
                        <th>Precio</th>
                        <th>Descuento</th>
                        <th>Sub Total</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyDetalleVenta">

                </tbody>

            </table>

        </div>

    </form>

</body>
</html>