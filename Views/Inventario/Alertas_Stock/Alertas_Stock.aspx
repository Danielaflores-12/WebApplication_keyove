
<%@ Page Title="Alertas Inteligentes de Stock"
    Language="vb"
    AutoEventWireup="false"
    CodeBehind="Alertas_Stock.aspx.vb"
    Inherits="WebApplication_keyove.Alertas_Stock"
    MasterPageFile="~/Views/Master/Site.Master" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

<style>
    .alertas-page {
        padding: 22px;
        background: #f5f8fd;
        min-height: 100vh;
        color: #142b4b;
    }

    .alertas-header {
        background: linear-gradient(110deg, #ffffff, #d9ecff);
        border-radius: 14px;
        padding: 23px;
        margin-bottom: 18px;
        border: 1px solid #e2ecf7;
    }

    .alertas-header h1 {
        margin: 0 0 6px;
        font-size: 25px;
        font-weight: 800;
    }

    .alertas-header p {
        margin: 0;
        color: #64748b;
        font-size: 13px;
    }

    .alertas-stats {
        display: grid;
        grid-template-columns: repeat(4, minmax(0,1fr));
        gap: 14px;
        margin-bottom: 18px;
    }

    .alertas-stat {
        padding: 18px;
        border-radius: 13px;
        background: #fff;
        border: 1px solid #e2e8f0;
        box-shadow: 0 3px 12px rgba(0,0,0,.04);
    }

    .alertas-stat .nombre {
        display: block;
        color: #64748b;
        font-size: 12px;
        font-weight: 600;
        margin-bottom: 9px;
    }

    .alertas-stat .valor {
        display: block;
        font-size: 28px;
        font-weight: 800;
    }

    .stat-rojo {
        border-top: 4px solid #ef4444;
    }

    .stat-naranja {
        border-top: 4px solid #f97316;
    }

    .stat-azul {
        border-top: 4px solid #0b91ea;
    }

    .stat-verde {
        border-top: 4px solid #16a34a;
    }

    .stat-rojo .valor {
        color: #dc2626;
    }

    .stat-naranja .valor {
        color: #ea580c;
    }

    .stat-azul .valor {
        color: #0876d4;
    }

    .stat-verde .valor {
        color: #15803d;
    }

    .alertas-panel {
        background: #fff;
        border-radius: 14px;
        border: 1px solid #e2e8f0;
        padding: 19px;
        margin-bottom: 16px;
        box-shadow: 0 3px 12px rgba(0,0,0,.04);
    }

    .alertas-panel h3 {
        font-size: 16px;
        margin: 0;
        font-weight: 800;
    }

    .alertas-toolbar {
        display: flex;
        gap: 12px;
        margin-bottom: 16px;
        flex-wrap: wrap;
    }

    .alertas-input,
    .alertas-select {
        padding: 11px 13px;
        border: 1px solid #d7e3f1;
        border-radius: 9px;
        font-size: 13px;
        outline: none;
        background: #fff;
        color: #1e293b;
    }

    .alertas-input {
        flex: 1;
        min-width: 200px;
    }

    .alertas-input:focus,
    .alertas-select:focus {
        border-color: #0b91ea;
        box-shadow: 0 0 0 3px rgba(11,145,234,.1);
    }

    .alertas-table-wrap {
        width: 100%;
        overflow-x: auto;
    }

    .alertas-table {
        width: 100%;
        border-collapse: collapse;
        font-size: 12px;
        border: none !important;
    }

    .alertas-table th {
        background: #dfedfb;
        padding: 13px 10px;
        text-align: left;
        font-weight: 800;
        border: none !important;
        white-space: nowrap;
    }

    .alertas-table td {
        padding: 13px 10px;
        border: none !important;
        border-bottom: 1px solid #edf2f8 !important;
        vertical-align: middle;
    }

    .alertas-table tr:hover td {
        background: #f8fbff;
    }

    .estado-badge {
        display: inline-block;
        border-radius: 20px;
        padding: 5px 10px;
        font-size: 11px;
        font-weight: 800;
        white-space: nowrap;
    }

    .estado-agotado {
        background: #fee2e2;
        color: #b91c1c;
    }

    .estado-critico {
        background: #ffedd5;
        color: #c2410c;
    }

    .estado-reponer {
        background: #fef3c7;
        color: #92400e;
    }

    .estado-normal {
        background: #dcfce7;
        color: #166534;
    }

    .alertas-btn {
        display: inline-block;
        border: none;
        border-radius: 8px;
        padding: 9px 13px;
        font-size: 12px;
        font-weight: 700;
        cursor: pointer;
        transition: background .2s;
    }

    .alertas-btn-config {
        background: #eaf4ff;
        color: #075cad;
        white-space: nowrap;
    }

    .alertas-btn-config:hover {
        background: #d7eaff;
    }

    .alertas-btn-primary {
        background: #0876d4;
        color: #fff;
    }

    .alertas-btn-primary:hover {
        background: #075cad;
    }

    .alertas-btn-secondary {
        background: #eef2f7;
        color: #334155;
    }

    .alertas-btn-secondary:hover {
        background: #e2e8f0;
    }

    .alertas-resultado {
        color: #64748b;
        font-size: 12px;
        margin-top: 12px;
    }

    .alertas-vacio {
        padding: 20px;
        text-align: center;
        color: #64748b;
        display: none;
    }

    .alertas-nota {
        margin: 10px 0 0;
        color: #64748b;
        font-size: 12px;
        line-height: 1.7;
    }

    /* VENTANA DE CONFIGURACIÓN */

    .alertas-modal {
        position: fixed;
        inset: 0;
        z-index: 9999;
        display: none;
        align-items: center;
        justify-content: center;
        padding: 18px;
        background: rgba(15,23,42,.55);
    }

    .alertas-modal.abierto {
        display: flex;
    }

    .alertas-modal-contenido {
        width: 100%;
        max-width: 520px;
        max-height: 90vh;
        overflow-y: auto;
        background: #fff;
        border-radius: 15px;
        box-shadow: 0 20px 60px rgba(0,0,0,.2);
    }

    .alertas-modal-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 20px 22px;
        border-bottom: 1px solid #e2e8f0;
    }

    .alertas-modal-header h3 {
        margin: 0;
        font-size: 18px;
        font-weight: 800;
    }

    .alertas-modal-cerrar {
        border: none;
        background: transparent;
        color: #64748b;
        font-size: 25px;
        cursor: pointer;
    }

    .alertas-modal-body {
        padding: 22px;
    }

    .alertas-campo {
        margin-bottom: 16px;
    }

    .alertas-campo label {
        display: block;
        margin-bottom: 7px;
        font-size: 13px;
        font-weight: 700;
        color: #334155;
    }

    .alertas-campo input {
        width: 100%;
        box-sizing: border-box;
        padding: 11px 12px;
        border: 1px solid #d7e3f1;
        border-radius: 9px;
        font-size: 14px;
    }

    .alertas-campo input:focus {
        outline: none;
        border-color: #0b91ea;
        box-shadow: 0 0 0 3px rgba(11,145,234,.1);
    }

    .alertas-campo small {
        display: block;
        margin-top: 5px;
        color: #64748b;
        font-size: 11px;
    }

    .alertas-modal-footer {
        display: flex;
        justify-content: flex-end;
        gap: 10px;
        padding: 17px 22px;
        border-top: 1px solid #e2e8f0;
    }

    .alertas-modal-mensaje {
        margin-top: 10px;
        font-size: 12px;
        color: #dc2626;
    }

    @media(max-width:1000px) {
        .alertas-stats {
            grid-template-columns: repeat(2,1fr);
        }
    }

    @media(max-width:600px) {
        .alertas-page {
            padding: 12px;
        }

        .alertas-stats {
            gap: 9px;
        }

        .alertas-stat {
            padding: 13px;
        }

        .alertas-stat .valor {
            font-size: 23px;
        }

        .alertas-header h1 {
            font-size: 21px;
        }

        .alertas-modal-body {
            padding: 16px;
        }
    }
