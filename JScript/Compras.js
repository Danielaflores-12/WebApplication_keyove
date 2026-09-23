$(document).ready(function () {

    listarProveedoresCombo();
    listarUsuariosCombo();
    listarProductosCombo();
    $("#buscarCompras").on("input", filtrarComprasRegistradas);
    $(document).on("click", "[data-vista-compras]", function (evento) {
        if (evento.ctrlKey || evento.metaKey || evento.shiftKey || evento.altKey) return;
        evento.preventDefault();
        mostrarVistaCompras($(this).attr("data-vista-compras"), true);
        if (window.matchMedia("(max-width: 768px)").matches) $("#fondoMenu").trigger("click");
    });
    $(window).on("popstate", function () { mostrarVistaCompras(vistaComprasDesdeUrl(), false); });
    mostrarVistaCompras(vistaComprasDesdeUrl(), false);

    $("#txtCantidadCompra, #txtPrecioCompra").on("input", function () {
        actualizarSubtotalLineaCompra();
    });

});


//==================================================
// CONSTANTES Y ESTADO DE LA COMPRA
//==================================================

var cIGV = 0.18;

var compraDetalles = [];


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
        url: "/Views/Operaciones/Compras/Compras.aspx/ListarProductosCombo",
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

function actualizarSubtotalLineaCompra() {

    var cantidad = parseFloat($("#txtCantidadCompra").val()) || 0;
    var precio = parseFloat($("#txtPrecioCompra").val()) || 0;

    var subtotal = Math.round(cantidad * precio * 100) / 100;

    $("#txtSubtotalLinea").val(formatoMoneda(subtotal));

}


//==================================================
// AGREGAR PRODUCTO AL DETALLE
//==================================================

function agregarProductoCompra() {

    var idProducto = parseInt($("#cboProducto").val() || 0, 10);
    var cantidad = parseInt($("#txtCantidadCompra").val() || 0, 10);
    var precio = parseFloat($("#txtPrecioCompra").val() || 0);

    if (!idProducto) {

        alert("Selecciona un producto.");

        return;

    }

    if (cantidad <= 0) {

        alert("La cantidad debe ser mayor a 0.");

        return;

    }

    if (precio <= 0) {

        alert("El precio de compra debe ser mayor a 0.");

        return;

    }

    var nombreProducto =
        $("#cboProducto option:selected").text();

    var subtotal = Math.round(precio * cantidad * 100) / 100;

    compraDetalles.push({
        iCodProducto: idProducto,
        cNombreProducto: nombreProducto,
        iCantidad: cantidad,
        nPrecioCompra: precio,
        nSubTotal: subtotal
    });

    $("#cboProducto").val("");
    $("#txtCantidadCompra").val("");
    $("#txtPrecioCompra").val("");
    $("#txtSubtotalLinea").val("");

    dibujarDetalleCompra();

}


//==================================================
// QUITAR PRODUCTO DEL DETALLE
//==================================================

function quitarProductoCompra(index) {

    if (index < 0 || index >= compraDetalles.length) {

        return;

    }

    compraDetalles.splice(index, 1);

    dibujarDetalleCompra();

}


//==================================================
// DIBUJAR DETALLE DE COMPRA
//==================================================

function dibujarDetalleCompra() {

    var filas = "";

    for (var i = 0; i < compraDetalles.length; i++) {

        filas += "<tr>";

        filas += "<td>" + compraDetalles[i].cNombreProducto + "</td>";
        filas += "<td>" + compraDetalles[i].iCantidad + "</td>";
        filas += "<td>" + formatoMoneda(compraDetalles[i].nPrecioCompra) + "</td>";
        filas += "<td>" + formatoMoneda(compraDetalles[i].nSubTotal) + "</td>";

        filas += "<td><div class='table-actions'>";

        filas += "<button type='button' class='btn btn-sm btn-danger' onclick='quitarProductoCompra(" +
            i +
            ")'>Quitar</button>";

        filas += "</div></td>";

        filas += "</tr>";

    }

    $("#bodyDetalleCompra").html(filas);

    recalcTotalesCompra();

}


//==================================================
// RECALCULAR TOTALES: SUBTOTAL, IGV 18% Y TOTAL
//==================================================

function recalcTotalesCompra() {

    var subtotal = 0;

    for (var i = 0; i < compraDetalles.length; i++) {

        subtotal += compraDetalles[i].nSubTotal;

    }

    subtotal = Math.round(subtotal * 100) / 100;

    var igv = Math.round(subtotal * cIGV * 100) / 100;
    var total = Math.round((subtotal + igv) * 100) / 100;

    $("#txtSubtotalCompraGral").val(formatoMoneda(subtotal));
    $("#txtIGVCompra").val(formatoMoneda(igv));
    $("#txtTotalCompra").val(formatoMoneda(total));

    $("#txtSubTotal").val(subtotal.toFixed(2));
    $("#txtIgv").val(igv.toFixed(2));
    $("#txtTotal").val(total.toFixed(2));

}


//==================================================
// LISTAR PROVEEDORES PARA COMBOBOX
//==================================================

