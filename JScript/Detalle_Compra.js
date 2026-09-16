$(document).ready(function () {

    listarComprasCombo();
    listarProductosCombo();
    listarDetallesCompra();

    $("#txtCantidad, #txtPrecioCompra").on("input", function () {
        calcularSubtotalDetalle();
    });

});
//==================================================
// CALCULAR EL SUBTOTAL PARA DETALLE
//==================================================

function calcularSubtotalDetalle() {

    var cantidad = parseFloat($("#txtCantidad").val()) || 0;
    var precio = parseFloat($("#txtPrecioCompra").val()) || 0;

    var subtotal = cantidad * precio;

    $("#txtSubTotal").val(subtotal.toFixed(2));
}

//==================================================
// LISTAR COMPRAS PARA COMBOBOX
//==================================================

function listarComprasCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/ListarComprasCombo",
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

            $("#cboCompra").html(html);

        },

        error: function (error) {

            console.log("Error al listar compras:");
            console.log(error);

        }
    });

}


//==================================================
// LISTAR PRODUCTOS PARA COMBOBOX
//==================================================

function listarProductosCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/ListarProductosCombo",
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

            $("#cboProducto").html(html);

        },

        error: function (error) {

            console.log("Error al listar productos:");
            console.log(error);

        }
    });

}


//==================================================
// LISTAR DETALLES DE COMPRA
//==================================================

function listarDetallesCompra() {

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/ListarDetallesCompra",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var detalles = response.d;

            var filas = "";

            for (var i = 0; i < detalles.length; i++) {

                filas += "<tr>";

                filas += "<td>" + detalles[i].iCodDetalleCompra + "</td>";
                filas += "<td>" + detalles[i].iCodCompra + "</td>";
                filas += "<td>" + detalles[i].cCodigo +
                    " - " + detalles[i].cNombreProducto + "</td>";
                filas += "<td>" + detalles[i].iCantidad + "</td>";
                filas += "<td>" + detalles[i].nPrecioCompra + "</td>";
                filas += "<td>" + detalles[i].nSubTotal + "</td>";

                filas += "<td>";

                filas += "<button type='button' onclick='seleccionarDetalleCompra(" +
                    detalles[i].iCodDetalleCompra +
                    ")'>Editar</button> ";

                filas += "<button type='button' onclick='eliminarDetalleCompra(" +
                    detalles[i].iCodDetalleCompra +
                    ")'>Eliminar</button>";

                filas += "</td>";

                filas += "</tr>";

            }

            $("#bodyDetalleCompra").html(filas);

        },

        error: function (error) {

            console.log("Error al listar detalles de compra:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR DETALLE DE COMPRA
//==================================================

function guardarDetalleCompra() {

    var detalle = {

        iCodDetalleCompra: 0,

        iCodCompra: parseInt($("#cboCompra").val() || 0),
        iCodProducto: parseInt($("#cboProducto").val() || 0),
        iCantidad: parseInt($("#txtCantidad").val()),
        nPrecioCompra: parseFloat($("#txtPrecioCompra").val()),
        nSubTotal: parseFloat($("#txtSubTotal").val())

    };

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/GuardarDetalleCompra",
        data: JSON.stringify({
            detalle: detalle
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Detalle registrado correctamente.");

                limpiarFormulario();
                listarDetallesCompra();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar el detalle.");

        }
    });

}


//==================================================
// SELECCIONAR DETALLE DE COMPRA PARA EDITAR
//==================================================

function seleccionarDetalleCompra(idDetalleCompra) {

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/ObtenerDetalleCompra",
        data: JSON.stringify({
            idDetalleCompra: idDetalleCompra
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var detalle = response.d;

            $("#txtIdDetalleCompra").val(detalle.iCodDetalleCompra);
            $("#cboCompra").val(detalle.iCodCompra);
            $("#cboProducto").val(detalle.iCodProducto);
            $("#txtCantidad").val(detalle.iCantidad);
            $("#txtPrecioCompra").val(detalle.nPrecioCompra);
            $("#txtSubTotal").val(detalle.nSubTotal);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener el detalle.");

        }
    });

}


//==================================================
// MODIFICAR DETALLE DE COMPRA
//==================================================

function modificarDetalleCompra() {

    var idDetalleCompra = $("#txtIdDetalleCompra").val();

    if (idDetalleCompra === "") {

        alert("Primero selecciona un detalle.");

        return;

    }

    var detalle = {

        iCodDetalleCompra: parseInt(idDetalleCompra),

        iCodCompra: parseInt($("#cboCompra").val() || 0),
        iCodProducto: parseInt($("#cboProducto").val() || 0),
        iCantidad: parseInt($("#txtCantidad").val()),
        nPrecioCompra: parseFloat($("#txtPrecioCompra").val()),
        nSubTotal: parseFloat($("#txtSubTotal").val())

    };

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/ModificarDetalleCompra",
        data: JSON.stringify({
            detalle: detalle
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Detalle modificado correctamente.");

                limpiarFormulario();
                listarDetallesCompra();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar el detalle.");

        }
    });

}


//==================================================
// ELIMINAR DETALLE DE COMPRA
//==================================================

function eliminarDetalleCompra(idDetalleCompra) {

    var confirmar = confirm(
        "¿Deseas eliminar este detalle?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/EliminarDetalleCompra",
        data: JSON.stringify({
            idDetalleCompra: idDetalleCompra
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Detalle eliminado correctamente.");

                limpiarFormulario();
                listarDetallesCompra();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al eliminar el detalle.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdDetalleCompra").val("");
    $("#cboCompra").val("");
    $("#cboProducto").val("");
    $("#txtCantidad").val("");
    $("#txtPrecioCompra").val("");
    $("#txtSubTotal").val("");

}