const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const path = require('node:path');

for (const tipo of ['Compra', 'Venta']) {
    const campos = new Map();
    const solicitudes = [];
    const document = { getElementById: () => ({ scrollIntoView() {} }) };
    function $(selector) {
        if (selector === document) return { ready() {} };
        if (!campos.has(selector)) campos.set(selector, { valor: '', texto: '', html: '', props: {} });
        const campo = campos.get(selector);
        const control = {
            val(v) { if (arguments.length === 0) return campo.valor; campo.valor = String(v); return control; },
            text(v) { campo.texto = String(v); campo.html = campo.texto.replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;'); return control; },
            html(v) { if (arguments.length === 0) return campo.html; campo.html = v; return control; },
            prop(k, v) { campo.props[k] = v; return control; },
            empty() { campo.html = ''; return control; },
            trigger() { return control; }
        };
        return control;
    }
    $.ajax = options => solicitudes.push(options);
    const contexto = vm.createContext({ $, document, console, alert() {}, confirm: () => true });
    vm.runInContext(fs.readFileSync(path.join(__dirname, '../JScript/Detalle_' + tipo + '.js'), 'utf8'), contexto);

    $('#cbo' + tipo).val('7');
    contexto.limpiarFormulario();
    assert.equal($('#cbo' + tipo).val(), '7', 'Limpiar conserva el comprobante');
    assert.equal($('#txtCantidad').val(), '1');
    $('#cboProducto').val('2');
    $('#txtPrecio' + tipo).val('10');
    $('#txtCantidad').val('1.5');
    contexto.enviarDetalle();
    assert.equal(solicitudes.length, 0, 'No permite cantidades fraccionarias');
    $('#txtCantidad').val('2');
    if (tipo === 'Venta') {
        $('#txtDescuento').val('21');
        contexto.enviarDetalle();
        assert.equal(solicitudes.length, 0, 'Rechaza descuentos mayores al importe');
        $('#txtDescuento').val('3');
    }
    contexto.enviarDetalle();
    const guardar = solicitudes[0];
    assert.ok(guardar.url.endsWith('/GuardarDetalle' + tipo));
    assert.equal(JSON.parse(guardar.data).detalle.nSubTotal, tipo === 'Venta' ? 17 : 20);
    contexto.enviarDetalle();
    assert.equal(solicitudes.length, 1, 'Bloquea el doble envío');
    guardar.success({ d: 'OK' });
    guardar.complete();
    assert.equal($('#cbo' + tipo).val(), '7');
    const listar = solicitudes[1];
    listar.success({ d: [
        { ['iCod' + tipo]: 7, ['iCodDetalle' + tipo]: 11, cCodigo: 'P1', cNombreProducto: '<Producto>', iCantidad: 2, ['nPrecio' + tipo]: 10, nDescuento: 0, nSubTotal: 20 },
        { ['iCod' + tipo]: 8, cNombreProducto: 'OTRO COMPROBANTE' }
    ] });
    const html = $('#bodyDetalle' + tipo).html();
    assert.ok(html.includes('&lt;Producto&gt;'));
    assert.ok(html.includes('10.00'));
    assert.ok(!html.includes('OTRO COMPROBANTE'));
    $('#txtIdDetalle' + tipo).val('11');
    $('#cboProducto').val('2');
    $('#txtPrecio' + tipo).val('10');
    contexto.enviarDetalle();
    assert.ok(solicitudes.at(-1).url.endsWith('/ModificarDetalle' + tipo));
    console.log(tipo + ': validación, cálculo, filtro, edición y doble envío OK');
}
