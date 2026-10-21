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
var catalogoProductos = [];
var catalogoProveedores = [];
var comprobanteOCRPendiente = null;
var guardandoCompra = false;
var extrayendoCompra = false;
var montosDocumento = false;


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
    return $.ajax({type: "POST", url: "/Views/Operaciones/Compras/Compras.aspx/ListarProductosCombo", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json"}).done(function (response) {
        catalogoProductos = response.d;
        var combo = $("#cboProducto").empty().append($("<option>").val("").text("-- Seleccionar --"));
        catalogoProductos.forEach(function(p) { combo.append($("<option>").val(p.v).text(p.t + " â€” stock: " + p.stock)); });
    });
}

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
    var cantidad = Number($("#txtCantidadCompra").val());
    var precio = parseFloat($("#txtPrecioCompra").val() || 0);

    if (!idProducto) {

        alert("Selecciona un producto.");

        return;

    }

    if (!Number.isInteger(cantidad) || cantidad <= 0) {

        alert("La cantidad debe ser mayor a 0.");

        return;

    }

    if (!Number.isFinite(precio) || precio < 0) {

        alert("El precio de compra debe ser mayor a 0.");

        return;

    }

    var nombreProducto =
        (catalogoProductos.find(function(p) { return Number(p.v) === idProducto; }) || {}).t || "Producto";

    var subtotal = Math.round(precio * cantidad * 100) / 100;

    $("#confirmarOCR").prop("checked", false);
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
    $("#confirmarOCR").prop("checked", false);

    dibujarDetalleCompra();

}


//==================================================
// DIBUJAR DETALLE DE COMPRA
//==================================================

function escaparCompra(texto) { return $("<span>").text(texto == null ? "" : texto).html(); }
function dibujarDetalleCompra() {
    var filas = compraDetalles.map(function(d, i) {
        var opciones = '<option value="0">Relacionar con inventario</option>';
        catalogoProductos.filter(function(p) { return Number(p.v)>0 || Number(p.v)===d.iCodProducto; }).forEach(function(p) { opciones += '<option value="' + Number(p.v) + '"' + (Number(p.v) === d.iCodProducto ? ' selected' : '') + '>' + escaparCompra(p.t + ' â€” stock: ' + p.stock) + '</option>'; });
        return '<tr><td>' + escaparCompra(d.cNombreProducto) + '<select class="input-control" aria-label="Producto de inventario" onchange="editarLineaCompra(' + i + ', \'producto\', this.value)">' + opciones + '</select></td>' +
            '<td><input class="input-control" aria-label="Cantidad" type="number" min="1" step="1" value="' + d.iCantidad + '" onchange="editarLineaCompra(' + i + ', \'cantidad\', this.value)" /></td>' +
            '<td><input class="input-control" aria-label="Precio" type="number" min="0" step="0.01" value="' + d.nPrecioCompra + '" onchange="editarLineaCompra(' + i + ', \'precio\', this.value)" /></td>' +
            '<td>' + formatoMoneda(d.nSubTotal) + '</td><td><button type="button" class="btn btn-sm btn-secondary" onclick="abrirProductoCompra(' + i + ')">Modificar</button> <button type="button" class="btn btn-sm btn-danger" onclick="quitarProductoCompra(' + i + ')">Quitar</button></td></tr>';
    }).join('');
    $("#bodyDetalleCompra").html(filas);
    recalcTotalesCompra();
}
function editarLineaCompra(index, campo, valor) {
    var d = compraDetalles[index];
    if (campo === 'producto') d.iCodProducto = Number(valor);
    if (campo === 'cantidad') d.iCantidad = Number(valor);
    if (campo === 'precio') d.nPrecioCompra = Number(valor);
    d.nSubTotal = Math.round(d.iCantidad * d.nPrecioCompra * 100) / 100;
    $("#confirmarOCR").prop("checked", false);
    dibujarDetalleCompra();
}
function ajustarMontosSinIgv() {
    if ($("#cboTipoComprobante").val() !== "TICKET") return false;
    var total = Number($("#txtTotalCompra").val() || $("#txtTotal").val());
    if (!Number.isFinite(total) || total < 0) return false;
    $("#txtSubtotalCompraGral, #txtSubTotal").val(total.toFixed(2));
    $("#txtIGVCompra, #txtIgv").val("0.00");
    $("#txtTotalCompra, #txtTotal").val(total.toFixed(2));
    return true;
}
function actualizarMontosCompra() {
    montosDocumento = true;
    if (ajustarMontosSinIgv()) {
        $("#confirmarOCR").prop("checked", false);
        return;
    }
    [["#txtSubTotal", "#txtSubtotalCompraGral"], ["#txtIgv", "#txtIGVCompra"], ["#txtTotal", "#txtTotalCompra"]].forEach(function(ids) { $(ids[0]).val($(ids[1]).val()); });
    $("#confirmarOCR").prop("checked", false);
}
function recalcTotalesCompra() {
    if (montosDocumento) return;
    var suma = compraDetalles.reduce(function(total, d) { return total + d.nSubTotal; }, 0);
    var subtotal = $("#preciosIncluyenIgv").val() === "si" ? Math.round(suma / 1.18 * 100) / 100 : Math.round(suma * 100) / 100;
    var igv = $("#preciosIncluyenIgv").val() === "si" ? Math.round((suma - subtotal) * 100) / 100 : Math.round(subtotal * cIGV * 100) / 100;
    var total = Math.round((subtotal + igv) * 100) / 100;
    $("#txtSubtotalCompraGral, #txtSubTotal").val(subtotal.toFixed(2));
    $("#txtIGVCompra, #txtIgv").val(igv.toFixed(2));
    $("#txtTotalCompra, #txtTotal").val(total.toFixed(2));
}

