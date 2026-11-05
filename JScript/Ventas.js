"use strict";

// ==========================================
// CONFIGURACIÓN GENERAL
// ==========================================

var cIGV = 0.18;
var ventaDetalles = [];
var ventaSeleccionadaId = 0;
var solicitudVentaEnCurso = false;

var URL_VENTAS = "/Views/Operaciones/Ventas/Ventas.aspx/";

// ==========================================
// PERMISOS
// ==========================================

function esAdministradorVentas() {
    return String($("body").attr("data-rol") || "")
        .trim()
        .toLowerCase() === "administrador";
}

function aplicarVistaVendedor() {
    var administrador = esAdministradorVentas();

    $("#cboUsuario").prop("disabled", !administrador);

    $("#cboEstado")
        .val("REGISTRADA")
        .prop("disabled", !administrador);

    if (!administrador) {
        $("#btnModificarVentaModal").prop("hidden", true).hide();
    }
}

// ==========================================
// INICIALIZACIÓN
// ==========================================

$(document).ready(function () {

    $("#buscarVentas").on("input", filtrarVentasRegistradas);

    $("#estadoBusquedaVentas").on(
        "change",
        filtrarVentasRegistradas
    );

    $("#txtCantidadVenta, #txtPrecioVenta").on(
        "input",
        actualizarSubtotalLineaVenta
    );

    $("#cboProducto").on(
        "change",
        actualizarSubtotalLineaVenta
    );

    $("#btnAgregarVenta, #btnAgregarVentaModal")
        .off("click.ventas")
        .on("click.ventas", function (e) {
            e.preventDefault();

            if (!this.hasAttribute("onclick")) {
                abrirModalVenta("nuevo");
            }
        });

    $("#btnAgregarProductoVenta")
        .off("click.ventas")
        .on("click.ventas", function (e) {
            e.preventDefault();

            if (!this.hasAttribute("onclick")) {
                agregarProductoVenta();
            }
        });

    $("#cboTipoComprobante").on("change", function () {
        $("#txtNumeroComprobante").val("");
    });

    listarProductosCombo();

    var usuariosListos = listarUsuariosCombo();

    listarVentas();

    $("#bodyVentas").on(
        "click",
        "tr[data-id-venta]",
        function (e) {

            if ($(e.target).closest("a, button").length) {
                return;
            }

            $("#bodyVentas tr")
                .removeClass("venta-seleccionada");

            $(this).addClass("venta-seleccionada");

            ventaSeleccionadaId =
                Number($(this).attr("data-id-venta")) || 0;
        }
    );

    $("#bodyVentas").on(
        "click",
        "[data-editar-venta]",
        function (e) {
            e.preventDefault();
            e.stopPropagation();

            abrirModalVenta(
                "editar",
                Number($(this).attr("data-editar-venta"))
            );
        }
    );

    $("#bodyVentas").on(
        "click",
        "[data-anular-venta]",
        function (e) {
            e.preventDefault();
            e.stopPropagation();

            eliminarVenta(
                Number($(this).attr("data-anular-venta"))
            );
        }
    );

    $(document).on(
        "click",
        "[data-vista-ventas]",
        function (e) {

            if (
                e.ctrlKey ||
                e.metaKey ||
                e.shiftKey ||
                e.altKey
            ) {
                return;
            }

            e.preventDefault();

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
        }
    );

    $("#modalVenta").on("click", function (e) {
        if (e.target === this) {
            cerrarModalVenta();
        }
    });

    $(document).on("keydown", function (e) {
        if (
            e.key === "Escape" &&
            !$("#modalVenta").prop("hidden")
        ) {
            cerrarModalVenta();
        }
    });

    var parametros = new URLSearchParams(
        window.location.search
    );

    var idUrl = parametros.get("id");

    if (/^[1-9]\d*$/.test(idUrl || "")) {
        usuariosListos.done(function () {
            abrirModalVenta("editar", Number(idUrl));
        });
    }

    aplicarVistaVendedor();
});

