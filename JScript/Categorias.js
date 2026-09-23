$(document).ready(function () {

    listarCategorias();

});


//==================================================
// FORMATEAR FECHA /Date(ms)/ A TEXTO LEGIBLE
//==================================================

function formatearFecha(valor) {

    if (!valor) {

        return "";

    }

    var coincidencia = /\/Date\((\d+)\)\//.exec(valor);

    if (coincidencia) {

        var fecha = new Date(parseInt(coincidencia[1]));

        return fecha.toLocaleDateString() +
            " " +
            fecha.toLocaleTimeString();

    }

    return valor;

}


//==================================================
// LISTAR CATEGORÍAS
//==================================================

function listarCategorias() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Categorias/Categorias.aspx/ListarCategorias",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var categorias = response.d;

var filas = "";

                for (var i = 0; i < categorias.length; i++) {

                    var estado =
                        categorias[i].bEstado
                            ? "<span class='badge badge-active'>Activo</span>"
                            : "<span class='badge badge-inactive'>Inactivo</span>";

                    filas += "<tr>";

                    filas += "<td>" + categorias[i].iCodCategoria + "</td>";
                    filas += "<td>" + categorias[i].cNombre + "</td>";
                    filas += "<td>" + (categorias[i].cDescripcion || "") + "</td>";
                    filas += "<td>" + estado + "</td>";
                    filas += "<td>" + formatearFecha(categorias[i].dFechaRegistro) + "</td>";

                    filas += "<td><div class='table-actions'>";

                    filas += "<button type='button' class='btn btn-sm btn-warning' onclick='seleccionarCategoria(" +
                        categorias[i].iCodCategoria +
                        ")'>Editar</button>";

                    filas += "<button type='button' class='btn btn-sm btn-danger' onclick='eliminarCategoria(" +
                        categorias[i].iCodCategoria +
                        ")'>Eliminar</button>";

                    filas += "</div></td>";

                    filas += "</tr>";

                }

            $("#bodyCategorias").html(filas);

        },

        error: function (error) {

            console.log("Error al listar categorías:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR CATEGORÍA
//==================================================

function guardarCategoria() {

    var categoria = {

        iCodCategoria: 0,

        cNombre: $("#txtNombre").val(),
        cDescripcion: $("#txtDescripcion").val(),
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Categorias/Categorias.aspx/GuardarCategoria",
        data: JSON.stringify({
            categoria: categoria
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Categoría registrada correctamente.");

                limpiarFormulario();
                listarCategorias();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar la categoría.");

        }
    });

}


//==================================================
// SELECCIONAR CATEGORÍA PARA EDITAR
//==================================================

function seleccionarCategoria(idCategoria) {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Categorias/Categorias.aspx/ObtenerCategoria",
        data: JSON.stringify({
            idCategoria: idCategoria
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var categoria = response.d;

            $("#txtIdCategoria").val(categoria.iCodCategoria);
            $("#txtNombre").val(categoria.cNombre);
            $("#txtDescripcion").val(categoria.cDescripcion);
            $("#chkEstado").prop("checked", categoria.bEstado === true);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener la categoría.");

        }
    });

}


//==================================================
// MODIFICAR CATEGORÍA
//==================================================

function modificarCategoria() {

    var idCategoria = $("#txtIdCategoria").val();

    if (idCategoria === "") {

        alert("Primero selecciona una categoría.");

        return;

    }

    var categoria = {

        iCodCategoria: parseInt(idCategoria),

        cNombre: $("#txtNombre").val(),
        cDescripcion: $("#txtDescripcion").val(),
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Categorias/Categorias.aspx/ModificarCategoria",
        data: JSON.stringify({
            categoria: categoria
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Categoría modificada correctamente.");

                limpiarFormulario();
                listarCategorias();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar la categoría.");

        }
    });

}


//==================================================
// ELIMINAR CATEGORÍA
//==================================================

function eliminarCategoria(idCategoria) {

    var confirmar = confirm(
        "¿Deseas desactivar esta categoría?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Categorias/Categorias.aspx/EliminarCategoria",
        data: JSON.stringify({
            idCategoria: idCategoria
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Categoría desactivada correctamente.");

                limpiarFormulario();
                listarCategorias();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al eliminar la categoría.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdCategoria").val("");
    $("#txtNombre").val("");
    $("#txtDescripcion").val("");
    $("#chkEstado").prop("checked", true);

}