$(document).ready(function () {

    listarUsuariosCombo();
    listarVentas();

});


//==================================================
// LISTAR USUARIOS PARA COMBOBOX
//==================================================

function listarUsuariosCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Ventas/Ventas.aspx/ListarUsuariosCombo",
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
        url: "/Views/Mantenimiento/Ventas/Ventas.aspx/ListarVentas",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var ventas = response.d;

            var filas = "";

            for (var i = 0; i < ventas.length; i++) {

                filas += "<tr>";

                filas += "<td>" + ventas[i].iCodVenta + "</td>";
                filas += "<td>" + ventas[i].cNombreUsuario + "</td>";
                filas += "<td>" + ventas[i].cTipoComprobante + "</td>";
                filas += "<td>" + ventas[i].cNumeroComprobante + "</td>";
                filas += "<td>" + (ventas[i].cDocumentoCliente || "") + "</td>";
                filas += "<td>" + (ventas[i].cNumeroCelular || "") + "</td>";
                filas += "<td>" + (ventas[i].dFechaVenta || "") + "</td>";
                filas += "<td>" + ventas[i].nSubTotal + "</td>";
                filas += "<td>" + ventas[i].nIgv + "</td>";
                filas += "<td>" + ventas[i].nTotal + "</td>";
                filas += "<td>" + ventas[i].cMetodoPago + "</td>";
                filas += "<td>" + ventas[i].cEstado + "</td>";

                filas += "<td>";

                filas += "<button type='button' onclick='seleccionarVenta(" +
                    ventas[i].iCodVenta +
                    ")'>Editar</button> ";

                filas += "<button type='button' onclick='eliminarVenta(" +
                    ventas[i].iCodVenta +
                    ")'>Anular</button>";

                filas += "</td>";

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

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Ventas/Ventas.aspx/GuardarVenta",
        data: JSON.stringify({
            venta: venta
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
        url: "/Views/Mantenimiento/Ventas/Ventas.aspx/ObtenerVenta",
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
        url: "/Views/Mantenimiento/Ventas/Ventas.aspx/ModificarVenta",
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
        url: "/Views/Mantenimiento/Ventas/Ventas.aspx/EliminarVenta",
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

}