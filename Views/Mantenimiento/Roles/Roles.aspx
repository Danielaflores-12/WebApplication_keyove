<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Roles.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Roles.Rol1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Roles.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Roles</h1>

        <p>Gestión de roles del sistema</p>

    </div>

    <div class="card">

        <div class="card-title">
            Nuevo rol
        </div>

        <input type="hidden" id="txtIdRol" />

        <div class="form-grid">

            <div class="form-group">

                <label for="txtNombre">Nombre</label>

                <input type="text"
                       id="txtNombre"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtDescripcion">Descripción</label>

                <input type="text"
                       id="txtDescripcion"
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
                    onclick="guardarRol()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarRol()">
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
            Roles registrados
        </div>

        <div class="table-container">

            <table class="table"
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

    </div>

</asp:Content>