<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Categorias.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Mantenimiento.Categorias.Categoria1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Categorias</title>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="../../../JScript/Categorias.js"></script>
</head>

<body>

    <form id="form1" runat="server">

        <div>

            <h2>Registro de Categorías</h2>

            <input type="hidden" id="txtIdCategoria" />

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
                    onclick="guardarCategoria()">
                Guardar
            </button>

            <button type="button"
                    onclick="modificarCategoria()">
                Modificar
            </button>

            <button type="button"
                    onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        <hr />

        <div>

            <h3>Categorías Registradas</h3>

            <table border="1"
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

    </form>

</body>
</html>