</style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

<div class="alertas-page">

    <!-- ENCABEZADO -->

    <div class="alertas-header">

        <h1>🔔 Alertas Inteligentes de Stock</h1>

        <p>
            Análisis de existencias, punto de reorden
            y recomendaciones de abastecimiento KEYOVE.
        </p>

    </div>

    <!-- INDICADORES -->

    <div class="alertas-stats">

        <div class="alertas-stat stat-rojo">

            <span class="nombre">
                🔴 Productos agotados
            </span>

            <asp:Label
                ID="lblAgotados"
                runat="server"
                CssClass="valor"
                Text="0" />

        </div>

        <div class="alertas-stat stat-naranja">

            <span class="nombre">
                ⚠ Productos críticos
            </span>

            <asp:Label
                ID="lblUrgentes"
                runat="server"
                CssClass="valor"
                Text="0" />

        </div>

        <div class="alertas-stat stat-azul">

            <span class="nombre">
                📦 Productos a reponer
            </span>

            <asp:Label
                ID="lblReponer"
                runat="server"
                CssClass="valor"
                Text="0" />

        </div>

        <div class="alertas-stat stat-verde">

            <span class="nombre">
                🛒 Unidades sugeridas
            </span>

            <asp:Label
                ID="lblUnidades"
                runat="server"
                CssClass="valor"
                Text="0" />

        </div>

    </div>

    <!-- TABLA DE REPOSICIÓN -->

    <div class="alertas-panel">

        <h3 style="margin-bottom:16px;">
            📊 Análisis y reposición de productos
        </h3>

        <div class="alertas-toolbar">

            <input
                type="text"
                id="buscarProductoStock"
                class="alertas-input"
                placeholder="Buscar producto..."
                oninput="filtrarAlertasKeyove()" />

            <select
                id="filtroEstadoStock"
                class="alertas-select"
                onchange="filtrarAlertasKeyove()">

                <option value="">
                    Todos los estados
                </option>

                <option value="AGOTADO">
                    Agotados
                </option>

                <option value="CRÍTICO">
                    Críticos
                </option>

                <option value="REPOSICIÓN">
                    Reponer
                </option>

                <option value="MÍNIMO">
                    Stock mínimo
                </option>

                <option value="NORMAL">
                    Normales
                </option>

            </select>

        </div>

        <div class="alertas-table-wrap">

            <asp:GridView
                ID="gvAlertasStock"
                runat="server"
                ClientIDMode="Static"
                AutoGenerateColumns="False"
                CssClass="alertas-table"
                ShowHeaderWhenEmpty="True"
                EmptyDataText="No existen productos registrados.">

                <Columns>

                    <asp:BoundField
                        DataField="NombreProducto"
                        HeaderText="Producto" />

                    <asp:BoundField
                        DataField="iStockActual"
                        HeaderText="Stock actual" />

                    <asp:BoundField
                        DataField="iStockMinimo"
                        HeaderText="Stock mínimo" />

                    <asp:BoundField
                        DataField="DemandaDiaria"
                        HeaderText="Demanda/día"
                        DataFormatString="{0:N2}" />

                    <asp:BoundField
                        DataField="PuntoReorden"
                        HeaderText="ROP" />

                    <asp:BoundField
                        DataField="CantidadReponer"
                        HeaderText="Comprar" />

                    <asp:TemplateField HeaderText="Prioridad">

                        <ItemTemplate>

                            <span class='<%# Eval("ClaseEstado") %>'>
                                <%# Eval("EstadoStock") %>
                            </span>

                        </ItemTemplate>

                    </asp:TemplateField>

                    <asp:BoundField
                        DataField="Recomendacion"
                        HeaderText="Recomendación" />

                    

                    <asp:TemplateField HeaderText="Acciones">

                        <ItemTemplate>

                            <button
                                type="button"
                                class="alertas-btn alertas-btn-config"
                                data-id='<%# Eval("iCodProducto") %>'
                                data-producto='<%# System.Web.HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("NombreProducto"))) %>'
                                data-entrega='<%# Eval("iDiasEntrega") %>'
                                data-cobertura='<%# Eval("iDiasCobertura") %>'
                                data-seguridad='<%# Eval("iStockSeguridad") %>'
                                onclick="abrirConfiguracionStock(this)">

                                ⚙ Configurar

                            </button>

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

        </div>

        <div
            id="mensajeSinResultados"
            class="alertas-vacio">

            No hay productos que coincidan con el filtro.

        </div>

        <div
            id="contadorResultados"
            class="alertas-resultado">
        </div>

    </div>

    <!-- INFORMACIÓN BREVE -->

    <div class="alertas-panel">

        <h3>
            🧠 Reposición inteligente
        </h3>

        <p class="alertas-nota">
            Las cantidades sugeridas se calculan
            automáticamente a partir de la demanda,
            las existencias y los parámetros
            de reposición de cada producto.
            Estas recomendaciones no generan
            compras automáticamente.
        </p>

    </div>