// ==========================================
// UTILIDADES
// ==========================================

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
    if (!valor) {
        return "";
    }

    var coincidencia =
        /\/Date\((\d+)\)\//.exec(String(valor));

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
    return response &&
        typeof response.d !== "undefined"
        ? response.d
        : null;
}

function mostrarErrorAjax(titulo, error) {
    console.error(titulo, error);

    var mensaje = titulo;

    if (
        error &&
        error.responseJSON &&
        error.responseJSON.Message
    ) {
        mensaje += "\n" + error.responseJSON.Message;
    }

    alert(mensaje);
}

// ==========================================
// CARGAR PRODUCTOS
// ==========================================

function listarProductosCombo() {
    return $.ajax({
        type: "POST",
        url: URL_VENTAS + "ListarProductosCombo",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {
            var opciones = obtenerRespuesta(response) || [];

            var html =
                "<option value=''>-- Seleccionar producto --</option>";

            opciones.forEach(function (producto) {
                html +=
                    "<option value='" +
                    escaparHtml(producto.v) +
                    "'>" +
                    escaparHtml(producto.t) +
                    "</option>";
            });

            $("#cboProducto").html(html);
        },

        error: function (error) {
            mostrarErrorAjax(
                "No se pudieron cargar los productos.",
                error
            );
        }
    });
}

// ==========================================
// SUBTOTAL POR PRODUCTO
// ==========================================

function actualizarSubtotalLineaVenta() {
    var cantidad =
        parseInt($("#txtCantidadVenta").val(), 10) || 0;

    var precio =
        parseFloat($("#txtPrecioVenta").val()) || 0;

    var subtotal =
        Math.round(cantidad * precio * 100) / 100;

    $("#txtSubtotalLineaVenta").val(
        formatoMoneda(subtotal)
    );
}

// ==========================================
// AGREGAR PRODUCTO
// ==========================================

