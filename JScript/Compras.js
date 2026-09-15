$(document).ready(function () {

    listarProveedoresCombo();
    listarUsuariosCombo();
    listarCompras();

});


//==================================================
// LISTAR PROVEEDORES PARA COMBOBOX
//==================================================

function listarProveedoresCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Compras/Compras.aspx/ListarProveedoresCombo",
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

            $("#cboProveedor").html(html);

        },

        error: function (error) {

            console.log("Error al listar proveedores:");
            console.log(error);

        }
    });

}


//==================================================
// LISTAR USUARIOS PARA COMBOBOX
//==================================================

function listarUsuariosCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Compras/Compras.aspx/ListarUsuariosCombo",
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
// LISTAR COMPRAS
//==================================================

function listarCompras() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Compras/Compras.aspx/ListarCompras",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var compras = response.d;

            var filas = "";

            for (var i = 0; i < compras.length; i++) {

                filas += "<tr>";

                filas += "<td>" + compras[i].iCodCompra + "</td>";
                filas += "<td>" + compras[i].iCodProveedor + "</td>";
                filas += "<td>" + compras[i].iCodUsuario + "</td>";
                filas += "<td>" + compras[i].cTipoComprobante + "</td>";
                filas += "<td>" + compras[i].cNumeroComprobante + "</td>";
                filas += "<td>" + (compras[i].dFechaCompra || "") + "</td>";
                filas += "<td>" + compras[i].nSubTotal + "</td>";
                filas += "<td>" + compras[i].nIgv + "</td>";
                filas += "<td>" + compras[i].nTotal + "</td>";
                filas += "<td>" + compras[i].cEstado + "</td>";

                filas += "<td>";

                filas += "<button type='button' onclick='seleccionarCompra(" +
                    compras[i].iCodCompra +
                    ")'>Editar</button> ";

                filas += "<button type='button' onclick='eliminarCompra(" +
                    compras[i].iCodCompra +
                    ")'>Anular</button>";

                filas += "</td>";

                filas += "</tr>";

            }

            $("#bodyCompras").html(filas);

        },

        error: function (error) {

            console.log("Error al listar compras:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR COMPRA
//==================================================

function guardarCompra() {

    var compra = {

        iCodCompra: 0,

        iCodProveedor: parseInt($("#cboProveedor").val() || 0),
        iCodUsuario: parseInt($("#cboUsuario").val() || 0),
        cTipoComprobante: $("#cboTipoComprobante").val(),
        cNumeroComprobante: $("#txtNumeroComprobante").val(),
        nSubTotal: parseFloat($("#txtSubTotal").val()),
        nIgv: parseFloat($("#txtIgv").val()),
        nTotal: parseFloat($("#txtTotal").val()),
        cObservacion: $("#txtObservacion").val(),
        cEstado: $("#cboEstado").val()

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Compras/Compras.aspx/GuardarCompra",
        data: JSON.stringify({
            compra: compra
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Compra registrada correctamente.");

                limpiarFormulario();
                listarCompras();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar la compra.");

        }
    });

}


//==================================================
// SELECCIONAR COMPRA PARA EDITAR
//==================================================

function seleccionarCompra(idCompra) {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Compras/Compras.aspx/ObtenerCompra",
        data: JSON.stringify({
            idCompra: idCompra
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var compra = response.d;

            $("#txtIdCompra").val(compra.iCodCompra);
            $("#cboProveedor").val(compra.iCodProveedor);
            $("#cboUsuario").val(compra.iCodUsuario);
            $("#cboTipoComprobante").val(compra.cTipoComprobante);
            $("#txtNumeroComprobante").val(compra.cNumeroComprobante);
            $("#txtSubTotal").val(compra.nSubTotal);
            $("#txtIgv").val(compra.nIgv);
            $("#txtTotal").val(compra.nTotal);
            $("#txtObservacion").val(compra.cObservacion);
            $("#cboEstado").val(compra.cEstado);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener la compra.");

        }
    });

}


//==================================================
// MODIFICAR COMPRA
//==================================================

function modificarCompra() {

    var idCompra = $("#txtIdCompra").val();

    if (idCompra === "") {

        alert("Primero selecciona una compra.");

        return;

    }

    var compra = {

        iCodCompra: parseInt(idCompra),

        iCodProveedor: parseInt($("#cboProveedor").val() || 0),
        iCodUsuario: parseInt($("#cboUsuario").val() || 0),
        cTipoComprobante: $("#cboTipoComprobante").val(),
        cNumeroComprobante: $("#txtNumeroComprobante").val(),
        nSubTotal: parseFloat($("#txtSubTotal").val()),
        nIgv: parseFloat($("#txtIgv").val()),
        nTotal: parseFloat($("#txtTotal").val()),
        cObservacion: $("#txtObservacion").val(),
        cEstado: $("#cboEstado").val()

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Compras/Compras.aspx/ModificarCompra",
        data: JSON.stringify({
            compra: compra
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Compra modificada correctamente.");

                limpiarFormulario();
                listarCompras();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar la compra.");

        }
    });

}


//==================================================
// ANULAR COMPRA
//==================================================

function eliminarCompra(idCompra) {

    var confirmar = confirm(
        "¿Deseas anular esta compra?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Compras/Compras.aspx/EliminarCompra",
        data: JSON.stringify({
            idCompra: idCompra
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Compra anulada correctamente.");

                limpiarFormulario();
                listarCompras();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al anular la compra.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdCompra").val("");
    $("#cboProveedor").val("");
    $("#cboUsuario").val("");
    $("#cboTipoComprobante").val("FACTURA");
    $("#txtNumeroComprobante").val("");
    $("#txtSubTotal").val("");
    $("#txtIgv").val("");
    $("#txtTotal").val("");
    $("#txtObservacion").val("");
    $("#cboEstado").val("REGISTRADA");

}