<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Personas.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Personas.Persona1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Personas</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Personas.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Personas</h2>

            <input type="hidden" id="txtIdPersona" />

            <div>
                <label>Nombres:</label>
                <input type="text" id="txtNombres" />
            </div>

            <br />

            <div>
                <label>Apellidos:</label>
                <input type="text" id="txtApellidos" />
            </div>

            <br />

            <div>
                <label>Género:</label>

                <select id="ddlGenero">
                    <option value="">Seleccione</option>
                    <option value="F">Femenino</option>
                    <option value="M">Masculino</option>
                </select>
            </div>

            <br />

            <div>
                <label>Correo:</label>
                <input type="email" id="txtCorreo" />
            </div>

            <br />

            <div>
                <label>Teléfono:</label>
                <input type="text"
                       id="txtTelefono"
                       maxlength="9" />
            </div>

            <br />

            <div>
                <label>Fecha de nacimiento:</label>
                <input type="date"
                       id="txtFechaNacimiento" />
            </div>

            <br />

            <button type="button"
                    onclick="guardarPersona()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarPersona()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Personas Registradas</h3>

            <table border="1"
                   id="tablaPersonas">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Nombres</th>
                        <th>Apellidos</th>
                        <th>Género</th>
                        <th>Correo</th>
                        <th>Teléfono</th>
                        <th>Fecha Nacimiento</th>
                        <th>Fecha Registro</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyPersonas">

                </tbody>

            </table>

        </div>

    </form>

</body>
</html>