function agregarProductoVenta() {
    var idProducto =
        parseInt($("#cboProducto").val() || "0", 10);

    var cantidad =
        Number($("#txtCantidadVenta").val());

    var precio =
        Number($("#txtPrecioVenta").val());

    if (!idProducto) {
        alert("Selecciona un producto.");
        return;
    }

    if (!Number.isInteger(cantidad) || cantidad <= 0) {
        alert("La cantidad debe ser un entero mayor que cero.");
        return;
    }

    if (!Number.isFinite(precio) || precio <= 0) {
        alert("El precio debe ser mayor que cero.");
        return;
    }

    var duplicado = ventaDetalles.some(function (detalle) {
        return Number(detalle.iCodProducto) === idProducto;
    });

    if (duplicado) {
        alert("Este producto ya está agregado a la venta.");
        return;
    }

    var nombre = $("#cboProducto option:selected").text();

    var subtotal =
        Math.round(cantidad * precio * 100) / 100;

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

// ==========================================
// QUITAR PRODUCTO
// ==========================================

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

// ==========================================
// DIBUJAR DETALLE DE VENTA
// ==========================================

function dibujarDetalleVenta() {
    var html = "";

    if (ventaDetalles.length === 0) {
        html =
            "<tr>" +
            "<td colspan='6' class='ventas-vacio'>" +
            "Agrega productos para registrar la venta." +
            "</td>" +
            "</tr>";
    } else {
        ventaDetalles.forEach(function (detalle, index) {
            var cantidad = Number(detalle.iCantidad) || 0;
            var precio = Number(detalle.nPrecioVenta) || 0;
            var descuento = Number(detalle.nDescuento) || 0;

            var subtotal = Math.round(
                (cantidad * precio - descuento) * 100
            ) / 100;

            detalle.nSubTotal = subtotal;

            html += "<tr>";
            html += "<td>" +
                escaparHtml(detalle.cNombreProducto) +
                "</td>";
            html += "<td>" + cantidad + "</td>";
            html += "<td>" + formatoMoneda(precio) + "</td>";
            html += "<td>" + formatoMoneda(descuento) + "</td>";
            html += "<td>" + formatoMoneda(subtotal) + "</td>";

            html +=
                "<td><button type='button' " +
                "class='btn btn-sm btn-danger' " +
                "onclick='quitarProductoVenta(" + index + ")'>" +
                "Quitar</button></td>";

            html += "</tr>";
        });
    }

    $("#bodyDetalleVenta").html(html);

    recalcTotalesVenta();
}

// ==========================================
// RECALCULAR TOTALES - IGV INCLUIDO
// ==========================================

function recalcTotalesVenta() {
    var totalConIGV = ventaDetalles.reduce(
        function (acumulado, detalle) {
            var cantidad = Number(detalle.iCantidad) || 0;
            var precio = Number(detalle.nPrecioVenta) || 0;
            var descuento = Number(detalle.nDescuento) || 0;

            var importe = Math.max(
                0,
                cantidad * precio - descuento
            );

            return acumulado + importe;
        },
        0
    );

    totalConIGV =
        Math.round((totalConIGV + Number.EPSILON) * 100) / 100;

    // El IGV ya está incluido en el precio.
    var igvIncluido = Math.round(
        (totalConIGV * cIGV / (1 + cIGV) +
            Number.EPSILON) * 100
    ) / 100;

    var subtotalSinIGV = Math.round(
        (totalConIGV - igvIncluido +
            Number.EPSILON) * 100
    ) / 100;

    $("#txtSubtotalVentaGral").text(
        formatoMoneda(subtotalSinIGV)
    );

    $("#txtIGVVenta").text(
        formatoMoneda(igvIncluido)
    );

    $("#txtTotalVenta").text(
        formatoMoneda(totalConIGV)
    );

    $("#txtSubTotal").val(subtotalSinIGV.toFixed(2));
    $("#txtIgv").val(igvIncluido.toFixed(2));
    $("#txtTotal").val(totalConIGV.toFixed(2));
}
// ==========================================
// CARGAR USUARIOS
// ==========================================

function listarUsuariosCombo() {
    return $.ajax({
        type: "POST",
        url: URL_VENTAS + "ListarUsuariosCombo",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {
            var opciones = obtenerRespuesta(response) || [];

            var html =
                "<option value=''>-- Seleccionar usuario --</option>";

            opciones.forEach(function (usuario) {
                html +=
                    "<option value='" +
                    escaparHtml(usuario.v) +
                    "'>" +
                    escaparHtml(usuario.t) +
                    "</option>";
            });

            $("#cboUsuario").html(html);
            aplicarVistaVendedor();
        },

        error: function (error) {
            mostrarErrorAjax(
                "No se pudieron cargar los usuarios.",
                error
            );
        }
    });
}

// ==========================================
// LISTAR VENTAS
// ==========================================

function listarVentas() {
    var cuerpo = $("#bodyVentas");

    if (!cuerpo.length) {
        return;
    }

    cuerpo.html(
        "<tr><td colspan='8' class='ventas-vacio'>" +
        "Cargando ventas..." +
        "</td></tr>"
    );

    return $.ajax({
        type: "POST",
        url: URL_VENTAS + "ListarVentas",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {
            var ventas = obtenerRespuesta(response) || [];
            var html = "";

            if (!Array.isArray(ventas) || ventas.length === 0) {
                cuerpo.html(
                    "<tr><td colspan='8' class='ventas-vacio'>" +
                    "Todavía no hay ventas registradas." +
                    "</td></tr>"
                );

                filtrarVentasRegistradas();
                return;
            }

            ventas.forEach(function (venta) {
                var id = Number(venta.iCodVenta) || 0;

                var estado = String(
                    venta.cEstado || "REGISTRADA"
                ).toUpperCase();

                var claseEstado =
                    estado === "ANULADA"
                        ? "anulada"
                        : "registrada";

                var numero = venta.cNumeroComprobante || "";
                var tipo = venta.cTipoComprobante || "";

                var usuario =
                    venta.cNombreUsuario ||
                    venta.NombreUsuario ||
                    "";

                var total = Number(venta.nTotal) || 0;

                html += "<tr data-id-venta='" + id + "'>";
                html += "<td>" + id + "</td>";

                html += "<td>" +
                    formatearFecha(venta.dFechaVenta) +
                    "</td>";

                html += "<td>" + escaparHtml(tipo) + "</td>";
                html += "<td>" + escaparHtml(numero) + "</td>";
                html += "<td>" + escaparHtml(usuario) + "</td>";
                html += "<td>" + formatoMoneda(total) + "</td>";

                html +=
                    "<td><span class='venta-estado " +
                    claseEstado +
                    "'>" +
                    escaparHtml(estado) +
                    "</span></td>";

                html += "<td><div class='venta-acciones-fila'>";

                html +=
                    "<a class='btn btn-sm btn-primary' " +
                    "target='_blank' rel='noopener' " +
                    "href='Comprobante.aspx?id=" +
                    encodeURIComponent(id) +
                    "'>Ver comprobante</a>";

                if (
                    esAdministradorVentas() &&
                    estado === "REGISTRADA"
                ) {
                    html +=
                        "<button type='button' " +
                        "class='btn btn-sm btn-warning' " +
                        "data-editar-venta='" + id + "'>" +
                        "Editar</button>";

                    html +=
                        "<button type='button' " +
                        "class='btn btn-sm btn-danger' " +
                        "data-anular-venta='" + id + "'>" +
                        "Anular</button>";
                }

                html += "</div></td></tr>";
            });

            cuerpo.html(html);
            ventaSeleccionadaId = 0;

            filtrarVentasRegistradas();
        },

        error: function (error) {
            cuerpo.html(
                "<tr><td colspan='8' class='ventas-vacio'>" +
                "No se pudieron cargar las ventas." +
                "</td></tr>"
            );

            mostrarErrorAjax(
                "Error al listar las ventas.",
                error
            );
        }
    });
}

// ==========================================
// NORMALIZAR BÚSQUEDA
// ==========================================

function normalizarBusquedaVenta(texto) {
    return String(texto || "")
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase();
}

// ==========================================
// FILTRAR VENTAS
// ==========================================

function filtrarVentasRegistradas() {
    var termino = normalizarBusquedaVenta(
        $("#buscarVentas").val()
    ).trim();

    var estadoFiltro = String(
        $("#estadoBusquedaVentas").val() || ""
    ).trim().toUpperCase();

    var filas = $("#bodyVentas tr[data-id-venta]");
    var visibles = 0;

    filas.each(function () {
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

        if (visible) {
            visibles++;
        }
    });

    var contador = $("#contadorVentas");

    if (contador.length) {
        contador.text(
            visibles +
            (visibles === 1
                ? " venta encontrada"
                : " ventas encontradas")
        );
    }
}

// ==========================================
// ABRIR MODAL DE VENTA
// ==========================================

function abrirModalVenta(modo, idVenta) {
    var editar =
        String(modo || "nuevo").toLowerCase() === "editar";

    var modal = $("#modalVenta");

    if (!modal.length) {
        console.error("No existe el modal #modalVenta.");

        alert(
            "No se encontró la ventana de ventas."
        );

        return;
    }

    if (editar && !esAdministradorVentas()) {
        alert("Solo el administrador puede modificar ventas.");
        return;
    }

    if (editar) {
        idVenta = Number(
            idVenta || ventaSeleccionadaId
        ) || 0;

        if (!idVenta) {
            idVenta = Number(
                $("#bodyVentas tr.venta-seleccionada")
                    .attr("data-id-venta")
            ) || 0;
        }

        if (!idVenta) {
            alert("Selecciona una venta para editar.");
            return;
        }
    }

    modal.prop("hidden", false)
        .removeAttr("hidden")
        .css({
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

            $("#btnModificarVentaModal")
                .prop("hidden", true)
                .hide();

            $("#cboTipoComprobante")
                .val("BOLETA")
                .prop("disabled", false);

            $("#cboEstado")
                .val("REGISTRADA")
                .prop("disabled", true);

            $("#txtCantidadVenta").val("1");
            $("#txtPrecioVenta").val("0.00");

            actualizarSubtotalLineaVenta();
            dibujarDetalleVenta();
            aplicarVistaVendedor();

            return;
        }

        $("#tituloModalVenta").text("Editar venta");

        $("#btnGuardarVentaModal")
            .prop("hidden", true)
            .hide();

        $("#btnModificarVentaModal")
            .prop("hidden", false)
            .prop("disabled", false)
            .show();

        $("#cboTipoComprobante").prop("disabled", true);

        $("#cboEstado")
            .val("REGISTRADA")
            .prop("disabled", true);

        seleccionarVenta(idVenta);

    } catch (error) {
        console.error(
            "Error al preparar el formulario:",
            error
        );

        alert(
            "Ocurrió un error al abrir el formulario."
        );
    }
}

// ==========================================
// CERRAR MODAL
// ==========================================

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

// ==========================================
// ACTUALIZAR URL
// ==========================================

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
        console.warn(
            "No se pudo actualizar la URL.",
            error
        );
    }
}

