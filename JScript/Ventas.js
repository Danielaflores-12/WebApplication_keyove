<<<<<<< Updated upstream
﻿function esAdministradorVentas() {
=======

"use strict";

// ==================================================
// CONFIGURACIÓN Y ESTADO
// ==================================================

var cIGV = 0.18;
var ventaDetalles = [];
var ventaSeleccionadaId = 0;
var solicitudVentaEnCurso = false;


// ==================================================
// PERMISOS
// ==================================================

function esAdministradorVentas() {
>>>>>>> Stashed changes
    return String($("body").attr("data-rol") || "")
        .trim()
        .toLowerCase() === "administrador";
}

function aplicarVistaVendedor() {
    var administrador = esAdministradorVentas();

    $("#btnModificarVentaModal").prop("hidden", !administrador);

    if (!administrador) {
        $("#cboUsuario").prop("disabled", true);
        $("#cboEstado").val("REGISTRADA").prop("disabled", true);
        $("#btnModificarVentaModal").prop("hidden", true);
    } else {
        $("#cboUsuario").prop("disabled", false);
        $("#cboEstado").prop("disabled", false);
    }
}


// ==================================================
// INICIALIZACIÓN
// ==================================================

$(document).ready(function() {

    // Apertura robusta del modal desde el botón Agregar.
    // Se evita duplicar la acción si el botón ya usa onclick="abrirModalVenta('nuevo')".
    $(document).on("click.ventasAbrirModal", "#btnAgregarVenta, #btnAgregarVentaModal, [data-abrir-venta='nuevo']", function(evento) {
        evento.preventDefault();
        abrirModalVenta("nuevo");
    });

    // Filtros.
    $("#buscarVentas").on("input", filtrarVentasRegistradas);
    $("#estadoBusquedaVentas").on("change", filtrarVentasRegistradas);

    // Cálculo de subtotal por producto.
    $("#txtCantidadVenta, #txtPrecioVenta").on(
        "input",
        actualizarSubtotalLineaVenta
    );

    $("#cboProducto").on("change", actualizarSubtotalLineaVenta);

    // Conectar botones aunque no tengan onclick en el HTML.
    $("#btnAgregarProductoVenta")
        .off("click.ventas")
        .on("click.ventas", function(e) {
            e.preventDefault();
            agregarProductoVenta();
        });

    $("#cboTipoComprobante").on("change", function() {
        $("#txtNumeroComprobante").val("");
    });

    // Cargar información inicial.
    listarProductosCombo();

    var usuariosListos = listarUsuariosCombo();

    listarVentas();

    // Seleccionar venta en la tabla.
    $("#bodyVentas").on("click", "tr[data-id-venta]", function(evento) {
        if ($(evento.target).closest("a, button").length) {
            return;
        }

        $("#bodyVentas tr").removeClass("venta-seleccionada");
        $(this).addClass("venta-seleccionada");

        ventaSeleccionadaId =
            Number($(this).attr("data-id-venta")) || 0;
    });

    // Editar desde la tabla.
    $("#bodyVentas").on("click", "[data-editar-venta]", function(evento) {
        evento.preventDefault();
        evento.stopPropagation();

        abrirModalVenta(
            "editar",
            Number($(this).attr("data-editar-venta"))
        );
    });

    // Anular desde la tabla.
    $("#bodyVentas").on("click", "[data-anular-venta]", function(evento) {
        evento.preventDefault();
        evento.stopPropagation();

        eliminarVenta(Number($(this).attr("data-anular-venta")));
    });

    // Enlaces del menú de ventas.
    $(document).on("click", "[data-vista-ventas]", function(evento) {
        if (
            evento.ctrlKey ||
            evento.metaKey ||
            evento.shiftKey ||
            evento.altKey
        ) {
            return;
        }

        evento.preventDefault();

        var vista = $(this).attr("data-vista-ventas");

        if (vista === "registro") {
            abrirModalVenta("nuevo");
        } else {
            cerrarModalVenta();
            listarVentas();
        }

        actualizarUrlVentas(vista);

        if (window.matchMedia("(max-width: 768px)").matches) {
            $("#fondoMenu").trigger("click");
        }
    });

    // Cerrar al pulsar fuera del modal.
    $("#modalVenta").on("click", function(evento) {
        if (evento.target === this) {
            cerrarModalVenta();
        }
    });

    // Escape cierra el modal.
    $(document).on("keydown", function(evento) {
        if (
            evento.key === "Escape" &&
            !$("#modalVenta").prop("hidden")
        ) {
            cerrarModalVenta();
        }
    });

    // Abrir una venta desde una URL con ?id=...
    var parametros = new URLSearchParams(window.location.search);
    var idUrl = parametros.get("id");

    if (/^[1-9]\d*$/.test(idUrl || "")) {
        usuariosListos.done(function() {
            abrirModalVenta("editar", Number(idUrl));
        });
    }

    aplicarVistaVendedor();
});


// ==================================================
// UTILIDADES
// ==================================================

function formatoMoneda(valor) {
    var numero = Number(valor || 0);

    if (!Number.isFinite(numero)) {
        numero = 0;
    }

    return "S/ " + numero.toLocaleString("es-PE", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

function escaparHtml(valor) {
    return String(valor == null ? "" : valor)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");
}

function formatearFecha(valor) {
    if (!valor) return "";

    var coincidencia = /\/Date\((\d+)\)\//.exec(String(valor));
    var fecha = coincidencia
        ? new Date(parseInt(coincidencia[1], 10))
        : new Date(valor);

    if (isNaN(fecha.getTime())) {
        return escaparHtml(valor);
    }

    return fecha.toLocaleDateString("es-PE") + " " +
        fecha.toLocaleTimeString("es-PE", {
            hour: "2-digit",
            minute: "2-digit"
        });
}

function obtenerRespuesta(response) {
    return response && typeof response.d !== "undefined"
        ? response.d
        : null;
}

function mostrarErrorAjax(titulo, error) {
    console.error(titulo, error);

    var mensaje = titulo;

    if (error && error.responseJSON && error.responseJSON.Message) {
        mensaje += "\n" + error.responseJSON.Message;
    }

    alert(mensaje);
}


// ==================================================
// CARGAR PRODUCTOS
// ==================================================

function listarProductosCombo() {
    return $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ListarProductosCombo",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function(response) {
            var opciones = obtenerRespuesta(response) || [];
            var html = "<option value=''>-- Seleccionar producto --</option>";

            opciones.forEach(function(producto) {
                html += "<option value='" +
                    escaparHtml(producto.v) + "'>" +
                    escaparHtml(producto.t) +
                    "</option>";
            });

            $("#cboProducto").html(html);
        },

        error: function(error) {
            mostrarErrorAjax("No se pudieron cargar los productos.", error);
        }
    });
}


