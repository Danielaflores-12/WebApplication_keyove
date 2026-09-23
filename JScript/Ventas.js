$(document).ready(function () {

    listarUsuariosCombo();
    listarProductosCombo();
    listarVentas();

    $("#txtCantidadVenta, #txtPrecioVenta").on("input", function () {
        actualizarSubtotalLineaVenta();
    });

});


//==================================================
// CONSTANTES Y ESTADO DE LA VENTA
//==================================================

var cIGV = 0.18;

var ventaDetalles = [];


//==================================================
// FORMATEAR MONEDA SOLES
//==================================================

function formatoMoneda(valor) {

    var numero = Number(valor || 0);

    var partes = numero.toFixed(2).split(".");

    partes[0] = partes[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");

    return "S/ " + partes.join(".");

}


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
// LISTAR PRODUCTOS PARA COMBOBOX
//==================================================

function listarProductosCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ListarProductosCombo",
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
// ACTUALIZAR SUBTOTAL DE LA LÍNEA EN EDICIÓN
//==================================================

function actualizarSubtotalLineaVenta() {

    var cantidad = parseFloat($("#txtCantidadVenta").val()) || 0;
    var precio = parseFloat($("#txtPrecioVenta").val()) || 0;

    var subtotal = Math.round(cantidad * precio * 100) / 100;

    $("#txtSubtotalLineaVenta").val(formatoMoneda(subtotal));

}


//==================================================
// AGREGAR PRODUCTO AL DETALLE
//==================================================

function agregarProductoVenta() {

    var idProducto = parseInt($("#cboProducto").val() || 0, 10);
    var cantidad = parseInt($("#txtCantidadVenta").val() || 0, 10);
    var precio = parseFloat($("#txtPrecioVenta").val() || 0);

    if (!idProducto) {

        alert("Selecciona un producto.");

        return;

    }

    if (cantidad <= 0) {

        alert("La cantidad debe ser mayor a 0.");

        return;

    }

    if (precio <= 0) {

        alert("El precio de venta debe ser mayor a 0.");

        return;

    }

    var nombreProducto =
        $("#cboProducto option:selected").text();

    var subtotal = Math.round(precio * cantidad * 100) / 100;

    ventaDetalles.push({
        iCodProducto: idProducto,
        cNombreProducto: nombreProducto,
        iCantidad: cantidad,
        nPrecioVenta: precio,
        nDescuento: 0,
        nSubTotal: subtotal
    });

    $("#cboProducto").val("");
    $("#txtCantidadVenta").val("");
    $("#txtPrecioVenta").val("");
    $("#txtSubtotalLineaVenta").val("");

    dibujarDetalleVenta();

}


//==================================================
// QUITAR PRODUCTO DEL DETALLE
//==================================================

function quitarProductoVenta(index) {

    if (index < 0 || index >= ventaDetalles.length) {

        return;

    }

    ventaDetalles.splice(index, 1);

    dibujarDetalleVenta();

}


//==================================================
// DIBUJAR DETALLE DE VENTA
//==================================================

function dibujarDetalleVenta() {

    var filas = "";

    for (var i = 0; i < ventaDetalles.length; i++) {

        filas += "<tr>";

        filas += "<td>" + ventaDetalles[i].cNombreProducto + "</td>";
        filas += "<td>" + ventaDetalles[i].iCantidad + "</td>";
        filas += "<td>" + formatoMoneda(ventaDetalles[i].nPrecioVenta) + "</td>";
        filas += "<td>" + formatoMoneda(ventaDetalles[i].nSubTotal) + "</td>";

        filas += "<td><div class='table-actions'>";

        filas += "<button type='button' class='btn btn-sm btn-danger' onclick='quitarProductoVenta(" +
            i +
            ")'>Quitar</button>";

        filas += "</div></td>";

        filas += "</tr>";

    }

    $("#bodyDetalleVenta").html(filas);

    recalcTotalesVenta();

}


//==================================================
// RECALCULAR TOTALES: SUBTOTAL, IGV 18% Y TOTAL
//==================================================

function recalcTotalesVenta() {

    var subtotal = 0;

    for (var i = 0; i < ventaDetalles.length; i++) {

        subtotal += ventaDetalles[i].nSubTotal;

    }

    subtotal = Math.round(subtotal * 100) / 100;

    var igv = Math.round(subtotal * cIGV * 100) / 100;
    var total = Math.round((subtotal + igv) * 100) / 100;

    $("#txtSubtotalVentaGral").val(formatoMoneda(subtotal));
    $("#txtIGVVenta").val(formatoMoneda(igv));
    $("#txtTotalVenta").val(formatoMoneda(total));

    $("#txtSubTotal").val(subtotal.toFixed(2));
    $("#txtIgv").val(igv.toFixed(2));
    $("#txtTotal").val(total.toFixed(2));

}


//==================================================
// LISTAR USUARIOS PARA COMBOBOX
//==================================================

function listarUsuariosCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ListarUsuariosCombo",
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

            $("#cboUsuario").html(html);

        },

        error: function (error) {

            console.log("Error al listar usuarios:");
            console.log(error);

        }
    });

}


