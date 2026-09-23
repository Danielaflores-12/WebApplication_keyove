<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Comprobante.aspx.vb" Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Ventas.Comprobante" %>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Comprobante de venta | KEYOVE</title>
    <style>
        * { box-sizing: border-box; }
        body { margin: 0; padding: 24px; background: #eef2f6; color: #182331; font: 15px/1.5 Arial, sans-serif; }
        .acciones, main { max-width: 850px; margin: 0 auto 20px; }
        .acciones { display: flex; gap: 12px; align-items: center; flex-wrap: wrap; }
        .acciones a, button { padding: 10px 16px; border: 0; border-radius: 6px; background: #163c60; color: white; font: inherit; text-decoration: none; cursor: pointer; }
        main { background: white; padding: 36px; border-radius: 10px; }
        header { display: flex; justify-content: space-between; gap: 20px; border-bottom: 2px solid #163c60; padding-bottom: 20px; }
        h1, h2, p { margin: 0 0 8px; }
        h1 { font-size: 28px; } h2 { font-size: 20px; }
        .datos { display: grid; grid-template-columns: 1fr 1fr; gap: 8px 24px; margin: 24px 0; }
        .tabla { overflow-x: auto; }
        table { width: 100%; border-collapse: collapse; }
        th, td { padding: 10px 6px; text-align: left; border-bottom: 1px solid #dce2e8; }
        th { background: #f3f6f9; } .numero { text-align: right; }
        .totales { margin: 24px 0 24px auto; max-width: 300px; }
        .totales p { display: flex; justify-content: space-between; gap: 20px; }
        .total { font-size: 21px; font-weight: bold; border-top: 2px solid #163c60; padding-top: 10px; }
        .anulada { color: #a51c30; border: 2px solid; padding: 12px; margin: 20px 0; font-weight: bold; }
        .observacion { white-space: pre-wrap; overflow-wrap: anywhere; }
        @media (max-width: 600px) { body { padding: 12px; } main { padding: 18px; } header { flex-direction: column; } .datos { grid-template-columns: 1fr; } }
        @media print { @page { margin: 15mm; } body { padding: 0; background: white; font-size: 11pt; } .acciones { display: none; } main { max-width: none; margin: 0; padding: 0; } .tabla { overflow: visible; } tr, .totales { break-inside: avoid; } thead { display: table-header-group; } }
    </style>
</head>
<body>
    <nav class="acciones" aria-label="Acciones del comprobante">
        <a href="Ventas.aspx">Volver a ventas</a>
        <% If Cabecera IsNot Nothing Then %>
        <button type="button" onclick="window.print()">Imprimir / Guardar PDF</button>
        <% End If %>
    </nav>
    <main>
        <% If Cabecera Is Nothing Then %>
        <h1>Comprobante no disponible</h1>
        <p role="alert"><%: Mensaje %></p>
        <% Else %>
        <header>
            <div><h1>KEYOVE</h1><p>Comprobante de venta</p></div>
            <div><h2><%: Cabecera("cTipoComprobante") %></h2><p><strong><%: Cabecera("cNumeroComprobante") %></strong></p></div>
        </header>
        <% If Convert.ToString(Cabecera("cEstado")) = "ANULADA" Then %>
        <p class="anulada">ANULADA</p>
        <% End If %>
        <section class="datos" aria-label="Datos de la venta">
            <p><strong>Fecha:</strong> <%: Convert.ToDateTime(Cabecera("dFechaVenta")).ToString("dd/MM/yyyy HH:mm") %></p>
            <p><strong>Atendido por:</strong> <%: Cabecera("cNombreUsuario") %></p>
            <p><strong>Documento del cliente:</strong> <%: Cabecera("cDocumentoCliente") %></p>
            <p><strong>Celular:</strong> <%: Cabecera("cNumeroCelular") %></p>
            <p><strong>Método de pago:</strong> <%: Cabecera("cMetodoPago") %></p>
            <p><strong>Estado:</strong> <%: Cabecera("cEstado") %></p>
        </section>
        <div class="tabla">
            <table aria-label="Productos de la venta">
                <thead><tr><th>Producto</th><th class="numero">Cant.</th><th class="numero">Precio</th><th class="numero">Descuento</th><th class="numero">Importe</th></tr></thead>
                <tbody>
                    <% For Each detalle As System.Data.DataRow In Detalles.Rows %>
                    <tr>
                        <td><%: detalle("cCodigo") %> — <%: detalle("cNombre") %></td>
                        <td class="numero"><%: detalle("iCantidad") %></td>
                        <td class="numero"><%: Importe(detalle("nPrecioVenta")) %></td>
                        <td class="numero"><%: Importe(detalle("nDescuento")) %></td>
                        <td class="numero"><%: Importe(detalle("nSubTotal")) %></td>
                    </tr>
                    <% Next %>
                </tbody>
            </table>
        </div>
        <section class="totales" aria-label="Totales en soles">
            <p><span>Subtotal</span><span>S/ <%: Importe(Cabecera("nSubTotal")) %></span></p>
            <p><span>IGV</span><span>S/ <%: Importe(Cabecera("nIgv")) %></span></p>
            <p class="total"><span>Total</span><span>S/ <%: Importe(Cabecera("nTotal")) %></span></p>
        </section>
        <% If Not String.IsNullOrWhiteSpace(Convert.ToString(Cabecera("cObservacion"))) Then %>
        <p><strong>Observaciones</strong></p>
        <p class="observacion"><%: Cabecera("cObservacion") %></p>
        <% End If %>
        <% End If %>
    </main>
</body>
</html>
