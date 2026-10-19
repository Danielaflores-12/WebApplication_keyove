Imports System.Data.SqlClient
Imports System.Web.Services
Imports WebApplication_keyove.WebApplication_Keyove.Data
Imports CompraModelo = WebApplication_keyove.WebApplication_Keyove.Model.Compras
Imports DetalleModelo = WebApplication_keyove.WebApplication_Keyove.Model.DetalleCompra

Namespace WebApplication_Keyove.Views.Operaciones.Compras
    Partial Public Class Compra1
        Public Class ProductoNuevoCompra
            Public temporal As Integer
            Public nombre As String
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
                    If compra Is Nothing OrElse compra.iCodCompra <> 0 OrElse detalles Is Nothing OrElse detalles.Count = 0 Then Throw New ArgumentException("Revisa la compra.")
                    If productosNuevos Is Nothing Then productosNuevos = New List(Of ProductoNuevoCompra)
                    Dim temporales As New HashSet(Of Integer)
                    For Each p In productosNuevos
                        If p Is Nothing OrElse p.temporal >= 0 OrElse Not temporales.Add(p.temporal) OrElse String.IsNullOrWhiteSpace(p.nombre) OrElse p.nombre.Trim().Length > 100 OrElse p.categoria <= 0 OrElse Not p.precioVenta.HasValue OrElse p.precioVenta < 0 OrElse Decimal.Round(p.precioVenta.Value, 2) <> p.precioVenta.Value Then Throw New ArgumentException("Completa nombre, categoría y precio de venta de cada producto nuevo.")
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
                        Dim id = Convert.ToInt32(db.ExecuteScalarTransact(
                            "IF NOT EXISTS(SELECT 1 FROM Categorias WITH(HOLDLOCK) WHERE iCodCategoria=@categoria AND bEstado=1) THROW 50001,'La categoría no está activa.',1; " &
                            "DECLARE @id int; IF (SELECT COUNT(*) FROM Productos WITH(UPDLOCK,HOLDLOCK) WHERE UPPER(LTRIM(RTRIM(cNombre)))=UPPER(@nombre))>1 THROW 50001,'Hay varios productos con ese nombre. Selecciona uno del inventario.',1; " &
                            "SELECT @id=iCodProducto FROM Productos WITH(UPDLOCK,HOLDLOCK) WHERE UPPER(LTRIM(RTRIM(cNombre)))=UPPER(@nombre); " &
                            "IF @id IS NOT NULL AND NOT EXISTS(SELECT 1 FROM Productos WHERE iCodProducto=@id AND bEstado=1) THROW 50001,'El producto está inactivo.',1; " &
                            "IF @id IS NULL BEGIN INSERT Productos(iCodCategoria,cCodigo,cNombre,nPrecioVenta,iStockActual) VALUES(@categoria,@codigo,@nombre,@venta,0); SET @id=SCOPE_IDENTITY(); END; SELECT @id;",
                            New List(Of SqlParameter) From {New SqlParameter("@nombre", p.nombre.Trim()), New SqlParameter("@categoria", p.categoria), New SqlParameter("@codigo", "OCR-" & Guid.NewGuid().ToString("N")), New SqlParameter("@venta", p.precioVenta.Value)}))
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
    End Class
End Namespace