<<<<<<< Updated upstream
//==================================================
// ACTUALIZAR SUBTOTAL DE LA LÃNEA EN EDICIÃ“N
//==================================================
=======
// ==================================================
// SUBTOTAL DE LÍNEA
// ==================================================
>>>>>>> Stashed changes

function actualizarSubtotalLineaVenta() {
    var cantidad = parseInt($("#txtCantidadVenta").val(), 10) || 0;
    var precio = parseFloat($("#txtPrecioVenta").val()) || 0;

    var subtotal = Math.round(cantidad * precio * 100) / 100;

    $("#txtSubtotalLineaVenta").val(formatoMoneda(subtotal));
}


// ==================================================
// AGREGAR PRODUCTO A LA VENTA
// ==================================================

function agregarProductoVenta() {
    var idProducto = parseInt($("#cboProducto").val() || "0", 10);
    var cantidad = parseInt($("#txtCantidadVenta").val() || "0", 10);
    var precio = parseFloat($("#txtPrecioVenta").val() || "0");

    if (!idProducto) {
        alert("Selecciona un producto.");
        return;
    }

    if (!Number.isInteger(cantidad) || cantidad <= 0) {
        alert("La cantidad debe ser un número entero mayor que cero.");
        return;
    }

    if (!Number.isFinite(precio) || precio <= 0) {
        alert("El precio de venta debe ser mayor que cero.");
        return;
    }

    var duplicado = ventaDetalles.some(function(detalle) {
        return Number(detalle.iCodProducto) === idProducto;
    });

    if (duplicado) {
        alert("Ese producto ya está agregado a la venta.");
        return;
    }

    var nombre = $("#cboProducto option:selected").text();
    var subtotal = Math.round(cantidad * precio * 100) / 100;

    ventaDetalles.push({
        iCodProducto: idProducto,
        cNombreProducto: nombre,
        iCantidad: cantidad,
        nPrecioVenta: precio,
        nDescuento: 0,
        nSubTotal: subtotal
    });

    $("#cboProducto").val("");
    $("#txtCantidadVenta").val("1");
    $("#txtPrecioVenta").val("0.00");

    actualizarSubtotalLineaVenta();
    dibujarDetalleVenta();
}


