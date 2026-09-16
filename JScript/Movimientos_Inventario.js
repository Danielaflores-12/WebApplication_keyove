$(document).ready(function () {

    listarProductosCombo();
    listarUsuariosCombo();
    listarMovimientos();

});


//==================================================
// LISTAR PRODUCTOS PARA COMBOBOX
//==================================================

function listarProductosCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Inventario/Movimientos_Inventario/Movimientos_Inventario.aspx/ListarProductosCombo",
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
// LISTAR USUARIOS PARA COMBOBOX
//==================================================

function listarUsuariosCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Inventario/Movimientos_Inventario/Movimientos_Inventario.aspx/ListarUsuariosCombo",
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
// LISTAR MOVIMIENTOS
//==================================================

function listarMovimientos() {

    $.ajax({
        type: "POST",
        url: "/Views/Inventario/Movimientos_Inventario/Movimientos_Inventario.aspx/ListarMovimientos",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var movimientos = response.d;

            var filas = "";

            for (var i = 0; i < movimientos.length; i++) {

                filas += "<tr>";

                filas += "<td>" + movimientos[i].iCodMovimiento + "</td>";
                filas += "<td>" + movimientos[i].cNombreProducto + "</td>";
                filas += "<td>" + movimientos[i].cNombreUsuario + "</td>";
                filas += "<td>" + (movimientos[i].iCodCompra || "-") + "</td>";
                filas += "<td>" + (movimientos[i].iCodVenta || "-") + "</td>";
                filas += "<td>" + movimientos[i].cTipoMovimiento + "</td>";
                filas += "<td>" + movimientos[i].iCantidad + "</td>";
                filas += "<td>" + movimientos[i].iStockAnterior + "</td>";
                filas += "<td>" + movimientos[i].iStockNuevo + "</td>";
                filas += "<td>" + (movimientos[i].cMotivo || "") + "</td>";
                filas += "<td>" + (movimientos[i].dFechaMovimiento || "") + "</td>";

                filas += "<td>";

                filas += "<button type='button' onclick='seleccionarMovimiento(" +
                    movimientos[i].iCodMovimiento +
                    ")'>Editar</button>";

                filas += "</td>";

                filas += "</tr>";

            }

            $("#bodyMovimientos").html(filas);

        },

        error: function (error) {

            console.log("Error al listar movimientos:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR MOVIMIENTO
//==================================================

function guardarMovimiento() {

    var idCompra = $("#txtIdCompra").val();
    var idVenta = $("#txtIdVenta").val();

    var movimiento = {

        iCodMovimiento: 0,

        iCodProducto: parseInt($("#cboProducto").val() || 0),
        iCodUsuario: parseInt($("#cboUsuario").val() || 0),
        iCodCompra: idCompra ? parseInt(idCompra) : null,
        iCodVenta: idVenta ? parseInt(idVenta) : null,
        cTipoMovimiento: $("#cboTipoMovimiento").val(),
        iCantidad: parseInt($("#txtCantidad").val()),
        iStockAnterior: parseInt($("#txtStockAnterior").val()),
        iStockNuevo: parseInt($("#txtStockNuevo").val()),
        cMotivo: $("#txtMotivo").val()

    };

    $.ajax({
        type: "POST",
        url: "/Views/Inventario/Movimientos_Inventario/Movimientos_Inventario.aspx/GuardarMovimiento",
        data: JSON.stringify({
            movimiento: movimiento
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Movimiento registrado correctamente.");

                limpiarFormulario();
                listarMovimientos();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar el movimiento.");

        }
    });

}


//==================================================
// SELECCIONAR MOVIMIENTO PARA EDITAR
//==================================================

function seleccionarMovimiento(idMovimiento) {

    $.ajax({
        type: "POST",
        url: "/Views/Inventario/Movimientos_Inventario/Movimientos_Inventario.aspx/ObtenerMovimiento",
        data: JSON.stringify({
            idMovimiento: idMovimiento
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var movimiento = response.d;

            $("#txtIdMovimiento").val(movimiento.iCodMovimiento);
            $("#cboProducto").val(movimiento.iCodProducto);
            $("#cboUsuario").val(movimiento.iCodUsuario);
            $("#cboTipoMovimiento").val(movimiento.cTipoMovimiento);
            $("#txtCantidad").val(movimiento.iCantidad);
            $("#txtStockAnterior").val(movimiento.iStockAnterior);
            $("#txtStockNuevo").val(movimiento.iStockNuevo);
            $("#txtMotivo").val(movimiento.cMotivo);
            $("#txtIdCompra").val(movimiento.iCodCompra || "");
            $("#txtIdVenta").val(movimiento.iCodVenta || "");

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener el movimiento.");

        }
    });

}


//==================================================
// MODIFICAR MOVIMIENTO
//==================================================

function modificarMovimiento() {

    var idMovimiento = $("#txtIdMovimiento").val();

    if (idMovimiento === "") {

        alert("Primero selecciona un movimiento.");

        return;

    }

    var idCompra = $("#txtIdCompra").val();
    var idVenta = $("#txtIdVenta").val();

    var movimiento = {

        iCodMovimiento: parseInt(idMovimiento),

        iCodProducto: parseInt($("#cboProducto").val() || 0),
        iCodUsuario: parseInt($("#cboUsuario").val() || 0),
        iCodCompra: idCompra ? parseInt(idCompra) : null,
        iCodVenta: idVenta ? parseInt(idVenta) : null,
        cTipoMovimiento: $("#cboTipoMovimiento").val(),
        iCantidad: parseInt($("#txtCantidad").val()),
        iStockAnterior: parseInt($("#txtStockAnterior").val()),
        iStockNuevo: parseInt($("#txtStockNuevo").val()),
        cMotivo: $("#txtMotivo").val()

    };

    $.ajax({
        type: "POST",
        url: "/Views/Inventario/Movimientos_Inventario/Movimientos_Inventario.aspx/ModificarMovimiento",
        data: JSON.stringify({
            movimiento: movimiento
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Movimiento modificado correctamente.");

                limpiarFormulario();
                listarMovimientos();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar el movimiento.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdMovimiento").val("");
    $("#cboProducto").val("");
    $("#cboUsuario").val("");
    $("#cboTipoMovimiento").val("ENTRADA");
    $("#txtCantidad").val("");
    $("#txtStockAnterior").val("");
    $("#txtStockNuevo").val("");
    $("#txtMotivo").val("");
    $("#txtIdCompra").val("");
    $("#txtIdVenta").val("");

}