// ==========================================
// ANULAR VENTA SELECCIONADA
// ==========================================

function anularVentaSeleccionada() {
    if (!esAdministradorVentas()) {
        alert("Solo el administrador puede anular ventas.");
        return;
    }

    var id = Number(ventaSeleccionadaId) || 0;

    if (!id) {
        id = Number(
            $("#bodyVentas tr.venta-seleccionada")
                .attr("data-id-venta")
        ) || 0;
    }

    if (!id) {
        alert("Selecciona una venta para anular.");
        return;
    }

    eliminarVenta(id);
}

// ==========================================
// PREPARAR DATOS DE VENTA
// ==========================================

function obtenerDatosFormularioVenta(idVenta) {
    return {
        iCodVenta: Number(idVenta) || 0,
        iCodUsuario:
            parseInt($("#cboUsuario").val() || "0", 10),

        cTipoComprobante:
            $("#cboTipoComprobante").val(),

        cDocumentoCliente:
            $("#txtDocumentoCliente").val(),

        cNumeroCelular:
            $("#txtNumeroCelular").val(),

        cNumeroComprobante:
            $("#txtNumeroComprobante").val(),

        nSubTotal:
            parseFloat($("#txtSubTotal").val()) || 0,

        nIgv:
            parseFloat($("#txtIgv").val()) || 0,

        nTotal:
            parseFloat($("#txtTotal").val()) || 0,

        cMetodoPago:
            $("#cboMetodoPago").val(),

        cObservacion:
            $("#txtObservacion").val(),

        cEstado: "REGISTRADA"
    };
}

