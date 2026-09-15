$(document).ready(function () {

    listarRoles();

});


//==================================================
// LISTAR ROLES
//==================================================

function listarRoles() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Roles/Roles.aspx/ListarRoles",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var roles = response.d;

            var filas = "";

            for (var i = 0; i < roles.length; i++) {

                filas += "<tr>";

                filas += "<td>" + roles[i].iCodRol + "</td>";
                filas += "<td>" + roles[i].cNombre + "</td>";
                filas += "<td>" + (roles[i].cDescripcion || "") + "</td>";
                filas += "<td>" + (roles[i].bEstado ? "Activo" : "Inactivo") + "</td>";
                filas += "<td>" + (roles[i].dFechaRegistro || "") + "</td>";

                filas += "<td>";

                filas += "<button type='button' onclick='seleccionarRol(" +
                    roles[i].iCodRol +
                    ")'>Editar</button> ";

                filas += "<button type='button' onclick='eliminarRol(" +
                    roles[i].iCodRol +
                    ")'>Eliminar</button>";

                filas += "</td>";

                filas += "</tr>";

            }

            $("#bodyRoles").html(filas);

        },

        error: function (error) {

            console.log("Error al listar roles:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR ROL
//==================================================

function guardarRol() {

    var rol = {

        iCodRol: 0,

        cNombre: $("#txtNombre").val(),
        cDescripcion: $("#txtDescripcion").val(),
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Roles/Roles.aspx/GuardarRol",
        data: JSON.stringify({
            rol: rol
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Rol registrado correctamente.");

                limpiarFormulario();
                listarRoles();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar el rol.");

        }
    });

}


//==================================================
// SELECCIONAR ROL PARA EDITAR
//==================================================

function seleccionarRol(idRol) {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Roles/Roles.aspx/ObtenerRol",
        data: JSON.stringify({
            idRol: idRol
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var rol = response.d;

            $("#txtIdRol").val(rol.iCodRol);
            $("#txtNombre").val(rol.cNombre);
            $("#txtDescripcion").val(rol.cDescripcion);
            $("#chkEstado").prop("checked", rol.bEstado === true);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener el rol.");

        }
    });

}


//==================================================
// MODIFICAR ROL
//==================================================

function modificarRol() {

    var idRol = $("#txtIdRol").val();

    if (idRol === "") {

        alert("Primero selecciona un rol.");

        return;

    }

    var rol = {

        iCodRol: parseInt(idRol),

        cNombre: $("#txtNombre").val(),
        cDescripcion: $("#txtDescripcion").val(),
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Roles/Roles.aspx/ModificarRol",
        data: JSON.stringify({
            rol: rol
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Rol modificado correctamente.");

                limpiarFormulario();
                listarRoles();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar el rol.");

        }
    });

}


//==================================================
// ELIMINAR ROL
//==================================================

function eliminarRol(idRol) {

    var confirmar = confirm(
        "¿Deseas desactivar este rol?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Roles/Roles.aspx/EliminarRol",
        data: JSON.stringify({
            idRol: idRol
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Rol desactivado correctamente.");

                limpiarFormulario();
                listarRoles();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al eliminar el rol.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdRol").val("");
    $("#txtNombre").val("");
    $("#txtDescripcion").val("");
    $("#chkEstado").prop("checked", true);

}