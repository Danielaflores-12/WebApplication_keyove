<%@ Page Language="vb"
    AutoEventWireup="false"
    MasterPageFile="~/Views/Master/Site.Master"
    CodeBehind="Compras.aspx.vb"
    Inherits="WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Compras.Compra1" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../../../JScript/Compras.js?v=20261026-ocr-campos-1"></script>
    <style>
      .compra-dialog { margin:auto; border:0; padding:0; border-radius:10px; width:min(520px,calc(100vw - 32px)); max-height:90vh; box-shadow:0 16px 55px #0005; color:#263247; }
      .compra-dialog::backdrop { background:#17233399; }
      .compra-dialog header { background:#28538b; color:white; padding:16px 20px; display:flex; justify-content:space-between; align-items:center; }
      .compra-dialog h2 { margin:0; font-size:18px; }
      .compra-dialog header button { border:0; background:none; color:white; font-size:24px; cursor:pointer; }
      .compra-dialog .modal-cuerpo { padding:20px; display:grid; gap:12px; }
      .compra-dialog footer { padding:14px 20px; background:#f3f5f8; display:flex; justify-content:flex-end; gap:8px; }
      .compra-dialog .modal-error { color:#b42318; margin:0; }
      .compra-dialog .form-grid { grid-template-columns:repeat(2,minmax(0,1fr)); }
      .compra-completa { width:min(1120px,calc(100vw - 24px)); }
      .compra-completa > header { position:sticky; top:0; z-index:2; }
      .compra-completa .card { margin:0; box-shadow:none; min-width:0; }
      .aviso-revision { padding:12px 20px; margin:0; background:#eef4fc; }
      .compra-subseccion { border:1px solid #cbd5e1; border-radius:8px; margin:16px 0; }
      .compra-subseccion[hidden] { display:none; }
      .compra-subseccion header { padding:12px; background:#eef4fc; color:#263247; }
      .compra-subseccion header button { color:#263247; }
      .compra-subseccion .modal-cuerpo { padding:16px; }
      .compra-subseccion h2 { font-size:16px; }
      .compra-completa input, .compra-completa select, .compra-completa textarea { min-width:0; max-width:100%; }
      @media(max-width:480px) { .compra-dialog .form-grid { grid-template-columns:1fr; } }

      #modalAltasCompra { width:min(880px,calc(100vw - 24px)); overflow:auto; background:#f5f7fb; }
      #modalAltasCompra > header { position:sticky; top:0; z-index:2; padding:20px 24px; }
      #modalAltasCompra > footer { position:sticky; bottom:0; z-index:2; border-top:1px solid #dbe3ef; background:#fff; }
      #modalAltasCompra .modal-cuerpo { padding:24px; gap:20px; }
      #modalAltasCompra .alta-intro { margin:0; color:#526078; line-height:1.6; }
      #proveedorAltaCampos { padding:18px; background:#fff; border:1px solid #dbe3ef; border-radius:12px; display:grid; grid-template-columns:1fr 2fr; gap:16px; }
      #proveedorAltaCampos[hidden] { display:none; }
      #productosAltaCampos { display:grid; gap:22px; min-width:0; }
      #modalAltasCompra label { display:flex; flex-direction:column; gap:7px; font-weight:600; font-size:13px; min-width:0; }
      #modalAltasCompra .input-control { box-sizing:border-box; width:100%; min-width:0; margin:0; background:#fff; color:#263247; border:1px solid #cbd5e1; border-radius:8px; padding:10px 12px; font:inherit; font-weight:400; }
      #modalAltasCompra .input-control:focus { outline:2px solid #3776bc; outline-offset:2px; }
      #modalAltasCompra textarea { resize:vertical; }
      #modalAltasCompra .alta-producto { min-width:0; margin:0; border:1px solid #dbe3ef; border-radius:12px; padding:20px; background:white; }
      #modalAltasCompra legend { width:auto; padding:0 8px; font-size:14px; font-weight:700; color:#28538b; }
      .alta-resumen { display:flex; flex-direction:column; gap:6px; margin-bottom:18px; overflow-wrap:anywhere; }
      .alta-resumen strong { font-size:16px; }
      .alta-resumen span, .alta-precio small { font-size:13px; color:#526078; }
      .alta-vincular { display:grid; gap:12px; padding:16px; border-radius:10px; background:#eef4fc; margin-bottom:20px; }
      .alta-resultados { color:#526078; line-height:1.5; }
      .alta-ficha-existente { margin:0; color:#17633d; line-height:1.5; }
      .alta-datos-nuevos { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:16px; }
      .alta-datos-nuevos[hidden] { display:none; }
      .alta-ancho { grid-column:1 / -1; }
      .alta-precio { display:flex; align-items:center; gap:16px; border-top:1px solid #e4eaf2; margin-top:20px; padding-top:18px; }
      .alta-precio label { flex:1; }
      .alta-precio small { flex:1; line-height:1.5; }
      .alta-advertencia { padding:12px 14px; background:#fff8e8; border-left:3px solid #d59a27; color:#775014; font-size:13px; line-height:1.6; border-radius:6px; overflow-wrap:anywhere; }
      .alta-texto-ocr { margin-top:16px; font-size:13px; overflow-wrap:anywhere; }
      .alta-texto-ocr summary { color:#28538b; cursor:pointer; padding:6px 0; }
      .alta-texto-ocr p { white-space:pre-wrap; padding:12px; background:#f5f7fb; border-radius:6px; }
      @media(max-width:600px) {
        #modalAltasCompra .modal-cuerpo { padding:14px; }
        #modalAltasCompra > header { padding:16px; }
        #modalAltasCompra .alta-producto { padding:14px; }
        #proveedorAltaCampos, .alta-datos-nuevos { grid-template-columns:1fr; }
        .alta-precio { flex-direction:column; align-items:stretch; }
        #modalAltasCompra > footer { flex-wrap:wrap; }
        #modalAltasCompra > footer button { flex:1; }
      }

      #modalAltasCompra { width:min(1440px,calc(100vw - 32px)); max-height:94vh; }
      #modalAltasCompra .ocr-presentacion { display:flex; justify-content:space-between; align-items:center; flex-wrap:wrap; gap:12px; }
      .ocr-layout { display:grid; grid-template-columns:minmax(0,1.25fr) minmax(280px,.85fr); gap:24px; align-items:start; }
      .ocr-edicion { min-width:0; }
      #modalAltasCompra h3 { margin:0 0 16px; font-size:16px; }
      .ocr-documento { padding:20px; background:white; border:1px solid #dbe3ef; border-radius:12px; margin-bottom:24px; }
      .ocr-campos { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:16px; margin-bottom:16px; }
      #proveedorAltaCampos { padding:0; border:0; border-radius:0; grid-template-columns:1fr; margin-bottom:16px; }
      .ocr-original { position:sticky; top:96px; min-width:0; background:white; padding:16px; border:1px solid #dbe3ef; border-radius:12px; }
      .ocr-original img { display:block; max-width:100%; height:auto; }
      .ocr-original iframe { width:100%; height:64vh; border:0; }
      .ocr-original pre { white-space:pre-wrap; overflow-wrap:anywhere; font-size:12px; max-height:300px; overflow:auto; }
      .ocr-original summary { padding:14px 0; cursor:pointer; }
      .ocr-etiqueta { display:inline-block; width:fit-content; padding:5px 10px; border-radius:20px; font-size:12px; font-weight:600; }
      .ocr-existente { color:#16603d; background:#e5f5ed; }
      .ocr-nuevo { color:#24568f; background:#e9f1ff; }
      .ocr-revision { color:#805716; background:#fff3d6; }
      .ocr-error { color:#a32128; background:#fdebee; }
      .ocr-campo-error { color:#a32128; font-size:12px; line-height:1.4; font-weight:400; }
      #modalAltasCompra [aria-invalid="true"] { border-color:#ba3037; outline-color:#ba3037; }
      #modalAltasCompra .ocr-nueva-marca { flex-direction:row; align-items:start; margin-top:6px; font-weight:400; }
      #modalAltasCompra .ocr-nueva-marca input { width:auto; margin-top:3px; }
      .ocr-sugerencias { padding:10px 0; font-size:13px; line-height:1.5; color:#526078; }
      .alta-entrada { display:block; background:#f0f7f5; padding:12px; border-radius:8px; line-height:1.7; }
      .alta-precio { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); }
      .alta-precio .alta-entrada { grid-column:1/-1; }
      #modalAltasCompra .input-control:disabled { background:#f5f7fb; color:#526078; opacity:1; }
      @media(max-width:980px) { .ocr-layout { grid-template-columns:1fr; } .ocr-original { position:static; } .ocr-original img { max-height:65vh; margin:auto; object-fit:contain; } }
      @media(max-width:600px) { #modalAltasCompra { width:calc(100vw - 12px); } .ocr-campos, .alta-precio { grid-template-columns:1fr; } .ocr-documento { padding:14px; } #modalAltasCompra > footer { position:static; } }

      #modalAltasCompra { display:none; flex-direction:column; height:min(900px,92vh); overflow:hidden; }
      #modalAltasCompra[open] { display:flex; }
      #modalAltasCompra > header, #modalAltasCompra > footer { position:static; flex-shrink:0; }
      #modalAltasCompra > .modal-cuerpo { overflow:auto; min-height:0; display:block; }
      #modalAltasCompra .ocr-presentacion { margin-bottom:20px; }
      #modalAltasCompra .ocr-original { top:0; }
      #modalAltasCompra .ocr-documento { padding:18px; }
      #modalAltasCompra .ocr-proveedor-cabecera { display:flex; align-items:center; justify-content:space-between; gap:12px; margin-bottom:8px; }
      .ocr-proveedor-cabecera h4 { margin:0; font-size:14px; }
      #estadoProveedorOCR { font-size:12px; color:#526078; }
      .ocr-cambiar-proveedor { margin-bottom:12px; font-size:13px; }
      .ocr-cambiar-proveedor summary, .alta-vincular summary { cursor:pointer; color:#28538b; padding:8px 0; }
      .ocr-cambiar-proveedor label { margin-top:8px; }
      #proveedorAltaCampos { grid-template-columns:minmax(120px,1fr) minmax(0,2fr); gap:12px; }
      #modalAltasCompra .ocr-campos { gap:12px; margin-bottom:12px; }
      .ocr-referencia-tabs { display:flex; gap:8px; border-bottom:1px solid #dbe3ef; margin-bottom:14px; }
      .ocr-referencia-tabs button { border:0; border-bottom:3px solid transparent; background:none; padding:10px 12px; color:#526078; cursor:pointer; font:inherit; font-size:13px; }
      .ocr-referencia-tabs button[aria-selected="true"] { color:#28538b; border-bottom-color:#28538b; font-weight:600; }
      #archivoOriginalOCR[hidden], #panelTextoOCR[hidden] { display:none; }
      #archivoOriginalOCR { max-height:52vh; overflow:auto; overscroll-behavior:contain; scrollbar-gutter:stable; border-radius:8px; }
      #archivoOriginalOCR img { max-width:none; max-height:none; height:auto; width:100%; margin:0; }
      .ocr-zoom { display:flex; align-items:center; gap:8px; margin-bottom:12px; flex-wrap:wrap; }
      .ocr-zoom[hidden] { display:none; }
      .ocr-zoom .btn { padding:6px 12px; min-width:36px; font-size:14px; }
      #porcentajeZoomOCR { min-width:48px; text-align:center; font-size:12px; color:#526078; }
      #archivoOriginalOCR:focus-visible { outline:2px solid #3776bc; outline-offset:3px; }
      #panelTextoOCR pre { margin:0; max-height:58vh; font:12px/1.7 Consolas,monospace; padding:12px; background:#f5f7fb; border-radius:8px; }
      #origenTextoOCR { font-size:12px; color:#526078; }
      .ocr-reescanear { margin-top:16px; }
      #modalAltasCompra .alta-producto { padding:16px; }
      #modalAltasCompra .alta-resumen { margin-bottom:12px; flex-direction:row; align-items:center; justify-content:space-between; flex-wrap:wrap; }
      #modalAltasCompra .alta-resumen strong { font-size:14px; }
      #modalAltasCompra .alta-resumen > span:last-child { width:100%; }
      #modalAltasCompra details.alta-vincular { display:block; padding:8px 12px; margin-bottom:14px; }
      #modalAltasCompra details.alta-vincular label { margin-bottom:10px; }
      .alta-referencia { display:flex; justify-content:space-between; flex-wrap:wrap; gap:8px; margin-top:12px; }
      #modalAltasCompra .alta-referencia button { font-size:12px; padding:7px 10px; }
      @media(max-width:600px) { #proveedorAltaCampos { grid-template-columns:1fr; } #modalAltasCompra > footer { padding:10px; gap:6px; } #modalAltasCompra > footer #confirmarRegistrarOCR { flex-basis:100%; } }

      .compras-pagina { --compras-azul:#28538b; color:#263247; min-width:0; }
      .compras-encabezado { padding:24px 28px; background:#28538b; border-radius:14px; margin-bottom:20px; color:#fff; }
      .compras-encabezado h1 { color:#fff; font-size:26px; margin:0 0 6px; }
      .compras-encabezado p { color:#e1eafb; margin:0; line-height:1.6; font-size:14px; }
      .compras-pagina > .ventas-vistas { display:flex; gap:6px; padding:5px; border:1px solid #dbe3ef; background:#eef3fa; border-radius:12px; width:fit-content; max-width:100%; margin-bottom:22px; }
      .compras-pagina > .ventas-vistas button { border:0; border-radius:8px; padding:11px 18px; font-size:14px; }
      .compras-pagina > .ventas-vistas [aria-pressed="false"] { background:transparent; color:#526078; }
      .compras-pagina > .ventas-vistas [aria-pressed="true"] { background:#28538b; color:#fff; box-shadow:0 2px 5px #193a6520; }
      #formularioCompletoCompra { background:transparent; padding:0; box-shadow:none; border:0; }
      .compras-carga, .compras-bloque { border:1px solid #dbe3ef; border-radius:14px; padding:24px; background:#fff; margin-bottom:20px; min-width:0; }
      .compras-carga { background:#eef4fc; border-color:#cedef3; }
      .compras-seccion-titulo { display:flex; align-items:flex-start; gap:12px; margin-bottom:22px; }
      .compras-paso { display:grid; place-items:center; flex-shrink:0; width:34px; height:34px; border-radius:10px; background:#e9f1ff; color:#28538b; font-size:12px; font-weight:700; }
      .compras-seccion-titulo h2, .compras-historial-titulo h2 { margin:0 0 5px; font-size:17px; color:#263247; }
      .compras-seccion-titulo p, .compras-historial-titulo p { margin:0; font-size:13px; color:#64748b; line-height:1.6; }
      .compras-insignia { margin-left:auto; background:white; color:#28538b; border:1px solid #cedef3; font-size:11px; font-weight:700; border-radius:20px; padding:5px 10px; flex-shrink:0; white-space:nowrap; }
      .compras-pagina .compras-bloque .form-grid { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:18px 22px; }
      .compras-bloque .form-group { margin:0; min-width:0; }
      .compras-bloque .form-group > label, .compras-carga .form-group > label { font-size:13px; font-weight:600; color:#334155; margin-bottom:7px; }
      .compras-bloque .input-control, .compras-carga .input-control, #panelComprasRegistradas .input-control { border:1px solid #cbd5e1; border-radius:8px; background:#fff; color:#263247; padding:11px 12px; font-size:14px; min-height:42px; width:100%; box-sizing:border-box; min-width:0; }
      .compras-bloque .input-control:focus, .compras-carga .input-control:focus { outline:2px solid #3776bc; outline-offset:2px; }
      .compras-bloque .input-control:disabled, .compras-bloque .input-control[readonly] { background:#f5f7fb; color:#64748b; }
      .compras-bloque .form-grid + .form-grid { margin-top:18px; }
      .compras-bloque .btn, .compras-carga .btn, #panelComprasRegistradas .btn { border-radius:8px; font-size:13px; }
      #btnEditarProveedorCompra { margin-top:8px; padding:7px 10px; }
      .compras-carga input[type="file"] { padding:14px; border:1px dashed #8eaed5; }
      .compras-archivo-fila { display:flex; align-items:center; gap:12px; }
      .compras-archivo-fila input[type="file"] { flex:1; min-width:0; }
      .compras-archivo-fila button { flex-shrink:0; white-space:nowrap; }
      #ocrEstado { display:block; margin-top:10px; font-size:13px; }
      #ocrEstado:empty { display:none; }
      @media(max-width:480px) { .compras-archivo-fila { flex-wrap:wrap; } .compras-archivo-fila input[type="file"] { flex-basis:100%; } .compras-archivo-fila button { margin-left:auto; } }
      .compras-carga input[type="file"]::file-selector-button { border:0; border-radius:6px; background:#e5edf9; color:#28538b; padding:9px 12px; margin-right:12px; cursor:pointer; }
      .compras-ayuda { color:#64748b; font-size:12px; line-height:1.7; margin-bottom:0; }
      #revisionOCR { background:white; border:1px solid #dbe3ef; border-radius:10px; padding:14px; margin-top:16px; }
      #advertenciasOCR { font-size:13px; color:#805716; line-height:1.6; }
      .compras-productos .table-container { border:1px solid #e0e7f0; border-radius:10px; margin-top:18px; }
      .compras-pagina .table th { background:#eef3fa; color:#526078; font-size:12px; }
      .compras-pagina .table td { font-size:13px; vertical-align:middle; }
      .compras-pagina .compras-resumen .form-grid { grid-template-columns:repeat(3,minmax(0,1fr)); padding:18px; border-radius:10px; background:#f5f8fc; }
      #txtTotalCompra { color:#28538b; font-size:20px; font-weight:700; border-color:#a9c1e0; }
      .compras-resumen > .form-actions { border-top:1px solid #e4eaf2; margin-top:20px; padding-top:18px; display:flex; gap:10px; flex-wrap:wrap; justify-content:flex-end; }
      .compras-resumen .form-check { flex-basis:100%; font-size:13px; color:#526078; }
      #confirmarOCRContenedor[hidden] { display:none; }
      .compras-historial-titulo { margin:4px 0 20px; }
      #panelComprasRegistradas > .card { border:1px solid #dbe3ef; border-radius:14px; box-shadow:none; padding:22px; }
      @media(max-width:640px) {
        .compras-encabezado { padding:20px; }
        .compras-carga, .compras-bloque, #panelComprasRegistradas > .card { padding:16px; }
        .compras-pagina > .ventas-vistas { width:auto; }
        .compras-pagina > .ventas-vistas button { flex:1; padding:10px; font-size:12px; }
        .compras-pagina .compras-bloque .form-grid, .compras-pagina .compras-resumen .form-grid { grid-template-columns:1fr; gap:14px; }
        .compras-seccion-titulo { gap:10px; }
        .compras-seccion-titulo h2 { font-size:16px; }
        .compras-resumen > .form-actions .btn { flex:1; }
        #btnGuardarCompra { flex-basis:100%; }
      }

      #formularioCompletoCompra:not(.compra-registrada) > .compras-bloque:not(#segundaRevisionCompra) { display:none; }
      #segundaRevisionCompra { scroll-margin-top:100px; }
      #segundaRevisionCompra .compras-seccion-titulo { flex-wrap:wrap; }
      #segundaRevisionCompra .ocr-etiqueta { white-space:nowrap; flex-shrink:0; }
      #revisionNumeroOCR[hidden] { display:none; }
      #revisionNumeroOCR > span { display:block; margin:5px 0; }
      #revisionNumeroOCR input { width:auto; }
      #modalAltasCompra #revisionNumeroOCR label { flex-direction:row; align-items:center; gap:8px; }
      #segundaRevisionCompra[hidden] { display:none; }
      #formularioCompletoCompra.en-segunda-revision > .compras-bloque:not(#segundaRevisionCompra), #formularioCompletoCompra.en-segunda-revision > .compras-carga { display:none; }
      .segunda-datos { display:grid; grid-template-columns:repeat(3,minmax(0,1fr)); gap:16px; margin:0 0 22px; }
      .segunda-datos dt, .segunda-totales dt { font-size:12px; color:#64748b; margin-bottom:5px; }
      .segunda-datos dd, .segunda-totales dd { margin:0; font-size:14px; font-weight:600; overflow-wrap:anywhere; }
      .segunda-totales { display:flex; flex-wrap:wrap; justify-content:flex-end; gap:28px; background:#f5f8fc; padding:18px; border-radius:10px; margin:20px 0; }
      .segunda-totales dd { font-size:20px; color:#28538b; }
      .segunda-confirmacion { display:flex; align-items:flex-start; gap:10px; font-size:13px; color:#526078; margin:18px 0; }
      .segunda-confirmacion input { margin-top:2px; }
      #segundaRevisionCompra td small { display:block; color:#64748b; margin-top:5px; }
      #segundaRevisionCompra > .form-actions { justify-content:flex-end; gap:10px; }
      @media(max-width:640px) { .segunda-datos { grid-template-columns:1fr 1fr; } .segunda-totales { justify-content:space-between; gap:14px; } #segundaRevisionCompra .table-container { overflow:auto; } }

      #panelComprasRegistradas .compras-historial { padding:0; overflow:hidden; }
      .historial-cabecera { display:flex; align-items:center; justify-content:space-between; flex-wrap:wrap; gap:16px; padding:24px; border-bottom:1px solid #e4eaf2; }
      .historial-cabecera h2 { margin:0 0 6px; font-size:18px; color:#263247; }
      .historial-cabecera p { margin:0; font-size:13px; color:#64748b; line-height:1.6; }
      .historial-herramientas { display:flex; align-items:flex-end; gap:14px; padding:22px 24px 12px; }
      .historial-herramientas .form-group { flex:1; margin:0; min-width:0; }
      .historial-herramientas label { color:#526078; font-size:13px; font-weight:600; }
      .historial-herramientas button { min-height:42px; flex-shrink:0; }
      .historial-resultado { color:#64748b; font-size:12px; padding:0 24px 16px; margin:0; }
      #panelComprasRegistradas .table-container { margin:0; border-top:1px solid #e4eaf2; }
      #tablaCompras { margin:0; border-collapse:separate; border-spacing:0; width:100%; }
      #tablaCompras th { background:#eef3fa; color:#526078; font-weight:600; font-size:11px; padding:14px 12px; white-space:nowrap; border-bottom:1px solid #dbe3ef; }
      #tablaCompras td { padding:16px 12px; border-bottom:1px solid #edf1f6; color:#334155; font-size:12px; }
      #tablaCompras tbody tr:last-child td { border-bottom:0; }
      #tablaCompras tbody tr:hover td { background:#f8faff; }
      #tablaCompras td:nth-child(2), #tablaCompras td:nth-child(5) { font-weight:600; color:#28538b; }
      #tablaCompras td:nth-child(7), #tablaCompras td:nth-child(8), #tablaCompras td:nth-child(9) { white-space:nowrap; font-variant-numeric:tabular-nums; }
      #tablaCompras td:nth-child(9) { font-weight:700; color:#28538b; }
      #tablaCompras th:last-child { background:#eef3fa; }
      #tablaCompras .badge { display:inline-block; padding:5px 9px; border-radius:20px; font-size:10px; font-weight:600; }
      #tablaCompras .badge-active { color:#16603d; background:#e5f5ed; }
      #tablaCompras .badge-inactive { color:#a32128; background:#fdebee; }
      #tablaCompras .table-actions { gap:6px; min-width:110px; }
      #tablaCompras .table-actions .btn { font-size:11px; border-radius:6px; min-height:30px; padding:7px 10px; }
      #tablaCompras .btn-warning { background:#eef3fa; border:1px solid #dbe3ef; color:#28538b; }
      #tablaCompras .btn-danger { background:#fff; color:#b42332; border:1px solid #f1cdd2; }
      @media(max-width:768px) {
        .historial-cabecera { padding:18px; }
        .historial-herramientas { padding:18px 18px 12px; flex-wrap:wrap; }
        .historial-herramientas .form-group { flex-basis:100%; }
        .historial-resultado { padding-left:18px; padding-right:18px; }
        #tablaCompras, #tablaCompras tbody { display:block; }
        #tablaCompras thead { display:none; }
        #tablaCompras tbody tr { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:12px; padding:18px; border-bottom:1px solid #dbe3ef; }
        #tablaCompras td { display:block; padding:0; border:0; min-width:0; overflow-wrap:anywhere; }
        #tablaCompras td::before { content:attr(data-label); display:block; color:#64748b; font-size:10px; font-weight:400; margin-bottom:5px; }
        #tablaCompras td:nth-child(2), #tablaCompras td:nth-child(5), #tablaCompras td:last-child { grid-column:1/-1; }
        #tablaCompras td:last-child { position:static; box-shadow:none; background:transparent; width:auto; }
        #tablaCompras .table-actions { flex-direction:row; flex-wrap:wrap; }
        #tablaCompras .table-actions .btn { flex:1; text-align:center; }
      }
    </style>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="compras-pagina">
    <div class="page-title compras-encabezado">

        <h1>Compras</h1>

        <p>Comprobantes, productos y entradas de inventario en un solo lugar.</p>

    </div>

    <nav class="ventas-vistas" aria-label="Secciones de compras">
        <button type="button" class="btn btn-primary" data-vista-compras="registro" aria-controls="panelRegistroCompra" aria-pressed="true">Registrar compra</button>
        <button type="button" class="btn btn-secondary" data-vista-compras="registros" aria-controls="panelComprasRegistradas" aria-pressed="false">Compras realizadas</button>
    </nav>
    <section id="panelRegistroCompra" aria-label="Registrar compra">
    <div class="card" id="formularioCompletoCompra">

        <section class="compras-carga">
<div class="compras-seccion-titulo"><span class="compras-paso">01</span><div><h2>Importar comprobante</h2><p>Carga una imagen o PDF para preparar la compra con OCR.</p></div><span class="compras-insignia">OCR</span></div>

        <div class="form-group">
            <label for="txtArchivoComprobante">Cargar comprobante (imagen/PDF) para OCR</label>
            <div class="compras-archivo-fila">
                <input type="file" id="txtArchivoComprobante" class="input-control" accept=".jpg,.jpeg,.png,.bmp,.tif,.tiff,.pdf" />
                <button type="button" class="btn btn-secondary" id="btnExtraerOCR" onclick="extraerOCRCompra()">Extraer datos con OCR</button>
            </div>
            <span id="ocrEstado" role="status" aria-live="polite"></span>
        </div>

        <p class="compras-ayuda">JPG, PNG, BMP, TIFF o PDF · Máximo 10 MB y 5 páginas. Los productos se cargarán desde el OCR.</p>
        <div id="revisionOCR" hidden>
            <p id="advertenciasOCR" role="status"></p>
            <button type="button" class="btn btn-secondary" onclick="prepararAltasCompra()">Revisar vista previa OCR</button>
            <pre id="textoOCR" hidden></pre>
        </div>
        </section>
    <dialog id="modalProveedorCompra" class="compra-dialog" aria-labelledby="tituloProveedorCompra">
      <header><h2 id="tituloProveedorCompra">Modificar proveedor</h2><button type="button" aria-label="Cerrar" onclick="cerrarModalCompra('modalProveedorCompra')">×</button></header>
      <div class="modal-cuerpo">
        <p>Actualiza los datos del proveedor en el catálogo. No cambia el proveedor asociado a compras ya guardadas.</p>
        <label>RUC<input id="rucProveedorModal" class="input-control" maxlength="11" inputmode="numeric" /></label>
        <label>Razón social<input id="razonProveedorModal" class="input-control" maxlength="150" /></label>
        <label>Representante<input id="representanteProveedorModal" class="input-control" maxlength="100" /></label>
        <label>Teléfono<input id="telefonoProveedorModal" class="input-control" maxlength="9" inputmode="numeric" /></label>
        <label>Correo<input id="correoProveedorModal" class="input-control" type="email" maxlength="100" /></label>
        <label>Dirección<input id="direccionProveedorModal" class="input-control" maxlength="150" /></label>
        <p id="errorProveedorModal" class="modal-error" role="alert"></p>
      </div>
      <footer><button id="guardarProveedorModal" type="button" class="btn btn-primary" onclick="guardarProveedorDesdeCompra()">Guardar cambios</button><button type="button" class="btn btn-secondary" onclick="cerrarModalCompra('modalProveedorCompra')">Cancelar</button></footer>
    </dialog>
    <dialog id="modalAltasCompra" class="compra-dialog" aria-labelledby="tituloAltasCompra">
      <header><h2 id="tituloAltasCompra">Vista previa de compra OCR</h2><button type="button" aria-label="Cerrar" onclick="cerrarModalCompra('modalAltasCompra')">×</button></header>
      <div class="modal-cuerpo">
        <div class="ocr-presentacion"><p class="alta-intro">Revise y confirme los datos extraídos antes de registrar la compra</p><span id="estadoVistaOCR" class="ocr-etiqueta ocr-revision" role="status">Pendiente de revisión</span></div>
        <div class="ocr-layout"><section class="ocr-edicion" aria-label="Datos de la compra">
        <section class="ocr-documento"><h3>Información del comprobante</h3>
          <div class="ocr-proveedor-cabecera"><h4>Proveedor</h4><span id="estadoProveedorOCR" role="status"></span></div><details class="ocr-cambiar-proveedor"><summary>Elegir otro proveedor del catálogo</summary><label>Proveedor registrado<select id="previewProveedor" class="input-control"></select></label></details>
        <div id="proveedorAltaCampos">
          <label>RUC del proveedor<input id="altaRuc" class="input-control" maxlength="11" inputmode="numeric" /></label>
          <label>Razón social del proveedor<input id="altaRazon" class="input-control" maxlength="150" placeholder="Completa el nombre si no se reconoció" /></label>
        </div>
          <div class="ocr-campos">
            <label>Fecha de emisión<input id="previewFecha" class="input-control" type="date" /></label>
            <label>Tipo de comprobante<select id="previewTipo" class="input-control"></select></label>
            <div class="alta-ancho"><label>Serie y número<input id="previewNumero" class="input-control" maxlength="50" list="numeroOCRAlternativas" /></label><datalist id="numeroOCRAlternativas"></datalist><div id="revisionNumeroOCR" class="alta-advertencia" hidden><p>El OCR detectó números distintos. Revisa el número propuesto con la imagen.</p><label><input type="checkbox" id="confirmarNumeroOCR" /> Confirmé el número con el original.</label></div></div>
            <label>Subtotal (S/)<input id="previewSubtotal" class="input-control" type="number" step="0.01" min="0" /></label>
            <label>IGV (S/)<input id="previewIgv" class="input-control" type="number" step="0.01" min="0" /></label>
            <label>Total (S/)<input id="previewTotal" class="input-control" type="number" step="0.01" min="0" /></label>
            <label>Precios del detalle<select id="previewIncluidos" class="input-control"><option value="si">Incluyen IGV</option><option value="no">Sin IGV</option></select><small>Usa precios netos después de descuentos.</small></label>
          </div>
          <label>Observación<textarea id="previewObservacion" class="input-control" rows="2" maxlength="300"></textarea></label>
          <p id="ayudaImpuestosOCR" class="alta-advertencia" hidden>El OCR no encontró un desglose completo de subtotal e IGV. Revísalos en el documento y complétalos; el total por sí solo no permite identificar el impuesto.</p>
          <p id="estadoDuplicadoOCR" role="status"></p>
        </section>
        <h3>Productos detectados</h3><datalist id="marcasCompraOCR"></datalist>
        <div id="productosAltaCampos"></div>
        </section><aside class="ocr-original" aria-label="Referencia del comprobante"><h3>Comprobante y lectura OCR</h3><div class="ocr-referencia-tabs" role="tablist" aria-label="Referencia"><button type="button" id="tabDocumentoOCR" role="tab" aria-controls="archivoOriginalOCR" onclick="mostrarReferenciaCompra('documento')">Documento</button><button type="button" id="tabTextoOCR" role="tab" aria-controls="panelTextoOCR" onclick="mostrarReferenciaCompra('texto')">Texto OCR</button></div><div id="controlesZoomOCR" class="ocr-zoom" role="group" aria-label="Zoom del comprobante" hidden><button type="button" id="alejarImagenOCR" class="btn btn-secondary" aria-label="Alejar imagen" onclick="cambiarZoomOCR(-25)">−</button><span id="porcentajeZoomOCR" role="status" aria-live="polite">100%</span><button type="button" id="acercarImagenOCR" class="btn btn-secondary" aria-label="Acercar imagen" onclick="cambiarZoomOCR(25)">+</button><button type="button" class="btn btn-secondary" onclick="ajustarImagenOCR()">Ajustar</button></div><div id="archivoOriginalOCR" role="tabpanel" tabindex="0" aria-labelledby="tabDocumentoOCR"></div><div id="panelTextoOCR" role="tabpanel" aria-labelledby="tabTextoOCR" hidden><p id="origenTextoOCR"></p><pre id="previewTextoOriginal"></pre></div><button type="button" class="btn btn-secondary ocr-reescanear" onclick="volverEscanearCompra()">Volver a escanear</button></aside></div>
        <p id="errorAltasCompra" class="modal-error" role="alert"></p>
      </div>
      <footer><button type="button" class="btn btn-secondary" onclick="cerrarModalCompra('modalAltasCompra')">Cancelar</button><button type="button" id="confirmarRegistrarOCR" class="btn btn-primary" onclick="registrarDesdeVistaPrevia()">Aplicar cambios</button></footer>
    </dialog>
        <section id="segundaRevisionCompra" class="compras-bloque" hidden aria-labelledby="tituloSegundaRevision">
          <div class="compras-seccion-titulo"><span class="compras-paso">02</span><div><h2 id="tituloSegundaRevision" tabindex="-1">Segunda revisión de la compra</h2><p>Verifica los datos aplicados desde la vista previa antes de registrar.</p></div><span class="ocr-etiqueta ocr-revision">Pendiente de registro</span></div>
          <dl id="datosSegundaRevision" class="segunda-datos"></dl>
          <div class="table-container"><table class="table"><thead><tr><th>Producto</th><th>Cantidad</th><th>Precio compra</th><th>Precio venta</th><th>Importe</th><th>Acción / stock previsto</th></tr></thead><tbody id="productosSegundaRevision"></tbody></table></div>
          <dl id="totalesSegundaRevision" class="segunda-totales"></dl>
          <label class="segunda-confirmacion"><input type="checkbox" id="confirmarSegundaRevision" /> Verifiqué los datos y autorizo registrar esta compra.</label>
          <p id="errorSegundaRevision" class="modal-error" role="alert"></p>
          <div class="form-actions"><button type="button" class="btn btn-secondary" onclick="prepararAltasCompra()">Volver a editar</button><button type="button" class="btn btn-primary" onclick="registrarSegundaRevisionCompra()">Confirmar y registrar compra</button></div>
        </section>
        <input type="hidden" id="txtIdCompra" />
        <input type="hidden" id="txtSubTotal" />
        <input type="hidden" id="txtIgv" />
        <input type="hidden" id="txtTotal" />
        <section class="compras-bloque"><div class="compras-seccion-titulo"><span class="compras-paso">02</span><div><h2>Datos del comprobante</h2><p>Selecciona al proveedor y revisa los datos de la compra.</p></div></div>

        <div class="form-grid">

            <div class="form-group">

                <label for="cboProveedor">Proveedor</label>

                <select id="cboProveedor"
                        class="input-control"></select>
                <button type="button" class="btn btn-secondary" id="btnEditarProveedorCompra" onclick="abrirProveedorCompra()">Modificar proveedor</button>

            </div>

            <div class="form-group">

                <label for="cboUsuario">Usuario</label>

                <select id="cboUsuario"
                        class="input-control"></select>

            </div>

            <div class="form-group">

                <label for="cboTipoComprobante">Tipo de Comprobante</label>

                <select id="cboTipoComprobante"
                        class="input-control">
                    <option value="BOLETA">Boleta de venta electrónica</option>
                    <option value="FACTURA">Factura electrónica</option>
                    <option value="PEDIDO">Pedido</option>
                    <option value="TICKET">Ticket</option>
                    <option value="NOTA_VENTA">Nota de venta</option>
                    <option value="OTRO">Otro documento / no identificado</option>
                </select>

            </div>

            <div class="form-group">

                <label for="txtNumeroComprobante">Número de Comprobante</label>

                <input type="text"
                       id="txtNumeroComprobante" maxlength="50" placeholder="Número del comprobante del proveedor"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="txtObservacion">Observación</label>

                <input type="text"
                       id="txtObservacion"
                       class="input-control" />

            </div>

            <div class="form-group">

                <label for="cboEstado">Estado</label>

                <select id="cboEstado" disabled
                        class="input-control">
                    <option value="REGISTRADA">REGISTRADA</option>
                    <option value="ANULADA">ANULADA</option>
                </select>

            </div>

        </div>

        <div class="form-grid">
            <div class="form-group"><label for="txtFechaCompra">Fecha del comprobante</label><input type="date" id="txtFechaCompra" class="input-control" /></div>
            <div class="form-group"><label for="preciosIncluyenIgv">Precios de los productos</label><select id="preciosIncluyenIgv" class="input-control" onchange="recalcTotalesCompra()"><option value="no">Sin IGV</option><option value="si">Incluyen IGV</option></select></div>
        </div>
        </section>
        <section id="resumenProductosOCR" class="compras-bloque compras-productos" hidden><div class="compras-seccion-titulo"><span class="compras-paso">OCR</span><div><h2>Productos del comprobante</h2><p>Productos cargados desde el OCR. Puedes revisarlos en la vista previa.</p></div></div>
        <div class="table-container">

            <table class="table"
                   id="tablaDetalleCompra">

                <thead>

                    <tr>
                        <th>Producto</th>
                        <th>Cantidad</th>
                        <th>Precio</th>
                        <th>Subtotal</th>
                        <th>Acción</th>
                    </tr>

                </thead>

                <tbody id="bodyDetalleCompra">

                </tbody>

            </table>

        </div>

        </section>
        <section class="compras-bloque compras-resumen"><div class="compras-seccion-titulo"><span class="compras-paso">03</span><div><h2>Resumen de la compra</h2><p>Comprueba los importes antes de registrar la entrada al inventario.</p></div></div>
        <div class="form-grid">

            <div class="form-group">

                <label for="txtSubtotalCompraGral">Subtotal general</label>

                <input type="number" step="0.01" min="0"
                       id="txtSubtotalCompraGral"
                       class="input-control"
                       onchange="actualizarMontosCompra()" />

            </div>

            <div class="form-group">

                <label for="txtIGVCompra">IGV del comprobante</label>

                <input type="number" step="0.01" min="0"
                       id="txtIGVCompra"
                       class="input-control"
                       onchange="actualizarMontosCompra()" />

            </div>

            <div class="form-group">

                <label for="txtTotalCompra">Total</label>

                <input type="number" step="0.01" min="0"
                       id="txtTotalCompra"
                       class="input-control"
                       onchange="actualizarMontosCompra()" />

            </div>

        </div>

        <div class="form-actions">

            <label id="confirmarOCRContenedor" class="form-check" hidden><input type="checkbox" id="confirmarOCR" /> Revisé el proveedor, los productos y los importes del documento.</label>

            <button type="button"
                    class="btn btn-primary"
                    id="btnGuardarCompra" onclick="guardarCompra()">
                Registrar compra
            </button>

            <button type="button"
                    class="btn btn-warning"
                    id="btnModificarCompra" onclick="modificarCompra()">
                Guardar observación
            </button>

            <button type="button"
                    class="btn btn-secondary"
                    id="btnLimpiarCompra" onclick="limpiarFormulario()">
                Limpiar
            </button>

        </div>

        </section>
    </div>

    </section>
    <section id="panelComprasRegistradas" aria-label="Compras realizadas" hidden>
        <div class="card compras-historial">
          <div class="historial-cabecera"><div><h2>Compras realizadas</h2><p>Consulta los comprobantes y las entradas registradas en tu inventario.</p></div><span class="ocr-etiqueta ocr-existente">Historial de compras</span></div>
          <div class="historial-herramientas"><div class="form-group"><label for="buscarCompras">Buscar en el historial</label><input type="search" id="buscarCompras" class="input-control" placeholder="Proveedor, comprobante, fecha o estado…" /></div><button type="button" class="btn btn-secondary" onclick="listarCompras()">Actualizar lista</button></div>
          <p id="estadoBusquedaCompras" class="historial-resultado" role="status" aria-live="polite"></p>
        <div class="table-container">

            <table class="table"
                   id="tablaCompras">

                <thead>

                    <tr>
                        <th>ID</th>
                        <th>Proveedor</th>
                        <th>Usuario</th>
                        <th>Comprobante</th>
                        <th>N° Comprobante</th>
                        <th>Fecha</th>
                        <th>Sub Total</th>
                        <th>IGV</th>
                        <th>Total</th>
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>

                </thead>

                <tbody id="bodyCompras">

                </tbody>

            </table>

        </div>

    </div>

    </section>
    </div>
</asp:Content>