//==================================================
// LISTAR VENTAS
//==================================================

function listarVentas() {

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ListarVentas",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var ventas = response.d;

            var filas = "";

            for (var i = 0; i < ventas.length; i++) {

                var estado =
                    ventas[i].cEstado === "ANULADA"
                        ? "<span class='badge badge-inactive'>" +
                            ventas[i].cEstado +
                            "</span>"
                        : "<span class='badge badge-active'>" +
                            ventas[i].cEstado +
                            "</span>";

                filas += "<tr>";

                filas += "<td>" + ventas[i].iCodVenta + "</td>";
                filas += "<td>" + ventas[i].cNombreUsuario + "</td>";
                filas += "<td>" + ventas[i].cTipoComprobante + "</td>";
                filas += "<td>" + ventas[i].cNumeroComprobante + "</td>";
                filas += "<td>" + (ventas[i].cDocumentoCliente || "") + "</td>";
                filas += "<td>" + (ventas[i].cNumeroCelular || "") + "</td>";
                filas += "<td>" + formatearFecha(ventas[i].dFechaVenta) + "</td>";
                filas += "<td>" + ventas[i].nSubTotal + "</td>";
                filas += "<td>" + ventas[i].nIgv + "</td>";
                filas += "<td>" + ventas[i].nTotal + "</td>";
                filas += "<td>" + ventas[i].cMetodoPago + "</td>";
                filas += "<td>" + estado + "</td>";

                filas += "<td><div class='table-actions'>";

                filas += "<button type='button' class='btn btn-sm btn-warning' onclick='seleccionarVenta(" +
                    ventas[i].iCodVenta +
                    ")'>Editar</button>";

                filas += "<button type='button' class='btn btn-sm btn-danger' onclick='eliminarVenta(" +
                    ventas[i].iCodVenta +
                    ")'>Anular</button>";

                filas += "</div></td>";

                filas += "</tr>";

            }

            $("#bodyVentas").html(filas);

        },

        error: function (error) {

            console.log("Error al listar ventas:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR VENTA
//==================================================

function guardarVenta() {

    if (ventaDetalles.length === 0) {

        alert("Agrega al menos un producto a la venta.");

        return;

    }

    var venta = {

        iCodVenta: 0,

        iCodUsuario: parseInt($("#cboUsuario").val() || 0),
        cTipoComprobante: $("#cboTipoComprobante").val(),
        cDocumentoCliente: $("#txtDocumentoCliente").val(),
        cNumeroCelular: $("#txtNumeroCelular").val(),
        cNumeroComprobante: $("#txtNumeroComprobante").val(),
        nSubTotal: parseFloat($("#txtSubTotal").val()),
        nIgv: parseFloat($("#txtIgv").val()),
        nTotal: parseFloat($("#txtTotal").val()),
        cMetodoPago: $("#cboMetodoPago").val(),
        cObservacion: $("#txtObservacion").val(),
        cEstado: $("#cboEstado").val()

    };

    var detalles = [];

    for (var i = 0; i < ventaDetalles.length; i++) {

        detalles.push({
            iCodProducto: ventaDetalles[i].iCodProducto,
            iCantidad: ventaDetalles[i].iCantidad,
            nPrecioVenta: ventaDetalles[i].nPrecioVenta,
            nDescuento: ventaDetalles[i].nDescuento,
            nSubTotal: ventaDetalles[i].nSubTotal
        });

    }

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/GuardarVenta",
        data: JSON.stringify({
            venta: venta,
            detalles: detalles
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Venta registrada correctamente.");

                limpiarFormulario();
                listarVentas();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar la venta.");

        }
    });

}


//==================================================
// SELECCIONAR VENTA PARA EDITAR
//==================================================

function seleccionarVenta(idVenta) {

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ObtenerVenta",
        data: JSON.stringify({
            idVenta: idVenta
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var venta = response.d;

            $("#txtIdVenta").val(venta.iCodVenta);
            $("#cboUsuario").val(venta.iCodUsuario);
            $("#cboTipoComprobante").val(venta.cTipoComprobante);
            $("#txtDocumentoCliente").val(venta.cDocumentoCliente);
            $("#txtNumeroCelular").val(venta.cNumeroCelular);
            $("#txtNumeroComprobante").val(venta.cNumeroComprobante);
            $("#txtSubTotal").val(venta.nSubTotal);
            $("#txtIgv").val(venta.nIgv);
            $("#txtTotal").val(venta.nTotal);
            $("#cboMetodoPago").val(venta.cMetodoPago);
            $("#txtObservacion").val(venta.cObservacion);
            $("#cboEstado").val(venta.cEstado);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener la venta.");

        }
    });

}


