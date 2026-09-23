<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Proveedores.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Proveedores.Proveedor1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Proveedores.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Proveedores</h1>

        <p>Gestión de proveedores del inventario</p>

    </div>

    <div class="card">

        <div class="card-title">
            Nuevo proveedor
        </div>

        <input type="hidden" id="txtIdProveedor" />

        <div class="form-grid">

            <div class="form-group">

                <label for="txtRuc">RUC</label>

                <input type="text"
                       id="txtRuc"
                       maxlength="11"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtRazonSocial">Razón Social</label>

                <input type="text"
                       id="txtRazonSocial"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtRepresentante">Representante</label>

                <input type="text"
                       id="txtRepresentante"
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

                <label for="txtCorreo">Correo</label>

                <input type="text"
                       id="txtCorreo"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtDireccion">Dirección</label>

                <input type="text"
                       id="txtDireccion"
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
                    onclick="guardarProveedor()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarProveedor()">
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
            Proveedores registrados
        </div>

        <div class="table-container">

            <table class="table"
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

    </div>

</asp:Content>