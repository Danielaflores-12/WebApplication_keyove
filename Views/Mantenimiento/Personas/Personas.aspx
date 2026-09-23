<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Personas.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Personas.Persona1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Personas.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Personas</h1>

        <p>Gestión de personas del sistema</p>

    </div>

    <div class="card">

        <div class="card-title">
            Nueva persona
        </div>

        <input type="hidden" id="txtIdPersona" />

        <div class="form-grid">

            <div class="form-group">

                <label for="txtNombres">Nombres</label>

                <input type="text"
                       id="txtNombres"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtApellidos">Apellidos</label>

                <input type="text"
                       id="txtApellidos"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="ddlGenero">Género</label>

                <select id="ddlGenero"
                        class="input-control">

                    <option value="">Seleccione</option>
                    <option value="F">Femenino</option>
                    <option value="M">Masculino</option>

                </select>

            </div>

            <div class="form-group">

                <label for="txtCorreo">Correo</label>

                <input type="email"
                       id="txtCorreo"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtTelefono">Teléfono</label>

                <input type="text"
                       id="txtTelefono"
                       maxlength="9"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtFechaNacimiento">Fecha de nacimiento</label>

                <input type="date"
                       id="txtFechaNacimiento"
                       class="input-control" />

            </div>

        </div>

        <div class="form-actions">

            <button type="button"
                    class="btn btn-primary"
                    onclick="guardarPersona()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarPersona()">
                Modificar
            </button>

            <button type="button"
                    class="btn btn-secondary"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

    </div>

    <div class="card">

        <div class="card-title">
            Personas registradas
        </div>

        <div class="table-container">

            <table class="table"
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

    </div>

</asp:Content>