// ==================================================
// QUITAR PRODUCTO
// ==================================================

function quitarProductoVenta(index) {
    index = Number(index);

    if (
        !Number.isInteger(index) ||
        index < 0 ||
        index >= ventaDetalles.length
    ) {
        return;
    }

    ventaDetalles.splice(index, 1);
    dibujarDetalleVenta();
}


// ==================================================
// DIBUJAR DETALLE
// ==================================================

function dibujarDetalleVenta() {
    var html = "";

    if (ventaDetalles.length === 0) {
        html = "<tr><td colspan='6' class='ventas-vacio'>" +
            "Agrega productos para registrar la venta." +
            "</td></tr>";
    } else {
        ventaDetalles.forEach(function(detalle, index) {
            var cantidad = Number(detalle.iCantidad) || 0;
            var precio = Number(detalle.nPrecioVenta) || 0;
            var descuento = Number(detalle.nDescuento) || 0;

            var subtotal = Math.round(
                (cantidad * precio - descuento) * 100
            ) / 100;

            detalle.nSubTotal = subtotal;

            html += "<tr>";
            html += "<td>" + escaparHtml(detalle.cNombreProducto) + "</td>";
            html += "<td>" + cantidad + "</td>";
            html += "<td>" + formatoMoneda(precio) + "</td>";
            html += "<td>" + formatoMoneda(descuento) + "</td>";
            html += "<td>" + formatoMoneda(subtotal) + "</td>";
            html += "<td><button type='button' class='btn btn-sm btn-danger' " +
                "onclick='quitarProductoVenta(" + index + ")'>Quitar</button></td>";
            html += "</tr>";
        });
    }

    $("#bodyDetalleVenta").html(html);
    recalcTotalesVenta();
}


// ==================================================
// RECALCULAR TOTALES
// ==================================================


function recalcTotalesVenta() {
    // El precio del producto YA incluye el IGV.
    // No sumar el IGV nuevamente al total.

    var totalConIGV = ventaDetalles.reduce(function(acumulado, detalle) {
        var cantidad = Number(detalle.iCantidad) || 0;
        var precio = Number(detalle.nPrecioVenta) || 0;
        var descuento = Number(detalle.nDescuento) || 0;

        var importe = Math.max(0, cantidad * precio - descuento);

        return acumulado + importe;
    }, 0);

    totalConIGV = Math.round((totalConIGV + Number.EPSILON) * 100) / 100;

    // Separar el IGV que ya está incluido en el importe final.
    var igvIncluido = Math.round(
        (totalConIGV * 18 / 118 + Number.EPSILON) * 100
    ) / 100;

    var subtotalSinIGV = Math.round(
        (totalConIGV - igvIncluido + Number.EPSILON) * 100
    ) / 100;

    $("#txtSubtotalVentaGral").text(formatoMoneda(subtotalSinIGV));
    $("#txtIGVVenta").text(formatoMoneda(igvIncluido));
    $("#txtTotalVenta").text(formatoMoneda(totalConIGV));

    $("#txtSubTotal").val(subtotalSinIGV.toFixed(2));
    $("#txtIgv").val(igvIncluido.toFixed(2));
    $("#txtTotal").val(totalConIGV.toFixed(2));
}


// ==================================================
// CARGAR USUARIOS
// ==================================================

function listarUsuariosCombo() {
    return $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ListarUsuariosCombo",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function(response) {
            var opciones = obtenerRespuesta(response) || [];
            var html = "<option value=''>-- Seleccionar usuario --</option>";

            opciones.forEach(function(usuario) {
                html += "<option value='" +
                    escaparHtml(usuario.v) + "'>" +
                    escaparHtml(usuario.t) +
                    "</option>";
            });

            $("#cboUsuario").html(html);
            aplicarVistaVendedor();
        },

        error: function(error) {
            mostrarErrorAjax("No se pudieron cargar los usuarios.", error);
        }
    });
}


// ==================================================
// LISTAR VENTAS
// ==================================================

