const { chromium } = require('playwright');
const fs = require('node:fs'), path = require('node:path'), assert = require('node:assert/strict');
(async () => {
    const root = path.resolve(__dirname, '..');
    const read = name => fs.readFileSync(path.join(root, name), 'utf8').replace(/<%=\s*ResolveUrl\("~([^"]+)"\)\s*%>/g, '$1');
    const view = read('Views/Operaciones/Compras/Compras.aspx');
    const content = id => view.match(new RegExp('<asp:Content\\s+ID="'+id+'"[\\s\\S]*?>([\\s\\S]*?)<\\/asp:Content>'))[1];
    const html = read('Views/Master/Site.Master').replace(/^<body.*$/m,'<body data-rol="Administrador">').replace(/<%@[^]*?%>/g,'')
        .replace(/<asp:ContentPlaceHolder\s+ID="ContentPlaceHolder1"[^]*?\/>/,content('Content2'))
        .replace(/<asp:ContentPlaceHolder\s+ID="head"[^]*?\/>/g,content('Content1').replace('../../../JScript/Compras.js','/JScript/Compras.js'));
    const browser = await chromium.launch({channel:'msedge',headless:true});
    try {
        const page = await browser.newPage(), errors = [], saved = [];
        let failSave=true, duplicate=false;
        let invoice={success:true,texto_ocr:'FACTURA ejemplo',datos:{campos_por_confirmar:{numero_comprobante:{valor:'F001-000123',alternativas:['F001-000123','F001-000128']}},ruc_emisor:'20123456789',numero_comprobante:'F001-000123',tipo_comprobante:'FACTURA',fecha_emision:'2026-10-13',subtotal:20,igv:3.6,total:23.6,advertencias:[],detalles:[{descripcion:'MARTILLO',cantidad:2,precio_unitario:10,importe:20}]}};
        page.on('pageerror',e=>errors.push(e.message));
        page.on('dialog',d=>d.accept());
        await page.route('http://keyove.test/**',async route=>{
            const request=route.request(), url=new URL(request.url()).pathname;
            if(request.resourceType()==='document') return route.fulfill({contentType:'text/html',body:html});
            if(/\.(js|css)$/.test(url)) return route.fulfill({contentType:url.endsWith('.js')?'text/javascript':'text/css',body:read(url.slice(1))});
            let data=[];
            if(url.endsWith('/ListarProductosCombo')) data=[{v:'1',t:'Martillo',codigo:'M001',stock:5,precioVenta:15}];
            if(url.endsWith('/ListarProveedoresCombo')) data=[{v:'3',t:'Proveedor',codigo:'20123456789'}];
            if(url.endsWith('/ListarUsuariosCombo')) data=[{v:'2',t:'Dueña'}];
            if(url.endsWith('/MarcasParaCompra')) data=[{v:'Forza',t:'Forza'}];
            if(url.endsWith('/CategoriasParaCompra')) data=[{v:'1',t:'Periféricos'}];
            if(url.endsWith('/ComprobanteYaRegistrado')) data=duplicate;
            if(url.endsWith('/ExtraerComprobante')) data=invoice;
            if(/\/(GuardarCompra|GuardarCompraConAltas)$/.test(url)) {
                saved.push(request.postDataJSON());data=failSave?{ok:false,mensaje:'Fallo simulado, no se guardó nada.'}:{ok:true,idCompra:42};
            }
            return route.fulfill({contentType:'application/json',body:JSON.stringify({d:data})});
        });
        await page.goto('http://keyove.test/Views/Operaciones/Compras/Compras.aspx');
        await page.waitForFunction(()=>catalogoProductos.length>0);
        const image={name:'factura.png',mimeType:'image/png',buffer:Buffer.from('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+j4i8AAAAASUVORK5CYII=','base64')};
        const apply=async()=>{if(await page.locator('#revisionNumeroOCR').isVisible())await page.locator('#confirmarNumeroOCR').check();await page.getByRole('button',{name:'Aplicar cambios',exact:true}).click();};
        const register=()=>page.getByRole('button',{name:'Confirmar y registrar compra',exact:true}).click();
        const wait=()=>page.waitForFunction(()=>!guardandoCompra&&!validandoSegundaRevision);
        assert.equal(await page.locator('#segundaRevisionCompra').isVisible(),false);
        await page.locator('#txtArchivoComprobante').setInputFiles(image);
        await page.locator('#modalAltasCompra').waitFor({state:'visible'});
        await page.locator('#archivoOriginalOCR img').evaluate(img=>img.decode());
        await page.getByRole('button',{name:'Acercar imagen',exact:true}).click();
        assert.equal(await page.locator('#porcentajeZoomOCR').textContent(),'125%');
        assert.ok(await page.locator('#archivoOriginalOCR').evaluate(el=>el.scrollWidth>el.clientWidth),'El zoom conserva desplazamiento independiente');
        await page.locator('#tabTextoOCR').click();
        assert.equal(await page.locator('#controlesZoomOCR').isVisible(),false);
        await page.locator('#tabDocumentoOCR').click();
        await page.getByRole('button',{name:'Ajustar',exact:true}).click();
        assert.equal(await page.locator('#porcentajeZoomOCR').textContent(),'100%');
        assert.equal(await page.locator('.alta-venta').isEditable(),true);
        await page.locator('.alta-venta').fill('19.90');await page.getByRole('button',{name:'Aplicar cambios',exact:true}).click();
        assert.equal(await page.locator('#modalAltasCompra').isVisible(),true);
        assert.equal(await page.locator('#previewNumero').getAttribute('aria-invalid'),'true','El número propuesto exige confirmación');
        await apply();
        await page.locator('#segundaRevisionCompra').waitFor({state:'visible'});
        assert.equal(saved.length,0,'Aplicar cambios no registra la compra');
        assert.equal(await page.locator('#modalAltasCompra').isVisible(),false);
        assert.match(await page.locator('#datosSegundaRevision').textContent(),/Proveedor.*20123456789.*F001-000123/);
        assert.match(await page.locator('#productosSegundaRevision').textContent(),/Martillo.*19.90/);
        assert.equal(await page.locator('#bodyDetalleCompra').isVisible(),false);
        await register();assert.match(await page.locator('#errorSegundaRevision').textContent(),/Marca la confirmación/);assert.equal(saved.length,0);
        await page.getByRole('button',{name:'Volver a editar',exact:true}).click();
        await page.locator('#modalAltasCompra').waitFor({state:'visible'});
        assert.equal(await page.locator('.alta-venta').inputValue(),'19.9');
        await page.locator('.alta-cantidad').fill('3');await page.locator('#previewSubtotal').fill('30');await page.locator('#previewIgv').fill('5.4');await page.locator('#previewTotal').fill('35.4');await page.locator('#previewObservacion').fill('Revisada dos veces');await apply();
        assert.equal(await page.locator('#confirmarSegundaRevision').isChecked(),false);
        assert.equal(await page.locator('#productosSegundaRevision tr td').nth(1).textContent(),'3');
        assert.match(await page.locator('#totalesSegundaRevision').textContent(),/35.40/);
        await page.getByRole('button',{name:'Volver a editar',exact:true}).click();
        await page.locator('#modalAltasCompra').waitFor({state:'visible'});await page.locator('.alta-cantidad').fill('9');
        await page.locator('#modalAltasCompra').getByRole('button',{name:'Cancelar',exact:true}).click();
        assert.equal(await page.locator('#productosSegundaRevision tr td').nth(1).textContent(),'3','Cancelar conserva los datos aplicados');
        for(const width of [1366,390]) {
            await page.setViewportSize({width,height:900});
            if(width<640) await page.evaluate(()=>{const b=document.getElementById('btnMenuToggle');if(b&&b.getAttribute('aria-expanded')==='true')b.click();});
            assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth));
            await page.screenshot({path:path.join(root,'Tests/artifacts/segunda-revision-'+width+'.png'),fullPage:true});
        }
        await page.setViewportSize({width:1280,height:900});await page.locator('#confirmarSegundaRevision').check();
        duplicate=true;await register();await wait();assert.equal(saved.length,0);assert.match(await page.locator('#errorSegundaRevision').textContent(),/ya está registrado/);
        duplicate=false;await page.evaluate(()=>{registrarSegundaRevisionCompra();registrarSegundaRevisionCompra();});await wait();
        assert.equal(saved.length,1,'Doble confirmación envía una sola solicitud');
        assert.equal(saved[0].detalles[0].precioVenta,19.9);assert.equal(saved[0].detalles[0].iCantidad,3);assert.equal(saved[0].compra.cObservacion,'Revisada dos veces');
        assert.equal(await page.locator('#segundaRevisionCompra').isVisible(),true);assert.match(await page.locator('#errorSegundaRevision').textContent(),/Fallo simulado/);
        failSave=false;await register();await wait();assert.equal(saved.length,2);assert.equal(await page.locator('#segundaRevisionCompra').isVisible(),false);
        invoice={success:true,texto_ocr:'4 REGLETAS FORZA',datos:{ruc_emisor:'20600855604',razon_social:'Proveedor nuevo',numero_comprobante:'F001-NUEVA',tipo_comprobante:'FACTURA',fecha_emision:'2026-10-13',subtotal:40,igv:7.2,total:47.2,advertencias:[],detalles:[{descripcion:'REGLETA FORZA FZ-001',nombre:'Regleta',marca:'Forza',modelo:'FZ-001',cantidad:4,precio_unitario:10,importe:40}]}};
        await page.locator('#txtArchivoComprobante').setInputFiles(image);await page.locator('#modalAltasCompra').waitFor({state:'visible'});
        await page.locator('.alta-categoria').selectOption('1');await page.locator('.alta-venta').fill('20');await apply();
        await page.locator('#segundaRevisionCompra').waitFor({state:'visible'});assert.equal(saved.length,2);
        assert.match(await page.locator('#productosSegundaRevision').textContent(),/Crear producto.*Stock inicial: 4/);
        await page.locator('#confirmarSegundaRevision').check();await register();await wait();assert.equal(saved.length,3);
        assert.equal(saved[2].proveedorNuevo.ruc,'20600855604');assert.equal(saved[2].productosNuevos[0].marca,'Forza');assert.equal(saved[2].productosNuevos[0].precioVenta,20);
        assert.deepEqual(errors,[]);
        console.log('OK: segunda revisión, edición, cancelación, duplicados, doble clic, recuperación de errores, precios y altas.');
    } finally {await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
