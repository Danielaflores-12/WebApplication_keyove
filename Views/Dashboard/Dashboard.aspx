
<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Dashboard.aspx.vb"
    Inherits="WebApplication_keyove.Views.Dashboard.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
    .key-dashboard {
        padding: 18px;
        background: #f5f8fd;
        color: #10264a;
    }

    .kd-header {
        padding: 16px 22px;
        margin-bottom: 14px;
        border-radius: 12px;
        background: linear-gradient(110deg, #fff 40%, #d8edff);
        display: flex;
        align-items: center;
        gap: 14px;
    }

    .kd-header-icon {
        color: #078be8;
        font-size: 32px;
        font-weight: 900;
    }

    .kd-header h1 {
        font-size: 25px;
        margin: 0;
        font-weight: 800;
    }

    .kd-header p {
        font-size: 12px;
        margin: 3px 0 0;
    }

    .kd-stats {
        display: grid;
        grid-template-columns: repeat(4, minmax(0, 1fr));
        gap: 12px;
        margin-bottom: 12px;
    }

    .kd-stat {
        position: relative;
        min-height: 110px;
        padding: 15px;
        color: white;
        border-radius: 12px;
        overflow: hidden;
        box-shadow: 0 4px 12px rgba(19, 54, 98, .10);
    }

    .kd-stat::after {
        content: "";
        position: absolute;
        width: 140px;
        height: 140px;
        right: -65px;
        top: -60px;
        border-radius: 50%;
        background: rgba(255,255,255,.12);
    }

    .kd-blue { background: linear-gradient(125deg,#0c9cf3,#0864e8); }
    .kd-green { background: linear-gradient(125deg,#08b64d,#00842b); }
    .kd-orange { background: linear-gradient(125deg,#ff9c06,#fa6500); }
    .kd-purple { background: linear-gradient(125deg,#a34ff0,#6314c5); }

    .kd-stat-label {
        font-size: 12px;
        font-weight: 600;
    }

    .kd-stat-value {
        display: block;
        margin-top: 10px;
        font-size: 25px;
        font-weight: 800;
    }

    .kd-stat-icon {
        position: absolute;
        right: 15px;
        bottom: 16px;
        font-size: 36px;
        opacity: .5;
    }

    .kd-stat-tag {
        display: inline-block;
        margin-top: 9px;
        padding: 3px 9px;
        border-radius: 20px;
        background: rgba(255,255,255,.85);
        color: #17406a;
        font-size: 10px;
        font-weight: 700;
    }

    .kd-grid {
        display: grid;
        grid-template-columns: 1.3fr 1fr;
        gap: 12px;
        margin-bottom: 12px;
    }

    .kd-panel {
        background: #fff;
        border: 1px solid #e0e8f2;
        border-radius: 13px;
        padding: 15px;
        min-width: 0;
        box-shadow: 0 3px 12px rgba(18, 53, 93, .04);
    }

    .kd-panel-title {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 10px;
        margin-bottom: 14px;
    }

    .kd-panel-title h3 {
        margin: 0;
        font-size: 14px;
        font-weight: 800;
    }

    .kd-panel-icon {
        color: #0988e8;
        margin-right: 6px;
    }

    .kd-chart-wrap {
        height: 240px;
        position: relative;
    }

    .kd-table-wrap {
        overflow-x: auto;
    }

    .kd-table {
        width: 100%;
        border-collapse: collapse;
        font-size: 12px;
    }

    .kd-table th {
        background: #dcecfb;
        padding: 10px 8px;
        text-align: left;
        font-weight: 800;
        white-space: nowrap;
    }

    .kd-table td {
        padding: 10px 8px;
        border-bottom: 1px solid #edf1f6;
    }

    .kd-table tr:last-child td {
        border-bottom: none;
    }

    .kd-badge {
        display: inline-block;
        padding: 4px 9px;
        border-radius: 20px;
        font-size: 11px;
        font-weight: 700;
        white-space: nowrap;
    }

    .kd-danger {
        background: #fee2e2;
        color: #b91c1c;
    }

    .kd-warning {
        background: #fff0bf;
        color: #995a00;
    }

    .kd-critical {
        background: #ffe1cf;
        color: #b74400;
    }

    .kd-success {
        background: #dcfce7;
        color: #166534;
    }

    .kd-info {
        background: #e3e6ff;
        color: #5142a7;
    }

    .kd-link {
        color: #0876d4;
        font-size: 11px;
        font-weight: 700;
        text-decoration: none;
    }

    .kd-link:hover { text-decoration: underline; }

    .kd-alert-summary {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
        margin-bottom: 12px;
    }

    .kd-alert-summary span {
        font-size: 11px;
        padding: 5px 9px;
        background: #f1f5fa;
        border-radius: 8px;
    }

    .kd-reponer {
        font-weight: 800;
        color: #c2410c;
    }

    .kd-empty {
        padding: 18px !important;
        text-align: center;
        color: #64748b;
    }

    .kd-muted {
        font-size: 11px;
        color: #64748b;
    }

    @media(max-width: 1100px) {
        .kd-stats { grid-template-columns: repeat(2,1fr); }
        .kd-grid { grid-template-columns: 1fr; }
    }

    @media(max-width: 600px) {
        .key-dashboard { padding: 10px; }
        .kd-stats { gap: 8px; }
        .kd-stat { padding: 12px; }
        .kd-stat-value { font-size: 20px; }
        .kd-stat-icon { font-size: 25px; }
        .kd-header h1 { font-size: 21px; }
        .kd-panel { padding: 11px; }
    }
    
/* ==========================================
   KEYOVE - TABLAS PROFESIONALES
========================================== */

.key-dashboard .kd-table {
    width: 100%;
    border-collapse: separate !important;
    border-spacing: 0;
    font-size: 12px;
    border: none !important;
}

.key-dashboard .kd-table th {
    background: #e1effc;
    color: #17345a;
    padding: 12px 10px;
    border: none !important;
    border-bottom: 1px solid #d4e3f1 !important;
    font-weight: 700;
}

.key-dashboard .kd-table td {
    padding: 12px 10px;
    border: none !important;
    border-bottom: 1px solid #edf2f8 !important;
    color: #334155;
    vertical-align: middle;
}

.key-dashboard .kd-table tr:hover td {
    background: #f6faff;
}

.key-dashboard .kd-table tr:last-child td {
    border-bottom: none !important;
}

/* ==========================================
   ALERTAS INTELIGENTES
========================================== */

.kd-alert-summary {
    display: flex;
    gap: 10px;
    flex-wrap: wrap;
    margin-bottom: 16px;
}

.kd-alert-summary span {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 8px 12px;
    border-radius: 9px;
    font-size: 12px;
    background: #f3f7fc;
    border: 1px solid #e5edf7;
}

.kd-alert-summary span:nth-child(1) {
    background: #fff1f2;
    border-color: #fecdd3;
    color: #be123c;
}

.kd-alert-summary span:nth-child(2) {
    background: #fff7ed;
    border-color: #fed7aa;
    color: #c2410c;
}

.kd-alert-summary span:nth-child(3) {
    background: #fffbeb;
    border-color: #fde68a;
    color: #92400e;
}

.kd-badge {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    padding: 5px 10px;
    border-radius: 20px;
    font-size: 11px;
    font-weight: 700;
}

.kd-danger {
    background: #fee2e2;
    color: #b91c1c;
}

.kd-critical {
    background: #ffedd5;
    color: #c2410c;
}

.kd-warning {
    background: #fef3c7;
    color: #92400e;
}

/* Destacar cantidad que debe reponerse */
.key-dashboard #gvAlertasStock td:nth-child(4) {
    font-weight: 800;
    color: #c2410c;
}

/* ==========================================
   PANEL DE ALERTAS
========================================== */

.kd-alert-help {
    display: flex;
    align-items: flex-start;
    gap: 10px;
    padding: 12px;
    margin-top: 15px;
    border-radius: 10px;
    background: #eff6ff;
    border: 1px solid #bfdbfe;
    color: #1e40af;
    font-size: 12px;
    line-height: 1.5;
}

.kd-alert-help strong {
    display: block;
    margin-bottom: 3px;
}

</style>
</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

<div class="key-dashboard">

    <div class="kd-header">
        <div class="kd-header-icon">▥</div>
        <div>
            <h1>Dashboard</h1>
            <p>Resumen general del sistema KEYOVE</p>
        </div>
    </div>

    <!-- INDICADORES -->

    <div class="kd-stats">

        <div class="kd-stat kd-blue">
            <div class="kd-stat-label">🛒 Ventas de hoy</div>
            <asp:Label ID="lblVentasHoy"
                runat="server"
                CssClass="kd-stat-value"
                Text="S/ 0.00" />
            <span class="kd-stat-tag">● Hoy</span>
            <span class="kd-stat-icon">▥</span>
        </div>

        <div class="kd-stat kd-green">
            <div class="kd-stat-label">⬡ Productos registrados</div>
            <asp:Label ID="lblProductosRegistrados"
                runat="server"
                CssClass="kd-stat-value"
                Text="0" />
            <span class="kd-stat-tag">● Inventario</span>
            <span class="kd-stat-icon">⬡</span>
        </div>

        <div class="kd-stat kd-orange">
            <div class="kd-stat-label">Productos a reponer</div>
            <asp:Label ID="lblStockBajo"
                runat="server"
                CssClass="kd-stat-value"
                Text="0" />
            <span class="kd-stat-tag">⚠ Requiere atención</span>
            <span class="kd-stat-icon">▣</span>
        </div>

        <div class="kd-stat kd-purple">
            <div class="kd-stat-label">🛍 Compras realizadas</div>
            <asp:Label ID="lblComprasRealizadas"
                runat="server"
                CssClass="kd-stat-value"
                Text="0" />
            <span class="kd-stat-tag">● Total registrado</span>
            <span class="kd-stat-icon">▤</span>
        </div>

    </div>

    <!-- GRÁFICO Y MÁS VENDIDOS -->

    <div class="kd-grid">

        <section class="kd-panel">
            <div class="kd-panel-title">
                <h3><span class="kd-panel-icon">▥</span>
                    Evolución de ventas (últimos 12 meses)
                </h3>
            </div>

            <div class="kd-chart-wrap">
                <canvas id="graficoVentasKeyove"></canvas>
            </div>

            <asp:Panel ID="pnlGraficoVentas"
                runat="server"
                Style="display:none">

                <asp:Repeater ID="rptGraficoVentas" runat="server">
                    <ItemTemplate>
                        <span class="kd-mes-dato"
                              data-mes='<%# Eval("MesNombre") %>'
                              data-total='<%# Eval("TotalVentas", "{0:0.00}") %>'>
                        </span>
                    </ItemTemplate>
                </asp:Repeater>

            </asp:Panel>

            <asp:Panel ID="pnlSinVentas"
                runat="server"
                Visible="False">
                <p class="kd-muted">
                    No existen ventas registradas.
                </p>
            </asp:Panel>
        </section>

        <section class="kd-panel">
            <div class="kd-panel-title">
                <h3><span class="kd-panel-icon">🏆</span>
                    Productos más vendidos
                </h3>
            </div>

            <div class="kd-table-wrap">
                <asp:GridView ID="gvMasVendidos"
                    runat="server"
                    CssClass="kd-table"
                    AutoGenerateColumns="False"
                    ShowHeaderWhenEmpty="True"
                    EmptyDataText="Aún no hay productos vendidos.">

                    <Columns>
                        <asp:BoundField DataField="Producto"
                            HeaderText="Producto" />

                        <asp:BoundField DataField="CantidadVendida"
                            HeaderText="Cantidad" />

                        <asp:BoundField DataField="TotalVendido"
                            HeaderText="Total vendido"
                            DataFormatString="S/ {0:N2}" />
                    </Columns>
                </asp:GridView>
            </div>
        </section>

    </div>

    <!-- ALERTAS Y MOVIMIENTOS -->

    <div class="kd-grid">

        <section class="kd-panel">

            <div class="kd-panel-title">
                <h3><span class="kd-panel-icon">🔔</span>
                    Alertas inteligentes de stock
                </h3>
                <a class="kd-link"
                   href='<%= ResolveUrl("~/Views/Inventario/Alertas_Stock/Alertas_Stock.aspx") %>'>
                    Ver todas →
                </a>
            </div>

            <div class="kd-alert-summary">
                <span>
                    Agotados:
                    <strong><asp:Label ID="lblAgotados"
                        runat="server" Text="0" /></strong>
                </span>
                <span>
                    Críticos:
                    <strong><asp:Label ID="lblCriticos"
                        runat="server" Text="0" /></strong>
                </span>
                <span>
                    A reponer:
                    <strong><asp:Label ID="lblBajos"
                        runat="server" Text="0" /></strong>
                </span>
            </div>

            <div class="kd-table-wrap">
                <asp:GridView ID="gvAlertasStock"
                    runat="server"
                    CssClass="kd-table"
                    AutoGenerateColumns="False"
                    ShowHeaderWhenEmpty="True"
                    EmptyDataText="No hay productos con alertas.">

                    <Columns>
                        <asp:BoundField DataField="Producto"
                            HeaderText="Producto" />

                        <asp:BoundField DataField="StockActual"
                            HeaderText="Actual" />

                        <asp:BoundField DataField="StockMinimo"
                            HeaderText="Mínimo" />

                        <asp:BoundField DataField="CantidadReponer"
                            HeaderText="Reponer" />

                        <asp:TemplateField HeaderText="Prioridad">
                            <ItemTemplate>
                                <span class='<%# Eval("ClaseEstado") %>'>
                                    <%# Eval("EstadoStock") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            
<div class="kd-alert-help">
    <span style="font-size:20px;">💡</span>

    <div>
        <strong>Recomendación de inventario</strong>

        Los productos agotados deben atenderse primero.
        Los productos críticos requieren reposición
        prioritaria y los de stock bajo deben
        incluirse en la planificación de compras.

        <div style="margin-top:6px;">
            La columna <b>Reponer</b> indica cuántas
            unidades se necesitan para alcanzar
            el stock mínimo establecido.
        </div>
    </div>
</div>

        </section>

        <section class="kd-panel">

            <div class="kd-panel-title">
                <h3><span class="kd-panel-icon">🔄</span>
                    Movimientos recientes
                </h3>
            </div>

            <div class="kd-table-wrap">
                <asp:GridView ID="gvMovimientos"
                    runat="server"
                    CssClass="kd-table"
                    AutoGenerateColumns="False"
                    ShowHeaderWhenEmpty="True"
                    EmptyDataText="No hay movimientos registrados.">

                    <Columns>
                        <asp:BoundField DataField="Fecha"
                            HeaderText="Fecha"
                            DataFormatString="{0:dd/MM/yyyy}" />

                        <asp:BoundField DataField="Tipo"
                            HeaderText="Tipo" />

                        <asp:BoundField DataField="Producto"
                            HeaderText="Producto" />

                        <asp:BoundField DataField="Cantidad"
                            HeaderText="Cantidad" />

                        <asp:BoundField DataField="Usuario"
                            HeaderText="Usuario" />
                    </Columns>
                </asp:GridView>
            </div>

        </section>

    </div>

</div>


<!-- ==========================================
     CHART.JS - GRÁFICO DE VENTAS KEYOVE
========================================== -->

<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.8/dist/chart.umd.min.js"></script>

<script type="text/javascript">
    // @ts-nocheck

    (function () {
        "use strict";

        // ==========================================
        // INICIALIZAR GRÁFICO
        // ==========================================

        function iniciarGraficoKeyove() {

            var canvas = document.getElementById(
                "graficoVentasKeyove"
            );

            if (!canvas) {
                console.warn(
                    "No se encontró el canvas del gráfico."
                );
                return;
            }

            // Acceder a Chart.js desde window.
            var ChartJS = window["Chart"];

            if (typeof ChartJS !== "function") {
                console.error(
                    "Chart.js no está disponible. " +
                    "Verifica la conexión a internet."
                );
                return;
            }

            // ==========================================
            // OBTENER DATOS DE ASP.NET
            // ==========================================

            var elementos = document.querySelectorAll(
                ".kd-mes-dato"
            );

            var meses = [];
            var importes = [];

            for (var i = 0; i < elementos.length; i++) {

                var elemento = elementos[i];

                var mes = elemento.getAttribute("data-mes") || "";

                var textoTotal =
                    elemento.getAttribute("data-total") || "0";

                // Convertir importe a número.
                var numero = parseFloat(
                    textoTotal.trim().replace(",", ".")
                );

                meses.push(mes);

                importes.push(
                    isNaN(numero) ? 0 : numero
                );
            }

            // ==========================================
            // EVITAR GRÁFICOS DUPLICADOS
            // ==========================================

            var graficoAnterior = ChartJS.getChart(canvas);

            if (graficoAnterior) {
                graficoAnterior.destroy();
            }

            // ==========================================
            // CREAR GRÁFICO DE BARRAS
            // ==========================================

            new ChartJS(canvas, {

                type: "bar",

                data: {

                    labels: meses,

                    datasets: [
                        {
                            label: "Ventas (S/)",

                            data: importes,

                            backgroundColor: "#0b92ed",

                            hoverBackgroundColor: "#076ac6",

                            borderRadius: 5,

                            maxBarThickness: 36,

                            borderSkipped: false
                        }
                    ]
                },

                options: {

                    responsive: true,

                    maintainAspectRatio: false,

                    animation: {
                        duration: 900
                    },

                    interaction: {
                        intersect: false,
                        mode: "index"
                    },

                    plugins: {

                        legend: {
                            display: false
                        },

                        tooltip: {

                            backgroundColor: "#10264a",

                            titleColor: "#ffffff",

                            bodyColor: "#ffffff",

                            padding: 10,

                            displayColors: false,

                            callbacks: {

                                label: function (contexto) {

                                    var valor = Number(
                                        contexto.parsed.y || 0
                                    );

                                    return "Ventas: S/ " +
                                        valor.toLocaleString(
                                            "es-PE",
                                            {
                                                minimumFractionDigits: 2,
                                                maximumFractionDigits: 2
                                            }
                                        );
                                }
                            }
                        }
                    },

                    scales: {
                        x: {
                            grid: {
                                display: false
                            },
                            border: {
                                display: false
                            },
                            ticks: {
                                color: "#64748b",
                                font: {
                                    size: 11,
                                    weight: "500"
                                }
                            }
                        },

                        y: {
                            beginAtZero: true,

                            grace: "10%",

                            border: {
                                display: false,
                                dash: [4, 4]
                            },

                            grid: {
                                color: "#e8eef6"
                            },


                            ticks: {
                                color: "#64748b",

                                callback: function (valor) {
                                    var numero = Number(valor);

                                    if (numero >= 1000000) {
                                        return "S/ " +
                                            (numero / 1000000).toLocaleString("es-PE", {
                                                maximumFractionDigits: 1
                                            }) + " M";
                                    }

                                    if (numero >= 1000) {
                                        return "S/ " +
                                            (numero / 1000).toLocaleString("es-PE", {
                                                maximumFractionDigits: 1
                                            }) + " mil";
                                    }

                                    return "S/ " + numero.toLocaleString("es-PE", {
                                        maximumFractionDigits: 2
                                    });
                                }
                            }
                        }
                    }
                }
            });
        }

        // ==========================================
        // EJECUTAR CUANDO CARGUE LA PÁGINA
        // ==========================================

        if (document.readyState === "loading") {

            document.addEventListener(
                "DOMContentLoaded",
                iniciarGraficoKeyove
            );

        } else {

            iniciarGraficoKeyove();

        }

    })();

</script>


</asp:Content>