function listarProveedoresCombo() {

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Compras/Compras.aspx/ListarProveedoresCombo",
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
        url: "/Views/Operaciones/Compras/Compras.aspx/ListarUsuariosCombo",
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
    $("#estadoBusquedaCompras").text("Cargando compras...");

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Compras/Compras.aspx/ListarCompras",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var compras = response.d;

            var filas = "";

            for (var i = 0; i < compras.length; i++) {

                var estado =
                    compras[i].cEstado === "ANULADA"
                        ? "<span class='badge badge-inactive'>" +
                            compras[i].cEstado +
                            "</span>"
                        : "<span class='badge badge-active'>" +
                            compras[i].cEstado +
                            "</span>";

                filas += "<tr>";

                filas += "<td>" + compras[i].iCodCompra + "</td>";
                filas += "<td>" + $("<span>").text(compras[i].cNombreProveedor || "Sin proveedor").html() + "</td>";
                filas += "<td>" + $("<span>").text(compras[i].cNombreUsuario || "Sin usuario").html() + "</td>";
                filas += "<td>" + compras[i].cTipoComprobante + "</td>";
                filas += "<td>" + compras[i].cNumeroComprobante + "</td>";
                filas += "<td>" + formatearFecha(compras[i].dFechaCompra) + "</td>";
                filas += "<td>" + compras[i].nSubTotal + "</td>";
                filas += "<td>" + compras[i].nIgv + "</td>";
                filas += "<td>" + compras[i].nTotal + "</td>";
                filas += "<td>" + estado + "</td>";

                filas += "<td><div class='table-actions'>";

                filas += "<button type='button' class='btn btn-sm btn-warning' onclick='seleccionarCompra(" +
                    compras[i].iCodCompra +
                    ")'>Editar</button>";

                filas += "<button type='button' class='btn btn-sm btn-danger' onclick='eliminarCompra(" +
                    compras[i].iCodCompra +
                    ")'>Anular</button>";

                filas += "</div></td>";

                filas += "</tr>";

            }

            $("#bodyCompras").html(filas);
            filtrarComprasRegistradas();

        },

        error: function (error) {

            $("#bodyCompras").empty();
            $("#estadoBusquedaCompras").text("No se pudieron cargar las compras. Pulsa Actualizar lista para reintentar.");
            console.log(error);

        }
    });

}


//==================================================
// GUARDAR COMPRA
//==================================================

function guardarCompra() {

    if (compraDetalles.length === 0) {

        alert("Agrega al menos un producto a la compra.");

        return;

    }

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

    var detalles = [];

    for (var i = 0; i < compraDetalles.length; i++) {

        detalles.push({
            iCodProducto: compraDetalles[i].iCodProducto,
            iCantidad: compraDetalles[i].iCantidad,
            nPrecioCompra: compraDetalles[i].nPrecioCompra,
            nSubTotal: compraDetalles[i].nSubTotal
        });

    }

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Compras/Compras.aspx/GuardarCompra",
        data: JSON.stringify({
            compra: compra,
            detalles: detalles
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
    mostrarVistaCompras("registro", true);

    $.ajax({
        type: "POST",
        url: "/Views/Operaciones/Compras/Compras.aspx/ObtenerCompra",
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
            $("#cboTipoComprobante").val(compra.cTipoComprobante).prop("disabled", true);
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
        url: "/Views/Operaciones/Compras/Compras.aspx/ModificarCompra",
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
        url: "/Views/Operaciones/Compras/Compras.aspx/EliminarCompra",
        data: JSON.stringify({
            idCompra: idCompra
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            if (response.d === "OK") {

                alert("Compra anulada correctamente.");

                if (String($("#txtIdCompra").val()) === String(idCompra)) limpiarFormulario();
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
    $("#cboTipoComprobante").val("FACTURA").prop("disabled", false);
    $("#txtNumeroComprobante").val("");
    $("#txtSubTotal").val("");
    $("#txtIgv").val("");
    $("#txtTotal").val("");
    $("#txtObservacion").val("");
    $("#cboEstado").val("REGISTRADA");

    compraDetalles = [];
    $("#bodyDetalleCompra").html("");
    $("#cboProducto").val("");
    $("#txtCantidadCompra").val("");
    $("#txtPrecioCompra").val("");
    $("#txtSubtotalLinea").val("");
    $("#txtSubtotalCompraGral").val("");
    $("#txtIGVCompra").val("");
    $("#txtTotalCompra").val("");

}

function normalizarBusquedaCompra(texto) {
    return String(texto || "").normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
}

function filtrarComprasRegistradas() {
    var terminos = normalizarBusquedaCompra($("#buscarCompras").val()).trim().split(/\s+/).filter(Boolean);
    var visibles = 0;
    var filas = $("#bodyCompras tr");
    filas.each(function () {
        var texto = normalizarBusquedaCompra($(this).children("td").not(":last").text());
        var coincide = terminos.every(function (termino) { return texto.indexOf(termino) !== -1; });
        $(this).toggle(coincide);
        if (coincide) visibles++;
    });
    $("#estadoBusquedaCompras").text(filas.length === 0 ? "Todavía no hay compras registradas." :
        visibles === 0 ? "No se encontraron compras para esta búsqueda." : visibles + " de " + filas.length + " compras");
}
function vistaComprasDesdeUrl() {
    return new URLSearchParams(window.location.search).get("vista") === "registros" ? "registros" : "registro";
}

function mostrarVistaCompras(vista, actualizarUrl) {
    var registros = vista === "registros";
    $("#panelRegistroCompra").prop("hidden", registros);
    $("#panelComprasRegistradas").prop("hidden", !registros);
    $("[data-vista-compras]").each(function () {
        var activo = $(this).attr("data-vista-compras") === vista;
        if (this.tagName === "BUTTON") {
            $(this).toggleClass("btn-primary", activo).toggleClass("btn-secondary", !activo).attr("aria-pressed", String(activo));
        } else {
            $(this).toggleClass("active", activo);
            if (activo) $(this).attr("aria-current", "page");
            else $(this).removeAttr("aria-current");
        }
    });
    $("#menuCompras").prop("open", true);
    if (actualizarUrl) {
        var url = new URL(window.location.href);
        url.searchParams.delete("id");
        if (registros) url.searchParams.set("vista", "registros");
        else url.searchParams.delete("vista");
        if (url.href !== window.location.href) history.pushState(null, "", url);
    }
    if (registros) listarCompras();
}
