$(document).ready(function () {

    listarVentasCombo();
    listarProductosCombo();
    listarDetallesVenta();

});


//==================================================
// LISTAR VENTAS PARA COMBOBOX
//==================================================

function listarVentasCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Detalle_Venta/Detalle_Venta.aspx/ListarVentasCombo",
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

            $("#cboVenta").html(html);

        },

        error: function (error) {

            console.log("Error al listar ventas:");
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
        url: "/Views/Mantenimiento/Detalle_Venta/Detalle_Venta.aspx/ListarProductosCombo",
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
// LISTAR DETALLES DE VENTA
//==================================================

function listarDetallesVenta() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Detalle_Venta/Detalle_Venta.aspx/ListarDetallesVenta",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var detalles = response.d;

            var filas = "";

            for (var i = 0; i < detalles.length; i++) {

                filas += "<tr>";

                filas += "<td>" + detalles[i].iCodDetalleVenta + "</td>";
                filas += "<td>" + detalles[i].iCodVenta + "</td>";
                filas += "<td>" + detalles[i].cCodigo +
                    " - " + detalles[i].cNombreProducto + "</td>";
                filas += "<td>" + detalles[i].iCantidad + "</td>";
                filas += "<td>" + detalles[i].nPrecioVenta + "</td>";
                filas += "<td>" + detalles[i].nDescuento + "</td>";
                filas += "<td>" + detalles[i].nSubTotal + "</td>";

                filas += "<td>";

                filas += "<button type='button' onclick='seleccionarDetalleVenta(" +
                    detalles[i].iCodDetalleVenta +
                    ")'>Editar</button> ";

                filas += "<button type='button' onclick='eliminarDetalleVenta(" +
                    detalles[i].iCodDetalleVenta +
                    ")'>Eliminar</button>";

                filas += "</td>";

                filas += "</tr>";

            }

            $("#bodyDetalleVenta").html(filas);

        },

        error: function (error) {

            console.log("Error al listar detalles de venta:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR DETALLE DE VENTA
//==================================================

function guardarDetalleVenta() {

    var detalle = {

        iCodDetalleVenta: 0,

        iCodVenta: parseInt($("#cboVenta").val() || 0),
        iCodProducto: parseInt($("#cboProducto").val() || 0),
        iCantidad: parseInt($("#txtCantidad").val()),
        nPrecioVenta: parseFloat($("#txtPrecioVenta").val()),
        nDescuento: parseFloat($("#txtDescuento").val()),
        nSubTotal: parseFloat($("#txtSubTotal").val())

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Detalle_Venta/Detalle_Venta.aspx/GuardarDetalleVenta",
        data: JSON.stringify({
            detalle: detalle
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Detalle registrado correctamente.");

                limpiarFormulario();
                listarDetallesVenta();

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
// SELECCIONAR DETALLE DE VENTA PARA EDITAR
//==================================================

function seleccionarDetalleVenta(idDetalleVenta) {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Detalle_Venta/Detalle_Venta.aspx/ObtenerDetalleVenta",
        data: JSON.stringify({
            idDetalleVenta: idDetalleVenta
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var detalle = response.d;

            $("#txtIdDetalleVenta").val(detalle.iCodDetalleVenta);
            $("#cboVenta").val(detalle.iCodVenta);
            $("#cboProducto").val(detalle.iCodProducto);
            $("#txtCantidad").val(detalle.iCantidad);
            $("#txtPrecioVenta").val(detalle.nPrecioVenta);
            $("#txtDescuento").val(detalle.nDescuento);
            $("#txtSubTotal").val(detalle.nSubTotal);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener el detalle.");

        }
    });

}


//==================================================
// MODIFICAR DETALLE DE VENTA
//==================================================

function modificarDetalleVenta() {

    var idDetalleVenta = $("#txtIdDetalleVenta").val();

    if (idDetalleVenta === "") {

        alert("Primero selecciona un detalle.");

        return;

    }

    var detalle = {

        iCodDetalleVenta: parseInt(idDetalleVenta),

        iCodVenta: parseInt($("#cboVenta").val() || 0),
        iCodProducto: parseInt($("#cboProducto").val() || 0),
        iCantidad: parseInt($("#txtCantidad").val()),
        nPrecioVenta: parseFloat($("#txtPrecioVenta").val()),
        nDescuento: parseFloat($("#txtDescuento").val()),
        nSubTotal: parseFloat($("#txtSubTotal").val())

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Detalle_Venta/Detalle_Venta.aspx/ModificarDetalleVenta",
        data: JSON.stringify({
            detalle: detalle
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Detalle modificado correctamente.");

                limpiarFormulario();
                listarDetallesVenta();

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
// ELIMINAR DETALLE DE VENTA
//==================================================

function eliminarDetalleVenta(idDetalleVenta) {

    var confirmar = confirm(
        "¿Deseas eliminar este detalle?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Detalle_Venta/Detalle_Venta.aspx/EliminarDetalleVenta",
        data: JSON.stringify({
            idDetalleVenta: idDetalleVenta
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Detalle eliminado correctamente.");

                limpiarFormulario();
                listarDetallesVenta();

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

    $("#txtIdDetalleVenta").val("");
    $("#cboVenta").val("");
    $("#cboProducto").val("");
    $("#txtCantidad").val("");
    $("#txtPrecioVenta").val("");
    $("#txtDescuento").val(0);
    $("#txtSubTotal").val("");

}