// ==========================================
// PREPARAR DETALLES PARA EL SERVIDOR
// ==========================================

function obtenerDetallesParaGuardar() {
    return ventaDetalles.map(function (detalle) {
        return {
            iCodProducto:
                Number(detalle.iCodProducto),

            iCantidad:
                Number(detalle.iCantidad),

            nPrecioVenta:
                Number(detalle.nPrecioVenta),

            nDescuento:
                Number(detalle.nDescuento) || 0,

            nSubTotal:
                Number(detalle.nSubTotal) || 0
        };
    });
}

// ==========================================
// VALIDAR FORMULARIO
// ==========================================

function validarFormularioVenta() {
    var idUsuario =
        parseInt($("#cboUsuario").val() || "0", 10);

    if (!idUsuario) {
        alert("Selecciona el usuario responsable.");
        return false;
    }

    if (ventaDetalles.length === 0) {
        alert("Agrega al menos un producto.");
        return false;
    }

    var documento =
        String($("#txtDocumentoCliente").val() || "").trim();

    if (
        documento !== "" &&
        !/^\d{8,11}$/.test(documento)
    ) {
        alert(
            "El documento debe contener entre 8 y 11 dígitos."
        );
        return false;
    }

    var celular =
        String($("#txtNumeroCelular").val() || "").trim();

    if (
        celular !== "" &&
        !/^\d{9}$/.test(celular)
    ) {
        alert("El celular debe contener 9 dígitos.");
        return false;
    }

    return true;
}

