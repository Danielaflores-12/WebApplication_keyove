<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Compras.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Compras.Compra1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Compras</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Compras.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Compras</h2>

            <input type="hidden" id="txtIdCompra" />

            <div>
                <label>Proveedor:</label>
                <select id="cboProveedor"></select>
            </div>

            <br />

            <div>
                <label>Usuario:</label>
                <select id="cboUsuario"></select>
            </div>

            <br />

            <div>
                <label>Tipo de Comprobante:</label>
                <select id="cboTipoComprobante">
                    <option value="BOLETA">BOLETA</option>
                    <option value="FACTURA">FACTURA</option>
                </select>
            </div>

            <br />

            <div>
                <label>Número de Comprobante:</label>
                <input type="text" id="txtNumeroComprobante" />
            </div>

            <br />

            <div>
                <label>Sub Total:</label>
                <input type="number" step="0.01" id="txtSubTotal" />
            </div>

            <br />

            <div>
                <label>IGV:</label>
                <input type="number" step="0.01" id="txtIgv" />
            </div>

            <br />

            <div>
                <label>Total:</label>
                <input type="number" step="0.01" id="txtTotal" />
            </div>

            <br />

            <div>
                <label>Observación:</label>
                <input type="text" id="txtObservacion" />
            </div>

            <br />

            <div>
                <label>Estado:</label>
                <select id="cboEstado">
                    <option value="REGISTRADA">REGISTRADA</option>
                    <option value="ANULADA">ANULADA</option>
                </select>
            </div>

            <br />

            <button type="button"
                    onclick="guardarCompra()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarCompra()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Compras Registradas</h3>

            <table border="1"
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

    </form>

</body>
</html>