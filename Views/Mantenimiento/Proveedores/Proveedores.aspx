<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Proveedores.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Proveedores.Proveedor1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Proveedores</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Proveedores.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Proveedores</h2>

            <input type="hidden" id="txtIdProveedor" />

            <div>
                <label>RUC:</label>
                <input type="text" id="txtRuc" maxlength="11" />
            </div>

            <br />

            <div>
                <label>Razón Social:</label>
                <input type="text" id="txtRazonSocial" />
            </div>

            <br />

            <div>
                <label>Representante:</label>
                <input type="text" id="txtRepresentante" />
            </div>

            <br />

            <div>
                <label>Teléfono:</label>
                <input type="text" id="txtTelefono" maxlength="9" />
            </div>

            <br />

            <div>
                <label>Correo:</label>
                <input type="text" id="txtCorreo" />
            </div>

            <br />

            <div>
                <label>Dirección:</label>
                <input type="text" id="txtDireccion" />
            </div>

            <br />

            <div>
                <label>Estado:</label>
                <input type="checkbox" id="chkEstado" checked />
            </div>

            <br />

            <button type="button"
                    onclick="guardarProveedor()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarProveedor()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Proveedores Registrados</h3>

            <table border="1"
                   id="tablaProveedores">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>RUC</th>
                        <th>Razón Social</th>
                        <th>Representante</th>
                        <th>Teléfono</th>
                        <th>Correo</th>
                        <th>Dirección</th>
                        <th>Estado</th>
                        <th>Fecha Registro</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyProveedores">

                </tbody>

            </table>

        </div>

    </form>

</body>
</html>