function listarVentas() {
    var cuerpo = $("#bodyVentas");

    if (!cuerpo.length) return;

    cuerpo.html(
        "<tr><td colspan='8' class='ventas-vacio'>Cargando ventas...</td></tr>"
    );

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ListarVentas",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function(response) {
            var ventas = obtenerRespuesta(response) || "";
            var html = "";

            if (!Array.isArray(ventas) || ventas.length === 0) {
                cuerpo.html(
                    "<tr><td colspan='8' class='ventas-vacio'>" +
                    "Todavía no hay ventas registradas.</td></tr>"
                );

                filtrarVentasRegistradas();
                return;
            }

            ventas.forEach(function(venta) {
                var id = Number(venta.iCodVenta) || 0;
                var estado = String(
                    venta.cEstado || "REGISTRADA"
                ).toUpperCase();

                var claseEstado =
                    estado === "ANULADA" ? "anulada" : "registrada";

                var numero = venta.cNumeroComprobante || "";
                var tipo = venta.cTipoComprobante || "";
                var usuario = venta.cNombreUsuario ||
                    venta.NombreUsuario || "";

                var total = Number(venta.nTotal) || 0;

                html += "<tr data-id-venta='" + id + "'>";
                html += "<td>" + id + "</td>";
                html += "<td>" + formatearFecha(venta.dFechaVenta) + "</td>";
                html += "<td>" + escaparHtml(tipo) + "</td>";
                html += "<td>" + escaparHtml(numero) + "</td>";
                html += "<td>" + escaparHtml(usuario) + "</td>";
                html += "<td>" + formatoMoneda(total) + "</td>";

                html += "<td><span class='venta-estado " + claseEstado + "'>" +
                    escaparHtml(estado) + "</span></td>";

                html += "<td><div class='venta-acciones-fila'>";

                html += "<a class='btn btn-sm btn-primary' " +
                    "target='_blank' rel='noopener' href='Comprobante.aspx?id=" +
                    encodeURIComponent(id) + "'>Ver comprobante</a>";

                if (esAdministradorVentas() && estado === "REGISTRADA") {
                    html += "<button type='button' class='btn btn-sm btn-warning' " +
                        "data-editar-venta='" + id + "'>Editar</button>";

                    html += "<button type='button' class='btn btn-sm btn-danger' " +
                        "data-anular-venta='" + id + "'>Anular</button>";
                }

                html += "</div></td></tr>";
            });

            cuerpo.html(html);
            ventaSeleccionadaId = 0;

            filtrarVentasRegistradas();
        },

        error: function(error) {
            cuerpo.html(
                "<tr><td colspan='8' class='ventas-vacio'>" +
                "No se pudieron cargar las ventas. Pulsa Actualizar para reintentar." +
                "</td></tr>"
            );

            mostrarErrorAjax("Error al listar las ventas.", error);
        }
    });
}


// ==================================================
// BUSCAR Y FILTRAR VENTAS
// ==================================================

function normalizarBusquedaVenta(texto) {
    return String(texto || "")
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase();
}

function filtrarVentasRegistradas() {
    var termino = normalizarBusquedaVenta(
        $("#buscarVentas").val()
    ).trim();

    var estadoFiltro = String(
        $("#estadoBusquedaVentas").val() || ""
    ).trim().toUpperCase();

    var filas = $("#bodyVentas tr[data-id-venta]");
    var visibles = 0;

    filas.each(function() {
        var fila = $(this);
        var texto = normalizarBusquedaVenta(fila.text());

        var estado = String(
            fila.find(".venta-estado").text() || ""
        ).trim().toUpperCase();

        var coincideTexto =
            !termino || texto.indexOf(termino) !== -1;

        var coincideEstado =
            !estadoFiltro || estado === estadoFiltro;

        var visible = coincideTexto && coincideEstado;

        fila.toggle(visible);

        if (visible) visibles++;
    });

    // No modificar el texto de las opciones del selector.
    var contador = $("#contadorVentas");

    if (contador.length) {
        contador.text(
            visibles + (visibles === 1
                ? " venta encontrada"
                : " ventas encontradas")
        );
    }
}


// ==================================================
// ABRIR MODAL: NUEVA VENTA O EDICIÓN
// ==================================================