//==================================================
// MODIFICAR VENTA
//==================================================

function modificarVenta() {

    var idVenta = $("#txtIdVenta").val();

    if (idVenta === "") {

        alert("Primero selecciona una venta.");

        return;

    }

    var venta = {

        iCodVenta: parseInt(idVenta),

        iCodUsuario: parseInt($("#cboUsuario").val() || 0),
        cTipoComprobante: $("#cboTipoComprobante").val(),
        cDocumentoCliente: $("#txtDocumentoCliente").val(),
        cNumeroCelular: $("#txtNumeroCelular").val(),
        cNumeroComprobante: $("#txtNumeroComprobante").val(),
        nSubTotal: parseFloat($("#txtSubTotal").val()),
        nIgv: parseFloat($("#txtIgv").val()),
        nTotal: parseFloat($("#txtTotal").val()),
        cMetodoPago: $("#cboMetodoPago").val(),
        cObservacion: $("#txtObservacion").val(),
        cEstado: $("#cboEstado").val()

    };

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ModificarVenta",
        data: JSON.stringify({
            venta: venta
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Venta modificada correctamente.");

                limpiarFormulario();
                listarVentas();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar la venta.");

        }
    });

}


//==================================================
// ANULAR VENTA
//==================================================

function eliminarVenta(idVenta) {

    var confirmar = confirm(
        "¿Deseas anular esta venta?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/EliminarVenta",
        data: JSON.stringify({
            idVenta: idVenta
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Venta anulada correctamente.");

                limpiarFormulario();
                listarVentas();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al anular la venta.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdVenta").val("");
    $("#cboUsuario").val("");
    $("#cboTipoComprobante").val("BOLETA");
    $("#txtDocumentoCliente").val("");
    $("#txtNumeroCelular").val("");
    $("#txtNumeroComprobante").val("");
    $("#txtSubTotal").val("");
    $("#txtIgv").val("");
    $("#txtTotal").val("");
    $("#cboMetodoPago").val("EFECTIVO");
    $("#txtObservacion").val("");
    $("#cboEstado").val("REGISTRADA");

    ventaDetalles = [];
    $("#bodyDetalleVenta").html("");
    $("#cboProducto").val("");
    $("#txtCantidadVenta").val("");
    $("#txtPrecioVenta").val("");
    $("#txtSubtotalLineaVenta").val("");
    $("#txtSubtotalVentaGral").val("");
    $("#txtIGVVenta").val("");
    $("#txtTotalVenta").val("");

}