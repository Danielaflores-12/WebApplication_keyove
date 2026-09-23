$(document).ready(function () {

    listarRolesCombo();
    listarPersonasCombo();
    listarUsuarios();

});


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
// LISTAR ROLES PARA COMBOBOX
//==================================================

function listarRolesCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Usuarios/Usuarios.aspx/ListarRolesCombo",
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

            $("#cboRol").html(html);

        },

        error: function (error) {

            console.log("Error al listar roles:");
            console.log(error);

        }
    });

}


//==================================================
// LISTAR PERSONAS PARA COMBOBOX
//==================================================

function listarPersonasCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Usuarios/Usuarios.aspx/ListarPersonasCombo",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var opciones = response.d;

            var html = "<option value=''>-- Opcional --</option>";

            for (var i = 0; i < opciones.length; i++) {

                html += "<option value='" +
                    opciones[i].v +
                    "'>" +
                    opciones[i].t +
                    "</option>";

            }

            $("#cboPersona").html(html);

        },

        error: function (error) {

            console.log("Error al listar personas:");
            console.log(error);

        }
    });

}


//==================================================
// LISTAR USUARIOS
//==================================================

function listarUsuarios() {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Usuarios/Usuarios.aspx/ListarUsuarios",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var usuarios = response.d;

            var filas = "";

            for (var i = 0; i < usuarios.length; i++) {

                var idPersona = usuarios[i].iCodPersona;

                var estado =
                    usuarios[i].bEstado
                        ? "<span class='badge badge-active'>Activo</span>"
                        : "<span class='badge badge-inactive'>Inactivo</span>";

                filas += "<tr>";

                filas += "<td>" + usuarios[i].iCodUsuario + "</td>";
                filas += "<td>" + (idPersona ? idPersona : "-") + "</td>";
                filas += "<td>" + usuarios[i].cNombreRol + "</td>";
                filas += "<td>" + usuarios[i].cNombreUsuario + "</td>";
                filas += "<td>" + estado + "</td>";
                filas += "<td>" + formatearFecha(usuarios[i].dFechaRegistro) + "</td>";

                filas += "<td><div class='table-actions'>";

                filas += "<button type='button' class='btn btn-sm btn-warning' onclick='seleccionarUsuario(" +
                    usuarios[i].iCodUsuario +
                    ")'>Editar</button>";

                filas += "<button type='button' class='btn btn-sm btn-danger' onclick='eliminarUsuario(" +
                    usuarios[i].iCodUsuario +
                    ")'>Eliminar</button>";

                filas += "</div></td>";

                filas += "</tr>";

            }

            $("#bodyUsuarios").html(filas);

        },

        error: function (error) {

            console.log("Error al listar usuarios:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR USUARIO
//==================================================

function guardarUsuario() {

    var usuario = {

        iCodUsuario: 0,

        iCodPersona: $("#cboPersona").val() ?
            parseInt($("#cboPersona").val()) :
            null,
        iCodRol: parseInt($("#cboRol").val() || 0),
        cNombreUsuario: $("#txtNombreUsuario").val(),
        cContrasenaHash: $("#txtContrasena").val(),
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Usuarios/Usuarios.aspx/GuardarUsuario",
        data: JSON.stringify({
            usuario: usuario
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Usuario registrado correctamente.");

                limpiarFormulario();
                listarUsuarios();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar el usuario.");

        }
    });

}


//==================================================
// SELECCIONAR USUARIO PARA EDITAR
//==================================================

function seleccionarUsuario(idUsuario) {

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Usuarios/Usuarios.aspx/ObtenerUsuario",
        data: JSON.stringify({
            idUsuario: idUsuario
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var usuario = response.d;

            $("#txtIdUsuario").val(usuario.iCodUsuario);

            $("#cboPersona").val(
                usuario.iCodPersona ? usuario.iCodPersona : ""
            );

            $("#cboRol").val(usuario.iCodRol);
            $("#txtNombreUsuario").val(usuario.cNombreUsuario);
            $("#txtContrasena").val("");
            $("#chkEstado").prop("checked", usuario.bEstado === true);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener el usuario.");

        }
    });

}


//==================================================
// MODIFICAR USUARIO
//==================================================

function modificarUsuario() {

    var idUsuario = $("#txtIdUsuario").val();

    if (idUsuario === "") {

        alert("Primero selecciona un usuario.");

        return;

    }

    var contrasena = $("#txtContrasena").val();

    var usuario = {

        iCodUsuario: parseInt(idUsuario),

        iCodPersona: $("#cboPersona").val() ?
            parseInt($("#cboPersona").val()) :
            null,
        iCodRol: parseInt($("#cboRol").val() || 0),
        cNombreUsuario: $("#txtNombreUsuario").val(),
        cContrasenaHash: contrasena,
        bEstado: $("#chkEstado").is(":checked")

    };

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Usuarios/Usuarios.aspx/ModificarUsuario",
        data: JSON.stringify({
            usuario: usuario
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Usuario modificado correctamente.");

                limpiarFormulario();
                listarUsuarios();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar el usuario.");

        }
    });

}


//==================================================
// ELIMINAR USUARIO
//==================================================

function eliminarUsuario(idUsuario) {

    var confirmar = confirm(
        "¿Deseas desactivar este usuario?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "/Views/Mantenimiento/Usuarios/Usuarios.aspx/EliminarUsuario",
        data: JSON.stringify({
            idUsuario: idUsuario
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Usuario desactivado correctamente.");

                limpiarFormulario();
                listarUsuarios();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al eliminar el usuario.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdUsuario").val("");
    $("#cboPersona").val("");
    $("#cboRol").val("");
    $("#txtNombreUsuario").val("");
    $("#txtContrasena").val("");
    $("#chkEstado").prop("checked", true);

}