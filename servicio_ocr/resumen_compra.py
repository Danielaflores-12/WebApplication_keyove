from datetime import date
from decimal import Decimal
from io import BytesIO
from xml.sax.saxutils import escape

from pydantic import BaseModel, Field
from reportlab.lib import colors
from reportlab.lib.enums import TA_RIGHT
from reportlab.lib.pagesizes import A4, landscape
from reportlab.lib.styles import ParagraphStyle
from reportlab.platypus import LongTable, Paragraph, SimpleDocTemplate, Spacer, Table, TableStyle


class ProductoResumen(BaseModel):
    nombre: str
    referencia: str = ''
    cantidad: int
    precioCompra: Decimal
    precioVentaActual: Decimal | None = None
    importe: Decimal
    stockAnterior: int | None = None
    stockNuevo: int | None = None


class ResumenCompra(BaseModel):
    id: int
    proveedor: str
    ruc: str
    tipo: str
    numero: str
    fecha: date
    estado: str
    observacion: str = ''
    subtotal: Decimal
    igv: Decimal
    total: Decimal
    productos: list[ProductoResumen] = Field(max_length=2000)


AZUL = colors.HexColor('#254e82')
TEXTO = colors.HexColor('#172c46')
SECUNDARIO = colors.HexColor('#4a6380')
FONDO = colors.HexColor('#eef4fa')
BORDE = colors.HexColor('#d7e2ef')
TIPOS = {'BOLETA': 'Boleta de venta', 'FACTURA': 'Factura', 'PEDIDO': 'Pedido',
         'TICKET': 'Ticket', 'NOTA_VENTA': 'Nota de venta', 'OTRO': 'Otro documento'}


def moneda(valor):
    return f'S/ {valor:,.2f}' if valor is not None else '—'


def generar_resumen(compra: ResumenCompra) -> bytes:
    salida = BytesIO()
    doc = SimpleDocTemplate(salida, pagesize=landscape(A4), title='Resumen de compra',
                            author='Keyove', leftMargin=36, rightMargin=36,
                            topMargin=32, bottomMargin=38)
    ancho = doc.width - 12
    etiquetas = ParagraphStyle('Etiqueta', fontSize=9, leading=12, textColor=SECUNDARIO)
    valores = ParagraphStyle('Valor', fontName='Helvetica-Bold', fontSize=11, leading=15, textColor=TEXTO)
    celda = ParagraphStyle('Celda', fontSize=10, leading=14, textColor=TEXTO)
    numero = ParagraphStyle('Importe', parent=celda, alignment=TA_RIGHT)
    cabecera = ParagraphStyle('Cabecera', parent=celda, fontName='Helvetica-Bold', textColor=AZUL)
    total = ParagraphStyle('Total', parent=valores, fontSize=17, leading=23, textColor=AZUL)
    titulo = ParagraphStyle('Título', parent=valores, fontSize=23, leading=29)

    def texto(valor, estilo):
        return Paragraph(escape(str(valor or '—')).replace('\n', '<br/>'), estilo)

    def campo(etiqueta, valor, estilo=valores):
        return [texto(etiqueta, etiquetas), texto(valor, estilo)]

    def pie(canvas, documento):
        canvas.setStrokeColor(BORDE)
        canvas.line(42, 29, documento.pagesize[0] - 42, 29)
        canvas.setFont('Helvetica', 8)
        canvas.setFillColor(SECUNDARIO)
        canvas.drawString(42, 17, 'KEYOVE')
        canvas.drawRightString(documento.pagesize[0] - 42, 17, f'Página {documento.page}')

    info = Table([
        [campo('Proveedor', compra.proveedor), campo('RUC', compra.ruc),
         campo('Tipo', TIPOS.get(compra.tipo, compra.tipo))],
        [campo('Serie y número', compra.numero), campo('Fecha', compra.fecha.strftime('%d/%m/%Y')),
         campo('Observación', compra.observacion or 'Sin observación')],
    ], colWidths=[ancho * .41, ancho * .23, ancho * .36], hAlign='LEFT')
    info.setStyle(TableStyle([('VALIGN', (0, 0), (-1, -1), 'TOP'),
                              ('LEFTPADDING', (0, 0), (-1, -1), 0),
                              ('TOPPADDING', (0, 0), (-1, -1), 7),
                              ('BOTTOMPADDING', (0, 0), (-1, -1), 7)]))
    filas = [[texto(etiqueta, cabecera) for etiqueta in (
        'Producto', 'Cantidad', 'Precio compra', 'Precio venta actual', 'Importe', 'Stock registrado')]]
    for producto in compra.productos:
        stock = '—'
        if producto.stockAnterior is not None and producto.stockNuevo is not None:
            stock = f'{producto.stockAnterior} + {producto.cantidad} = {producto.stockNuevo}'
        ficha = [texto(producto.nombre, celda)]
        if producto.referencia:
            ficha.append(texto(producto.referencia, etiquetas))
        filas.append([ficha, texto(str(producto.cantidad), numero), texto(moneda(producto.precioCompra), numero),
                      texto(moneda(producto.precioVentaActual), numero), texto(moneda(producto.importe), numero),
                      texto(stock, celda)])
    if not compra.productos:
        filas.append([texto('Sin productos registrados', celda), '', '', '', '', ''])
    tabla = LongTable(filas, colWidths=[ancho * proporcion for proporcion in (.28, .09, .135, .15, .14, .205)],
                      repeatRows=1, hAlign='LEFT')
    tabla.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), FONDO), ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, FONDO]),
        ('LINEBELOW', (0, 0), (-1, 0), 0.8, BORDE), ('LINEBELOW', (0, 1), (-1, -1), 0.45, BORDE),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'), ('LEFTPADDING', (0, 0), (-1, -1), 10),
        ('RIGHTPADDING', (0, 0), (-1, -1), 10), ('TOPPADDING', (0, 0), (-1, -1), 10),
        ('BOTTOMPADDING', (0, 0), (-1, -1), 10),
    ]))
    importes = [campo('Total', moneda(compra.total), total)]
    if compra.tipo != 'PEDIDO' or compra.igv != 0 or compra.subtotal != compra.total:
        importes = [campo('Subtotal', moneda(compra.subtotal)), campo('IGV', moneda(compra.igv))] + importes
    banda = Table([([''] * (4 - len(importes))) + importes], colWidths=[ancho - 330, 105, 105, 120], hAlign='LEFT')
    banda.setStyle(TableStyle([('BACKGROUND', (0, 0), (-1, -1), FONDO),
                               ('VALIGN', (0, 0), (-1, -1), 'TOP'),
                               ('TOPPADDING', (0, 0), (-1, -1), 11),
                               ('BOTTOMPADDING', (0, 0), (-1, -1), 11)]))
    estado = 'Anulada' if compra.estado == 'ANULADA' else 'Registrada'
    doc.build([texto('Resumen de compra', titulo),
               texto(f'Compra {compra.id} · {estado}', etiquetas),
               Spacer(1, 13), info, Spacer(1, 16), tabla, Spacer(1, 15), banda],
              onFirstPage=pie, onLaterPages=pie)
    return salida.getvalue()
