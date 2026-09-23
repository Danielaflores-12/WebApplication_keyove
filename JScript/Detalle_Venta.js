$(document).ready(function () {
    $("#cboVenta").on("change", function () {
        limpiarFormulario();
        $("#editorDetalle").prop("disabled", !$(this).val());
        listarDetallesVenta();
    });
    limpiarFormulario();

    listarVentasCombo();
    listarProductosCombo();
    listarDetallesVenta();
    $("#txtCantidad, #txtPrecioVenta, #txtDescuento").on("input", function () {
            calcularSubtotalVenta();
        });

});

//==================================================
// CALCULAR EL SUBTOTAL PARA VENTA 
//==================================================

function calcularSubtotalVenta() {

    var cantidad = parseFloat($("#txtCantidad").val()) || 0;
    var precio = parseFloat($("#txtPrecioVenta").val()) || 0;
    var descuento = parseFloat($("#txtDescuento").val() || 0) || 0;

    var subtotal = (cantidad * precio) - descuento;

    if (subtotal < 0) {
        subtotal = 0;
    }

    $("#txtSubTotal").val(subtotal.toFixed(2));
    $("#importeDetalle").text("S/ " + subtotal.toFixed(2));
}

//==================================================
// LISTAR VENTAS PARA COMBOBOX
//==================================================

