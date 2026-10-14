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
            var migration = File.ReadAllText(Path.Combine(root,"Scripts/02_compras_comprobante_activo.sql"));
            migration = Regex.Replace(migration,@"(?im)^USE DB_keyove_inventario;\s*$","");
            foreach(var batch in Regex.Split(migration,@"(?im)^\s*GO\s*$")) if(!String.IsNullOrWhiteSpace(batch)) Sql(batch);
            var purchases = File.ReadAllText(Path.Combine(root,"Scripts/03_registrar_compra_transaccion.sql"));
            purchases = Regex.Replace(purchases,@"(?im)^USE DB_keyove_inventario;\s*$","");
            for(int run=0;run<2;run++)
                foreach(var batch in Regex.Split(purchases,@"(?im)^\s*GO\s*$")) if(!String.IsNullOrWhiteSpace(batch)) Sql(batch);
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
            // Force a runtime SQL error after stock and archive writes, only in the test database.
            Sql("CREATE TRIGGER FalloArchivo ON Comprobantes_OCR AFTER INSERT AS BEGIN THROW 51001, 'Fallo de archivo simulado', 1; END");
            int stockBefore=Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1"), countBefore=Value("SELECT COUNT(*) FROM Compras");
            var archiveFailed = Page.GuardarCompra(Purchase("F001-000126"),Lines(),file,"2026-10-13",false);
            Assert(!archiveFailed.ok && Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==stockBefore && Value("SELECT COUNT(*) FROM Compras")==countBefore,"Fallo al archivar revierte compra e inventario");
            Assert(Value("SELECT COUNT(*) FROM Detalle_Compra")==1 && Value("SELECT COUNT(*) FROM Movimientos_Inventario")==1 && Value("SELECT COUNT(*) FROM Comprobantes_OCR")==1,"Rollback elimina detalles, movimientos y archivo fallidos");
            Sql("DROP TRIGGER FalloArchivo");
            var mixed = Lines(); mixed.Add(new Detalle { iCodProducto=2, iCantidad=2, nPrecioCompra=0m });
            var mixedFailed = Page.GuardarCompra(Purchase("F001-MIXED"),mixed,null,"2026-10-13",false);
            Assert(!mixedFailed.ok && Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==stockBefore && Value("SELECT COUNT(*) FROM Compras")==countBefore,"Segundo producto inactivo revierte también el primero");
            Assert(Page.EliminarCompra(result.idCompra)=="OK", "Anular compra");
            int remaining=Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1");
            Assert(remaining==stockBefore-2, "Anulación revierte su entrada de stock");
            Assert(Page.EliminarCompra(result.idCompra)=="OK" && Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==remaining,"Anulación repetida no descuenta dos veces");
            var corrected = Page.GuardarCompra(Purchase("F001-000123"),Lines(),file,"2026-10-13",false);
            Assert(corrected.ok && corrected.idCompra != result.idCompra, "Comprobante anulado puede registrarse corregido conservando historial");
            Assert(Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==2, "Compra corregida actualiza stock una vez");
            Assert(WebApplication_keyove.WebApplication_Keyove.Views.Operaciones.Detalle_Compra.DetalleCompra1.GuardarDetalleCompra(Lines()[0]).StartsWith("ERROR:"), "No permite alterar detalles registrados fuera de la operación");
            Sql("INSERT Productos(iCodCategoria,cCodigo,cNombre) VALUES(1,'TERCERO','TERCER PRODUCTO')");
            var multiple = Lines(); multiple.Add(new Detalle { iCodProducto=3,iCantidad=3,nPrecioCompra=5m });
            var multiPurchase=Purchase("F001-MULTI"); multiPurchase.nSubTotal=35m; multiPurchase.nIgv=6.3m; multiPurchase.nTotal=41.3m;
            var multi=Page.GuardarCompra(multiPurchase,multiple,null,"2026-10-13",false);
            Assert(multi.ok && Value("SELECT COUNT(*) FROM Detalle_Compra WHERE iCodCompra="+multi.idCompra)==2,"Una cabecera registra varios productos sin archivo opcional");
            Assert(Value("SELECT iStockActual FROM Productos WHERE iCodProducto=1")==4 && Value("SELECT iStockActual FROM Productos WHERE iCodProducto=3")==3 && Value("SELECT COUNT(*) FROM Movimientos_Inventario WHERE iCodCompra="+multi.idCompra)==2,"Compra múltiple incrementa cada producto una vez");
            bool invalidSql=false;
            try { Sql("DECLARE @d dbo.TipoDetalleCompra; INSERT @d VALUES(1,0,10); EXEC dbo.sp_RegistrarCompra 1,1,'FACTURA','INVALID','20261013',20,3.6,23.6,0,@d;"); }
            catch(SqlException ex) { invalidSql=ex.Number==50001; }
            Assert(invalidSql && Value("SELECT COUNT(*) FROM Compras WHERE cNumeroComprobante='INVALID'")==0,"SQL rechaza cantidades inválidas sin depender del formulario");
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
