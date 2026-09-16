<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Ventas.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Ventas.Venta1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Ventas</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Ventas.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Ventas</h2>

            <input type="hidden" id="txtIdVenta" />

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
                <label>Documento del Cliente:</label>
                <input type="text" id="txtDocumentoCliente" maxlength="11" />
            </div>

            <br />

            <div>
                <label>Número de Celular:</label>
                <input type="text" id="txtNumeroCelular" maxlength="9" />
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
                <label>Método de Pago:</label>
                <select id="cboMetodoPago">
                    <option value="EFECTIVO">EFECTIVO</option>
                    <option value="TARJETA">TARJETA</option>
                    <option value="TRANSFERENCIA">TRANSFERENCIA</option>
                </select>
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
                    onclick="guardarVenta()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarVenta()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Ventas Registradas</h3>

            <table border="1"
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

    </form>

</body>
</html>