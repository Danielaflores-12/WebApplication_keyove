$(document).ready(function () {

    listarPersonas();

});


//==================================================
// LISTAR PERSONAS
//==================================================

function listarPersonas() {

    $.ajax({
        type: "POST",
        url: "Views/Mantenimientos/Personas/Personas.aspx/ListarPersonas",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var personas = response.d;

            var filas = "";

            for (var i = 0; i < personas.length; i++) {

                filas += "<tr>";

                filas += "<td>" + personas[i].iCodPersona + "</td>";
                filas += "<td>" + personas[i].cNombres + "</td>";
                filas += "<td>" + personas[i].cApellidos + "</td>";
                filas += "<td>" + personas[i].cGenero + "</td>";
                filas += "<td>" + (personas[i].cCorreo || "") + "</td>";
                filas += "<td>" + (personas[i].cTelefono || "") + "</td>";
                filas += "<td>" + (personas[i].dFechaNacimiento || "") + "</td>";
                filas += "<td>" + (personas[i].dFechaRegistro || "") + "</td>";

                filas += "<td>";

                filas += "<button type='button' onclick='seleccionarPersona(" +
                    personas[i].iCodPersona +
                    ")'>Editar</button> ";

                filas += "<button type='button' onclick='eliminarPersona(" +
                    personas[i].iCodPersona +
                    ")'>Eliminar</button>";

                filas += "</td>";

                filas += "</tr>";

            }

            $("#bodyPersonas").html(filas);

        },

        error: function (error) {

            console.log("Error al listar personas:");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR PERSONA
//==================================================

function guardarPersona() {

    var persona = {

        iCodPersona: 0,

        cNombres: $("#txtNombres").val(),
        cApellidos: $("#txtApellidos").val(),
        cGenero: $("#ddlGenero").val(),
        cCorreo: $("#txtCorreo").val(),
        cTelefono: $("#txtTelefono").val(),
        dFechaNacimiento: $("#txtFechaNacimiento").val()

    };

    $.ajax({
        type: "POST",
        url: "Views/Mantenimientos/Personas/Personas.aspx/GuardarPersona",
        data: JSON.stringify({
            persona: persona
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Persona registrada correctamente.");

                limpiarFormulario();
                listarPersonas();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al registrar la persona.");

        }
    });

}


//==================================================
// SELECCIONAR PERSONA PARA EDITAR
//==================================================

function seleccionarPersona(idPersona) {

    $.ajax({
        type: "POST",
        url: "Views/Mantenimientos/Personas/Personas.aspx/ObtenerPersona",
        data: JSON.stringify({
            idPersona: idPersona
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var persona = response.d;

            $("#txtIdPersona").val(persona.iCodPersona);
            $("#txtNombres").val(persona.cNombres);
            $("#txtApellidos").val(persona.cApellidos);
            $("#ddlGenero").val(persona.cGenero);
            $("#txtCorreo").val(persona.cCorreo);
            $("#txtTelefono").val(persona.cTelefono);
            $("#txtFechaNacimiento").val(persona.dFechaNacimiento);

        },

        error: function (error) {

            console.log(error);

            alert("Error al obtener la persona.");

        }
    });

}


//==================================================
// MODIFICAR PERSONA
//==================================================

function modificarPersona() {

    var idPersona = $("#txtIdPersona").val();

    if (idPersona === "") {

        alert("Primero selecciona una persona.");

        return;

    }

    var persona = {

        iCodPersona: parseInt(idPersona),

        cNombres: $("#txtNombres").val(),
        cApellidos: $("#txtApellidos").val(),
        cGenero: $("#ddlGenero").val(),
        cCorreo: $("#txtCorreo").val(),
        cTelefono: $("#txtTelefono").val(),
        dFechaNacimiento: $("#txtFechaNacimiento").val()

    };

    $.ajax({
        type: "POST",
        url: "Views/Mantenimientos/Personas/Personas.aspx/ModificarPersona",
        data: JSON.stringify({
            persona: persona
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Persona modificada correctamente.");

                limpiarFormulario();
                listarPersonas();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al modificar la persona.");

        }
    });

}


//==================================================
// ELIMINAR PERSONA
//==================================================

function eliminarPersona(idPersona) {

    var confirmar = confirm(
        "¿Deseas eliminar esta persona?"
    );

    if (!confirmar) {

        return;

    }

    $.ajax({
        type: "POST",
        url: "Views/Mantenimientos/Personas/Personas.aspx/EliminarPersona",
        data: JSON.stringify({
            idPersona: idPersona
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Persona eliminada correctamente.");

                limpiarFormulario();
                listarPersonas();

            } else {

                alert(response.d);

            }

        },

        error: function (error) {

            console.log(error);

            alert("Error al eliminar la persona.");

        }
    });

}


//==================================================
// LIMPIAR FORMULARIO
//==================================================

function limpiarFormulario() {

    $("#txtIdPersona").val("");
    $("#txtNombres").val("");
    $("#txtApellidos").val("");
    $("#ddlGenero").val("");
    $("#txtCorreo").val("");
    $("#txtTelefono").val("");
    $("#txtFechaNacimiento").val("");

}