</div>

<!-- CONTROLES ASP.NET PARA GUARDAR -->

<asp:HiddenField
    ID="hfConfigProductoId"
    runat="server" />

<asp:HiddenField
    ID="hfConfigDiasEntrega"
    runat="server" />

<asp:HiddenField
    ID="hfConfigDiasCobertura"
    runat="server" />

<asp:HiddenField
    ID="hfConfigStockSeguridad"
    runat="server" />

<asp:Button
    ID="btnGuardarConfiguracionServidor"
    runat="server"
    Text="Guardar en servidor"
    Style="display:none;"
    UseSubmitBehavior="false" />

<!-- VENTANA MODAL -->

<div
    id="modalConfiguracionStock"
    class="alertas-modal"
    role="dialog"
    aria-modal="true"
    aria-labelledby="tituloModalStock">

    <div class="alertas-modal-contenido">

        <div class="alertas-modal-header">

            <h3 id="tituloModalStock">
                ⚙ Configurar reposición
            </h3>

            <button
                type="button"
                class="alertas-modal-cerrar"
                aria-label="Cerrar"
                onclick="cerrarConfiguracionStock()">

                &times;

            </button>

        </div>

        <div class="alertas-modal-body">

            <div class="alertas-campo">

                <label for="configProductoNombre">
                    Producto
                </label>

                <input
                    type="text"
                    id="configProductoNombre"
                    readonly />

            </div>

            <div class="alertas-campo">

                <label for="configDiasEntrega">
                    Días de entrega
                </label>

                <input
                    type="number"
                    id="configDiasEntrega"
                    min="1"
                    max="365"
                    step="1" />

                <small>
                    Tiempo estimado que tarda el proveedor
                    en entregar el producto.
                </small>

            </div>

            <div class="alertas-campo">

                <label for="configDiasCobertura">
                    Días de cobertura
                </label>

                <input
                    type="number"
                    id="configDiasCobertura"
                    min="0"
                    max="365"
                    step="1" />

                <small>
                    Días adicionales de inventario
                    que se desean cubrir.
                </small>

            </div>

            <div class="alertas-campo">

                <label for="configStockSeguridad">
                    Stock de seguridad
                </label>

                <input
                    type="number"
                    id="configStockSeguridad"
                    min="0"
                    max="1000000"
                    step="1" />

                <small>
                    Unidades de reserva para prevenir
                    faltantes de inventario.
                </small>

            </div>

            <div
                id="mensajeConfiguracionStock"
                class="alertas-modal-mensaje"
                role="status">
            </div>

        </div>

        <div class="alertas-modal-footer">

            <button
                type="button"
                class="alertas-btn alertas-btn-secondary"
                onclick="cerrarConfiguracionStock()">

                Cancelar

            </button>

            <button
                type="button"
                id="btnGuardarConfiguracionStock"
                class="alertas-btn alertas-btn-primary"
                onclick="guardarConfiguracionStock()">

                Guardar cambios

            </button>

        </div>

    </div>

