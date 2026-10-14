const { chromium } = require('playwright');
const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');
(async () => {
    const root = path.resolve(__dirname, '..');
    const read = name => fs.readFileSync(path.join(root, name), 'utf8').replace(/<%=\s*ResolveUrl\("~([^"]+)"\)\s*%>/g, '$1');
    const view = read('Views/Operaciones/Compras/Compras.aspx');
    const body = view.match(/<asp:Content\s+ID="Content2"[\s\S]*?>([\s\S]*?)<\/asp:Content>/)[1];
    const html = read('Views/Master/Site.Master').replace(/^<body.*$/m, '<body data-rol="Administrador">').replace(/<%@[^]*?%>/g, '')
        .replace(/<asp:ContentPlaceHolder\s+ID="ContentPlaceHolder1"[^]*?\/>/, body)
        .replace(/<asp:ContentPlaceHolder\s+ID="head"[^]*?\/>/g, '<script src="/JScript/Compras.js"></script>');
    const browser = await chromium.launch({ channel: 'msedge', headless: true });
    try {
        const page = await browser.newPage();
        const errors = [], alerts = [], saved = [];
        let failSave = true;
        page.on('pageerror', e => { errors.push(e.message); console.error(e.message); });
        page.on('dialog', d => { alerts.push(d.message()); d.accept(); });
        await page.route('http://keyove.test/**', async route => {
            const request = route.request(), pathname = new URL(request.url()).pathname;
            if(request.resourceType() === 'document') return route.fulfill({contentType:'text/html', body:html});
            if(/\.(js|css)$/.test(pathname)) return route.fulfill({contentType:pathname.endsWith('.js')?'text/javascript':'text/css',body:read(pathname.slice(1))});
            let data = [];
            if(pathname.endsWith('/ListarProductosCombo')) data=[{v:'1',t:'Martillo',codigo:'M001',stock:5}];
            if(pathname.endsWith('/ListarProveedoresCombo')) data=[{v:'3',t:'Proveedor',codigo:'20123456789'}];
            if(pathname.endsWith('/ListarUsuariosCombo')) data=[{v:'2',t:'Dueña'}];
            if(pathname.endsWith('/ExtraerComprobante')) data={success:true,texto_ocr:'FACTURA ejemplo',datos:{ruc_emisor:'20123456789',numero_comprobante:'F001-000123',tipo_comprobante:'FACTURA',fecha_emision:'2026-10-13',subtotal:20,igv:3.6,total:23.6,advertencias:[],detalles:[{descripcion:'MARTILLO',cantidad:2,precio_unitario:10,importe:20}]}};
            if(pathname.endsWith('/GuardarCompra')) {
                saved.push(request.postDataJSON());
                data=failSave?{ok:false,mensaje:'Fallo simulado, no se guardó nada.'}:{ok:true,idCompra:42};
            }
            return route.fulfill({contentType:'application/json',body:JSON.stringify({d:data})});
        });
        await page.goto('http://keyove.test/Views/Operaciones/Compras/Compras.aspx');
        await page.locator('#cboProducto option[value="1"]').waitFor({state:'attached'});
        await page.locator('#txtArchivoComprobante').setInputFiles({name:'factura.png',mimeType:'image/png',buffer:Buffer.from('imagen de prueba')});
        await page.locator('#revisionOCR').waitFor({state:'visible'});
        assert.equal(await page.locator('#txtNumeroComprobante').inputValue(),'F001-000123');
        assert.equal(await page.locator('#txtFechaCompra').inputValue(),'2026-10-13');
        assert.equal(await page.locator('#cboProveedor').inputValue(),'3');
        assert.equal(await page.locator('#bodyDetalleCompra select').inputValue(),'1');
        assert.equal(await page.locator('#txtTotal').inputValue(),'23.6');
        await page.locator('#confirmarOCR').check();
        await page.evaluate(() => { guardarCompra(); guardarCompra(); });
        await page.waitForFunction(() => !guardandoCompra);
        assert.equal(saved.length,1,'Doble clic genera una sola solicitud');
        assert.equal(await page.locator('#txtNumeroComprobante').inputValue(),'F001-000123','Conserva borrador ante error');
        assert.equal(saved[0].comprobante.nombreArchivo,'factura.png');
        assert.ok(saved[0].comprobante.contenidoBase64);
        assert.equal(saved[0].detalles[0].iCodProducto,1);
        assert.equal(saved[0].fechaCompra,'2026-10-13');
        failSave=false;
        await page.locator('#btnGuardarCompra').click();
        await page.waitForFunction(() => !guardandoCompra);
        assert.equal(saved.length,2);
        assert.equal(await page.locator('#txtNumeroComprobante').inputValue(),'');
        assert.equal(await page.locator('#txtArchivoComprobante').inputValue(),'');
        assert.equal(await page.evaluate(() => comprobanteOCRPendiente),null);
        await page.evaluate(() => {
            aplicarOCRCompra({texto_ocr:'texto',datos:{detalles:[{descripcion:'Producto desconocido',cantidad:1,precio_unitario:10}],advertencias:[]}}, {nombreArchivo:'otro.png',contenidoBase64:'YQ=='});
        });
        assert.equal(await page.locator('#bodyDetalleCompra select').inputValue(),'0','No inventa correspondencia de producto');
        assert.equal(await page.locator('#txtTotal').inputValue(),'','Monto ausente no se convierte en cero');
        assert.deepEqual(errors,[]);
        console.log('OK: foto, autollenado, inventario, solicitud atómica, doble clic, error recuperable y limpieza.');
    } finally { await browser.close(); }
})().catch(e => { console.error(e); process.exitCode=1; });
