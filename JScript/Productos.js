$(document).ready(function () {

    listarCategoriasCombo();
    listarProductos();

});


//==================================================
// LISTAR CATEGORÍAS PARA COMBOBOX
//==================================================

function listarCategoriasCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Productos/Productos.aspx/ListarCategoriasCombo",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var opciones = response.d;

            var html = "<option value=''>-- Seleccionar --</option>";

            for (var i = 0; i < opciones.length; i++) {

                html += "<option value='" +
                    opciones[i].v +
                    "'>" +
                    opciones[i].t +
                    "</option>";

            }

            $("#cboCategoria").html(html);

        },

        error: function (error) {

            console.log("Error al listar categorías:");
            console.log(error);

        }
    });

}


//==================================================
// LISTAR PRODUCTOS
//==================================================

function listarProductos() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Productos/Productos.aspx/ListarProductos",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var productos = response.d;

            var filas = "";

            for (var i = 0; i < productos.length; i++) {

                var estado =
                    productos[i].bEstado
                        ? "<span class='badge badge-active'>Activo</span>"
                        : "<span class='badge badge-inactive'>Inactivo</span>";

                filas += "<tr>";

                filas += "<td>" + productos[i].iCodProducto + "</td>";
                filas += "<td>" + productos[i].cCodigo + "</td>";
                filas += "<td>" + productos[i].cNombre + "</td>";
                filas += "<td>" + productos[i].cNombreCategoria + "</td>";
                filas += "<td>" + (productos[i].cMarca || "") + "</td>";
                filas += "<td>" + (productos[i].cModelo || "") + "</td>";
                filas += "<td>" + productos[i].nPrecioCompra + "</td>";
                filas += "<td>" + productos[i].nPrecioVenta + "</td>";
                filas += "<td>" + productos[i].iStockActual + "</td>";
                filas += "<td>" + productos[i].iStockMinimo + "</td>";
                filas += "<td>" + productos[i].cUnidadMedida + "</td>";
                filas += "<td>" + estado + "</td>";

                filas += "<td><div class='table-actions'>";

                filas += "<button type='button' class='btn btn-sm btn-warning' onclick='seleccionarProducto(" +
                    productos[i].iCodProducto +
                    ")'>Editar</button>";

                filas += "<button type='button' class='btn btn-sm btn-danger' onclick='eliminarProducto(" +
                    productos[i].iCodProducto +
                    ")'>Eliminar</button>";

                filas += "</div></td>";

                filas += "</tr>";

            }

            $("#bodyProductos").html(filas);

        },

        error: function (error) {

            console.log("Error al listar productos:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR PRODUCTO
//==================================================

function guardarProducto() {

    var producto = {

        iCodProducto: 0,

        iCodCategoria: parseInt($("#cboCategoria").val() || 0),
        cCodigo: $("#txtCodigo").val(),
        cNombre: $("#txtNombre").val(),
        cDescripcion: $("#txtDescripcion").val(),
        cMarca: $("#txtMarca").val(),
        cModelo: $("#txtModelo").val(),
        nPrecioCompra: parseFloat($("#txtPrecioCompra").val()),
        nPrecioVenta: parseFloat($("#txtPrecioVenta").val()),
        iStockActual: parseInt($("#txtStockActual").val()),
        iStockMinimo: parseInt($("#txtStockMinimo").val()),
        cUnidadMedida: $("#cboUnidadMedida").val(),
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Productos/Productos.aspx/GuardarProducto",
        data: JSON.stringify({
            producto: producto
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Producto registrado correctamente.");

                limpiarFormulario();
                listarProductos();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar el producto.");

        }
    });

}


//==================================================
// SELECCIONAR PRODUCTO PARA EDITAR
//==================================================

function seleccionarProducto(idProducto) {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Productos/Productos.aspx/ObtenerProducto",
        data: JSON.stringify({
            idProducto: idProducto
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var producto = response.d;

            $("#txtIdProducto").val(producto.iCodProducto);
            $("#cboCategoria").val(producto.iCodCategoria);
            $("#txtCodigo").val(producto.cCodigo);
            $("#txtNombre").val(producto.cNombre);
            $("#txtDescripcion").val(producto.cDescripcion);
            $("#txtMarca").val(producto.cMarca);
            $("#txtModelo").val(producto.cModelo);
            $("#txtPrecioCompra").val(producto.nPrecioCompra);
            $("#txtPrecioVenta").val(producto.nPrecioVenta);
            $("#txtStockActual").val(producto.iStockActual);
            $("#txtStockMinimo").val(producto.iStockMinimo);
            $("#cboUnidadMedida").val(producto.cUnidadMedida);
            $("#chkEstado").prop("checked", producto.bEstado === true);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener el producto.");

        }
    });

}


//==================================================
// MODIFICAR PRODUCTO
//==================================================

function modificarProducto() {

    var idProducto = $("#txtIdProducto").val();

    if (idProducto === "") {

        alert("Primero selecciona un producto.");

        return;

    }

    var producto = {

        iCodProducto: parseInt(idProducto),

        iCodCategoria: parseInt($("#cboCategoria").val() || 0),
        cCodigo: $("#txtCodigo").val(),
        cNombre: $("#txtNombre").val(),
        cDescripcion: $("#txtDescripcion").val(),
        cMarca: $("#txtMarca").val(),
        cModelo: $("#txtModelo").val(),
        nPrecioCompra: parseFloat($("#txtPrecioCompra").val()),
        nPrecioVenta: parseFloat($("#txtPrecioVenta").val()),
        iStockActual: parseInt($("#txtStockActual").val()),
        iStockMinimo: parseInt($("#txtStockMinimo").val()),
        cUnidadMedida: $("#cboUnidadMedida").val(),
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Productos/Productos.aspx/ModificarProducto",
        data: JSON.stringify({
            producto: producto
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Producto modificado correctamente.");

                limpiarFormulario();
                listarProductos();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar el producto.");

        }
    });

}


//==================================================
// ELIMINAR PRODUCTO
//==================================================

function eliminarProducto(idProducto) {

    var confirmar = confirm(
        "¿Deseas desactivar este producto?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Productos/Productos.aspx/EliminarProducto",
        data: JSON.stringify({
            idProducto: idProducto
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Producto desactivado correctamente.");

                limpiarFormulario();
                listarProductos();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al eliminar el producto.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdProducto").val("");
    $("#cboCategoria").val("");
    $("#txtCodigo").val("");
    $("#txtNombre").val("");
    $("#txtDescripcion").val("");
    $("#txtMarca").val("");
    $("#txtModelo").val("");
    $("#txtPrecioCompra").val("");
    $("#txtPrecioVenta").val("");
    $("#txtStockActual").val("");
    $("#txtStockMinimo").val(5);
    $("#cboUnidadMedida").val("UNIDAD");
    $("#chkEstado").prop("checked", true);

}