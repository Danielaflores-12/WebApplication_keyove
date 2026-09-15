$(document).ready(function () {

    listarProveedores();

});


//==================================================
// LISTAR PROVEEDORES
//==================================================

function listarProveedores() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Proveedores/Proveedores.aspx/ListarProveedores",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var proveedores = response.d;

            var filas = "";

            for (var i = 0; i < proveedores.length; i++) {

                filas += "<tr>";

                filas += "<td>" + proveedores[i].iCodProveedor + "</td>";
                filas += "<td>" + proveedores[i].cRuc + "</td>";
                filas += "<td>" + proveedores[i].cRazonSocial + "</td>";
                filas += "<td>" + (proveedores[i].cRepresentante || "") + "</td>";
                filas += "<td>" + (proveedores[i].cTelefono || "") + "</td>";
                filas += "<td>" + (proveedores[i].cCorreo || "") + "</td>";
                filas += "<td>" + (proveedores[i].cDireccion || "") + "</td>";
                filas += "<td>" + (proveedores[i].bEstado ? "Activo" : "Inactivo") + "</td>";
                filas += "<td>" + (proveedores[i].dFechaRegistro || "") + "</td>";

                filas += "<td>";

                filas += "<button type='button' onclick='seleccionarProveedor(" +
                    proveedores[i].iCodProveedor +
                    ")'>Editar</button> ";

                filas += "<button type='button' onclick='eliminarProveedor(" +
                    proveedores[i].iCodProveedor +
                    ")'>Eliminar</button>";

                filas += "</td>";

                filas += "</tr>";

            }

            $("#bodyProveedores").html(filas);

        },

        error: function (error) {

            console.log("Error al listar proveedores:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR PROVEEDOR
//==================================================

function guardarProveedor() {

    var proveedor = {

        iCodProveedor: 0,

        cRuc: $("#txtRuc").val(),
        cRazonSocial: $("#txtRazonSocial").val(),
        cRepresentante: $("#txtRepresentante").val(),
        cTelefono: $("#txtTelefono").val(),
        cCorreo: $("#txtCorreo").val(),
        cDireccion: $("#txtDireccion").val(),
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Proveedores/Proveedores.aspx/GuardarProveedor",
        data: JSON.stringify({
            proveedor: proveedor
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Proveedor registrado correctamente.");

                limpiarFormulario();
                listarProveedores();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar el proveedor.");

        }
    });

}


//==================================================
// SELECCIONAR PROVEEDOR PARA EDITAR
//==================================================

function seleccionarProveedor(idProveedor) {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Proveedores/Proveedores.aspx/ObtenerProveedor",
        data: JSON.stringify({
            idProveedor: idProveedor
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var proveedor = response.d;

            $("#txtIdProveedor").val(proveedor.iCodProveedor);
            $("#txtRuc").val(proveedor.cRuc);
            $("#txtRazonSocial").val(proveedor.cRazonSocial);
            $("#txtRepresentante").val(proveedor.cRepresentante);
            $("#txtTelefono").val(proveedor.cTelefono);
            $("#txtCorreo").val(proveedor.cCorreo);
            $("#txtDireccion").val(proveedor.cDireccion);
            $("#chkEstado").prop("checked", proveedor.bEstado === true);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener el proveedor.");

        }
    });

}


//==================================================
// MODIFICAR PROVEEDOR
//==================================================

function modificarProveedor() {

    var idProveedor = $("#txtIdProveedor").val();

    if (idProveedor === "") {

        alert("Primero selecciona un proveedor.");

        return;

    }

    var proveedor = {

        iCodProveedor: parseInt(idProveedor),

        cRuc: $("#txtRuc").val(),
        cRazonSocial: $("#txtRazonSocial").val(),
        cRepresentante: $("#txtRepresentante").val(),
        cTelefono: $("#txtTelefono").val(),
        cCorreo: $("#txtCorreo").val(),
        cDireccion: $("#txtDireccion").val(),
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Proveedores/Proveedores.aspx/ModificarProveedor",
        data: JSON.stringify({
            proveedor: proveedor
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Proveedor modificado correctamente.");

                limpiarFormulario();
                listarProveedores();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar el proveedor.");

        }
    });

}


//==================================================
// ELIMINAR PROVEEDOR
//==================================================

function eliminarProveedor(idProveedor) {

    var confirmar = confirm(
        "¿Deseas desactivar este proveedor?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Proveedores/Proveedores.aspx/EliminarProveedor",
        data: JSON.stringify({
            idProveedor: idProveedor
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Proveedor desactivado correctamente.");

                limpiarFormulario();
                listarProveedores();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al eliminar el proveedor.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdProveedor").val("");
    $("#txtRuc").val("");
    $("#txtRazonSocial").val("");
    $("#txtRepresentante").val("");
    $("#txtTelefono").val("");
    $("#txtCorreo").val("");
    $("#txtDireccion").val("");
    $("#chkEstado").prop("checked", true);

}