// ==========================================
// REGISTRAR VENTA
// ==========================================

function guardarVenta() {
    if (solicitudVentaEnCurso) {
        return;
    }

    if (!validarFormularioVenta()) {
        return;
    }

    recalcTotalesVenta();

    var venta = obtenerDatosFormularioVenta(0);
    var detalles = obtenerDetallesParaGuardar();

    solicitudVentaEnCurso = true;

    $("#btnGuardarVentaModal").prop("disabled", true);

    $.ajax({
        type: "POST",
        url: URL_VENTAS + "GuardarVenta",

        data: JSON.stringify({
            venta: venta,
            detalles: detalles
        }),

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {
            var resultado = obtenerRespuesta(response);

            if (resultado === "OK") {
                alert("Venta registrada correctamente.");

                cerrarModalVenta();
                limpiarFormulario();
                listarVentas();

            } else {
                alert(
                    resultado ||
                    "El servidor no confirmó el registro."
                );
            }
        },

        error: function (error) {
            mostrarErrorAjax(
                "No se pudo registrar la venta.",
                error
            );
        },

        complete: function () {
            solicitudVentaEnCurso = false;

            $("#btnGuardarVentaModal")
                .prop("disabled", false);
        }
    });
}

// ==========================================
// SELECCIONAR VENTA PARA EDITAR
// ==========================================

function seleccionarVenta(idVenta) {
    if (!esAdministradorVentas()) {
        alert("Solo el administrador puede editar ventas.");
        return;
    }

    idVenta = Number(idVenta);

    if (!Number.isInteger(idVenta) || idVenta <= 0) {
        alert("El código de venta no es válido.");
        return;
    }

    $.ajax({
        type: "POST",
        url: URL_VENTAS + "ObtenerVenta",

        data: JSON.stringify({
            idVenta: idVenta
        }),

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {
            var venta = obtenerRespuesta(response);

            if (!venta || !venta.iCodVenta) {
                alert("No se encontró la venta.");
                cerrarModalVenta();
                return;
            }

            if (
                String(venta.cEstado || "").toUpperCase()
                !== "REGISTRADA"
            ) {
                alert(
                    "Solo se pueden modificar ventas registradas."
                );

                cerrarModalVenta();
                return;
            }

            $("#txtIdVenta").val(venta.iCodVenta);

            $("#cboUsuario").val(venta.iCodUsuario);

            $("#cboTipoComprobante")
                .val(venta.cTipoComprobante)
                .prop("disabled", true);

            $("#txtDocumentoCliente")
                .val(venta.cDocumentoCliente || "");

            $("#txtNumeroCelular")
                .val(venta.cNumeroCelular || "");

            $("#txtNumeroComprobante")
                .val(venta.cNumeroComprobante || "");

            $("#cboMetodoPago")
                .val(venta.cMetodoPago || "EFECTIVO");

            $("#txtObservacion")
                .val(venta.cObservacion || "");

            $("#cboEstado")
                .val("REGISTRADA")
                .prop("disabled", true);

            cargarDetallesVenta(idVenta);
        },

        error: function (error) {
            mostrarErrorAjax(
                "No se pudo obtener la venta.",
                error
            );
        }
    });
}

// ==========================================
// CARGAR DETALLES DE UNA VENTA
// ==========================================

