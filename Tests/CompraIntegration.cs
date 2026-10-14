using System;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using Page = WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Compras.Compra1;
using Compra = WebApplication_keyove.WebApplication_Keyove.Model.Compras;
using Detalle = WebApplication_keyove.WebApplication_Keyove.Model.DetalleCompra;

class CompraIntegration {
    static string connection;
    static void Sql(string sql) { using (var c = new SqlConnection(connection)) { c.Open(); using (var cmd = new SqlCommand(sql,c)) { cmd.ExecuteNonQuery(); } } }
    static int Value(string sql) { using (var c = new SqlConnection(connection)) { c.Open(); using (var cmd = new SqlCommand(sql,c)) { return Convert.ToInt32(cmd.ExecuteScalar()); } } }
    static void Assert(bool condition, string message) { if(!condition) throw new Exception(message); Console.WriteLine("OK: " + message); }
    static Compra Purchase(string number) { return new Compra { iCodProveedor=1, cTipoComprobante="FACTURA", cNumeroComprobante=number, nSubTotal=20m, nIgv=3.6m, nTotal=23.6m }; }
    static List<Detalle> Lines(int product=1) { return new List<Detalle> { new Detalle {iCodProducto=product, iCantidad=2, nPrecioCompra=10m} }; }
    static void Main(string[] args) {
        var root = args[0];
        var name = "KEYOVE_OCR_TEST_" + Guid.NewGuid().ToString("N");
        var master = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;";
        bool created = false;
        try {
            connection = master;
            Sql("CREATE DATABASE [" + name + "]"); created = true;
            connection = master.Replace("Initial Catalog=master", "Initial Catalog="+name);
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings.Remove("ConexionInventario");
            config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("ConexionInventario",connection));
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
            var schema = File.ReadAllText(Path.Combine(root,"Data/use DB_keyove_Inventario.txt"));
            schema = Regex.Replace(schema,@"(?im)^\s*(CREATE DATABASE|USE)\s+DB_keyove_inventario;?\s*$","");
            foreach(var batch in Regex.Split(schema,@"(?im)^\s*GO\s*$")) if(!String.IsNullOrWhiteSpace(batch)) Sql(batch);
            var ocr = File.ReadAllText(Path.Combine(root,"Scripts/01_crear_comprobantes_ocr.sql"));
            ocr = Regex.Replace(ocr,@"(?im)^USE DB_keyove_inventario;\s*$","");
            foreach(var batch in Regex.Split(ocr,@"(?im)^\s*GO\s*$")) if(!String.IsNullOrWhiteSpace(batch)) Sql(batch);
            // Simulate an existing installation with the original unfiltered index.
            Sql("DROP INDEX UQ_Compras_Comprobante ON Compras; CREATE UNIQUE INDEX UQ_Compras_Comprobante ON Compras(iCodProveedor,cTipoComprobante,cNumeroComprobante)");
            var migration = File.ReadAllText(Path.Combine(root,"Scripts/03_compras_comprobante_activo.sql"));
            migration = Regex.Replace(migration,@"(?im)^USE DB_keyove_inventario;\s*$","");
            foreach(var batch in Regex.Split(migration,@"(?im)^\s*GO\s*$")) if(!String.IsNullOrWhiteSpace(batch)) Sql(batch);
            var sales = File.ReadAllText(Path.Combine(root,"Scripts/02_registro_ventas_inventario.sql"));
            sales = Regex.Replace(sales,@"(?im)^USE DB_keyove_inventario;\s*$","");
            foreach(var batch in Regex.Split(sales,@"(?im)^\s*GO\s*$")) if(!String.IsNullOrWhiteSpace(batch)) Sql(batch);
            Sql("INSERT Roles(cNombre) VALUES('Administrador'); INSERT Usuarios(iCodRol,cNombreUsuario,cContrasenaHash) VALUES(1,'PRUEBA','NO_LOGIN'); INSERT Categorias(cNombre) VALUES('PRUEBA'); INSERT Proveedores(cRuc,cRazonSocial) VALUES('20123456789','PRUEBA'); INSERT Productos(iCodCategoria,cCodigo,cNombre) VALUES(1,'TEST','MARTILLO'); INSERT Productos(iCodCategoria,cCodigo,cNombre,bEstado) VALUES(1,'INACTIVO','INACTIVO',0);");
            var context = new HttpContext(new HttpRequest("", "http://localhost/", ""),new HttpResponse(new StringWriter()));
            HttpContext.Current = context;
            var session = new HttpSessionStateContainer("test",new SessionStateItemCollection(),new HttpStaticObjectsCollection(),10,true,HttpCookieMode.UseCookies,SessionStateMode.InProc,false);
            SessionStateUtility.AddHttpSessionStateToContext(context,session);
            context.Session["iCodUsuario"] = 1;
            context.Session["Rol"] = "Administrador";
            var content = new byte[3 * 1024 * 1024]; content[0]=137; content[1]=80; content[2]=78; content[3]=71;
            var file = new Page.ArchivoCompra {nombreArchivo="prueba.png",contenidoBase64=Convert.ToBase64String(content),textoOcr="texto de prueba"};
            var result = Page.GuardarCompra(Purchase("F001-000123"),Lines(),file,"2026-10-13",false);
            Assert(result.ok, "Guardar compra: " + result.mensaje);
            Assert(Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==2, "Stock actualizado una vez");
            Assert(Value("SELECT COUNT(*) FROM Movimientos_Inventario WHERE iCodCompra="+result.idCompra)==1, "Movimiento asociado al ID devuelto");
            Assert(Value("SELECT DATALENGTH(imgComprobante) FROM Comprobantes_OCR WHERE iCodCompra="+result.idCompra)==content.Length, "Archivo mayor de 2 MB conservado completo");
            Assert(Value("SELECT COUNT(*) FROM Compras WHERE cNumeroComprobante='F001-000123' AND dFechaCompra='20261013'")==1, "Número y fecha originales conservados");
            var duplicate = Page.GuardarCompra(Purchase("F001-000123"),Lines(),file,"2026-10-13",false);
            Assert(!duplicate.ok && Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==2, "Duplicado no incrementa stock");
            var failed = Page.GuardarCompra(Purchase("F001-000124"),Lines(2),file,"2026-10-13",false);
            Assert(!failed.ok && Value("SELECT COUNT(*) FROM Compras")==1 && Value("SELECT COUNT(*) FROM Detalle_Compra")==1, "Producto inactivo revierte cabecera y detalles");
            // Force an actual SQL error after stock update: temporarily remove archive table.
            Sql("EXEC sp_rename 'Comprobantes_OCR', 'Comprobantes_OCR_temporal'");
            int stockBefore=Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1"), countBefore=Value("SELECT COUNT(*) FROM Compras");
            var archiveFailed = Page.GuardarCompra(Purchase("F001-000126"),Lines(),file,"2026-10-13",false);
            Assert(!archiveFailed.ok && Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==stockBefore && Value("SELECT COUNT(*) FROM Compras")==countBefore,"Fallo al archivar revierte compra e inventario");
            Sql("EXEC sp_rename 'Comprobantes_OCR_temporal', 'Comprobantes_OCR'");
            var venta = new WebApplication_keyove.WebApplication_Keyove.Model.Ventas { cTipoComprobante="BOLETA", cMetodoPago="EFECTIVO", cNumeroComprobante="" };
            var lineasVenta = new List<WebApplication_keyove.WebApplication_Keyove.Model.DetalleVenta> { new WebApplication_keyove.WebApplication_Keyove.Model.DetalleVenta {iCodProducto=1,iCantidad=1,nPrecioVenta=15m,nDescuento=0m} };
            var ventaPage = typeof(WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Ventas.Venta1);
            string ventaOk = WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Ventas.Venta1.GuardarVenta(venta,lineasVenta);
            Assert(ventaOk=="OK", "Registro normal de venta: " + ventaOk);
            Assert(Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==1, "Venta descuenta stock de la compra");
            Assert(Page.EliminarCompra(result.idCompra)!="OK", "No permite anular una compra cuyo stock ya se vendió");
            lineasVenta[0].iCantidad=100;
            Assert(WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Ventas.Venta1.GuardarVenta(venta,lineasVenta)!="OK" && Value("SELECT COUNT(*) FROM Ventas")==1,"Venta sin stock se revierte");
            int ventaId=Value("SELECT iCodVenta FROM Ventas");
            Assert(WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Ventas.Venta1.EliminarVenta(ventaId)=="OK", "Anular venta repone stock");
            Assert(Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==2,"Stock repuesto después de anular venta");
            Assert(Page.EliminarCompra(result.idCompra)=="OK", "Anular compra");
            int remaining=Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1");
            Assert(remaining==stockBefore-2, "Anulación revierte su entrada de stock");
            Assert(Page.EliminarCompra(result.idCompra)=="OK" && Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==remaining,"Anulación repetida no descuenta dos veces");
            var corrected = Page.GuardarCompra(Purchase("F001-000123"),Lines(),file,"2026-10-13",false);
            Assert(corrected.ok && corrected.idCompra != result.idCompra, "Comprobante anulado puede registrarse corregido conservando historial");
            Assert(Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==2, "Compra corregida actualiza stock una vez");
            Assert(WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Detalle_Compra.DetalleCompra1.GuardarDetalleCompra(Lines()[0]).StartsWith("ERROR:"), "No permite alterar detalles registrados fuera de la operación");
            context.Session["Rol"] = "Vendedor";
            bool denied=false;
            try { Page.GuardarCompra(Purchase("F001-000127"),Lines(),null,"2026-10-13",false); } catch(HttpException ex) { denied=ex.GetHttpCode()==403; }
            Assert(denied,"Vendedor no puede registrar compras");
            Console.WriteLine("PRUEBAS SQL COMPLETADAS");
        } finally {
            HttpContext.Current = null;
            SqlConnection.ClearAllPools();
            if(created && Regex.IsMatch(name,@"^KEYOVE_OCR_TEST_[a-f0-9]{32}$")) { connection=master; Sql("ALTER DATABASE ["+name+"] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE ["+name+"]"); }
        }
    }
}
