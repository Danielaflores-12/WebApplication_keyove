<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Usuarios.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Usuarios.Usuario1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Usuarios.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Usuarios</h1>

        <p>Gestión de usuarios del sistema</p>

    </div>

    <div class="card">

        <div class="card-title">
            Nuevo usuario
        </div>

        <input type="hidden" id="txtIdUsuario" />

        <div class="form-grid">

            <div class="form-group">

                <label for="cboPersona">Persona</label>

                <select id="cboPersona"
                        class="input-control">
                </select>

            </div>

            <div class="form-group">

                <label for="cboRol">Rol</label>

                <select id="cboRol"
                        class="input-control">
                </select>

            </div>

            <div class="form-group">

                <label for="txtNombreUsuario">Nombre de Usuario</label>

                <input type="text"
                       id="txtNombreUsuario"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtContrasena">Contraseña</label>

                <input type="password"
                       id="txtContrasena"
                       class="input-control" />

            </div>

            <div class="form-group form-group-check">

                <label class="form-check" for="chkEstado">

                    <input type="checkbox"
                           id="chkEstado"
                           class="input-control"
                           checked />

                    Activo

                </label>

            </div>

        </div>

        <div class="form-actions">

            <button type="button"
                    class="btn btn-primary"
                    onclick="guardarUsuario()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarUsuario()">
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
            Usuarios registrados
        </div>

        <div class="table-container">

            <table class="table"
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

    </div>

</asp:Content>