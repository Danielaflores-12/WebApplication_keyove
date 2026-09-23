<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Categorias.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Categorias.Categoria1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Categorias.js"></script>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="page-title">

        <h1>Categorías</h1>

        <p>Gestión de categorías del inventario</p>

    </div>

    <div class="card">

        <div class="card-title">
            Nueva categoría
        </div>

        <input type="hidden" id="txtIdCategoria" />

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
                    onclick="guardarCategoria()">
                Guardar
            </button>

            <button type="button"
                    class="btn btn-warning"
                    onclick="modificarCategoria()">
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
            Categorías registradas
        </div>

        <div class="table-container">

            <table class="table"
                   id="tablaCategorias">

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

                <tbody id="bodyCategorias">

                </tbody>

            </table>

        </div>

    </div>

</asp:Content>