function listarVentasCombo() {

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Venta/Detalle_Venta.aspx/ListarVentasCombo",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var opciones = response.d;

            var html = "<option value=''>-- Seleccionar --</option>";

            for (var i = 0; i < opciones.length; i++) {

                html += "<option value='" +
                    escaparDetalle(opciones[i].v) +
                    "'>" +
                    escaparDetalle(opciones[i].t) +
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

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Venta/Detalle_Venta.aspx/ListarProductosCombo",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var opciones = response.d;

            var html = "<option value=''>-- Seleccionar --</option>";

            for (var i = 0; i < opciones.length; i++) {

                html += "<option value='" +
                    escaparDetalle(opciones[i].v) +
                    "'>" +
                    escaparDetalle(opciones[i].t) +
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
    var comprobante = $("#cboVenta").val();
    if (!comprobante) {
        $("#bodyDetalleVenta").html("<tr><td colspan='6'>Selecciona un comprobante para ver sus productos.</td></tr>");
        return;
    }
    $("#bodyDetalleVenta").html("<tr><td colspan='6'>Cargando productos...</td></tr>");

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Venta/Detalle_Venta.aspx/ListarDetallesVenta",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if ($("#cboVenta").val() !== comprobante) return;
            var detalles = response.d.filter(function (detalle) {
                return String(detalle.iCodVenta) === comprobante;
            });

            var filas = "";

            for (var i = 0; i < detalles.length; i++) {

                filas += "<tr>";

                filas += "<td>" + escaparDetalle(detalles[i].cCodigo) +
                    " - " + escaparDetalle(detalles[i].cNombreProducto) + "</td>";
                filas += "<td>" + detalles[i].iCantidad + "</td>";
                filas += "<td>" + Number(detalles[i].nPrecioVenta).toFixed(2) + "</td>";
                filas += "<td>" + Number(detalles[i].nDescuento).toFixed(2) + "</td>";
                filas += "<td>" + Number(detalles[i].nSubTotal).toFixed(2) + "</td>";

                filas += "<td><div class='table-actions'>";

                filas += "<button type='button' class='btn btn-sm btn-warning' onclick='seleccionarDetalleVenta(" +
                    detalles[i].iCodDetalleVenta +
                    ")'>Editar</button>";

                filas += "<button type='button' class='btn btn-sm btn-danger' onclick='eliminarDetalleVenta(" +
                    detalles[i].iCodDetalleVenta +
                    ")'>Eliminar</button>";

                filas += "</div></td>";

                filas += "</tr>";

            }

            $("#bodyDetalleVenta").html(filas || "<tr><td colspan='6'>Este comprobante todavía no tiene productos.</td></tr>");

        },

        error: function (error) {

            $("#bodyDetalleVenta").empty();
            $("#mensajeDetalle").text("No se pudieron cargar los productos. Selecciona nuevamente el comprobante para reintentar.");
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
        nDescuento: parseFloat($("#txtDescuento").val() || 0),
        nSubTotal: parseFloat($("#txtSubTotal").val())

    };

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Venta/Detalle_Venta.aspx/GuardarDetalleVenta",
        data: JSON.stringify({
            detalle: detalle
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {



                limpiarFormulario();
                $("#mensajeDetalle").text("Comprobante actualizado.");
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

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Venta/Detalle_Venta.aspx/ObtenerDetalleVenta",
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
            $("#cboVenta").prop("disabled", true);
            $("#tituloEditor").text("Editar producto");
            $("#btnGuardarDetalle").text("Guardar cambios");
            $("#btnCancelarDetalle").text("Cancelar edición");
            $("#opcionesDescuento").prop("open", Number(detalle.nDescuento) > 0);
            $("#mensajeDetalle").text("");
            calcularSubtotalVenta();
            document.getElementById("tituloEditor").scrollIntoView({ behavior: "smooth", block: "center" });
            $("#txtCantidad").trigger("focus");

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
        nDescuento: parseFloat($("#txtDescuento").val() || 0),
        nSubTotal: parseFloat($("#txtSubTotal").val())

    };

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Venta/Detalle_Venta.aspx/ModificarDetalleVenta",
        data: JSON.stringify({
            detalle: detalle
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {



                limpiarFormulario();
                $("#mensajeDetalle").text("Comprobante actualizado.");
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

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Venta/Detalle_Venta.aspx/EliminarDetalleVenta",
        data: JSON.stringify({
            idDetalleVenta: idDetalleVenta
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {



                limpiarFormulario();
                $("#mensajeDetalle").text("Comprobante actualizado.");
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
    $("#cboVenta").prop("disabled", false);
    $("#cboProducto").val("");
    $("#txtCantidad").val(1);
    $("#txtPrecioVenta").val("");
    $("#txtDescuento").val(0);
    $("#txtSubTotal").val("0.00");
    $("#importeDetalle").text("S/ 0.00");
    $("#tituloEditor").text("2. Agrega un producto");
    $("#btnGuardarDetalle").text("Agregar producto");
    $("#btnCancelarDetalle").text("Limpiar producto");
    $("#opcionesDescuento").prop("open", false);
    $("#mensajeDetalle").text("");

}
function escaparDetalle(texto) {
    return $("<span>").text(texto || "").html();
}

function enviarDetalle() {
    var cantidad = Number($("#txtCantidad").val());
    var precioTexto = $("#txtPrecioVenta").val();
    var precio = Number(precioTexto);
    var descuento = Number($("#txtDescuento").val() || 0);
    var mensaje = "";
    if (!$("#cboVenta").val() || !$("#cboProducto").val()) mensaje = "Selecciona el comprobante y el producto.";
    else if (!Number.isInteger(cantidad) || cantidad < 1) mensaje = "La cantidad debe ser un número entero mayor que cero.";
    else if (precioTexto === "" || !Number.isFinite(precio) || precio < 0) mensaje = "Ingresa un precio unitario válido.";
    else if (!Number.isFinite(descuento) || descuento < 0 || descuento > cantidad * precio) mensaje = "El descuento no puede superar el importe del producto.";
    $("#mensajeDetalle").text(mensaje);
    if (mensaje) return;
    calcularSubtotalVenta();
    if ($("#txtIdDetalleVenta").val()) modificarDetalleVenta();
    else guardarDetalleVenta();
}
var guardandoDetalle = false;
function solicitarDetalle(opciones) {
    var modifica = /\/(Guardar|Modificar|Eliminar)/.test(opciones.url);
    if (guardandoDetalle && modifica) return;
    if (modifica) {
        guardandoDetalle = true;
        $("#editorDetalle, #cboVenta, #bodyDetalleVenta button").prop("disabled", true);
        opciones.complete = function () {
            guardandoDetalle = false;
            $("#editorDetalle").prop("disabled", !$("#cboVenta").val());
            $("#cboVenta").prop("disabled", !!$("#txtIdDetalleVenta").val());
            $("#bodyDetalleVenta button").prop("disabled", false);
        };
    }
    return $.ajax(opciones);
}