function listarProveedoresCombo() {
    return $.ajax({type: "POST", url: "/Views/Operaciones/Compras/Compras.aspx/ListarProveedoresCombo", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json"}).done(function(response) {
        catalogoProveedores = response.d;
        var combo = $("#cboProveedor").empty().append($("<option>").val("").text("-- Seleccionar --"));
        catalogoProveedores.forEach(function(p) { combo.append($("<option>").val(p.v).text(p.t)); });
    });
}

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

            $("#cboUsuario").html(html).val(opciones.length ? opciones[0].v : "").prop("disabled", true);

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
                filas += "<td>" + escaparCompra(compras[i].cNumeroComprobante) + "</td>";
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

                filas += "<a class='btn btn-sm btn-secondary' href='?archivoCompra=" + compras[i].iCodCompra + "'>Comprobante</a>";
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
    if (proveedorGuardando || proveedorCargando) return;
    if ($('#modalProveedorCompra')[0].open) { alert('Aplica o cancela la ediciÃ³n de la ficha del proveedor antes de guardar la compra.'); return; }
    if ((!$('#cboProveedor').val() || compraDetalles.some(function(d) { return !d.iCodProducto; })) && comprobanteOCRPendiente) { prepararAltasCompra(); return; }
    if (guardandoCompra || extrayendoCompra) return;
    if (document.getElementById("txtArchivoComprobante").files.length && !comprobanteOCRPendiente) { alert("Primero extrae los datos del archivo seleccionado."); return; }
    if ($("#txtIdCompra").val()) { alert("Esta compra ya estÃ¡ registrada. Limpia el formulario para crear otra."); return; }
    if (!compraDetalles.length || compraDetalles.some(function(d) { return !d.iCodProducto || !Number.isInteger(d.iCantidad) || d.iCantidad <= 0 || !Number.isFinite(d.nPrecioCompra) || d.nPrecioCompra < 0; })) { alert("Relaciona todos los productos y revisa cantidades y precios."); return; }
    if (new Set(compraDetalles.map(function(d) { return d.iCodProducto; })).size !== compraDetalles.length) { alert("Dos lÃ­neas estÃ¡n asociadas al mismo producto. Si son modelos distintos, elige o crea el producto correcto para cada lÃ­nea; si son idÃ©nticos, agrupa sus cantidades."); return; }
    if (!$("#cboProveedor").val() || !$("#txtNumeroComprobante").val().trim() || !$("#txtFechaCompra").val()) { alert("Completa proveedor, nÃºmero y fecha del comprobante."); return; }
    ajustarMontosSinIgv();
    var subtotal = parseFloat($("#txtSubTotal").val()), igv = parseFloat($("#txtIgv").val()), total = parseFloat($("#txtTotal").val());
    if (![subtotal, igv, total].every(Number.isFinite) || subtotal < 0 || igv < 0 || total <= 0 || Math.abs(subtotal + igv - total) > .005) { alert("Revisa los montos: subtotal mÃ¡s IGV debe ser igual al total."); return; }
    var incluidos = $("#preciosIncluyenIgv").val() === "si";
    var suma = compraDetalles.reduce(function(t,d) { return t + d.nSubTotal; }, 0);
    if (Math.abs(suma - (incluidos ? total : subtotal)) > .02) { alert("La suma de productos no coincide con el comprobante. Revisa las lÃ­neas y si los precios incluyen IGV."); return; }
    if (comprobanteOCRPendiente && !$("#confirmarOCR").prop("checked")) { alert("Revisa los datos leÃ­dos y marca la confirmaciÃ³n antes de guardar."); return; }
    var compra = { iCodCompra: 0, iCodProveedor: Number($("#cboProveedor").val()), cTipoComprobante: $("#cboTipoComprobante").val(), cNumeroComprobante: $("#txtNumeroComprobante").val().trim(), nSubTotal: subtotal, nIgv: igv, nTotal: total, cObservacion: $("#txtObservacion").val(), cEstado: "REGISTRADA" };
    guardandoCompra = true;
    bloquearCompra(true);
    var payload = { compra: compra, detalles: compraDetalles, comprobante: comprobanteOCRPendiente, fechaCompra: $("#txtFechaCompra").val(), preciosIncluyenIgv: incluidos };
    var nuevas = compraDetalles.filter(function(d) { return d.iCodProducto < 0; }).map(function(d) { return Object.assign({}, d.nuevoProducto, {temporal:d.iCodProducto}); });
    var altaProveedor = compra.iCodProveedor === -1 ? proveedorNuevoCompra : null;
    var endpoint = 'GuardarCompra';
    if (nuevas.length || altaProveedor) { endpoint='GuardarCompraConAltas'; payload.proveedorNuevo=altaProveedor; payload.productosNuevos=nuevas; }
    $.ajax({type: "POST", url: "/Views/Operaciones/Compras/Compras.aspx/"+endpoint, contentType: "application/json; charset=utf-8", dataType: "json", data: JSON.stringify(payload)})
        .done(function(response) {
            if (!response.d.ok) { alert(response.d.mensaje); return; }
            guardandoCompra = false;
            limpiarFormulario();
            $("#ocrEstado").text("Compra " + response.d.idCompra + " registrada. Inventario actualizado.");
            listarCompras(); listarProductosCombo(); listarProveedoresCombo();
        }).fail(function() { alert("No se recibiÃ³ confirmaciÃ³n. Revisa las compras realizadas antes de reintentar; se conserva el formulario."); })
        .always(function() { guardandoCompra = false; bloquearCompra(false); });
}
function bloquearCompra(bloquear) {
    $("#panelRegistroCompra input, #panelRegistroCompra select, #panelRegistroCompra button").prop("disabled", bloquear);
    $("#cboUsuario, #cboEstado").prop("disabled", true);
}