function abrirModalVenta(modo, idVenta) {
    var editar = String(modo || "nuevo").toLowerCase() === "editar";
    var $modal = $("#modalVenta");

    if (!$modal.length) {
        console.error("No se encontró #modalVenta en Ventas.aspx.");
        alert("No se encontró la ventana de ventas. Verifica que el modal tenga id=\"modalVenta\".");
        return;
    }

    if (editar && !esAdministradorVentas()) {
        alert("Solo el administrador puede modificar ventas.");
        return;
    }

    if (editar) {
        idVenta = Number(idVenta || ventaSeleccionadaId) || 0;

        if (!idVenta) {
            idVenta = Number(
                $("#bodyVentas tr.venta-seleccionada").attr("data-id-venta")
            ) || 0;
        }

        if (!idVenta) {
            alert("Selecciona una venta de la tabla para editar.");
            return;
        }
    }

    // Mostrar primero el modal para evitar que se quede oculto por estilos o errores
    // que ocurran al limpiar o preparar los campos.
    $modal.prop("hidden", false).removeAttr("hidden");
    $modal.css({
        display: "flex",
        visibility: "visible",
        opacity: "1",
        zIndex: "99999"
    });
    $("body").css("overflow", "hidden");

    try {
        limpiarFormulario();

        if (!editar) {
            $("#tituloModalVenta").text("Registrar venta");
            $("#btnGuardarVentaModal")
                .prop("hidden", false)
                .prop("disabled", false)
                .show();
            $("#btnModificarVentaModal").prop("hidden", true).hide();

            $("#cboTipoComprobante").val("BOLETA").prop("disabled", false);
            $("#cboEstado").val("REGISTRADA").prop("disabled", !esAdministradorVentas());
            $("#txtCantidadVenta").val("1");
            $("#txtPrecioVenta").val("0.00");

            actualizarSubtotalLineaVenta();
            dibujarDetalleVenta();
            aplicarVistaVendedor();

            // Para Vendedor, seleccionar su usuario si el combo ya tiene opciones.
            if (!esAdministradorVentas()) {
                var $usuario = $("#cboUsuario");
                if ($usuario.find("option").length > 1 && !$usuario.val()) {
                    $usuario.val($usuario.find("option").eq(1).val());
                }
            }

            return;
        }

        $("#tituloModalVenta").text("Editar venta");
        $("#btnGuardarVentaModal").prop("hidden", true).hide();
        $("#btnModificarVentaModal")
            .prop("hidden", false)
            .prop("disabled", false)
            .show();
        $("#cboTipoComprobante").prop("disabled", true);
        $("#cboEstado").val("REGISTRADA").prop("disabled", true);

        seleccionarVenta(idVenta);
    } catch (error) {
        console.error("Error al preparar el formulario de venta:", error);
        alert("La ventana se abrió, pero hubo un error al preparar el formulario. Revisa la consola del navegador.");
    }
}

// ==================================================
// CERRAR MODAL
// ==================================================

function cerrarModalVenta() {
    $("#modalVenta")
        .prop("hidden", true)
        .attr("hidden", "hidden")
        .css({
            display: "none",
            visibility: "",
            opacity: "",
            zIndex: ""
        });

    $("body").css("overflow", "");
}

// ==================================================
// ACTUALIZAR URL
// ==================================================

function actualizarUrlVentas(vista) {
    try {
        var url = new URL(window.location.href);

        url.searchParams.delete("id");

        if (vista === "registros") {
            url.searchParams.set("vista", "registros");
        } else {
            url.searchParams.delete("vista");
        }

        history.pushState(null, "", url.href);
    } catch (error) {
        console.warn("No se pudo actualizar la URL de ventas.", error);
    }
}


// ==================================================
// ANULAR VENTA SELECCIONADA
// ==================================================

function anularVentaSeleccionada() {
    if (!esAdministradorVentas()) {
        alert("Solo el administrador puede anular ventas.");
        return;
    }

    var id = Number(ventaSeleccionadaId) || 0;

    if (!id) {
        id = Number(
            $("#bodyVentas tr.venta-seleccionada").attr("data-id-venta")
        ) || 0;
    }

    if (!id) {
        alert("Selecciona una venta de la tabla antes de anularla.");
        return;
    }

    eliminarVenta(id);
}


// ==================================================
// REGISTRAR VENTA
// ==================================================

