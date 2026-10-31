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
var camposConfirmarOCR = {};
var marcasCompra = [], archivoOCRUrl = null, registrandoVistaOCR = false;


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
    $("#resumenProductosOCR").prop("hidden",compraDetalles.length===0);
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
                filas += "<td>" + formatoMoneda(compras[i].nSubTotal) + "</td>";
                filas += "<td>" + formatoMoneda(compras[i].nIgv) + "</td>";
                filas += "<td>" + formatoMoneda(compras[i].nTotal) + "</td>";
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
            var etiquetas=['ID','Proveedor','Usuario','Tipo de comprobante','Número','Fecha','Subtotal','IGV','Total','Estado','Acciones'];
            $('#bodyCompras tr').each(function() {$(this).children('td').each(function(i) {$(this).attr('data-label',etiquetas[i]);});});
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
            if (!response.d.ok) {
                if ($('#modalAltasCompra')[0].open) { $('#errorAltasCompra').text(response.d.mensaje); $('#estadoVistaOCR').text('No se registró la compra').attr('class','ocr-etiqueta ocr-error'); }
                else if ($('#formularioCompletoCompra').hasClass('en-segunda-revision')) $('#errorSegundaRevision').text(response.d.mensaje);
                else alert(response.d.mensaje);
                return;
            }
            guardandoCompra = false;
            limpiarFormulario();
            $("#ocrEstado").text("Compra " + response.d.idCompra + " registrada. Inventario actualizado.");
            listarCompras(); listarProductosCombo(); listarProveedoresCombo();
        }).fail(function() {
            var mensaje='No se recibió confirmación. Revisa las compras realizadas antes de reintentar; se conserva el formulario.';
            if ($('#modalAltasCompra')[0].open) $('#errorAltasCompra').text(mensaje); else if ($('#formularioCompletoCompra').hasClass('en-segunda-revision')) $('#errorSegundaRevision').text(mensaje); else alert(mensaje);
        })
        .always(function() { guardandoCompra = false; bloquearCompra(false); restaurarCamposVistaCompra(); });
}
function bloquearCompra(bloquear) {
    $("#panelRegistroCompra input, #panelRegistroCompra select, #panelRegistroCompra textarea, #panelRegistroCompra button").prop("disabled", bloquear);
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
            $("#formularioCompletoCompra").addClass("compra-registrada");
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
    $('#formularioCompletoCompra').removeClass('en-segunda-revision compra-registrada');
    $('#segundaRevisionCompra').prop('hidden',true);
    $('#confirmarSegundaRevision').prop('checked',false);
    $('#errorSegundaRevision').text('');
    proveedorNuevoCompra = null;
    $('#cboProveedor option[value="-1"]').remove();
    catalogoProductos = catalogoProductos.filter(function(p) { return Number(p.v)>0; });
    comprobanteOCRPendiente = null;
    liberarArchivoOCR();
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
    $("#resumenProductosOCR").prop("hidden",true);
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
    $("#menuCompras").addClass("active").attr("aria-current", "page");
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
    var marca = linea.marca || '', modelo = linea.modelo || '', categoria = '', avisos = (linea.advertencias_producto || []).slice();
    var marcas = Array.from(new Set(catalogoProductos.map(function(p) {return p.marca;}).filter(Boolean))).filter(function(m) {return contieneDatoOCR(limpio,m);});
    if (marca) {
        var canonicas=marcas.filter(function(m) {return claveProductoOCR(m)===claveProductoOCR(marca);});
        if (canonicas.length===1) marca=canonicas[0];
    } else {
        if (marcas.length === 1) marca = marcas[0];
        if (marcas.length > 1) {marca='';avisos.push('Varias marcas posibles; selecciona manualmente.');}
    }
    var coincidencias = catalogoProductos.filter(function(p) {return p.modelo && contieneDatoOCR(limpio,p.modelo) && (!marca || claveProductoOCR(p.marca) === claveProductoOCR(marca));});
    var modelos = Array.from(new Set(coincidencias.map(function(p) {return p.modelo;})));
    if (modelos.length === 1 && !avisos.some(function(a) {return /lecturas difieren.*modelo/i.test(a);})) modelo=modelos[0];
    if (coincidencias.length === 1) {marca=coincidencias[0].marca || marca; categoria=coincidencias[0].categoria;}
    var nombre=linea.nombre || limpio;
    if (!categoria && marca && nombre) {
        var categoriaCoincidente=catalogoProductos.filter(function(p) {
            return claveProductoOCR(p.marca)===claveProductoOCR(marca) && claveProductoOCR(p.t).indexOf(claveProductoOCR(nombre))!==-1 && Number(p.categoria)>0;
        });
        var categoriasPosibles=Array.from(new Set(categoriaCoincidente.map(function(p) {return Number(p.categoria);})));
        if (categoriasPosibles.length===1) {categoria=categoriasPosibles[0];avisos.push('Categoría sugerida por productos de la misma marca y nombre; revísala.');}
    }
    [marca,modelo,linea.modelo_candidato].filter(Boolean).forEach(function(v) {if (contieneDatoOCR(nombre,v)) nombre=nombre.replace(new RegExp(v.replace(/[.*+?^${}()|[\]\\]/g,'\\$&'),'gi'),'');});
    nombre=nombre.replace(/\s+/g,' ').trim();
    avisos=avisos.filter(function(a) {return !(marca && /marca/i.test(a) && /pendiente|no reconocida/i.test(a)) && !(modelo && /modelo/i.test(a) && /pendiente|no reconocido/i.test(a));});
    if (!marca) avisos.push('Marca pendiente.');
    if (!modelo) avisos.push('Modelo pendiente. Candidato OCR: '+(linea.modelo_candidato || 'sin confirmar'));
    return {codigo:linea.codigo || '',nombre:nombre,descripcion:linea.descripcion_detallada != null ? linea.descripcion_detallada : limpio,marca:marca,modelo:modelo,categoria:categoria,textoOriginal:original,advertencias:avisos};
}
function productoParaOCR(descripcion, datos) {
    if (datos && datos.codigo) {
        var porCodigo=catalogoProductos.filter(function(p) {return Number(p.v)>0 && claveProductoOCR(p.codigo)===claveProductoOCR(datos.codigo);});
        if (porCodigo.length) return porCodigo.length===1 && (!datos.marca || claveProductoOCR(porCodigo[0].marca)===claveProductoOCR(datos.marca)) && (!datos.modelo || claveProductoOCR(porCodigo[0].modelo)===claveProductoOCR(datos.modelo)) ? Number(porCodigo[0].v) : 0;
    }
    if (datos && datos.marca && datos.modelo) {
        var porFicha=catalogoProductos.filter(function(p) {
            return Number(p.v)>0 && claveProductoOCR(p.marca)===claveProductoOCR(datos.marca) && claveProductoOCR(p.modelo)===claveProductoOCR(datos.modelo);
        });
        if (porFicha.length===1) return Number(porFicha[0].v);
        if (porFicha.length>1) return 0;
    }
    var coincidencias=catalogoProductos.filter(function(p) { return Number(p.v)>0 && (claveProductoOCR(p.t)===claveProductoOCR(descripcion) || (datos && claveProductoOCR(p.t)===claveProductoOCR(datos.nombre) && claveProductoOCR(p.descripcion)===claveProductoOCR(datos.descripcion))); });
    if (datos) coincidencias=coincidencias.filter(function(p) {
        return (!datos.marca || claveProductoOCR(p.marca)===claveProductoOCR(datos.marca)) && (!datos.modelo || claveProductoOCR(p.modelo)===claveProductoOCR(datos.modelo));
    });
    return coincidencias.length===1 ? Number(coincidencias[0].v) : 0;
}
function sugerenciasProductoOCR(datos) {
    var palabras=new Set(normalizarBusquedaCompra([datos.nombre,datos.descripcion,datos.marca,datos.modelo].join(' ')).match(/[a-z0-9]{2,}/g) || []);
    return catalogoProductos.filter(function(p) {return Number(p.v)>0;}).map(function(p) {
        var otras=new Set(normalizarBusquedaCompra([p.t,p.descripcion,p.marca,p.modelo].join(' ')).match(/[a-z0-9]{2,}/g) || []);
        var comunes=Array.from(palabras).filter(function(x) {return otras.has(x);}).length;
        return {producto:p,puntaje:comunes / Math.max(1,Math.min(palabras.size,otras.size))};
    }).filter(function(x) {return x.puntaje>=.4;}).sort(function(a,b) {return b.puntaje-a.puntaje;}).slice(0,5);
}
function aplicarOCRCompra(respuesta, archivo) {
    camposConfirmarOCR=respuesta.datos.campos_por_confirmar || {};
    $('#formularioCompletoCompra').removeClass('en-segunda-revision compra-registrada');
    $('#segundaRevisionCompra').prop('hidden',true);
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
        var idProducto = productoParaOCR(linea.descripcion, producto);
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
    prepararAltasCompra();
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
                    prepararAltasCompra();
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
        liberarArchivoOCR();
        if (this.files && this.files[0]) archivoOCRUrl=URL.createObjectURL(this.files[0]);
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
    if (id === 'modalAltasCompra' && (guardandoCompra || registrandoVistaOCR)) return;
    var modal=document.getElementById(id);
    if (modal.open) modal.close();
}
function abrirProductoCompra(index) {
    if (guardandoCompra || extrayendoCompra || $('#txtIdCompra').val() || !compraDetalles[index]) return;
    prepararAltasCompra();
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
        proveedorGuardando=false;
        cerrarModalCompra('modalProveedorCompra');
      }).fail(function() { $('#errorProveedorModal').text('No se pudo guardar el proveedor. Reintenta.'); })
      .always(function() { proveedorGuardando=false; $('#guardarProveedorModal').prop('disabled',false); });
}
var proveedorNuevoCompra=null, proveedorSugeridoCompra={}, categoriasCompra=[], preparandoAltas=false;
function prepararAltasCompra() {
    if (guardandoCompra || extrayendoCompra || $('#txtIdCompra').val() || preparandoAltas) return;
    preparandoAltas=true;
    $.when($.ajax({type:'POST',url:'/Views/Operaciones/Compras/Compras.aspx/CategoriasParaCompra',data:'{}',contentType:'application/json; charset=utf-8',dataType:'json'}), $.ajax({type:'POST',url:'/Views/Operaciones/Compras/Compras.aspx/MarcasParaCompra',data:'{}',contentType:'application/json; charset=utf-8',dataType:'json'}))
      .done(function(categorias,marcas) {
        categoriasCompra=categorias[0].d;
        marcasCompra=marcas[0].d;
        $('#marcasCompraOCR').empty();
        marcasCompra.forEach(function(m) {$('#marcasCompraOCR').append($('<option>').val(m.t));});
        var nuevoProveedor=Number($('#cboProveedor').val())<=0;
        $('#proveedorAltaCampos').prop('hidden',false);
        $('#previewProveedor').empty().append($('<option>').val('nuevo').text('Registrar proveedor nuevo'));
        catalogoProveedores.forEach(function(p) {$('#previewProveedor').append($('<option>').val(p.v).text(p.t+' · '+p.codigo));});
        $('#previewProveedor').val(nuevoProveedor ? 'nuevo' : $('#cboProveedor').val());
        var prov=proveedorNuevoCompra || proveedorSugeridoCompra;
        $('#altaRuc').val(prov.ruc || ''); $('#altaRazon').val(prov.razonSocial || '');
        actualizarProveedorVistaCompra();
        $('#previewTipo').html($('#cboTipoComprobante').html());
        [['Observacion','#txtObservacion'],['Fecha','#txtFechaCompra'],['Tipo','#cboTipoComprobante'],['Numero','#txtNumeroComprobante'],['Subtotal','#txtSubtotalCompraGral'],['Igv','#txtIGVCompra'],['Total','#txtTotalCompra'],['Incluidos','#preciosIncluyenIgv']].forEach(function(c) {$('#preview'+c[0]).val($(c[1]).val());});
        $('#numeroOCRAlternativas').empty();
        var numeroPropuesto=camposConfirmarOCR.numero_comprobante;
        if (numeroPropuesto) (numeroPropuesto.alternativas || []).forEach(function(n) {$('#numeroOCRAlternativas').append($('<option>').val(n));});
        $('#revisionNumeroOCR').prop('hidden',!numeroPropuesto);
        $('#confirmarNumeroOCR').prop('checked',false);
        $('#estadoVistaOCR').text('Pendiente de revisión').attr('class','ocr-etiqueta ocr-revision');
        $('#previewTextoOriginal').text(comprobanteOCRPendiente && comprobanteOCRPendiente.textoOcr || $('#textoOCR').text());
        $('#ayudaImpuestosOCR').prop('hidden',$('#previewSubtotal').val()!=='' && $('#previewIgv').val()!=='');
        mostrarArchivoOriginalOCR();
        var html='';
        compraDetalles.forEach(function(d,i) {
            var nuevo=d.nuevoProducto || d.productoOcr || {};
            if (!nuevo.categoria && /\b(switch|router|access point)\b/i.test(nuevo.descripcion || '')) {
                var redes=categoriasCompra.filter(function(c) {return /^(redes|redes y conectividad|conectividad)$/i.test(c.t.trim());});
                if (redes.length===1) nuevo.categoria=redes[0].v;
            }
            html+='<fieldset class="alta-producto" data-indice="'+i+'"><legend>Producto '+(i+1)+'</legend>';
            html+='<div class="alta-resumen"><strong class="alta-accion">Revisar producto</strong><span class="ocr-etiqueta ocr-revision alta-estado">Requiere revisión</span><span>Importe del comprobante: '+(d.importeOcr != null && Number.isFinite(Number(d.importeOcr)) ? Number(d.importeOcr).toFixed(2) : 'Pendiente')+'</span></div>';
            html+='<details class="alta-vincular"><summary>Buscar o cambiar producto vinculado</summary><label>Buscar producto existente<input type="search" class="input-control alta-busqueda" placeholder="Nombre, código, marca o modelo" autocomplete="off" /></label><label>Seleccionar producto registrado<select class="input-control alta-existente"><option value="">Selecciona un producto</option><option value="nuevo">Crear producto nuevo</option></select></label><small class="alta-resultados" role="status" aria-live="polite"></small><p class="alta-ficha-existente" hidden></p></details>';
            html+='<div class="ocr-sugerencias"></div><div class="alta-datos-nuevos" hidden><label class="alta-ancho">Nombre<input class="input-control alta-nombre" maxlength="100" value="'+escaparCompra(nuevo.nombre || d.cNombreProducto).replace(/"/g,'&quot;')+'" /></label><div><label>Marca<input class="input-control alta-marca" list="marcasCompraOCR" autocomplete="off" maxlength="100" value="'+escaparCompra(nuevo.marca || '').replace(/"/g,'&quot;')+'" /></label><label class="ocr-nueva-marca"><input type="checkbox" class="alta-marca-nueva"'+(nuevo.marcaNueva?' checked':'')+' />Añadir esta marca si aún no existe</label></div><label>Modelo<input class="input-control alta-modelo" placeholder="Opcional: si aparece en el comprobante" maxlength="100" value="'+escaparCompra(nuevo.modelo || '').replace(/"/g,'&quot;')+'" /></label><label class="alta-ancho">Descripción<textarea class="input-control alta-descripcion" maxlength="500" rows="3">'+escaparCompra(nuevo.descripcion || '')+'</textarea></label><label>Categoría<select class="input-control alta-categoria"><option value="">Seleccionar categoría</option>';
            categoriasCompra.forEach(function(c) {html+='<option value="'+Number(c.v)+'"'+(Number(c.v)===Number(nuevo.categoria)?' selected':'')+'>'+escaparCompra(c.t)+'</option>';});
            html+='</select></label><label>Código interno<input class="input-control alta-codigo" maxlength="50" placeholder="Automático al guardar" value="'+escaparCompra(nuevo.codigo || '').replace(/"/g,'&quot;')+'" /></label><label>Precio de venta (S/)<input class="input-control alta-venta" placeholder="Define el precio de venta" type="number" min="0" step="0.01" value="'+(nuevo.precioVenta == null?'':Number(nuevo.precioVenta))+'" /></label></div>';
            html+='<div class="alta-precio"><label>Cantidad que ingresa<input class="input-control alta-cantidad" type="number" min="1" step="1" value="'+Number(d.iCantidad)+'" /></label><label>Precio unitario de compra (S/)<input class="input-control alta-compra" type="number" min="0" step="0.01" value="'+(d.nPrecioCompra == null ? '' : Number(d.nPrecioCompra).toFixed(2))+'" /></label><small class="alta-entrada" role="status"></small></div>';
            var avisos=((d.productoOcr || {}).advertencias || []).join(' ');
            if (avisos) html+='<p class="alta-advertencia">'+escaparCompra(avisos)+'</p>';
            html+='<div class="alta-referencia"><button type="button" class="btn btn-secondary alta-ver-texto">Ver lectura de esta línea</button><button type="button" class="btn btn-secondary alta-quitar">Quitar producto</button></div></fieldset>';

        });
        $('#productosAltaCampos').html(html || '<p>No hay productos nuevos pendientes.</p>');
        $('#productosAltaCampos .alta-producto').each(function() {
            var fila=$(this), detalle=compraDetalles[Number(fila.attr('data-indice'))];
            fila.data('borrador', {nombre:fila.find('.alta-nombre').val(),marca:fila.find('.alta-marca').val(),modelo:fila.find('.alta-modelo').val(),descripcion:fila.find('.alta-descripcion').val(),categoria:fila.find('.alta-categoria').val(),codigo:fila.find('.alta-codigo').val(),venta:fila.find('.alta-venta').val()});
            var sugerencias=sugerenciasProductoOCR(detalle.productoOcr || detalle.nuevoProducto || {nombre:detalle.cNombreProducto});
            if (sugerencias.length) fila.find('.ocr-sugerencias').text('Posibles coincidencias para revisar: '+sugerencias.map(function(s) {return [s.producto.t,s.producto.marca,s.producto.modelo].filter(Boolean).join(' · ');}).join(' / '));
            filtrarExistentesAltaCompra(fila);
            fila.find('.alta-existente').val(detalle.iCodProducto>0 ? String(detalle.iCodProducto) : 'nuevo').trigger('change');
        });
        $('#errorAltasCompra').text('');
        $('#estadoDuplicadoOCR').text('');
        $('.ocr-cambiar-proveedor').prop('open',false);
        mostrarSeccionCompra('modalAltasCompra');
        $('#modalAltasCompra > .modal-cuerpo').scrollTop(0);
      }).fail(function() {alert('No se pudieron cargar las categorÃ­as. Reintenta.');})
      .always(function() {preparandoAltas=false;});
}
function confirmarAltasCompra(mantenerAbierta) {
    if (guardandoCompra || (registrandoVistaOCR && !mantenerAbierta)) return false;
    limpiarErroresVistaCompra();
    var proveedor=null, productos=[], error='', identidades=new Set();
    function exigir(campo,condicion,mensaje) {if (!condicion) {error=mensaje; errorCampoVistaCompra(campo,mensaje);}}
    var idProveedor=Number($('#previewProveedor').val());
    if (!idProveedor) {
        proveedor={ruc:$('#altaRuc').val().trim(),razonSocial:$('#altaRazon').val().trim()};
        exigir($('#altaRuc'),/^\d{11}$/.test(proveedor.ruc),'El RUC debe tener 11 dígitos.');
        exigir($('#altaRazon'),proveedor.razonSocial && proveedor.razonSocial.length<=150,'Completa la razón social (máximo 150 caracteres).');
    } else exigir($('#previewProveedor'),catalogoProveedores.some(function(p) {return Number(p.v)===idProveedor;}),'Selecciona un proveedor registrado.');
    $('#productosAltaCampos .alta-producto').each(function() {
        var row=$(this), get=function(c) {return String(row.find('.alta-'+c).val() || '').trim();};
        var existente=Number(get('existente')) || 0, marca=get('marca'), marcaNueva=row.find('.alta-marca-nueva').prop('checked');
        var canonical=marcasCompra.find(function(m) {return normalizarBusquedaCompra(m.t).replace(/\s/g,'')===normalizarBusquedaCompra(marca).replace(/\s/g,'');});
        if (canonical) marca=canonical.t;
        var producto={indice:Number(row.attr('data-indice')),existente:existente,codigo:get('codigo'),nombre:get('nombre'),descripcion:get('descripcion'),marca:marca,marcaNueva:marcaNueva,modelo:get('modelo'),categoria:Number(get('categoria')),precioVenta:Number(get('venta')),precioCompra:Number(get('compra')),cantidad:Number(get('cantidad'))};
        exigir(row.find('.alta-existente'),get('existente')==='nuevo' || catalogoProductos.some(function(p) {return Number(p.v)>0 && Number(p.v)===existente;}),'Selecciona un producto real del inventario o crea una ficha nueva.');
        exigir(row.find('.alta-cantidad'),Number.isSafeInteger(producto.cantidad) && producto.cantidad>0 && producto.cantidad<=2147483647,'Introduce una cantidad entera mayor que cero.');
        exigir(row.find('.alta-compra'),get('compra')!=='' && Number.isFinite(producto.precioCompra) && producto.precioCompra>=0 && Math.abs(producto.precioCompra*100-Math.round(producto.precioCompra*100))<.000001,'Introduce un precio no negativo con hasta dos decimales.');
        exigir(row.find('.alta-venta'),get('venta')!=='' && Number.isFinite(producto.precioVenta) && producto.precioVenta>=0 && Math.abs(producto.precioVenta*100-Math.round(producto.precioVenta*100))<.000001,'Completa el precio de venta con hasta dos decimales.');
        if (!existente) {
            exigir(row.find('.alta-nombre'),producto.nombre && producto.nombre.length<=100,'Completa el nombre (máximo 100 caracteres).');
            exigir(row.find('.alta-marca'),marca && marca.length<=100 && (canonical || marcaNueva),'Selecciona una marca del catálogo o confirma añadir la nueva.');
            exigir(row.find('.alta-modelo'),producto.modelo.length<=100,'El modelo admite hasta 100 caracteres.');
            exigir(row.find('.alta-descripcion'),producto.descripcion.length<=500,'La descripción admite hasta 500 caracteres.');
            exigir(row.find('.alta-codigo'),producto.codigo.length<=50,'El código admite hasta 50 caracteres.');
            exigir(row.find('.alta-categoria'),categoriasCompra.some(function(c) {return Number(c.v)===producto.categoria;}),'Selecciona una categoría.');
            exigir(row.find('.alta-venta'),get('venta')!=='' && Number.isFinite(producto.precioVenta) && producto.precioVenta>=0 && Math.abs(producto.precioVenta*100-Math.round(producto.precioVenta*100))<.000001,'Completa el precio de venta con hasta dos decimales.');
            var vinculo=productoParaOCR(producto.nombre,producto);
            exigir(row.find('.alta-existente'),!vinculo,'Esta ficha ya coincide con un producto registrado. Selecciónalo para aumentar su stock.');
        }
        var identidad=existente ? 'id:'+existente : producto.codigo ? 'codigo:'+claveProductoOCR(producto.codigo) : 'ficha:'+claveProductoOCR(producto.marca)+'|'+claveProductoOCR(producto.modelo || producto.nombre);
        exigir(row.find('.alta-existente'),!identidades.has(identidad),'Producto repetido: agrupa sus cantidades en una sola línea antes de guardar.');
        identidades.add(identidad);
        if (row.find('[aria-invalid="true"]').length) row.find('.alta-estado').text('Datos incompletos').attr('class','ocr-etiqueta ocr-error alta-estado');
        productos.push(producto);
    });
    if (!productos.length) error='Incluye al menos un producto en la compra.';
    if (error) {$('#errorAltasCompra').text(productos.length ? 'Revisa los campos señalados antes de continuar.' : error);$('#estadoVistaOCR').text('Requiere revisión').attr('class','ocr-etiqueta ocr-error');return false;}
    if (proveedor) {
        var existente=catalogoProveedores.filter(function(p) {return p.codigo===proveedor.ruc;});
        if (existente.length===1) { $('#cboProveedor').val(existente[0].v); proveedorNuevoCompra=null; }
        else { proveedorNuevoCompra=proveedor; $('#cboProveedor option[value="-1"]').remove(); $('#cboProveedor').append($('<option>').val('-1').text('Nuevo: '+proveedor.razonSocial)).val('-1'); }
    } else {$('#cboProveedor').val(idProveedor);proveedorNuevoCompra=null;}
    [['Observacion','#txtObservacion'],['Fecha','#txtFechaCompra'],['Tipo','#cboTipoComprobante'],['Numero','#txtNumeroComprobante'],['Subtotal','#txtSubtotalCompraGral, #txtSubTotal'],['Igv','#txtIGVCompra, #txtIgv'],['Total','#txtTotalCompra, #txtTotal'],['Incluidos','#preciosIncluyenIgv']].forEach(function(c) {$(c[1]).val($('#preview'+c[0]).val());});
    productos.forEach(function(p) {
        var d=compraDetalles[p.indice], existente=p.existente;
        d.precioVenta=p.precioVenta;
        d.iCantidad=p.cantidad; d.nPrecioCompra=p.precioCompra; d.nSubTotal=Math.round(d.iCantidad*d.nPrecioCompra*100)/100;
        if (existente>0) {d.iCodProducto=existente;d.nuevoProducto=null;}
        else {
            var id=d.iCodProducto<0 ? d.iCodProducto : Math.min.apply(null,[0].concat(catalogoProductos.map(function(x) {return Number(x.v);}))) - 1;
            d.iCodProducto=id;d.nuevoProducto={temporal:id,codigo:p.codigo,nombre:p.nombre,descripcion:p.descripcion,marca:p.marca,marcaNueva:p.marcaNueva,modelo:p.modelo,categoria:p.categoria,precioVenta:p.precioVenta};
            catalogoProductos=catalogoProductos.filter(function(x) {return Number(x.v)!==id;});
            catalogoProductos.push({v:id,t:p.nombre,codigo:p.codigo || 'Pendiente',stock:0,marca:p.marca,modelo:p.modelo,descripcion:p.descripcion,categoria:p.categoria});
        }
        d.cNombreProducto=existente>0 ? (catalogoProductos.find(function(x) {return Number(x.v)===existente;}) || {}).t : p.nombre;
    });
    if (!mantenerAbierta) compraDetalles=compraDetalles.filter(function(d,i) {return productos.some(function(p) {return p.indice===i;});});
    else compraDetalles=productos.map(function(p) {return compraDetalles[p.indice];});
    $('#productosAltaCampos .alta-producto').each(function(i) {$(this).attr('data-indice',i);});
    $('#confirmarOCR').prop('checked',false);dibujarDetalleCompra();
    if (!mantenerAbierta) {cerrarModalCompra('modalAltasCompra');mostrarSegundaRevisionCompra();}
    $('#estadoVistaOCR').text('Datos revisados').attr('class','ocr-etiqueta ocr-existente');
    $('#ocrEstado').text('Vista previa confirmada. Pulsa Guardar para registrar la compra y sus entradas de stock.');
    return true;
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
    if (fila.data('modo')==='nuevo') {
        var borrador={};
        ['nombre','marca','modelo','descripcion','categoria','codigo','venta'].forEach(function(c) {borrador[c]=fila.find('.alta-'+c).val();});
        fila.data('borrador',borrador);
    }
    var valores=producto ? {nombre:producto.t,marca:producto.marca,modelo:producto.modelo,descripcion:producto.descripcion,categoria:producto.categoria,codigo:producto.codigo,venta:detallePrecioVentaCompra(fila,producto)} : fila.data('borrador');
    if (valores) ['nombre','marca','modelo','descripcion','categoria','codigo','venta'].forEach(function(c) {fila.find('.alta-'+c).val(valores[c] == null ? '' : valores[c]);});
    fila.data('modo',crear?'nuevo':'existente');
    fila.find('.alta-datos-nuevos').prop('hidden',false).find('input,select,textarea').prop('disabled',!crear);
    fila.find('.alta-venta').prop('disabled',false);
    fila.find('.alta-accion').text(producto ? 'Producto existente · entrada de stock' : 'Producto nuevo · revisar ficha');
    fila.find('.alta-ficha-existente').prop('hidden',!producto).text(producto ? 'Se vinculará con '+producto.t+'. La compra sumará unidades a esta ficha.' : '');
    actualizarEntradaAltaCompra(fila);
});
function actualizarEntradaAltaCompra(fila) {
    var id=Number(fila.find('.alta-existente').val()), p=catalogoProductos.find(function(x) {return Number(x.v)===id && id>0;});
    var cantidad=Number(fila.find('.alta-cantidad').val()), precio=Number(fila.find('.alta-compra').val());
    var stock=p ? Number(p.stock || 0) : 0;
    fila.find('.alta-estado').text(p ? 'Producto existente' : 'Producto nuevo').attr('class','ocr-etiqueta '+(p?'ocr-existente':'ocr-nuevo')+' alta-estado');
    if (!p && (!fila.find('.alta-nombre').val().trim() || !fila.find('.alta-marca').val().trim() || !fila.find('.alta-categoria').val() || fila.find('.alta-venta').val()==='')) fila.find('.alta-estado').text('Requiere revisión').attr('class','ocr-etiqueta ocr-revision alta-estado');
    fila.find('.alta-entrada').text((p ? 'Acción: aumentar stock · Stock actual: '+stock+' · Entrada: +'+cantidad+' · Stock previsto: '+(stock+cantidad) : 'Acción: crear producto · Stock inicial: '+cantidad)+' · Importe calculado: S/ '+(cantidad*precio).toFixed(2));
}
$(document).on('input','#productosAltaCampos .alta-cantidad, #productosAltaCampos .alta-compra',function() {actualizarEntradaAltaCompra($(this).closest('.alta-producto'));});
$(document).on('click','#productosAltaCampos .alta-quitar',function() {if (!guardandoCompra && !registrandoVistaOCR) $(this).closest('.alta-producto').remove();});

function liberarArchivoOCR() {
    if (archivoOCRUrl) URL.revokeObjectURL(archivoOCRUrl);
    archivoOCRUrl=null;$('#archivoOriginalOCR').empty();$('#controlesZoomOCR').prop('hidden',true);zoomImagenOCR=100;
}
function mostrarArchivoOriginalOCR() {
    zoomImagenOCR=100;actualizarControlesZoomOCR();
    var panel=$('#archivoOriginalOCR').empty(), input=document.getElementById('txtArchivoComprobante');
    var file=input.files && input.files[0], archivo=comprobanteOCRPendiente;
    var nombre=file ? file.name : archivo && archivo.nombreArchivo || '';
    if (!archivoOCRUrl) {
        if (file) archivoOCRUrl=URL.createObjectURL(file);
        else if (archivo && archivo.contenidoBase64) {
            try {
                var bytes=atob(archivo.contenidoBase64), datos=new Uint8Array(bytes.length);
                for (var i=0;i<bytes.length;i++) datos[i]=bytes.charCodeAt(i);
                var mime=/\.pdf$/i.test(nombre)?'application/pdf':/\.png$/i.test(nombre)?'image/png':/\.bmp$/i.test(nombre)?'image/bmp':'image/jpeg';
                archivoOCRUrl=URL.createObjectURL(new Blob([datos],{type:mime}));
            } catch(e) {archivoOCRUrl=null;}
        }
    }
    var visible=false;
    if (archivoOCRUrl) {
        if (/\.pdf$/i.test(nombre)) {panel.append($('<iframe>').attr({src:archivoOCRUrl,title:'Vista del comprobante PDF'}));visible=true;}
        else if (/\.(jpe?g|png|bmp)$/i.test(nombre)) {
            panel.append($('<img>').attr({src:archivoOCRUrl,alt:'Comprobante original: '+nombre}).on('error',function() {$(this).remove();mostrarReferenciaCompra('texto');}));visible=true;
        } else panel.append($('<p>').text('Abre el archivo original para revisar este formato.'));
        panel.append($('<a>').attr({href:archivoOCRUrl,target:'_blank',rel:'noopener'}).text('Abrir archivo original'));
    } else panel.text('No hay un archivo disponible para visualizar. Puedes consultar la lectura OCR.');
    mostrarReferenciaCompra(visible?'documento':'texto');
}
function mostrarReferenciaCompra(vista,texto,origen) {
    var mostrarTexto=vista==='texto';
    $('#archivoOriginalOCR').prop('hidden',mostrarTexto);$('#panelTextoOCR').prop('hidden',!mostrarTexto);
    $('#controlesZoomOCR').prop('hidden',mostrarTexto || !$('#archivoOriginalOCR img').length);
    $('#tabDocumentoOCR').attr({'aria-selected':String(!mostrarTexto),tabindex:mostrarTexto?'-1':'0'});
    $('#tabTextoOCR').attr({'aria-selected':String(mostrarTexto),tabindex:mostrarTexto?'0':'-1'});
    if (mostrarTexto) {
        var lectura=comprobanteOCRPendiente && comprobanteOCRPendiente.textoOcr || $('#textoOCR').text();
        if (!lectura) lectura=compraDetalles.map(function(d) {return (d.productoOcr || {}).textoOriginal || '';}).filter(Boolean).join('\n\n');
        $('#previewTextoOriginal').text(texto || lectura || 'No hay texto OCR disponible. Vuelve a escanear el comprobante.');
        $('#origenTextoOCR').text(origen || 'Lectura completa del comprobante');
    }
}
function actualizarProveedorVistaCompra(evento) {
    var id=Number($('#previewProveedor').val()), proveedor=catalogoProveedores.find(function(p) {return Number(p.v)>0 && Number(p.v)===id;});
    if (proveedor) {$('#altaRuc').val(proveedor.codigo);$('#altaRazon').val(proveedor.t);}
    else {
        $('#previewProveedor').val('nuevo');
        if (evento && evento.type) $('#altaRuc, #altaRazon').val('');
    }
    $('#altaRuc').prop('readOnly',false);$('#altaRazon').prop('readOnly',!!proveedor);
    $('#estadoProveedorOCR').text(proveedor?'Proveedor registrado':'Proveedor nuevo · revisar datos');
}
function sincronizarProveedorPorRuc() {
    var ruc=$('#altaRuc').val().trim(), anterior=$('#previewProveedor').val();
    var encontrados=catalogoProveedores.filter(function(p) {return String(p.codigo).trim()===ruc && /^\d{11}$/.test(ruc);});
    if (encontrados.length===1) {$('#previewProveedor').val(String(encontrados[0].v));actualizarProveedorVistaCompra();}
    else {
        $('#previewProveedor').val('nuevo');$('#altaRazon').prop('readOnly',false);
        if (anterior!=='nuevo') $('#altaRazon').val(ruc===proveedorSugeridoCompra.ruc ? proveedorSugeridoCompra.razonSocial || '' : '');
        $('#estadoProveedorOCR').text(ruc.length===11?'Proveedor nuevo · completa la razón social':'Escribe el RUC para buscar en el catálogo');
    }
}
function limpiarErroresVistaCompra() {
    $('#modalAltasCompra .ocr-campo-error').remove();
    $('#modalAltasCompra [aria-invalid]').removeAttr('aria-invalid aria-describedby');
    $('#errorAltasCompra').text('');
}
var secuenciaErrorOCR=0;
function errorCampoVistaCompra(campo,mensaje) {
    campo=$(campo); if (!campo.length || campo.attr('aria-invalid')==='true') return;
    campo.parents('details').prop('open',true);
    var id='errorOCR'+(++secuenciaErrorOCR);
    campo.attr({'aria-invalid':'true','aria-describedby':id}).after($('<small>').attr('id',id).addClass('ocr-campo-error').text(mensaje));
}
function editarVistaCompra() {
    $('#estadoVistaOCR').text('Pendiente de revisión').attr('class','ocr-etiqueta ocr-revision');
    var input=$('#modalAltasCompra [aria-invalid="true"]').first();
    if (!input.length) input=$('#altaRuc');
    input.parents('details').prop('open',true);
    input[0].scrollIntoView({block:'center'});input.trigger('focus');
}
function volverEscanearCompra() {
    if (guardandoCompra || registrandoVistaOCR) return;
    cerrarModalCompra('modalAltasCompra');
    if (document.getElementById('txtArchivoComprobante').files.length) extraerOCRCompra();
    else document.getElementById('txtArchivoComprobante').click();
}
function restaurarCamposVistaCompra() {
    $('#productosAltaCampos .alta-producto').each(function() {
        $(this).find('.alta-datos-nuevos input, .alta-datos-nuevos select, .alta-datos-nuevos textarea').prop('disabled',$(this).find('.alta-existente').val()!=='nuevo');
        $(this).find('.alta-venta').prop('disabled',false);
    });
}
function validarDocumentoVistaCompra() {
    limpiarErroresVistaCompra();
    if (camposConfirmarOCR.numero_comprobante && !$('#confirmarNumeroOCR').prop('checked')) errorCampoVistaCompra('#previewNumero','Compara el número propuesto con el original y marca su confirmación. Puedes corregirlo o elegir otra lectura.');
    if (!$('#previewNumero').val().trim()) errorCampoVistaCompra('#previewNumero','Completa la serie y el número del comprobante.');
    if (!/^\d{4}-\d{2}-\d{2}$/.test($('#previewFecha').val())) errorCampoVistaCompra('#previewFecha','Completa una fecha válida.');
    if (!$('#previewTipo').val()) errorCampoVistaCompra('#previewTipo','Selecciona el tipo de comprobante.');
    ['Subtotal','Igv','Total'].forEach(function(c) {
        var texto=$('#preview'+c).val(), n=Number(texto);
        if (texto==='' || !Number.isFinite(n) || n<0 || (c==='Total' && n<=0) || Math.abs(n*100-Math.round(n*100))>.000001) errorCampoVistaCompra('#preview'+c,'Completa un importe válido con hasta dos decimales.');
    });
    var subtotal=Number($('#previewSubtotal').val()), igv=Number($('#previewIgv').val()), total=Number($('#previewTotal').val());
    if (Math.abs(subtotal+igv-total)>.02) errorCampoVistaCompra('#previewTotal','Subtotal más IGV debe coincidir con el total.');
    var suma=0;
    $('#productosAltaCampos .alta-producto').each(function() {suma+=Math.round(Number($(this).find('.alta-cantidad').val())*Number($(this).find('.alta-compra').val())*100)/100;});
    if (Math.abs(suma-($('#previewIncluidos').val()==='si'?total:subtotal))>.02) errorCampoVistaCompra('#previewIncluidos','Los productos no cuadran con el comprobante. Revisa impuestos, descuentos y precios netos.');
    if ($('#modalAltasCompra [aria-invalid="true"]').length) {$('#errorAltasCompra').text('Corrige los campos señalados antes de registrar.');editarVistaCompra();return false;}
    return true;
}
function registrarDesdeVistaPrevia() {
    if (guardandoCompra || extrayendoCompra || registrandoVistaOCR) return;
    if (!validarDocumentoVistaCompra()) return;
    confirmarAltasCompra(false);
}
var validandoSegundaRevision=false;
function mostrarSegundaRevisionCompra() {
    var panel=$('#datosSegundaRevision').empty(), proveedor=proveedorNuevoCompra || catalogoProveedores.find(function(p) {return Number(p.v)===Number($('#cboProveedor').val());}) || {};
    [['Proveedor',proveedor.razonSocial || proveedor.t],['RUC',proveedor.ruc || proveedor.codigo],['Tipo',$('#cboTipoComprobante option:selected').text()],['Serie y número',$('#txtNumeroComprobante').val()],['Fecha',$('#txtFechaCompra').val()],['Observación',$('#txtObservacion').val() || 'Sin observación']].forEach(function(c) {panel.append($('<div>').append($('<dt>').text(c[0]),$('<dd>').text(c[1] || 'Pendiente')));});
    $('#productosSegundaRevision').html(compraDetalles.map(function(d) {
        var p=d.nuevoProducto || catalogoProductos.find(function(p) {return Number(p.v)===Number(d.iCodProducto);}) || {};
        var nuevo=d.iCodProducto<0, venta=d.precioVenta!=null?d.precioVenta:p.precioVenta;
        var stock=Number(p.stock), operacion=nuevo?'Crear producto · Stock inicial: '+d.iCantidad:Number.isFinite(stock)?'Aumentar stock: '+stock+' + '+d.iCantidad+' = '+(stock+d.iCantidad):'Aumentar stock · se verificará al registrar';
        return '<tr><td>'+escaparCompra(d.cNombreProducto)+'<small>'+escaparCompra([p.marca,p.modelo,p.codigo].filter(Boolean).join(' · '))+'</small></td><td>'+Number(d.iCantidad)+'</td><td>'+formatoMoneda(d.nPrecioCompra)+'</td><td>'+(venta==null?'Pendiente':formatoMoneda(venta))+'</td><td>'+formatoMoneda(d.nSubTotal)+'</td><td>'+escaparCompra(operacion)+'</td></tr>';
    }).join(''));
    var totales=$('#totalesSegundaRevision').empty();
    [['Subtotal','#txtSubTotal'],['IGV','#txtIgv'],['Total','#txtTotal']].forEach(function(c) {var valor=$(c[1]).val();totales.append($('<div>').append($('<dt>').text(c[0]),$('<dd>').text(valor===''?'Pendiente':formatoMoneda(valor))));});
    $('#confirmarSegundaRevision').prop('checked',false);$('#confirmarOCR').prop('checked',false);
    $('#errorSegundaRevision').text('');$('#formularioCompletoCompra').addClass('en-segunda-revision');$('#segundaRevisionCompra').prop('hidden',false);
    document.getElementById('tituloSegundaRevision').focus();
    document.getElementById('segundaRevisionCompra').scrollIntoView({block:'start',behavior:'smooth'});
}
async function registrarSegundaRevisionCompra() {
    if (guardandoCompra || extrayendoCompra || validandoSegundaRevision) return;
    if (!$('#confirmarSegundaRevision').prop('checked')) {$('#errorSegundaRevision').text('Marca la confirmación después de revisar los datos de la compra.');return;}
    if (!$('#formularioCompletoCompra').hasClass('en-segunda-revision')) return;
    validandoSegundaRevision=true;$('#segundaRevisionCompra button, #confirmarSegundaRevision').prop('disabled',true);$('#errorSegundaRevision').text('Verificando comprobante y productos…');
    try {
        await listarProductosCombo();
        var duplicado=await $.ajax({type:'POST',url:'/Views/Operaciones/Compras/Compras.aspx/ComprobanteYaRegistrado',contentType:'application/json; charset=utf-8',dataType:'json',data:JSON.stringify({idProveedor:Number($('#cboProveedor').val()) || 0,ruc:proveedorNuevoCompra ? proveedorNuevoCompra.ruc : '',tipo:$('#cboTipoComprobante').val(),numero:$('#txtNumeroComprobante').val()})});
        if (duplicado.d) {$('#errorSegundaRevision').text('Este comprobante ya está registrado para el proveedor. No se duplicó la compra.');return;}
        if (compraDetalles.some(function(d) {return d.iCodProducto>0 && !catalogoProductos.some(function(p) {return Number(p.v)===d.iCodProducto;});})) {$('#errorSegundaRevision').text('Un producto ya no está disponible. Vuelve a editar la compra.');return;}
        $('#errorSegundaRevision').text('');$('#confirmarOCR').prop('checked',true);guardarCompra();
    } catch(e) {$('#errorSegundaRevision').text('No se pudo verificar el catálogo. Reintenta; no se envió la compra.');}
    finally {validandoSegundaRevision=false;if (!guardandoCompra) $('#segundaRevisionCompra button, #confirmarSegundaRevision').prop('disabled',false);}
}
$(function() {document.getElementById('modalAltasCompra').addEventListener('cancel',function(e) {if (guardandoCompra || registrandoVistaOCR) e.preventDefault();});});
$(document).on('change','#previewProveedor',actualizarProveedorVistaCompra)
.on('input change','#modalAltasCompra input, #modalAltasCompra select, #modalAltasCompra textarea',function() {
    $(this).removeAttr('aria-invalid aria-describedby').siblings('.ocr-campo-error').remove();
    $('#estadoVistaOCR').text('Pendiente de revisión').attr('class','ocr-etiqueta ocr-revision');
});

$(document).on('input','#altaRuc',sincronizarProveedorPorRuc)
.on('click','#productosAltaCampos .alta-ver-texto',function() {
    var fila=$(this).closest('.alta-producto'), indice=Number(fila.attr('data-indice')), detalle=compraDetalles[indice];
    mostrarReferenciaCompra('texto',(detalle && detalle.productoOcr || {}).textoOriginal || 'No hay texto original disponible para esta línea.','Producto '+(indice+1)+' · lectura original');
    document.getElementById('panelTextoOCR').scrollIntoView({block:'nearest',behavior:'smooth'});
})
.on('keydown','.ocr-referencia-tabs button',function(e) {
    if (e.key==='ArrowLeft' || e.key==='ArrowRight') {e.preventDefault();var otro=this.id==='tabDocumentoOCR'?'#tabTextoOCR':'#tabDocumentoOCR';$(otro).trigger('click').trigger('focus');}
});

function detallePrecioVentaCompra(fila,producto) {
    var detalle=compraDetalles[Number(fila.attr('data-indice'))];
    return detalle && Number(detalle.iCodProducto)===Number(producto.v) && detalle.precioVenta!=null ? detalle.precioVenta : producto.precioVenta;
}

var zoomImagenOCR=100;
function actualizarControlesZoomOCR() {
    $('#porcentajeZoomOCR').text(zoomImagenOCR+'%');
    $('#alejarImagenOCR').prop('disabled',zoomImagenOCR<=50);
    $('#acercarImagenOCR').prop('disabled',zoomImagenOCR>=400);
}
function cambiarZoomOCR(paso) {
    var panel=document.getElementById('archivoOriginalOCR'), img=panel.querySelector('img');
    if (!img) return;
    var anterior=zoomImagenOCR;
    zoomImagenOCR=Math.max(50,Math.min(400,zoomImagenOCR+paso));
    var centroX=panel.scrollLeft+panel.clientWidth/2, centroY=panel.scrollTop+panel.clientHeight/2;
    img.style.width=zoomImagenOCR+'%';
    panel.scrollLeft=centroX*zoomImagenOCR/anterior-panel.clientWidth/2;
    panel.scrollTop=centroY*zoomImagenOCR/anterior-panel.clientHeight/2;
    actualizarControlesZoomOCR();
}
function ajustarImagenOCR() {
    zoomImagenOCR=100;
    $('#archivoOriginalOCR img').css('width','100%');
    $('#archivoOriginalOCR').scrollTop(0).scrollLeft(0);
    actualizarControlesZoomOCR();
}

$(document).on('input','#previewNumero',function() {$('#confirmarNumeroOCR').prop('checked',false);});
// El registro manual utiliza el mismo guardado transaccional, sin archivo OCR.
function modoEntradaCompra(manual) {
    $('#cargaOCRCompra, #ayudaCargaOCRCompra').prop('hidden',manual);
    $('#formularioCompletoCompra').toggleClass('compra-manual',manual);
    $('#productosManualesCompra').prop('hidden',!manual);
    $('#indicacionCompraManual').prop('hidden',!manual);
    $('#btnCompraManual').attr('aria-pressed',String(manual)).toggleClass('btn-primary',manual).toggleClass('btn-secondary',!manual);
    $('#btnCompraOCR').attr('aria-pressed',String(!manual)).toggleClass('btn-primary',!manual).toggleClass('btn-secondary',manual);
}
function iniciarCompraManual() {
    if (guardandoCompra || extrayendoCompra || proveedorGuardando || proveedorCargando) return;
    var tieneArchivo=comprobanteOCRPendiente || $('#txtArchivoComprobante').val();
    if (tieneArchivo && !confirm('Se limpiarán los datos del comprobante OCR para iniciar una compra manual. ¿Continuar?')) return;
    if (tieneArchivo || $('#txtIdCompra').val()) limpiarFormulario();
    modoEntradaCompra(true);
    if (!$('#txtFechaCompra').val()) $('#txtFechaCompra').val(fechaCompraInput(new Date()));
    $('#cboProveedor').trigger('focus');
}
function mostrarCargaOCRCompra() {
    if (guardandoCompra || extrayendoCompra || proveedorGuardando || proveedorCargando) return;
    modoEntradaCompra(false);
    $('#txtArchivoComprobante').trigger('focus');
}