function seleccionarCompra(idCompra) {
    if (guardandoCompra || extrayendoCompra) return;
    limpiarFormulario();
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
            $("#txtFechaCompra").val(fechaCompraInput(compra.dFechaCompra));
            $("#txtSubtotalCompraGral").val(compra.nSubTotal);
            $("#txtIGVCompra").val(compra.nIgv);
            $("#txtTotalCompra").val(compra.nTotal);
            $("#panelRegistroCompra input, #panelRegistroCompra select, #panelRegistroCompra button").prop("disabled", true);
            $("#txtObservacion, #btnModificarCompra, #btnLimpiarCompra").prop("disabled", false);
            $("#ocrEstado").text("Compra registrada: puedes actualizar su observaciÃ³n o consultar sus productos en Detalle de compra.");
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
        "Â¿Deseas anular esta compra?"
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
    if (guardandoCompra || extrayendoCompra) return;
    cerrarModalCompra('modalProveedorCompra');
    cerrarModalCompra('modalAltasCompra');
    proveedorNuevoCompra = null;
    $('#cboProveedor option[value="-1"]').remove();
    catalogoProductos = catalogoProductos.filter(function(p) { return Number(p.v)>0; });
    comprobanteOCRPendiente = null;
    montosDocumento = false;
    $("#panelRegistroCompra input, #panelRegistroCompra select, #panelRegistroCompra button").prop("disabled", false);
    $("#cboUsuario, #cboEstado").prop("disabled", true);
    $("#txtArchivoComprobante").val("");
    $("#ocrEstado, #advertenciasOCR, #textoOCR").text("");
    $("#confirmarOCR").prop("checked", false);
    $("#confirmarOCRContenedor").prop("hidden", true);
    $("#revisionOCR").prop("hidden", true);
    $("#txtFechaCompra").val(fechaCompraInput(new Date()));
    $("#preciosIncluyenIgv").val("no");

    $("#txtIdCompra").val("");
    $("#cboProveedor").val("");

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
    $("#estadoBusquedaCompras").text(filas.length === 0 ? "TodavÃ­a no hay compras registradas." :
        visibles === 0 ? "No se encontraron compras para esta bÃºsqueda." : visibles + " de " + filas.length + " compras");
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


function fechaCompraInput(valor) {
    var m = /\/Date\((\d+)\)\//.exec(String(valor));
    var fecha = m ? new Date(Number(m[1])) : new Date(valor);
    if (isNaN(fecha.getTime())) return "";
    return fecha.getFullYear() + '-' + String(fecha.getMonth() + 1).padStart(2, '0') + '-' + String(fecha.getDate()).padStart(2, '0');
}
function claveProductoOCR(texto) {
    return normalizarBusquedaCompra(texto).replace(/\bal copilot\b/g, 'ai copilot').replace(/[^a-z0-9]/g, '');
}
function contieneDatoOCR(texto, dato) {
    var tokens = normalizarBusquedaCompra(dato || '').match(/[a-z0-9]+/g);
    if (!tokens) return false;
    var patron = tokens.join('[\\s-]*');
    return new RegExp('(?:^|[^a-z0-9])' + patron + '(?=$|[^a-z0-9])', 'i').test(normalizarBusquedaCompra(texto));
}
function separarProductoOCR(texto, linea) {
    linea = linea || {};
    var original = linea.texto_original || String(texto || '');
    var limpio = linea.descripcion_producto || original.replace(/\b(?:S\s*\/\s*N|SERIE|SERIAL)\s*[:#].*$/i, '').replace(/\b(?:UND|UNID|UNI|UNIDAD|PZA|PCS)\b(?:\s*X\s*\d+)?/gi, '').replace(/\s+/g, ' ').trim();
    var marca = linea.marca || '', modelo = '', categoria = '', avisos = (linea.advertencias_producto || []).slice();
    var marcas = Array.from(new Set(catalogoProductos.map(function(p) {return p.marca;}).filter(Boolean))).filter(function(m) {return contieneDatoOCR(limpio,m);});
    if (marcas.length === 1) marca = marcas[0];
    if (marcas.length > 1) {marca='';avisos.push('Varias marcas posibles; selecciona manualmente.');}
    var coincidencias = catalogoProductos.filter(function(p) {return p.modelo && contieneDatoOCR(limpio,p.modelo) && (!marca || claveProductoOCR(p.marca) === claveProductoOCR(marca));});
    var modelos = Array.from(new Set(coincidencias.map(function(p) {return p.modelo;})));
    if (modelos.length === 1) modelo=modelos[0];
    if (coincidencias.length === 1) {marca=coincidencias[0].marca || marca; categoria=coincidencias[0].categoria;}
    var nombre=linea.nombre || limpio;
    [marca,modelo,linea.modelo_candidato].filter(Boolean).forEach(function(v) {if (contieneDatoOCR(nombre,v)) nombre=nombre.replace(new RegExp(v.replace(/[.*+?^${}()|[\]\\]/g,'\\$&'),'gi'),'');});
    nombre=nombre.replace(/\s+/g,' ').trim();
    if (!marca) avisos.push('Marca pendiente.');
    if (!modelo) avisos.push('Modelo pendiente. Candidato OCR: '+(linea.modelo_candidato || 'sin confirmar'));
    return {codigo:'',nombre:nombre,descripcion:limpio,marca:marca,modelo:modelo,categoria:categoria,textoOriginal:original,advertencias:avisos};
}
function productoParaOCR(descripcion) {
    var coincidencias=catalogoProductos.filter(function(p) { return Number(p.v)>0 && claveProductoOCR(p.t)===claveProductoOCR(descripcion); });
    return coincidencias.length===1 ? Number(coincidencias[0].v) : 0;
}
function aplicarOCRCompra(respuesta, archivo) {
    var d = respuesta.datos;
    proveedorNuevoCompra = null;
    proveedorSugeridoCompra = {ruc:d.ruc_emisor || '',razonSocial:d.razon_social || ''};
    $('#cboProveedor option[value="-1"]').remove();
    catalogoProductos = catalogoProductos.filter(function(p) { return Number(p.v)>0; });
    comprobanteOCRPendiente = Object.assign({}, archivo, { textoOcr: respuesta.texto_ocr || '', rucEmisor: d.ruc_emisor || '' });
    montosDocumento = true;
    $("#txtNumeroComprobante").val(d.numero_comprobante || '');
    $("#txtFechaCompra").val(d.fecha_emision || '');
    $("#cboTipoComprobante").val(d.tipo_comprobante || '');
    var proveedores = catalogoProveedores.filter(function(p) { return d.ruc_emisor && p.codigo === d.ruc_emisor; });
    $("#cboProveedor").val(proveedores.length === 1 ? proveedores[0].v : '');
    [["subtotal", "#txtSubtotalCompraGral", "#txtSubTotal"], ["igv", "#txtIGVCompra", "#txtIgv"], ["total", "#txtTotalCompra", "#txtTotal"]].forEach(function(c) {
        $(c[1] + ', ' + c[2]).val(d[c[0]] == null ? '' : d[c[0]]);
    });
    if (d.tipo_comprobante === 'TICKET' && d.total != null && (d.subtotal == null || d.igv == null)) ajustarMontosSinIgv();
    var productosAsignados = {};
    var lineasRequierenModelo = false;
    compraDetalles = (d.detalles || []).map(function(linea) {
        var producto = separarProductoOCR(linea.descripcion, linea);
        if (linea.requiere_revision) producto.advertencias.push("Confirma cantidad y precios de esta fila.");
        var idProducto = productoParaOCR(linea.descripcion);
        // Una coincidencia repetida puede ocultar modelos diferentes en la factura.
        // Se deja pendiente para que el usuario elija o cree el modelo correcto.
        if (idProducto && productosAsignados[idProducto]) {
            idProducto = 0;
            lineasRequierenModelo = true;
        }
        if (idProducto) productosAsignados[idProducto] = true;
        return { iCodProducto: idProducto, cNombreProducto: producto.nombre, productoOcr: producto, iCantidad: linea.cantidad, nPrecioCompra: linea.precio_unitario, importeOcr: linea.importe, requiereRevision: linea.requiere_revision, nSubTotal: linea.importe };
    });
    var suma = compraDetalles.reduce(function(t, linea) { return t + linea.nSubTotal; }, 0);
    $("#preciosIncluyenIgv").val(d.total != null && Math.abs(suma - d.total) < .02 ? 'si' : 'no');
    dibujarDetalleCompra();
    var avisos = (d.advertencias || []).slice();
    compraDetalles.forEach(function(x,i) { (x.productoOcr.advertencias || []).forEach(function(a) {avisos.push('Producto '+(i+1)+': '+a);}); });
    if (lineasRequierenModelo) avisos.push('Hay lÃ­neas con la misma coincidencia de producto. Revisa el modelo de cada una antes de guardar.');
    if (proveedores.length !== 1) avisos.push('Revisa los datos del proveedor nuevo para crearlo junto con la compra.');
    if (compraDetalles.some(function(linea) { return !linea.iCodProducto; })) avisos.push('Relaciona los productos pendientes o prepara sus fichas nuevas para esta compra.');
    $("#advertenciasOCR").text(avisos.join(' '));
    $("#textoOCR").text(respuesta.texto_ocr || '');
    $("#revisionOCR").prop('hidden', false);
    $("#confirmarOCR").prop('checked', false);
    $("#confirmarOCRContenedor").prop('hidden', false);
    $("#ocrEstado").text('Datos leÃ­dos. Revisa las lÃ­neas e importes y pulsa Guardar.');
    if (proveedores.length !== 1 || compraDetalles.some(function(x) { return !x.iCodProducto; })) prepararAltasCompra();
}
function extraerOCRCompra() {
    if (extrayendoCompra || guardandoCompra || $("#txtIdCompra").val()) return;
    var input = document.getElementById('txtArchivoComprobante');
    var file = input.files && input.files[0];
    if (!file || !/\.(png|jpe?g|bmp|tiff?|pdf)$/i.test(file.name) || file.size === 0 || file.size > 10 * 1024 * 1024) { alert('Selecciona una imagen o PDF de hasta 10 MB.'); return; }
    extrayendoCompra = true;
    bloquearCompra(true);
    $("#ocrEstado").text('Leyendo comprobante...');
    var lector = new FileReader();
    lector.onerror = function() { terminar('No se pudo leer el archivo.'); };
    lector.onload = function() {
        var archivo = { nombreArchivo: file.name, tipoMime: file.type, contenidoBase64: String(lector.result).split(',')[1] };
        $.when(listarProductosCombo(), listarProveedoresCombo()).done(function() {
            $.ajax({ type: 'POST', url: '/Views/Operaciones/Compras/Compras.aspx/ExtraerComprobante', contentType: 'application/json; charset=utf-8', dataType: 'json', timeout: 175000, data: JSON.stringify({ archivo: archivo }) })
                .done(function(response) {
                    if (!response.d || !response.d.success) { terminar(response.d && response.d.mensaje || 'No se pudo leer el comprobante.'); return; }
                    aplicarOCRCompra(response.d, archivo);
                    terminar();
                    if (!$('#cboProveedor').val() || compraDetalles.some(function(x) {return !x.iCodProducto;})) prepararAltasCompra();
                }).fail(function() { terminar('No se pudo extraer el comprobante. Verifica tu sesiÃ³n y el servicio OCR.'); });
        }).fail(function() { terminar('No se pudo cargar el catÃ¡logo de productos y proveedores. Reintenta.'); });
    };
    lector.readAsDataURL(file);
    function terminar(mensaje) {
        extrayendoCompra = false;
        bloquearCompra(false);
        if (mensaje) $("#ocrEstado").text(mensaje);
    }
}
$(function() {
    $("#txtFechaCompra").val(fechaCompraInput(new Date()));
    $("#cboTipoComprobante").on('change', function() { actualizarMontosCompra(); });
    $("#txtArchivoComprobante").on('change', function() {
        comprobanteOCRPendiente = null;
        compraDetalles = [];
        montosDocumento = true;
        $("#revisionOCR").prop('hidden', true);
        $("#confirmarOCR").prop('checked', false);
        $("#confirmarOCRContenedor").prop('hidden', true);
        $("#txtNumeroComprobante, #txtFechaCompra, #txtSubTotal, #txtIgv, #txtTotal, #txtSubtotalCompraGral, #txtIGVCompra, #txtTotalCompra, #cboProveedor").val('');
        dibujarDetalleCompra();
        $("#ocrEstado").text('Archivo seleccionado.');
        if (this.files && this.files.length) extraerOCRCompra();
    });
});

var proveedorModalId = 0, proveedorCargando = false, proveedorGuardando = false;
function mostrarSeccionCompra(id) {
    var seccion=document.getElementById(id);
    if (!seccion.open) seccion.showModal();
}
function cerrarModalCompra(id) {
    if (id === 'modalProveedorCompra' && proveedorGuardando) return;
    var modal=document.getElementById(id);
    if (modal.open) modal.close();
}
function abrirProductoCompra(index) {
    if (guardandoCompra || extrayendoCompra || $('#txtIdCompra').val() || !compraDetalles[index]) return;
    var fila=document.querySelectorAll('#bodyDetalleCompra tr')[index];
    fila.scrollIntoView({block:'center'});
    fila.querySelector('select').focus();
}
function abrirProveedorCompra() {
    if (guardandoCompra || extrayendoCompra || proveedorCargando || proveedorGuardando || $('#txtIdCompra').val()) return;
    var id = Number($('#cboProveedor').val());
    if (id === -1) { prepararAltasCompra(); return; }
    if (!id) { alert('Selecciona el proveedor que quieres modificar.'); return; }
    proveedorCargando = true;
    $.ajax({type:'POST',url:'/Views/Operaciones/Compras/Compras.aspx/ObtenerProveedorCompra',data:JSON.stringify({idProveedor:id}),contentType:'application/json; charset=utf-8',dataType:'json'})
      .done(function(r) {
        var p = r.d; proveedorModalId = id;
        $('#rucProveedorModal').val(p.ruc); $('#razonProveedorModal').val(p.razonSocial);
        $('#representanteProveedorModal').val(p.representante); $('#telefonoProveedorModal').val(p.telefono);
        $('#correoProveedorModal').val(p.correo); $('#direccionProveedorModal').val(p.direccion);
        $('#errorProveedorModal').text(''); mostrarSeccionCompra('modalProveedorCompra');
      }).fail(function() { alert('No se pudo cargar el proveedor.'); }).always(function() { proveedorCargando=false; });
}
function guardarProveedorDesdeCompra() {
    if (proveedorGuardando) return;
    var proveedor = {id:proveedorModalId,ruc:$('#rucProveedorModal').val().trim(),razonSocial:$('#razonProveedorModal').val().trim(),representante:$('#representanteProveedorModal').val().trim(),telefono:$('#telefonoProveedorModal').val().trim(),correo:$('#correoProveedorModal').val().trim(),direccion:$('#direccionProveedorModal').val().trim()};
    if (!/^\d{11}$/.test(proveedor.ruc) || !proveedor.razonSocial || (proveedor.telefono && !/^\d{9}$/.test(proveedor.telefono))) {
        $('#errorProveedorModal').text('Completa RUC de 11 dÃ­gitos, razÃ³n social y telÃ©fono de 9 dÃ­gitos si corresponde.'); return;
    }
    proveedorGuardando=true; $('#guardarProveedorModal').prop('disabled',true);
    $.ajax({type:'POST',url:'/Views/Operaciones/Compras/Compras.aspx/ActualizarProveedorCompra',data:JSON.stringify({proveedor:proveedor}),contentType:'application/json; charset=utf-8',dataType:'json'})
      .done(function(r) {
        if (!r.d.ok) { $('#errorProveedorModal').text(r.d.mensaje); return; }
        var p = catalogoProveedores.find(function(p) { return Number(p.v)===proveedor.id; });
        if (p) { p.t=proveedor.razonSocial; p.codigo=proveedor.ruc; }
        $('#cboProveedor option[value="'+proveedor.id+'"]').text(proveedor.razonSocial);
        $('#confirmarOCR').prop('checked',false);
        cerrarModalCompra('modalProveedorCompra');
      }).fail(function() { $('#errorProveedorModal').text('No se pudo guardar el proveedor. Reintenta.'); })
      .always(function() { proveedorGuardando=false; $('#guardarProveedorModal').prop('disabled',false); });
}
var proveedorNuevoCompra=null, proveedorSugeridoCompra={}, categoriasCompra=[], preparandoAltas=false;
function prepararAltasCompra() {
    if (guardandoCompra || extrayendoCompra || $('#txtIdCompra').val() || preparandoAltas) return;
    preparandoAltas=true;
    $.ajax({type:'POST',url:'/Views/Operaciones/Compras/Compras.aspx/CategoriasParaCompra',data:'{}',contentType:'application/json; charset=utf-8',dataType:'json'})
      .done(function(r) {
        categoriasCompra=r.d;
        var nuevoProveedor=Number($('#cboProveedor').val())<=0;
        $('#proveedorAltaCampos').prop('hidden',!nuevoProveedor);
        var prov=proveedorNuevoCompra || proveedorSugeridoCompra;
        $('#altaRuc').val(prov.ruc || ''); $('#altaRazon').val(prov.razonSocial || '');
        var html='';
        compraDetalles.forEach(function(d,i) {
            var nuevo=d.nuevoProducto || d.productoOcr || {};
            if (!nuevo.categoria && /\b(switch|router|access point)\b/i.test(nuevo.descripcion || '')) {
                var redes=categoriasCompra.filter(function(c) {return /^(redes|redes y conectividad|conectividad)$/i.test(c.t.trim());});
                if (redes.length===1) nuevo.categoria=redes[0].v;
            }
            html+='<fieldset class="alta-producto" data-indice="'+i+'"><legend>Producto '+(i+1)+'</legend>';
            html+='<div class="alta-resumen"><strong>'+escaparCompra(d.cNombreProducto || nuevo.nombre || 'Producto por revisar')+'</strong><span>Cantidad: '+Number(d.iCantidad)+' &nbsp; · &nbsp; Importe OCR: '+(Number.isFinite(Number(d.importeOcr)) ? Number(d.importeOcr).toFixed(2) : 'Pendiente')+'</span></div>';
            html+='<div class="alta-vincular"><label>Buscar producto existente<input type="search" class="input-control alta-busqueda" placeholder="Nombre, código, marca o modelo" autocomplete="off" /></label><label>Seleccionar producto registrado<select class="input-control alta-existente"><option value="">Selecciona un producto</option><option value="nuevo">Crear producto nuevo</option></select></label><small class="alta-resultados" role="status" aria-live="polite"></small><p class="alta-ficha-existente" hidden></p></div>';
            html+='<div class="alta-datos-nuevos" hidden><label class="alta-ancho">Nombre<input class="input-control alta-nombre" maxlength="100" value="'+escaparCompra(nuevo.nombre || d.cNombreProducto).replace(/"/g,'&quot;')+'" /></label><label>Marca<input class="input-control alta-marca" maxlength="100" value="'+escaparCompra(nuevo.marca || '').replace(/"/g,'&quot;')+'" /></label><label>Modelo<input class="input-control alta-modelo" maxlength="100" value="'+escaparCompra(nuevo.modelo || '').replace(/"/g,'&quot;')+'" /></label><label class="alta-ancho">Descripción<textarea class="input-control alta-descripcion" maxlength="500" rows="3">'+escaparCompra(nuevo.descripcion || '')+'</textarea></label><label>Categoría<select class="input-control alta-categoria"><option value="">Seleccionar categoría</option>';
            categoriasCompra.forEach(function(c) {html+='<option value="'+Number(c.v)+'"'+(Number(c.v)===Number(nuevo.categoria)?' selected':'')+'>'+escaparCompra(c.t)+'</option>';});
            html+='</select></label><label>Código interno<input class="input-control alta-codigo" maxlength="50" placeholder="Automático al guardar" value="'+escaparCompra(nuevo.codigo || '').replace(/"/g,'&quot;')+'" /></label><label>Precio de venta (S/)<input class="input-control alta-venta" type="number" min="0" step="0.01" value="'+(nuevo.precioVenta == null?'':Number(nuevo.precioVenta))+'" /></label></div>';
            html+='<div class="alta-precio"><label>Precio unitario de compra (S/)<input class="input-control alta-compra" type="number" min="0" step="0.01" value="'+Number(d.nPrecioCompra).toFixed(2)+'" /></label><small>Confirma el precio de esta compra antes de continuar.</small></div>';
            var avisos=((d.productoOcr || {}).advertencias || []).join(' ');
            if (avisos) html+='<p class="alta-advertencia">'+escaparCompra(avisos)+'</p>';
            html+='<details class="alta-texto-ocr"><summary>Consultar texto original del OCR</summary><p>'+escaparCompra((d.productoOcr || {}).textoOriginal || 'Sin texto OCR disponible.')+'</p></details></fieldset>';

        });
        $('#productosAltaCampos').html(html || '<p>No hay productos nuevos pendientes.</p>');
        $('#productosAltaCampos .alta-producto').each(function() {
            var fila=$(this), detalle=compraDetalles[Number(fila.attr('data-indice'))];
            filtrarExistentesAltaCompra(fila);
            fila.find('.alta-existente').val(detalle.iCodProducto>0 ? String(detalle.iCodProducto) : (detalle.nuevoProducto ? 'nuevo' : '')).trigger('change');
        });
        $('#errorAltasCompra').text('');
        mostrarSeccionCompra('modalAltasCompra');
      }).fail(function() {alert('No se pudieron cargar las categorÃ­as. Reintenta.');})
      .always(function() {preparandoAltas=false;});
}
function confirmarAltasCompra() {
    var proveedor=null, productos=[], error='';
    if (!$('#proveedorAltaCampos').prop('hidden')) {
        proveedor={ruc:$('#altaRuc').val().trim(),razonSocial:$('#altaRazon').val().trim()};
        if (!/^\d{11}$/.test(proveedor.ruc) || !proveedor.razonSocial || proveedor.razonSocial.length>150) error='Completa RUC de 11 dÃ­gitos y razÃ³n social.';
    }
    $('#productosAltaCampos .alta-producto').each(function() {
        var row=$(this), nombre=row.find('.alta-nombre').val().trim(), codigo=row.find('.alta-codigo').val().trim(), descripcion=row.find('.alta-descripcion').val().trim(), marca=row.find('.alta-marca').val().trim(), modelo=row.find('.alta-modelo').val().trim(), categoria=Number(row.find('.alta-categoria').val()), textoVenta=row.find('.alta-venta').val(), venta=Number(textoVenta), textoCompra=row.find('.alta-compra').val(), compra=Number(textoCompra), existente=Number(row.find('.alta-existente').val()) || 0;
        if (!row.find('.alta-existente').val()) { error='Busca y selecciona un producto registrado o elige Crear producto nuevo.'; return; }
        if ((!existente && (!nombre || nombre.length>100 || codigo.length>50 || descripcion.length>500 || marca.length>100 || modelo.length>100 || !categoria || textoVenta==='' || !Number.isFinite(venta) || venta<0 || Math.abs(venta*100-Math.round(venta*100))>.000001)) || textoCompra==='' || !Number.isFinite(compra) || compra<0 || Math.abs(compra*100-Math.round(compra*100))>.000001) error='Elige un producto existente o completa los datos, categorÃ­a, precio de venta y precio de compra del producto nuevo.';
        productos.push({indice:Number(row.attr('data-indice')),existente:existente,codigo:codigo,nombre:nombre,descripcion:descripcion,marca:marca,modelo:modelo,categoria:categoria,precioVenta:venta,precioCompra:compra});
    });
    if (error) {$('#errorAltasCompra').text(error);return;}
    if (proveedor) {
        var existente=catalogoProveedores.filter(function(p) {return p.codigo===proveedor.ruc;});
        if (existente.length===1) { $('#cboProveedor').val(existente[0].v); proveedorNuevoCompra=null; }
        else { proveedorNuevoCompra=proveedor; $('#cboProveedor option[value="-1"]').remove(); $('#cboProveedor').append($('<option>').val('-1').text('Nuevo: '+proveedor.razonSocial)).val('-1'); }
    }
    productos.forEach(function(p) {
        var d=compraDetalles[p.indice], existente=p.existente;
        d.nPrecioCompra=p.precioCompra;
        d.nSubTotal=Math.round(d.iCantidad*d.nPrecioCompra*100)/100;
        if (existente>0) {d.iCodProducto=existente; d.nuevoProducto=null;}
        else {
            var id=d.iCodProducto<0 ? d.iCodProducto : Math.min.apply(null,[0].concat(catalogoProductos.map(function(x) {return Number(x.v);}))) - 1; d.iCodProducto=id; d.nuevoProducto={temporal:id,codigo:p.codigo,nombre:p.nombre,descripcion:p.descripcion,marca:p.marca,modelo:p.modelo,categoria:p.categoria,precioVenta:p.precioVenta};
            catalogoProductos=catalogoProductos.filter(function(x) {return Number(x.v)!==id;});
            catalogoProductos.push({v:id,t:p.nombre,codigo:'Pendiente',stock:0});
        }
        d.cNombreProducto=existente>0 ? (catalogoProductos.find(function(x) {return Number(x.v)===existente;}) || {}).t : p.nombre;
    });
    $('#confirmarOCR').prop('checked',false); dibujarDetalleCompra(); cerrarModalCompra('modalAltasCompra');
    $('#ocrEstado').text('Nuevos registros preparados. Revisa la compra y pulsa Guardar para crearlos.');
}

function filtrarExistentesAltaCompra(fila) {
    var selector=fila.find('.alta-existente'), seleccionado=selector.val();
    var palabras=normalizarBusquedaCompra(fila.find('.alta-busqueda').val() || '').split(/\s+/).filter(Boolean);
    var productos=catalogoProductos.filter(function(p) {
        var texto=normalizarBusquedaCompra([p.t,p.codigo,p.marca,p.modelo].join(' '));
        return Number(p.v)>0 && palabras.every(function(palabra) {return texto.indexOf(palabra)!==-1;});
    });
    selector.empty().append($('<option>').val('').text('Selecciona un producto')).append($('<option>').val('nuevo').text('Crear producto nuevo'));
    var visibles=productos.slice();
    var actual=catalogoProductos.find(function(p) {return Number(p.v)>0 && String(p.v)===String(seleccionado);});
    if (actual && !visibles.some(function(p) {return String(p.v)===String(actual.v);})) visibles.unshift(actual);
    visibles.forEach(function(p) { selector.append($('<option>').val(p.v).text([p.t,p.codigo,p.marca,p.modelo].filter(Boolean).join(' — '))); });
    selector.val(seleccionado || '');
    fila.find('.alta-resultados').text(productos.length ? productos.length+' producto(s) encontrado(s). Selecciona una ficha para reutilizarla.' : 'Sin coincidencias. Prueba otra búsqueda o elige Crear producto nuevo.');
}
$(document).on('input', '#productosAltaCampos .alta-busqueda', function() {
    filtrarExistentesAltaCompra($(this).closest('.alta-producto'));
}).on('change', '#productosAltaCampos .alta-existente', function() {
    var fila=$(this).closest('.alta-producto'), id=Number($(this).val()), crear=$(this).val()==='nuevo';
    var producto=catalogoProductos.find(function(p) {return Number(p.v)===id && id>0;});
    fila.find('.alta-datos-nuevos').prop('hidden',!crear).find('input,select,textarea').prop('disabled',!crear);
    fila.find('.alta-ficha-existente').prop('hidden',!producto).text(producto ? 'Se usará la ficha de '+producto.t+'. Sus datos de catálogo se conservarán.' : '');
});