function guardarVenta() {
    if (solicitudVentaEnCurso) return;

    if (ventaDetalles.length === 0) {
        alert("Agrega al menos un producto a la venta.");
        return;
    }

    var idUsuario = parseInt($("#cboUsuario").val() || "0", 10);

    if (!idUsuario) {
        alert("Selecciona el usuario responsable de la venta.");
        return;
    }

    var venta = {
        iCodVenta: 0,
        iCodUsuario: idUsuario,
        cTipoComprobante: $("#cboTipoComprobante").val(),
        cDocumentoCliente: $("#txtDocumentoCliente").val(),
        cNumeroCelular: $("#txtNumeroCelular").val(),
        cNumeroComprobante: $("#txtNumeroComprobante").val(),
        nSubTotal: parseFloat($("#txtSubTotal").val()) || 0,
        nIgv: parseFloat($("#txtIgv").val()) || 0,
        nTotal: parseFloat($("#txtTotal").val()) || 0,
        cMetodoPago: $("#cboMetodoPago").val(),
        cObservacion: $("#txtObservacion").val(),
        cEstado: "REGISTRADA"
    };

    var detalles = ventaDetalles.map(function(detalle) {
        return {
            iCodProducto: Number(detalle.iCodProducto),
            iCantidad: Number(detalle.iCantidad),
            nPrecioVenta: Number(detalle.nPrecioVenta),
            nDescuento: Number(detalle.nDescuento) || 0,
            nSubTotal: Number(detalle.nSubTotal) || 0
        };
    });

    solicitudVentaEnCurso = true;
    $("#btnGuardarVentaModal").prop("disabled", true);

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/GuardarVenta",
        data: JSON.stringify({
            venta: venta,
            detalles: detalles
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function(response) {
            var resultado = obtenerRespuesta(response);

            if (resultado === "OK") {
                alert("Venta registrada correctamente.");

<<<<<<< Updated upstream

                limpiarFormulario();
                listarVentas();
            } else {
                alert(response.d);
=======
                cerrarModalVenta();
                limpiarFormulario();
                listarVentas();
            } else {
                alert(
                    resultado ||
                    "El servidor no confirmó el registro de la venta."
                );
>>>>>>> Stashed changes
            }
        },
<<<<<<< Updated upstream
        error: function (error) {
            console.log(error);
            alert('Error al registrar la venta.');
        }
    });
}
//==================================================
function seleccionarVenta(idVenta) {
    // CAMBIO (paso 5)
    if (!esAdministradorVentas()) {
        alert('Esta opción está disponible solo para el administrador.');
=======

        error: function(error) {
            mostrarErrorAjax("No se pudo registrar la venta.", error);
        },

        complete: function() {
            solicitudVentaEnCurso = false;
            $("#btnGuardarVentaModal").prop("disabled", false);
        }
    });
}


// ==================================================
// OBTENER VENTA Y DETALLES
// ==================================================

