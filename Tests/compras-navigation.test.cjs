const { chromium } = require('playwright');
const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');

(async () => {
    const root = path.resolve(__dirname, '..');
    const read = name => fs.readFileSync(path.join(root, name), 'utf8');
    const master = read('Views/Master/Site.Master');
    const view = read('Views/Operaciones/Compras/Compras.aspx');
    const content = view.match(/<asp:Content\s+ID="Content2"[\s\S]*?>([\s\S]*?)<\/asp:Content>/)[1];
    const html = master.replace(/<%@[^]*?%>/g, '')
        .replace(/<asp:ContentPlaceHolder\s+ID="ContentPlaceHolder1"[^]*?<\/asp:ContentPlaceHolder>/, content)
        .replace(/<asp:ContentPlaceHolder[^]*?<\/asp:ContentPlaceHolder>/g, '<script src="/JScript/Compras.js"></script>');
    const browser = await chromium.launch({ channel: 'msedge', headless: true });
    try {
        const page = await browser.newPage();
        const errors = [];
        page.on('pageerror', error => errors.push(error.message));
        const jquery = await page.request.get('https://code.jquery.com/jquery-3.7.1.min.js');
        assert.ok(jquery.ok(), 'jQuery disponible');
        const jqueryBody = await jquery.text();
        await page.route('https://code.jquery.com/**', route => route.fulfill({ contentType: 'text/javascript', body: jqueryBody }));
        let documents = 0;
        await page.route('http://keyove.test/**', async route => {
            const request = route.request();
            const pathname = new URL(request.url()).pathname;
            if (request.resourceType() === 'document') {
                documents++;
                return route.fulfill({ contentType: 'text/html', body: html });
            }
            if (pathname.endsWith('.css') || pathname.endsWith('.js')) {
                return route.fulfill({ contentType: pathname.endsWith('.css') ? 'text/css' : 'text/javascript', body: read(pathname.slice(1)) });
            }
            let data = [];
            if (pathname.endsWith('Combo')) data = [{ v: '1', t: 'Opción de prueba' }];
            if (pathname.endsWith('/ObtenerCompra')) data = { iCodCompra: 1, iCodProveedor: 1, iCodUsuario: 1, cTipoComprobante: 'BOLETA', cNumeroComprobante: 'BO-2900000', nSubTotal: 10, nIgv: 1.8, nTotal: 11.8, cEstado: 'REGISTRADA', cObservacion: '' };
            if (pathname.endsWith('/ListarCompras')) data = [{ iCodCompra: 1, cNombreUsuario: 'José', cTipoComprobante: 'BOLETA', cNumeroComprobante: 'BO-2900000', cDocumentoCliente: '12345678', cNumeroCelular: '999999999', dFechaCompra: '/Date(1750000000000)/', nSubTotal: 10, nIgv: 1.8, nTotal: 11.8, cMetodoPago: 'EFECTIVO', cEstado: 'REGISTRADA' }];
            return route.fulfill({ contentType: 'application/json', body: JSON.stringify({ d: data }) });
        });
        await page.goto('http://keyove.test/Views/Operaciones/Compras/Compras.aspx');
        await page.locator('#cboUsuario option[value="1"]').waitFor({ state: 'attached' });
        await page.locator('#btnOcultarPanel').click();
        assert.equal(await page.locator('#btnMenuToggle').getAttribute('aria-expanded'), 'false');
        assert.equal(await page.locator('#textoPanelControl').textContent(), 'Mostrar panel');
        assert.equal(await page.evaluate(() => localStorage.getItem('keyoveMenuContraido')), 'true');
        await page.locator('#btnMenuToggle').click();
        assert.equal(await page.locator('#btnMenuToggle').getAttribute('aria-expanded'), 'true');
        await page.locator('#txtObservacion').fill('87654321');
        await page.locator('.ventas-vistas [data-vista-compras="registros"]').click();
        await page.locator('#bodyCompras tr').waitFor();
        await page.locator('#buscarCompras').fill('jose BO-2900000');
        assert.equal(await page.locator('#bodyCompras tr:visible').count(), 1);
        await page.locator('#buscarCompras').fill('inexistente');
        assert.equal(await page.locator('#bodyCompras tr:visible').count(), 0);
        await page.locator('#menuCompras [data-vista-compras="registro"]').click();
        assert.equal(await page.locator('#txtObservacion').inputValue(), '87654321');
        await page.goBack();
        assert.equal(await page.locator('#panelComprasRegistradas').isVisible(), true);
        assert.equal(documents, 1, 'Cambiar secciones y retroceder no recarga el documento');
        for (const width of [1366, 1024, 768, 390, 320]) {
            await page.setViewportSize({ width, height: 800 });
            for (const vista of ['registro', 'registros']) {
                await page.locator(`.ventas-vistas [data-vista-compras="${vista}"]`).click();
                assert.ok(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth), `Ancho ${width}, ${vista}`);
            }
        }
                await page.locator('.ventas-vistas [data-vista-compras="registros"]').click();
        await page.locator('#buscarCompras').fill('');
        await page.locator('#bodyCompras tr').waitFor();
        var accion = await page.locator('#bodyCompras tr:first-child td:last-child').boundingBox();
        assert.ok(accion.x >= 0 && accion.x + accion.width <= 320, 'Acciones visibles en móvil');
        await page.locator('#bodyCompras tr:first-child button').first().click();
        assert.equal(await page.locator('#panelRegistroCompra').isVisible(), true, 'Editar abre el formulario sin navegación');
        assert.equal(documents, 1);
        assert.deepEqual(errors, []);
        console.log('OK: navegación AJAX, borrador conservado, búsqueda, historial y 5 anchos sin desbordamiento.');
    } finally { await browser.close(); }
})().catch(error => { console.error(error); process.exitCode = 1; });
