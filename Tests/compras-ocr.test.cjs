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
        .replace(/<asp:ContentPlaceHolder\s+ID="head"[^]*?\/>/g, view.match(/<asp:Content\s+ID="Content1"[\s\S]*?>([\s\S]*?)<\/asp:Content>/)[1].replace('../../../JScript/Compras.js','/JScript/Compras.js'));
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
            if(pathname.endsWith('/ObtenerProveedorCompra')) data={id:3,ruc:'20123456789',razonSocial:'Proveedor',representante:'',telefono:'',correo:'',direccion:''};
            if(pathname.endsWith('/ActualizarProveedorCompra')) data={ok:true};
            if(pathname.endsWith('/ExtraerComprobante')) data={success:true,texto_ocr:'FACTURA ejemplo',datos:{ruc_emisor:'20123456789',numero_comprobante:'F001-000123',tipo_comprobante:'FACTURA',fecha_emision:'2026-10-13',subtotal:20,igv:3.6,total:23.6,advertencias:[],detalles:[{descripcion:'MARTILLO',cantidad:2,precio_unitario:10,importe:20}]}};
            if(pathname.endsWith('/CategoriasParaCompra')) data=[{v:'1',t:'Periféricos'}];
            if(pathname.endsWith('/GuardarCompra') || pathname.endsWith('/GuardarCompraConAltas')) {
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
        await page.getByRole('button',{name:'Modificar',exact:true}).click();
        await page.locator('#modalRevisionCompra').waitFor({state:'visible'});
        assert.equal(await page.locator('dialog').count(),1,'Una sola ventana de edición');
        for (const selector of ['#cboProveedor','#txtFechaCompra','#txtNumeroComprobante','#cboTipoComprobante','#txtObservacion','#txtSubtotalCompraGral','#txtIGVCompra','#txtTotalCompra','#btnGuardarCompra']) {
            assert.equal(await page.locator('#modalRevisionCompra '+selector).count(),1,'Campo incluido: '+selector);
        }
        await page.locator('#bodyDetalleCompra input[aria-label="Cantidad"]').fill('3');
        await page.locator('#bodyDetalleCompra input[aria-label="Cantidad"]').dispatchEvent('change');
        await page.getByRole('button',{name:'Cerrar revisión',exact:true}).click();
        await page.locator('#modalRevisionCompra').waitFor({state:'hidden'});
        assert.equal(await page.evaluate(()=>compraDetalles[0].iCantidad),3,'Cerrar conserva el borrador');
        await page.locator('#btnRevisarCompra').click();
        await page.locator('#bodyDetalleCompra input[aria-label="Cantidad"]').fill('2');
        await page.locator('#bodyDetalleCompra input[aria-label="Cantidad"]').dispatchEvent('change');
        for (const width of [1280,390]) {
            await page.setViewportSize({width,height:800});
            assert.ok(await page.locator('#modalRevisionCompra').evaluate(el=>el.scrollWidth<=el.clientWidth+1),'Sin desbordamiento de la ventana');
        }
        await page.setViewportSize({width:1280,height:900});
        await page.screenshot({path:path.join(root,'Tests/artifacts/revision-compra-completa.png')});
        await page.locator('#btnEditarProveedorCompra').click();
        await page.locator('#modalProveedorCompra').waitFor({state:'visible'});
        await page.locator('#razonProveedorModal').fill('Proveedor actualizado');
        await page.locator('#guardarProveedorModal').click();
        await page.locator('#modalProveedorCompra').waitFor({state:'hidden'});
        assert.equal(await page.locator('#cboProveedor option:checked').textContent(),'Proveedor actualizado');
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
        await page.locator('#modalAltasCompra').waitFor({state:'visible'});
        assert.equal(saved.length,2,'Preparar no guarda registros');
        await page.locator('#modalAltasCompra').getByRole('button',{name:'Cancelar'}).click();
        assert.equal(await page.evaluate(()=>compraDetalles[0].iCodProducto),0,'Cancelar no crea ni asigna un producto');
        await page.getByRole('button',{name:'Revisar proveedor y productos nuevos'}).click();
        await page.locator('#modalAltasCompra').waitFor({state:'visible'});
        await page.locator('#altaRuc').fill('20600855604');
        await page.locator('#altaRazon').fill('Proveedor nuevo de prueba');
        await page.locator('.alta-categoria').selectOption('1');
        await page.locator('.alta-venta').fill('15');
        await page.screenshot({path:path.join(root,'Tests/artifacts/modal-altas-compra.png')});
        await page.locator('#modalAltasCompra').getByRole('button',{name:'Usar estos datos'}).click();
        assert.equal(await page.locator('#cboProveedor').inputValue(),'-1');
        assert.equal(await page.evaluate(()=>compraDetalles[0].iCodProducto),-1);
        assert.equal(saved.length,2,'Confirmar fichas aún no escribe en SQL');
        await page.locator('#btnEditarProveedorCompra').click();
        await page.locator('#modalAltasCompra').waitFor({state:'visible'});
        assert.equal(await page.locator('#altaRuc').inputValue(),'20600855604');
        await page.locator('#modalAltasCompra').getByRole('button',{name:'Cancelar'}).click();
        await page.evaluate(()=>{
            $('#txtNumeroComprobante').val('F001-NUEVA'); $('#txtFechaCompra').val('2026-10-18'); $('#cboTipoComprobante').val('FACTURA');
            $('#txtSubtotalCompraGral').val('10');$('#txtIGVCompra').val('1.8');$('#txtTotalCompra').val('11.8');actualizarMontosCompra();
        });
        await page.locator('#confirmarOCR').check();
        await page.locator('#btnGuardarCompra').click();
        await page.waitForFunction(()=>!guardandoCompra);
        assert.equal(saved.length,3);
        assert.equal(saved[2].proveedorNuevo.ruc,'20600855604');
        assert.equal(saved[2].productosNuevos[0].categoria,1);
        assert.equal(saved[2].productosNuevos[0].precioVenta,15);
        assert.deepEqual(errors,[]);
        console.log('OK: foto, autollenado, inventario, solicitud atómica, doble clic, error recuperable y limpieza.');
    } finally { await browser.close(); }
})().catch(e => { console.error(e); process.exitCode=1; });
