Namespace Views.Dashboard

    Public Class Dashboard
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(
            ByVal sender As Object,
            ByVal e As EventArgs
        ) Handles Me.Load

            If Not IsPostBack Then
                CargarDashboard()
            End If

        End Sub

        Private Sub CargarDashboard()

            Using conexion As New WebApplication_Keyove.Data.ConexionBD()

                Dim consultaVentasHoy As String =
                    "SELECT COUNT(*) FROM dbo.Ventas " &
                    "WHERE dFechaVenta >= CONVERT(date, GETDATE()) " &
                    "AND dFechaVenta < DATEADD(day, 1, CONVERT(date, GETDATE())) " &
                    "AND cEstado <> 'ANULADA'"

                lblVentasHoy.Text =
                    Convert.ToString(conexion.ExecuteScalar(consultaVentasHoy))

                Dim consultaProductos As String =
                    "SELECT COUNT(*) FROM dbo.Productos"

                lblProductosRegistrados.Text =
                    Convert.ToString(conexion.ExecuteScalar(consultaProductos))

                Dim consultaStockBajo As String =
                    "SELECT COUNT(*) FROM dbo.Productos " &
                    "WHERE iStockActual <= iStockMinimo AND bEstado = 1"

                lblStockBajo.Text =
                    Convert.ToString(conexion.ExecuteScalar(consultaStockBajo))

                Dim consultaCompras As String =
                    "SELECT COUNT(*) FROM dbo.Compras " &
                    "WHERE cEstado <> 'ANULADA'"

                lblComprasRealizadas.Text =
                    Convert.ToString(conexion.ExecuteScalar(consultaCompras))

                Dim consultaMasVendidos As String =
                    "SELECT TOP (5) " &
                    "p.cNombre AS Producto, " &
                    "SUM(d.iCantidad) AS CantidadVendida, " &
                    "SUM(d.nSubTotal) AS TotalVendido " &
                    "FROM dbo.Detalle_Venta AS d " &
                    "INNER JOIN dbo.Productos AS p " &
                    "ON p.iCodProducto = d.iCodProducto " &
                    "INNER JOIN dbo.Ventas AS v " &
                    "ON v.iCodVenta = d.iCodVenta " &
                    "WHERE v.cEstado <> 'ANULADA' " &
                    "GROUP BY p.iCodProducto, p.cNombre " &
                    "ORDER BY CantidadVendida DESC, TotalVendido DESC"

                gvMasVendidos.DataSource =
                    conexion.ExecuteDataTable(consultaMasVendidos)

                gvMasVendidos.DataBind()

            End Using

        End Sub

    End Class

End Namespace