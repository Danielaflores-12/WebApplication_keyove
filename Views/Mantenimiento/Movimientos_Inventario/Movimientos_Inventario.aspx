<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Movimientos_Inventario.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Movimientos_Inventario.Movimiento1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Movimientos de Inventario</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Movimientos_Inventario.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Movimientos de Inventario</h2>

            <input type="hidden" id="txtIdMovimiento" />

            <div>
                <label>Producto:</label>
                <select id="cboProducto"></select>
            </div>

            <br />

            <div>
                <label>Usuario:</label>
                <select id="cboUsuario"></select>
            </div>

            <br />

            <div>
                <label>Tipo de Movimiento:</label>
                <select id="cboTipoMovimiento">
                    <option value="ENTRADA">ENTRADA</option>
                    <option value="SALIDA">SALIDA</option>
                </select>
            </div>

            <br />

            <div>
                <label>Cantidad:</label>
                <input type="number" id="txtCantidad" />
            </div>

            <br />

            <div>
                <label>Stock Anterior:</label>
                <input type="number" id="txtStockAnterior" />
            </div>

            <br />

            <div>
                <label>Stock Nuevo:</label>
                <input type="number" id="txtStockNuevo" />
            </div>

            <br />

            <div>
                <label>Motivo:</label>
                <input type="text" id="txtMotivo" />
            </div>

            <br />

            <div>
                <label>ID Compra (opcional):</label>
                <input type="number" id="txtIdCompra" />
            </div>

            <br />

            <div>
                <label>ID Venta (opcional):</label>
                <input type="number" id="txtIdVenta" />
            </div>

            <br />

            <button type="button"
                    onclick="guardarMovimiento()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarMovimiento()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Movimientos Registrados</h3>

            <table border="1"
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

    </form>

</body>
</html>