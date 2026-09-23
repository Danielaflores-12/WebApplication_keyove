Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports WebApplication_keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Views.Operaciones.Ventas
    Public Class Comprobante
        Inherits Global.System.Web.UI.Page

        Protected Cabecera As DataRow
        Protected Detalles As New DataTable()
        Protected Mensaje As String

        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            Response.TrySkipIisCustomErrors = True
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.Cache.SetNoStore()
            Dim idVenta As Integer
            If Not Integer.TryParse(Request.QueryString("id"), idVenta) OrElse idVenta <= 0 Then
                Response.StatusCode = 400
                Mensaje = "Seleccione una venta válida para ver su comprobante."
                Return
            End If

            Try
                Using db As New ConexionBD()
                    Dim parametros As New List(Of SqlParameter) From {
                        New SqlParameter("@Id", SqlDbType.Int) With {.Value = idVenta}
                    }
                    Dim tabla = db.ExecuteDataTable(
                        "SELECT V.*, U.cNombreUsuario FROM Ventas V " &
                        "LEFT JOIN Usuarios U ON V.iCodUsuario = U.iCodUsuario WHERE V.iCodVenta = @Id", parametros)
                    If tabla.Rows.Count = 0 Then
                        Response.StatusCode = 404
                        Mensaje = "No se encontró la venta solicitada."
                        Return
                    End If

                    Detalles = db.ExecuteDataTable(
                        "SELECT P.cCodigo, P.cNombre, D.iCantidad, D.nPrecioVenta, D.nDescuento, D.nSubTotal " &
                        "FROM Detalle_Venta D LEFT JOIN Productos P ON D.iCodProducto = P.iCodProducto " &
                        "WHERE D.iCodVenta = @Id ORDER BY D.iCodDetalleVenta",
                        New List(Of SqlParameter) From {New SqlParameter("@Id", SqlDbType.Int) With {.Value = idVenta}})
                    Cabecera = tabla.Rows(0)
                End Using
            Catch ex As Exception
                Response.StatusCode = 500
                Mensaje = "No se pudo cargar el comprobante. Inténtelo nuevamente."
                Trace.Warn("Comprobante", "Error al consultar la venta", ex)
            End Try
        End Sub

        Protected Function Importe(valor As Object) As String
            Return Convert.ToDecimal(valor).ToString("N2", CultureInfo.GetCultureInfo("es-PE"))
        End Function
    End Class
End Namespace
