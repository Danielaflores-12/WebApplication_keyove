Imports System.Data.SqlClient
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Web.Services
Imports WebApplication_keyove.WebApplication_Keyove.Data
Imports CompraModelo = WebApplication_keyove.WebApplication_Keyove.Model.Compras
Imports DetalleModelo = WebApplication_keyove.WebApplication_Keyove.Model.DetalleCompra

Namespace WebApplication_Keyove.Views.Operaciones.Compras
    Partial Public Class Compra1
        Private Shared infraestructuraCompraLista As Boolean
        Private Shared ReadOnly puertaInfraestructuraCompra As New Object()

        Public Class ProductoNuevoCompra
            Public temporal As Integer
            Public codigo As String
            Public nombre As String
            Public descripcion As String
            Public marca As String
            Public modelo As String
            Public categoria As Integer
            Public precioVenta As Decimal?
        End Class

        <WebMethod(EnableSession:=True)>
        Public Shared Function CategoriasParaCompra() As List(Of OpcionCombo)
            ExigirAdministradora()
            Using db As New ConexionBD()
                Dim lista As New List(Of OpcionCombo)
                For Each r As DataRow In db.ExecuteDataTable("SELECT iCodCategoria,cNombre FROM Categorias WHERE bEstado=1 ORDER BY cNombre").Rows
                    lista.Add(New OpcionCombo With {.v = Convert.ToString(r("iCodCategoria")), .t = Convert.ToString(r("cNombre"))})
                Next
                Return lista
            End Using
        End Function

        <WebMethod(EnableSession:=True)>
        Public Shared Function GuardarCompraConAltas(compra As CompraModelo, detalles As List(Of DetalleModelo), comprobante As ArchivoCompra, fechaCompra As String, preciosIncluyenIgv As Boolean, proveedorNuevo As ProveedorEdicionCompra, productosNuevos As List(Of ProductoNuevoCompra)) As ResultadoCompra
            ExigirAdministradora()
            Using db As New ConexionBD()
                Try
                    GarantizarRucProveedor(db)
                    GarantizarInfraestructuraCompra(db)
                    If compra Is Nothing OrElse compra.iCodCompra <> 0 OrElse detalles Is Nothing OrElse detalles.Count = 0 Then Throw New ArgumentException("Revisa la compra.")
                    If productosNuevos Is Nothing Then productosNuevos = New List(Of ProductoNuevoCompra)
                    Dim temporales As New HashSet(Of Integer)
                    For Each p In productosNuevos
                        If p Is Nothing OrElse p.temporal >= 0 OrElse Not temporales.Add(p.temporal) OrElse String.IsNullOrWhiteSpace(p.nombre) OrElse p.nombre.Trim().Length > 100 OrElse If(p.codigo, "").Trim().Length > 50 OrElse If(p.descripcion, "").Trim().Length > 500 OrElse If(p.marca, "").Trim().Length > 100 OrElse If(p.modelo, "").Trim().Length > 100 OrElse p.categoria <= 0 OrElse Not p.precioVenta.HasValue OrElse p.precioVenta < 0 OrElse Decimal.Round(p.precioVenta.Value, 2) <> p.precioVenta.Value Then Throw New ArgumentException("Completa los datos del producto nuevo y revisa sus longitudes.")
                        If Not detalles.Any(Function(d) d IsNot Nothing AndAlso d.iCodProducto = p.temporal) Then Throw New ArgumentException("Hay un producto nuevo que no pertenece a la compra.")
                    Next
                    If proveedorNuevo IsNot Nothing Then
                        If compra.iCodProveedor <> -1 OrElse Not System.Text.RegularExpressions.Regex.IsMatch(If(proveedorNuevo.ruc, ""), "^[0-9]{11}$") OrElse String.IsNullOrWhiteSpace(proveedorNuevo.razonSocial) OrElse proveedorNuevo.razonSocial.Trim().Length > 150 Then Throw New ArgumentException("Completa RUC y razón social del proveedor nuevo.")
                    ElseIf compra.iCodProveedor <= 0 Then
                        Throw New ArgumentException("Selecciona o prepara el proveedor.")
                    End If
                    db.BeginTransaction()
                    ' Serializa resolución de altas para evitar dos productos iguales en guardados simultáneos.
                    db.ExecuteQueryTransact("DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource='KeyoveComprasAltas',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000; IF @r<0 THROW 50001,'Reintenta: hay otra compra creando productos.',1;", Nothing)
                    If proveedorNuevo IsNot Nothing Then
                        compra.iCodProveedor = Convert.ToInt32(db.ExecuteScalarTransact(
                            "DECLARE @id int; SELECT @id=iCodProveedor FROM Proveedores WITH(UPDLOCK,HOLDLOCK) WHERE cRuc=@ruc; IF @id IS NOT NULL AND NOT EXISTS(SELECT 1 FROM Proveedores WHERE iCodProveedor=@id AND bEstado=1) THROW 50001,'El proveedor está inactivo.',1; IF @id IS NULL BEGIN INSERT Proveedores(cRuc,cRazonSocial) VALUES(@ruc,@nombre); SET @id=SCOPE_IDENTITY(); END; SELECT @id;",
                            New List(Of SqlParameter) From {New SqlParameter("@ruc", proveedorNuevo.ruc), New SqlParameter("@nombre", proveedorNuevo.razonSocial.Trim())}))
                    End If
                    For Each p In productosNuevos
                        Dim precioCompra = detalles.First(Function(d) d IsNot Nothing AndAlso d.iCodProducto = p.temporal).nPrecioCompra
                        Dim codigo = If(p.codigo, "").Trim()
                        If codigo.Length = 0 Then codigo = "OCR-" & Guid.NewGuid().ToString("N")
                        Dim id = Convert.ToInt32(db.ExecuteScalarTransact(
                            "IF NOT EXISTS(SELECT 1 FROM Categorias WITH(HOLDLOCK) WHERE iCodCategoria=@categoria AND bEstado=1) THROW 50001,'La categoría no está activa.',1; " &
                            "DECLARE @id int; IF (SELECT COUNT(*) FROM Productos WITH(UPDLOCK,HOLDLOCK) WHERE UPPER(LTRIM(RTRIM(cNombre)))=UPPER(@nombre))>1 THROW 50001,'Hay varios productos con ese nombre. Selecciona uno del inventario.',1; " &
                            "SELECT @id=iCodProducto FROM Productos WITH(UPDLOCK,HOLDLOCK) WHERE UPPER(LTRIM(RTRIM(cNombre)))=UPPER(@nombre); " &
                            "IF @id IS NOT NULL AND NOT EXISTS(SELECT 1 FROM Productos WHERE iCodProducto=@id AND bEstado=1) THROW 50001,'El producto está inactivo.',1; " &
                            "IF @id IS NULL BEGIN IF EXISTS(SELECT 1 FROM Productos WITH(HOLDLOCK) WHERE cCodigo=@codigo) THROW 50001,'El código del producto ya existe. Corrígelo antes de guardar.',1; INSERT Productos(iCodCategoria,cCodigo,cNombre,cDescripcion,cMarca,cModelo,nPrecioCompra,nPrecioVenta,iStockActual) VALUES(@categoria,@codigo,@nombre,NULLIF(@descripcion,''),NULLIF(@marca,''),NULLIF(@modelo,''),@compra,@venta,0); SET @id=SCOPE_IDENTITY(); END; SELECT @id;",
                            New List(Of SqlParameter) From {New SqlParameter("@nombre", p.nombre.Trim()), New SqlParameter("@categoria", p.categoria), New SqlParameter("@codigo", codigo), New SqlParameter("@descripcion", If(p.descripcion, "").Trim()), New SqlParameter("@marca", If(p.marca, "").Trim()), New SqlParameter("@modelo", If(p.modelo, "").Trim()), New SqlParameter("@compra", precioCompra), New SqlParameter("@venta", p.precioVenta.Value)}))
                        For Each d In detalles.Where(Function(x) x IsNot Nothing AndAlso x.iCodProducto = p.temporal)
                            d.iCodProducto = id
                        Next
                    Next
                    Dim resultado = GuardarCompraInterna(compra, detalles, comprobante, fechaCompra, preciosIncluyenIgv, db)
                    If Not resultado.ok Then Throw New ArgumentException(resultado.mensaje)
                    db.CommitTransaction()
                    Return resultado
                Catch ex As Exception
                    Try
                        db.RollbackTransaction()
                    Catch rollbackEx As InvalidOperationException
                        ' El procedimiento almacenado puede haber revertido ya la transacción completa.
                    End Try
                    Return New ResultadoCompra With {.mensaje = "No se guardaron cambios. " & ex.Message}
                End Try
            End Using
        End Function

        Private Shared Sub GarantizarRucProveedor(db As ConexionBD)
            Dim longitud = Convert.ToInt32(db.ExecuteScalar("SELECT ISNULL(COL_LENGTH('dbo.Proveedores','cRuc'),0)"))
            If longitud > 0 AndAlso longitud < 11 Then
                ' Sólo amplía el campo: no modifica ni elimina proveedores existentes.
                db.ExecuteQuery("ALTER TABLE dbo.Proveedores ALTER COLUMN cRuc VARCHAR(11) NOT NULL")
            End If
            ' Algunas bases antiguas tenían la regla del RUC con diez dígitos.
            ' Se conserva la información previa y se habilitan los RUC actuales de once dígitos.
            Dim regla = Convert.ToString(db.ExecuteScalar("SELECT ISNULL(definition,'') FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Proveedores') AND name='CK_Proveedores_Ruc'"))
            If Not regla.Replace(" ", "").ToUpperInvariant().Contains("LEN(CRUC)IN(10,11)") Then
                db.ExecuteQuery("IF OBJECT_ID('dbo.CK_Proveedores_Ruc','C') IS NOT NULL ALTER TABLE dbo.Proveedores DROP CONSTRAINT CK_Proveedores_Ruc; ALTER TABLE dbo.Proveedores ADD CONSTRAINT CK_Proveedores_Ruc CHECK (LEN(cRuc) IN (10,11) AND cRuc NOT LIKE '%[^0-9]%')")
            End If
        End Sub

        Private Shared Sub GarantizarInfraestructuraCompra(db As ConexionBD)
            SyncLock puertaInfraestructuraCompra
                If infraestructuraCompraLista Then Return
                Dim tipoExiste = Convert.ToInt32(db.ExecuteScalar("SELECT CASE WHEN TYPE_ID('dbo.TipoDetalleCompra') IS NULL THEN 0 ELSE 1 END")) = 1
                Dim comprobantesExiste = Convert.ToInt32(db.ExecuteScalar("SELECT CASE WHEN OBJECT_ID('dbo.Comprobantes_OCR','U') IS NULL THEN 0 ELSE 1 END")) = 1
                Dim definicion = Convert.ToString(db.ExecuteScalar("SELECT ISNULL(OBJECT_DEFINITION(OBJECT_ID('dbo.sp_RegistrarCompra')),'')"))
                If tipoExiste AndAlso comprobantesExiste AndAlso definicion.Contains("'NOTA_VENTA'") Then
                    infraestructuraCompraLista = True
                    Return
                End If
                EjecutarScriptCompra(db, "01_crear_comprobantes_ocr.sql")
                EjecutarScriptCompra(db, "03_registrar_compra_transaccion.sql")
                infraestructuraCompraLista = True
            End SyncLock
        End Sub

        Private Shared Sub EjecutarScriptCompra(db As ConexionBD, nombre As String)
            Dim ruta = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Scripts", nombre)
            If Not File.Exists(ruta) Then Throw New InvalidOperationException("Falta el script " & nombre & ".")
            For Each bloque In Regex.Split(File.ReadAllText(ruta), "(?im)^\s*GO\s*(?:--.*)?$")
                bloque = Regex.Replace(bloque, "(?im)^\s*USE\s+[^;]+;?\s*$", "").Trim()
                If bloque.Length > 0 Then db.ExecuteQuery(bloque)
            Next
        End Sub
    End Class
End Namespace
