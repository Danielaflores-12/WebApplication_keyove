$(document).ready(function () {
    $("#cboCompra").on("change", function () {
        limpiarFormulario();
        $("#editorDetalle").prop("disabled", !$(this).val());
        listarDetallesCompra();
    });
    limpiarFormulario();

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
    $("#importeDetalle").text("S/ " + subtotal.toFixed(2));
}

//==================================================
// LISTAR COMPRAS PARA COMBOBOX
//==================================================

function listarComprasCombo() {

    solicitarDetalle({
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
                    escaparDetalle(opciones[i].v) +
                    "'>" +
                    escaparDetalle(opciones[i].t) +
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

    solicitarDetalle({
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
// LISTAR DETALLES DE COMPRA
//==================================================

function listarDetallesCompra() {
    var comprobante = $("#cboCompra").val();
    if (!comprobante) {
        $("#bodyDetalleCompra").html("<tr><td colspan='5'>Selecciona un comprobante para ver sus productos.</td></tr>");
        return;
    }
    $("#bodyDetalleCompra").html("<tr><td colspan='5'>Cargando productos...</td></tr>");

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/ListarDetallesCompra",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if ($("#cboCompra").val() !== comprobante) return;
            var detalles = response.d.filter(function (detalle) {
                return String(detalle.iCodCompra) === comprobante;
            });

            var filas = "";

            for (var i = 0; i < detalles.length; i++) {

                filas += "<tr>";

                filas += "<td>" + escaparDetalle(detalles[i].cCodigo) +
                    " - " + escaparDetalle(detalles[i].cNombreProducto) + "</td>";
                filas += "<td>" + detalles[i].iCantidad + "</td>";
                filas += "<td>" + Number(detalles[i].nPrecioCompra).toFixed(2) + "</td>";
                filas += "<td>" + Number(detalles[i].nSubTotal).toFixed(2) + "</td>";

                filas += "<td><div class='table-actions'>";

                filas += "<button type='button' class='btn btn-sm btn-warning' onclick='seleccionarDetalleCompra(" +
                    detalles[i].iCodDetalleCompra +
                    ")'>Editar</button>";

                filas += "<button type='button' class='btn btn-sm btn-danger' onclick='eliminarDetalleCompra(" +
                    detalles[i].iCodDetalleCompra +
                    ")'>Eliminar</button>";

                filas += "</div></td>";

                filas += "</tr>";

            }

            $("#bodyDetalleCompra").html(filas || "<tr><td colspan='5'>Este comprobante todavía no tiene productos.</td></tr>");

        },

        error: function (error) {

            $("#bodyDetalleCompra").empty();
            $("#mensajeDetalle").text("No se pudieron cargar los productos. Selecciona nuevamente el comprobante para reintentar.");
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

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/GuardarDetalleCompra",
        data: JSON.stringify({
            detalle: detalle
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {



                limpiarFormulario();
                $("#mensajeDetalle").text("Comprobante actualizado.");
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

    solicitarDetalle({
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
            $("#cboCompra").prop("disabled", true);
            $("#tituloEditor").text("Editar producto");
            $("#btnGuardarDetalle").text("Guardar cambios");
            $("#btnCancelarDetalle").text("Cancelar edición");
            $("#opcionesDescuento").prop("open", Number(detalle.nDescuento) > 0);
            $("#mensajeDetalle").text("");
            calcularSubtotalDetalle();
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

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/ModificarDetalleCompra",
        data: JSON.stringify({
            detalle: detalle
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {



                limpiarFormulario();
                $("#mensajeDetalle").text("Comprobante actualizado.");
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

    solicitarDetalle({
        type: "POST",
        url: "/Views/Operaciones/Detalle_Compra/Detalle_Compra.aspx/EliminarDetalleCompra",
        data: JSON.stringify({
            idDetalleCompra: idDetalleCompra
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {



                limpiarFormulario();
                $("#mensajeDetalle").text("Comprobante actualizado.");
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
    $("#cboCompra").prop("disabled", false);
    $("#cboProducto").val("");
    $("#txtCantidad").val(1);
    $("#txtPrecioCompra").val("");
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
    var precioTexto = $("#txtPrecioCompra").val();
    var precio = Number(precioTexto);
    var descuento = Number($("#txtDescuento").val() || 0);
    var mensaje = "";
    if (!$("#cboCompra").val() || !$("#cboProducto").val()) mensaje = "Selecciona el comprobante y el producto.";
    else if (!Number.isInteger(cantidad) || cantidad < 1) mensaje = "La cantidad debe ser un número entero mayor que cero.";
    else if (precioTexto === "" || !Number.isFinite(precio) || precio < 0) mensaje = "Ingresa un precio unitario válido.";
    else if (!Number.isFinite(descuento) || descuento < 0 || descuento > cantidad * precio) mensaje = "El descuento no puede superar el importe del producto.";
    $("#mensajeDetalle").text(mensaje);
    if (mensaje) return;
    calcularSubtotalDetalle();
    if ($("#txtIdDetalleCompra").val()) modificarDetalleCompra();
    else guardarDetalleCompra();
}
var guardandoDetalle = false;
function solicitarDetalle(opciones) {
    var modifica = /\/(Guardar|Modificar|Eliminar)/.test(opciones.url);
    if (guardandoDetalle && modifica) return;
    if (modifica) {
        guardandoDetalle = true;
        $("#editorDetalle, #cboCompra, #bodyDetalleCompra button").prop("disabled", true);
        opciones.complete = function () {
            guardandoDetalle = false;
            $("#editorDetalle").prop("disabled", !$("#cboCompra").val());
            $("#cboCompra").prop("disabled", !!$("#txtIdDetalleCompra").val());
            $("#bodyDetalleCompra button").prop("disabled", false);
        };
    }
    return $.ajax(opciones);
}