function cargarDetallesVenta(idVenta) {
    return $.ajax({
        type: "POST",
        url: URL_VENTAS + "ObtenerDetallesVenta",

        data: JSON.stringify({
            idVenta: idVenta
        }),

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {
            var detalles = obtenerRespuesta(response) || [];

            if (!Array.isArray(detalles)) {
                alert("El servidor devolvió detalles inválidos.");
                return;
            }

            ventaDetalles = detalles.map(function (detalle) {
                var cantidad =
                    Number(detalle.iCantidad) || 0;

                var precio =
                    Number(detalle.nPrecioVenta) || 0;

                var descuento =
                    Number(detalle.nDescuento) || 0;

                return {
                    iCodProducto:
                        Number(detalle.iCodProducto),

                    cNombreProducto:
                        detalle.cNombreProducto ||
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

        error: function (error) {
            mostrarErrorAjax(
                "No se pudieron cargar los detalles.",
                error
            );
        }
    });
}

// ==========================================
// MODIFICAR VENTA
// ==========================================

function modificarVenta() {
    if (!esAdministradorVentas()) {
        alert("Solo el administrador puede modificar ventas.");
        return;
    }

    if (solicitudVentaEnCurso) {
        return;
    }

    var idVenta =
        parseInt($("#txtIdVenta").val() || "0", 10);

    if (!idVenta || idVenta <= 0) {
        alert("Selecciona una venta válida.");
        return;
    }

    if (!validarFormularioVenta()) {
        return;
    }

    recalcTotalesVenta();

    var venta = obtenerDatosFormularioVenta(idVenta);
    var detalles = obtenerDetallesParaGuardar();

    solicitudVentaEnCurso = true;

    $("#btnModificarVentaModal").prop("disabled", true);

    $.ajax({
        type: "POST",
        url: URL_VENTAS + "ModificarVenta",

        data: JSON.stringify({
            venta: venta,
            detalles: detalles
        }),

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {
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

        error: function (error) {
            mostrarErrorAjax(
                "No se pudo modificar la venta.",
                error
            );
        },

        complete: function () {
            solicitudVentaEnCurso = false;

            $("#btnModificarVentaModal")
                .prop("disabled", false);
        }
    });
}

// ==========================================
// ANULAR VENTA
// ==========================================

function eliminarVenta(idVenta) {
    if (!esAdministradorVentas()) {
        alert("Solo el administrador puede anular ventas.");
        return;
    }

    idVenta = Number(idVenta);

    if (!Number.isInteger(idVenta) || idVenta <= 0) {
        alert("El código de venta no es válido.");
        return;
    }

    var confirmar = confirm(
        "¿Deseas anular la venta N.º " + idVenta + "?"
    );

    if (!confirmar) {
        return;
    }

    $.ajax({
        type: "POST",
        url: URL_VENTAS + "EliminarVenta",

        data: JSON.stringify({
            idVenta: idVenta
        }),

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {
            var resultado = obtenerRespuesta(response);

            if (resultado === "OK") {
                alert("Venta anulada correctamente.");

                if (
                    Number($("#txtIdVenta").val()) === idVenta
                ) {
                    cerrarModalVenta();
                    limpiarFormulario();
                }

                ventaSeleccionadaId = 0;
                listarVentas();

            } else {
                alert(
                    resultado ||
                    "El servidor no confirmó la anulación."
                );
            }
        },

        error: function (error) {
            mostrarErrorAjax(
                "No se pudo anular la venta.",
                error
            );
        }
    });
}

// ==========================================
// LIMPIAR FORMULARIO
// ==========================================

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
        .prop("disabled", true);

    $("#cboProducto").val("");
    $("#txtCantidadVenta").val("1");
    $("#txtPrecioVenta").val("0.00");

    $("#txtSubtotalLineaVenta").val(
        formatoMoneda(0)
    );

    ventaDetalles = [];
    ventaSeleccionadaId = 0;

    dibujarDetalleVenta();

    $("#btnGuardarVentaModal")
        .prop("hidden", false);

    $("#btnModificarVentaModal")
        .prop("hidden", true);

    aplicarVistaVendedor();
}

// ==========================================
// FIN DEL ARCHIVO VENTAS.JS
// ==========================================