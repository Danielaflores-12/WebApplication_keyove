<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Roles.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Roles.Rol1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Roles</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Roles.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Roles</h2>

            <input type="hidden" id="txtIdRol" />

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
                <label>Estado:</label>
                <input type="checkbox" id="chkEstado" checked />
            </div>

            <br />

            <button type="button"
                    onclick="guardarRol()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarRol()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Roles Registrados</h3>

            <table border="1"
                   id="tablaRoles">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Nombre</th>
                        <th>Descripción</th>
                        <th>Estado</th>
                        <th>Fecha Registro</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyRoles">

                </tbody>

            </table>

        </div>

    </form>

</body>
</html>