function seleccionarVenta(idVenta) {
    if (!esAdministradorVentas()) {
        alert("Solo el administrador puede modificar ventas.");
        return;
    }

    idVenta = Number(idVenta);

    if (!idVenta || idVenta <= 0) {
        alert("El código de venta no es válido.");
>>>>>>> Stashed changes
        return;
    }
    mostrarVistaVentas("registro", true);
    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ObtenerVenta",
<<<<<<< Updated upstream
        data: JSON.stringify({idVenta: idVenta}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
=======
        data: JSON.stringify({ idVenta: idVenta }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function(response) {
            var venta = obtenerRespuesta(response);
>>>>>>> Stashed changes

            if (!venta || !venta.iCodVenta) {
                alert("No se encontró la venta seleccionada.");
                cerrarModalVenta();
                return;
            }

            if (
                String(venta.cEstado || "").toUpperCase() !== "REGISTRADA"
            ) {
                alert("Solo se pueden modificar ventas registradas.");
                cerrarModalVenta();
                return;
            }

            $("#txtIdVenta").val(venta.iCodVenta);
            $("#cboUsuario").val(venta.iCodUsuario);

            $("#cboTipoComprobante")
                .val(venta.cTipoComprobante)
                .prop("disabled", true);

            $("#txtDocumentoCliente").val(venta.cDocumentoCliente || "");
            $("#txtNumeroCelular").val(venta.cNumeroCelular || "");
            $("#txtNumeroComprobante").val(venta.cNumeroComprobante || "");
            $("#cboMetodoPago").val(venta.cMetodoPago || "EFECTIVO");
            $("#txtObservacion").val(venta.cObservacion || "");
            $("#cboEstado").val("REGISTRADA").prop("disabled", true);

            $.ajax({
                type: "POST",
                url: "/Views/Operaciones/Ventas/Ventas.aspx/ObtenerDetallesVenta",
                data: JSON.stringify({ idVenta: idVenta }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function(respuestaDetalles) {
                    var detalles = obtenerRespuesta(respuestaDetalles) || [];

                    ventaDetalles = detalles.map(function(detalle) {
                        var cantidad = Number(detalle.iCantidad) || 0;
                        var precio = Number(detalle.nPrecioVenta) || 0;
                        var descuento = Number(detalle.nDescuento) || 0;

                        return {
                            iCodProducto: Number(detalle.iCodProducto),
                            cNombreProducto: detalle.cNombreProducto ||
                                detalle.cNombre ||
                                ("Producto " + detalle.iCodProducto),
                            iCantidad: cantidad,
                            nPrecioVenta: precio,
                            nDescuento: descuento,
                            nSubTotal: Math.round(
                                (cantidad * precio - descuento) * 100
                            ) / 100
                        };
                    });

                    dibujarDetalleVenta();
                    aplicarVistaVendedor();
                },

                error: function(error) {
                    mostrarErrorAjax(
                        "No se pudieron cargar los productos de la venta.",
                        error
                    );
                }
            });
        },

        error: function(error) {
            mostrarErrorAjax("No se pudo obtener la venta.", error);
        }
    });
}


// ==================================================
// MODIFICAR VENTA
// ==================================================

function modificarVenta() {
    if (!esAdministradorVentas()) {
<<<<<<< Updated upstream
        alert("Esta opciÃ³n estÃ¡ disponible solo para el administrador.");
=======
        alert("Solo el administrador puede modificar ventas.");
>>>>>>> Stashed changes
        return;
    }

    if (solicitudVentaEnCurso) return;

    var idVenta = parseInt($("#txtIdVenta").val() || "0", 10);
    var idUsuario = parseInt($("#cboUsuario").val() || "0", 10);

    if (idVenta <= 0) {
        alert("Selecciona una venta válida.");
        return;
    }

    if (!idUsuario) {
        alert("Selecciona el usuario responsable.");
        return;
    }

    if (ventaDetalles.length === 0) {
        alert("La venta debe contener al menos un producto.");
        return;
    }

    var venta = {
        iCodVenta: idVenta,
        iCodUsuario: idUsuario,
        cTipoComprobante: $("#cboTipoComprobante").val(),
        cDocumentoCliente: $("#txtDocumentoCliente").val(),
        cNumeroCelular: $("#txtNumeroCelular").val(),
        cNumeroComprobante: $("#txtNumeroComprobante").val(),
        nSubTotal: parseFloat($("#txtSubTotal").val()) || 0,
        nIgv: parseFloat($("#txtIgv").val()) || 0,
        nTotal: parseFloat($("#txtTotal").val()) || 0,
        cMetodoPago: $("#cboMetodoPago").val(),
        cObservacion: $("#txtObservacion").val(),
        cEstado: "REGISTRADA"
    };

    var detalles = ventaDetalles.map(function(detalle) {
        return {
            iCodProducto: Number(detalle.iCodProducto),
            iCantidad: Number(detalle.iCantidad),
            nPrecioVenta: Number(detalle.nPrecioVenta),
            nDescuento: Number(detalle.nDescuento) || 0,
            nSubTotal: Number(detalle.nSubTotal) || 0
        };
    });

    solicitudVentaEnCurso = true;
    $("#btnModificarVentaModal").prop("disabled", true);

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/ModificarVenta",
        data: JSON.stringify({
            venta: venta,
            detalles: detalles
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function(response) {
            var resultado = obtenerRespuesta(response);

            if (resultado === "OK") {
                alert("Venta modificada correctamente.");
                cerrarModalVenta();
                limpiarFormulario();
                listarVentas();
            } else {
                alert(
                    resultado ||
                    "El servidor no confirmó la modificación."
                );
            }
        },

        error: function(error) {
            mostrarErrorAjax("No se pudo modificar la venta.", error);
        },

        complete: function() {
            solicitudVentaEnCurso = false;
            $("#btnModificarVentaModal").prop("disabled", false);
        }
    });
}


// ==================================================
// ANULAR VENTA
// ==================================================

function eliminarVenta(idVenta) {
    if (!esAdministradorVentas()) {
<<<<<<< Updated upstream
        alert("Esta opciÃ³n estÃ¡ disponible solo para el administrador.");
        return;
    }

    var confirmar = confirm(
        "Â¿Deseas anular esta venta?"
    );

    if (!confirmar) {
=======
        alert("Solo el administrador puede anular ventas.");
        return;
    }

    idVenta = Number(idVenta);
>>>>>>> Stashed changes

    if (!idVenta || idVenta <= 0) {
        alert("El código de venta no es válido.");
        return;
    }

    if (!confirm("¿Deseas anular la venta N.º " + idVenta + "?")) {
        return;
    }

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Ventas/Ventas.aspx/EliminarVenta",
        data: JSON.stringify({ idVenta: idVenta }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function(response) {
            var resultado = obtenerRespuesta(response);

            if (resultado === "OK") {
                alert("Venta anulada correctamente.");

                if (Number($("#txtIdVenta").val()) === idVenta) {
                    cerrarModalVenta();
                    limpiarFormulario();
                }

                ventaSeleccionadaId = 0;
                listarVentas();
            } else {
                alert(resultado || "El servidor no confirmó la anulación.");
            }
        },

        error: function(error) {
            mostrarErrorAjax("No se pudo anular la venta.", error);
        }
    });
}


