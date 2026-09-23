const { chromium } = require('playwright');
const fs = require('node:fs');
const assert = require('node:assert/strict');
const path = require('node:path');

(async () => {
    const root = path.resolve(__dirname, '..');
    const master = fs.readFileSync(path.join(root, 'Views/Master/Site.Master'), 'utf8');
    const inline = master.match(/<script>([\s\S]*?)<\/script>/)[1];
    new Function(inline);
    const css = fs.readFileSync(path.join(root, 'Assets/css/site.css'), 'utf8');
    const browser = await chromium.launch({ channel: 'msedge', headless: true });
    try {
        const page = await browser.newPage();
        for (const name of ['Ventas']) {
            const view = fs.readFileSync(path.join(root, `Views/Operaciones/Ventas/${name}.aspx`), 'utf8');
            const content = view.match(/<asp:Content\s+ID="Content2"[\s\S]*?>([\s\S]*?)<\/asp:Content>/)[1];
            let html = master.replace(/<%@[^]*?%>/g, '')
                .replace(/<asp:ContentPlaceHolder\s+ID="ContentPlaceHolder1"[^]*?<\/asp:ContentPlaceHolder>/, content)
                .replace(/<asp:ContentPlaceHolder[^]*?<\/asp:ContentPlaceHolder>/g, '')
                .replace(/<script[^]*?<\/script>/g, '').replace(/<link[^>]*>/g, '')
                .replace('</head>', `<style>${css}</style></head>`);
            await page.setContent(html);
            await page.locator('#bodyVentas').evaluateAll(elements => elements.forEach(element => {
                element.innerHTML = '<tr>' + '<td>Comprobante BO-2900000</td>'.repeat(13) + '</tr>';
            }));
            for (const width of [1366, 1024, 768, 390, 320]) {
                await page.setViewportSize({ width, height: 800 });
                for (const collapsed of [false, true]) {
                    await page.locator('.contenedor').evaluate((element, value) => element.classList.toggle('menu-contraido', value), collapsed);
                    const sizes = await page.evaluate(() => ({ width: innerWidth, scroll: document.documentElement.scrollWidth }));
                    assert.ok(sizes.scroll <= sizes.width, `${name}: desbordamiento a ${width}px, contraído=${collapsed}`);
                }
            }
            await page.locator('#menuVentas').evaluate(element => { element.open = true; });
            assert.equal(await page.locator('#menuVentas a').count(), 2);
            console.log(`${name}: sin desbordamiento en 5 anchos y ambos estados del menú`);
        }
    } finally { await browser.close(); }
})().catch(error => { console.error(error); process.exitCode = 1; });