</div>

<script type="text/javascript">
    // @ts-nocheck

    // ==========================================
    // FILTRAR PRODUCTOS
    // ==========================================

    function filtrarAlertasKeyove() {

        var tabla = document.getElementById(
            "gvAlertasStock"
        );

        if (!tabla) {
            return;
        }

        var buscar = document.getElementById(
            "buscarProductoStock"
        ).value.toLowerCase().trim();

        var estado = document.getElementById(
            "filtroEstadoStock"
        ).value.toUpperCase();

        var filas = tabla.querySelectorAll("tr");
        var visibles = 0;

        for (var i = 1; i < filas.length; i++) {

            var columnas = filas[i].querySelectorAll("td");

            if (columnas.length < 8) {
                continue;
            }

            var producto = columnas[0].textContent
                .toLowerCase();

            var prioridad = columnas[6].textContent
                .toUpperCase()
                .trim();

            var coincideNombre =
                producto.indexOf(buscar) !== -1;

            var coincideEstado =
                estado === "" ||
                prioridad === estado;

            var mostrar =
                coincideNombre && coincideEstado;

            filas[i].style.display =
                mostrar ? "" : "none";

            if (mostrar) {
                visibles++;
            }

        }

        var mensaje = document.getElementById(
            "mensajeSinResultados"
        );

        var contador = document.getElementById(
            "contadorResultados"
        );

        if (mensaje) {
            mensaje.style.display =
                visibles === 0 ? "block" : "none";
        }

        if (contador) {
            contador.textContent =
                "Productos mostrados: " + visibles;
        }

    }

    // ==========================================
    // ABRIR CONFIGURACIÓN
    // ==========================================

    function abrirConfiguracionStock(boton) {

        var id = boton.getAttribute("data-id");
        var producto = boton.getAttribute("data-producto");
        var entrega = boton.getAttribute("data-entrega");
        var cobertura = boton.getAttribute("data-cobertura");
        var seguridad = boton.getAttribute("data-seguridad");

        document.getElementById(
            "<%= hfConfigProductoId.ClientID %>"
        ).value = id;

        document.getElementById(
            "configProductoNombre"
        ).value = producto;

        document.getElementById(
            "configDiasEntrega"
        ).value = entrega;

        document.getElementById(
            "configDiasCobertura"
        ).value = cobertura;

        document.getElementById(
            "configStockSeguridad"
        ).value = seguridad;

        document.getElementById(
            "mensajeConfiguracionStock"
        ).textContent = "";

        document.getElementById(
            "modalConfiguracionStock"
        ).classList.add("abierto");

        document.getElementById(
            "configDiasEntrega"
        ).focus();

    }

    // ==========================================
    // CERRAR CONFIGURACIÓN
    // ==========================================

    function cerrarConfiguracionStock() {

        document.getElementById(
            "modalConfiguracionStock"
        ).classList.remove("abierto");

    }

    // ==========================================
    // GUARDAR CONFIGURACIÓN
    // ==========================================

    function guardarConfiguracionStock() {

        var id = Number(
            document.getElementById(
                "<%= hfConfigProductoId.ClientID %>"
            ).value
        );

        var entregaTexto = document.getElementById(
            "configDiasEntrega"
        ).value.trim();

        var coberturaTexto = document.getElementById(
            "configDiasCobertura"
        ).value.trim();

        var seguridadTexto = document.getElementById(
            "configStockSeguridad"
        ).value.trim();

        var entrega = Number(entregaTexto);
        var cobertura = Number(coberturaTexto);
        var seguridad = Number(seguridadTexto);

        var mensaje = document.getElementById(
            "mensajeConfiguracionStock"
        );

        if (!Number.isInteger(id) || id <= 0) {

            mensaje.textContent =
                "Seleccione un producto válido.";

            return;
        }

        if (entregaTexto === "" ||
            !Number.isInteger(entrega) ||
            entrega < 1 ||
            entrega > 365) {

            mensaje.textContent =
                "Los días de entrega deben estar entre 1 y 365.";

            return;
        }

        if (coberturaTexto === "" ||
            !Number.isInteger(cobertura) ||
            cobertura < 0 ||
            cobertura > 365) {

            mensaje.textContent =
                "Los días de cobertura deben estar entre 0 y 365.";

            return;
        }

        if (seguridadTexto === "" ||
            !Number.isInteger(seguridad) ||
            seguridad < 0 ||
            seguridad > 1000000) {

            mensaje.textContent =
                "El stock de seguridad debe ser un entero válido.";

            return;
        }

        document.getElementById(
            "<%= hfConfigDiasEntrega.ClientID %>"
        ).value = entrega;

        document.getElementById(
            "<%= hfConfigDiasCobertura.ClientID %>"
        ).value = cobertura;

        document.getElementById(
            "<%= hfConfigStockSeguridad.ClientID %>"
        ).value = seguridad;

        // Ejecutar evento VB.NET.
        document.getElementById(
            "<%= btnGuardarConfiguracionServidor.ClientID %>"
        ).click();

    }

    // ==========================================
    // TECLA ESCAPE
    // ==========================================

    document.addEventListener(
        "keydown",
        function (evento) {

            if (evento.key === "Escape") {
                cerrarConfiguracionStock();
            }

        }
    );

    // ==========================================
    // INICIALIZAR FILTROS
    // ==========================================

    if (document.readyState === "loading") {

        document.addEventListener(
            "DOMContentLoaded",
            filtrarAlertasKeyove
        );

    } else {

        filtrarAlertasKeyove();

    }
</script>

</asp:Content>
