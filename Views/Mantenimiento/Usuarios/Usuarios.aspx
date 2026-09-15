<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Usuarios.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Usuarios.Usuario1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Usuarios</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Usuarios.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Usuarios</h2>

            <input type="hidden" id="txtIdUsuario" />

            <div>
                <label>Persona:</label>
                <select id="cboPersona"></select>
            </div>

            <br />

            <div>
                <label>Rol:</label>
                <select id="cboRol"></select>
            </div>

            <br />

            <div>
                <label>Nombre de Usuario:</label>
                <input type="text" id="txtNombreUsuario" />
            </div>

            <br />

            <div>
                <label>Contraseña:</label>
                <input type="password" id="txtContrasena" />
            </div>

            <br />

            <div>
                <label>Estado:</label>
                <input type="checkbox" id="chkEstado" checked />
            </div>

            <br />

            <button type="button"
                    onclick="guardarUsuario()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarUsuario()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Usuarios Registrados</h3>

            <table border="1"
                   id="tablaUsuarios">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Persona</th>
                        <th>Rol</th>
                        <th>Nombre de Usuario</th>
                        <th>Estado</th>
                        <th>Fecha Registro</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyUsuarios">

                </tbody>

            </table>

        </div>

    </form>

</body>
</html>