// ==================================================
// LIMPIAR FORMULARIO
// ==================================================

function limpiarFormulario() {
    $("#txtIdVenta").val("");
    $("#cboUsuario").val("");

    $("#cboTipoComprobante")
        .val("BOLETA")
        .prop("disabled", false);

    $("#txtDocumentoCliente").val("");
    $("#txtNumeroCelular").val("");
    $("#txtNumeroComprobante").val("");

    $("#cboMetodoPago").val("EFECTIVO");
    $("#txtObservacion").val("");

    $("#cboEstado")
        .val("REGISTRADA")
        .prop("disabled", false);

    $("#cboProducto").val("");
    $("#txtCantidadVenta").val("1");
    $("#txtPrecioVenta").val("0.00");
    $("#txtSubtotalLineaVenta").val(formatoMoneda(0));

    ventaDetalles = [];
    dibujarDetalleVenta();

    $("#btnGuardarVentaModal").prop("hidden", false);
    $("#btnModificarVentaModal").prop("hidden", true);

<<<<<<< Updated upstream
}

function normalizarBusquedaVenta(texto) {
    return String(texto || "").normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
}

function filtrarVentasRegistradas() {
    var terminos = normalizarBusquedaVenta($("#buscarVentas").val()).trim().split(/\s+/).filter(Boolean);
    var visibles = 0;
    var filas = $("#bodyVentas tr");
    filas.each(function () {
        var texto = normalizarBusquedaVenta($(this).children("td").not(":last").text());
        var coincide = terminos.every(function (termino) { return texto.indexOf(termino) !== -1; });
        $(this).toggle(coincide);
        if (coincide) visibles++;
    });
    $("#estadoBusquedaVentas").text(filas.length === 0 ? "TodavÃ­a no hay ventas registradas." :
        visibles === 0 ? "No se encontraron ventas para esta bÃºsqueda." : visibles + " de " + filas.length + " ventas");
}
function vistaVentasDesdeUrl() {
    return new URLSearchParams(window.location.search).get("vista") === "registros" ? "registros" : "registro";
}

function mostrarVistaVentas(vista, actualizarUrl) {
    var registros = vista === "registros";
    $("#panelRegistroVenta").prop("hidden", registros);
    $("#panelVentasRegistradas").prop("hidden", !registros);
    $("[data-vista-ventas]").each(function () {
        var activo = $(this).attr("data-vista-ventas") === vista;
        if (this.tagName === "BUTTON") {
            $(this).toggleClass("btn-primary", activo).toggleClass("btn-secondary", !activo).attr("aria-pressed", String(activo));
        } else {
            $(this).toggleClass("active", activo);
            if (activo) $(this).attr("aria-current", "page");

            else $(this).removeAttr("aria-current");
        }
    });
    $("#menuVentas").prop("open", true);
    if (actualizarUrl) {
        var url = new URL(window.location.href);
        url.searchParams.delete("id");
        if (registros) url.searchParams.set("vista", "registros");
        else url.searchParams.delete("vista");
        if (url.href !== window.location.href) history.pushState(null, "", url);
    }
    if (registros) listarVentas();
}

//==================================================
// LEER EL ARCHIVO SELECCIONADO COMO BASE64
// Usa FileReader.readAsDataURL y devuelve unicamente
// la parte base64 que sigue a la coma.
//==================================================

function leerArchivoBase64(input, callback) {

    if (!input || !input.files || input.files.length === 0) {

        callback(null);

        return;

    }

    var lector = new FileReader();

    lector.onload = function (evento) {

        var resultado = String(evento.target.result || "");

        var posicion = resultado.indexOf(",");

        callback(posicion >= 0 ? resultado.substring(posicion + 1) : resultado);

    };

    lector.onerror = function () {

        callback(null);

    };

    lector.readAsDataURL(input.files[0]);

=======
    aplicarVistaVendedor();
>>>>>>> Stashed changes
}
