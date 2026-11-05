
Imports System
Imports System.Data
Imports System.Globalization

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

                ' Consultar una sola vez el algoritmo ROP.
                Dim datosReposicion As DataTable =
                    conexion.ExecuteDataTable(
                        "EXEC dbo.sp_AnalizarReposicionStock"
                    )

                CargarIndicadores(conexion, datosReposicion)
                CargarVentasMensuales(conexion)
                CargarProductosMasVendidos(conexion)
                CargarAlertasStock(datosReposicion)
                CargarMovimientosRecientes(conexion)

            End Using

        End Sub

        ' ==========================================
        ' INDICADORES PRINCIPALES
        ' ==========================================

        Private Sub CargarIndicadores(
            ByVal conexion As WebApplication_Keyove.Data.ConexionBD,
            ByVal datosReposicion As DataTable
        )

            Dim ventasHoy As Decimal = Convert.ToDecimal(
                conexion.ExecuteScalar(
                    "SELECT ISNULL(SUM(nTotal), 0) " &
                    "FROM dbo.Ventas " &
                    "WHERE dFechaVenta >= CONVERT(date, GETDATE()) " &
                    "AND dFechaVenta < DATEADD(day, 1, CONVERT(date, GETDATE())) " &
                    "AND cEstado <> 'ANULADA'"
                )
            )

            lblVentasHoy.Text =
                "S/ " & ventasHoy.ToString("N2")

            lblProductosRegistrados.Text =
                Convert.ToString(
                    conexion.ExecuteScalar(
                        "SELECT COUNT(*) " &
                        "FROM dbo.Productos " &
                        "WHERE bEstado = 1"
                    )
                )

            ' Productos con recomendación de reposición.
            Dim productosReponer As Integer = 0

            For Each fila As DataRow In datosReposicion.Rows

                Dim cantidad As Integer =
                    Convert.ToInt32(
                        fila("CantidadReponer")
                    )

                If cantidad > 0 Then
                    productosReponer += 1
                End If

            Next

            lblStockBajo.Text =
                productosReponer.ToString()

            lblComprasRealizadas.Text =
                Convert.ToString(
                    conexion.ExecuteScalar(
                        "SELECT COUNT(*) " &
                        "FROM dbo.Compras " &
                        "WHERE cEstado <> 'ANULADA'"
                    )
                )

        End Sub

        ' ==========================================
        ' VENTAS DE LOS ÚLTIMOS 12 MESES
        ' ==========================================

        Private Sub CargarVentasMensuales(
            ByVal conexion As WebApplication_Keyove.Data.ConexionBD
        )

            Dim consulta As String =
                "WITH Meses AS (" &
                " SELECT DATEADD(month, -11, " &
                " DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS FechaMes " &
                " UNION ALL " &
                " SELECT DATEADD(month, 1, FechaMes) " &
                " FROM Meses " &
                " WHERE FechaMes < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) " &
                "), Totales AS (" &
                " SELECT DATEFROMPARTS(YEAR(dFechaVenta), MONTH(dFechaVenta), 1) AS FechaMes, " &
                " SUM(nTotal) AS TotalVentas " &
                " FROM dbo.Ventas " &
                " WHERE cEstado <> 'ANULADA' " &
                " AND dFechaVenta >= DATEADD(month, -11, " &
                " DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) " &
                " GROUP BY YEAR(dFechaVenta), MONTH(dFechaVenta) " &
                ") " &
                "SELECT m.FechaMes, " &
                "ISNULL(t.TotalVentas, 0) AS TotalVentas " &
                "FROM Meses m " &
                "LEFT JOIN Totales t " &
                "ON m.FechaMes = t.FechaMes " &
                "ORDER BY m.FechaMes " &
                "OPTION (MAXRECURSION 12)"

            Dim tabla As DataTable =
                conexion.ExecuteDataTable(consulta)

            tabla.Columns.Add(
                "MesNombre",
                GetType(String)
            )

            For Each fila As DataRow In tabla.Rows

                Dim fecha As DateTime =
                    Convert.ToDateTime(
                        fila("FechaMes")
                    )

                fila("MesNombre") =
                    fecha.ToString(
                        "MMM",
                        New CultureInfo("es-PE")
                    )

            Next

            rptGraficoVentas.DataSource = tabla
            rptGraficoVentas.DataBind()

            pnlGraficoVentas.Visible = True
            pnlSinVentas.Visible = False

        End Sub

        ' ==========================================
        ' PRODUCTOS MÁS VENDIDOS
        ' ==========================================

        Private Sub CargarProductosMasVendidos(
            ByVal conexion As WebApplication_Keyove.Data.ConexionBD
        )

            Dim consulta As String =
                "SELECT TOP (5) " &
                "p.cNombre AS Producto, " &
                "SUM(d.iCantidad) AS CantidadVendida, " &
                "SUM(d.nSubTotal) AS TotalVendido " &
                "FROM dbo.Detalle_Venta d " &
                "INNER JOIN dbo.Productos p " &
                "ON p.iCodProducto = d.iCodProducto " &
                "INNER JOIN dbo.Ventas v " &
                "ON v.iCodVenta = d.iCodVenta " &
                "WHERE v.cEstado <> 'ANULADA' " &
                "GROUP BY p.iCodProducto, p.cNombre " &
                "ORDER BY CantidadVendida DESC, TotalVendido DESC"

            gvMasVendidos.DataSource =
                conexion.ExecuteDataTable(consulta)

            gvMasVendidos.DataBind()

        End Sub

        ' ==========================================
        ' ALERTAS CENTRALIZADAS CON ALGORITMO ROP
        ' ==========================================

        Private Sub CargarAlertasStock(
            ByVal datosReposicion As DataTable
        )

            Dim tabla As New DataTable()

            tabla.Columns.Add(
                "Producto",
                GetType(String)
            )

            tabla.Columns.Add(
                "StockActual",
                GetType(Integer)
            )

            tabla.Columns.Add(
                "StockMinimo",
                GetType(Integer)
            )

            tabla.Columns.Add(
                "CantidadReponer",
                GetType(Integer)
            )

            tabla.Columns.Add(
                "EstadoStock",
                GetType(String)
            )

            tabla.Columns.Add(
                "ClaseEstado",
                GetType(String)
            )

            Dim totalAgotados As Integer = 0
            Dim totalCriticos As Integer = 0
            Dim totalReposicion As Integer = 0

            For Each fila As DataRow In datosReposicion.Rows

                Dim estado As String =
                    Convert.ToString(
                        fila("EstadoStock")
                    ).Trim().ToUpperInvariant()

                Dim cantidad As Integer =
                    Convert.ToInt32(
                        fila("CantidadReponer")
                    )

                Select Case estado

                    Case "AGOTADO"
                        totalAgotados += 1

                    Case "CRÍTICO"
                        totalCriticos += 1

                End Select

                If cantidad > 0 Then
                    totalReposicion += 1
                End If

                ' Mostrar solo productos que requieren
                ' reposición, hasta un máximo de ocho.
                If cantidad > 0 AndAlso tabla.Rows.Count < 8 Then

                    Dim clase As String

                    Select Case estado

                        Case "AGOTADO"
                            clase = "kd-badge kd-danger"

                        Case "CRÍTICO"
                            clase = "kd-badge kd-critical"

                        Case Else
                            clase = "kd-badge kd-warning"

                    End Select

                    Dim nombreEstado As String

                    Select Case estado

                        Case "AGOTADO"
                            nombreEstado = "Agotado"

                        Case "CRÍTICO"
                            nombreEstado = "Crítico"

                        Case "REPOSICIÓN"
                            nombreEstado = "Reposición"

                        Case "MÍNIMO"
                            nombreEstado = "Stock mínimo"

                        Case Else
                            nombreEstado = "Normal"

                    End Select

                    tabla.Rows.Add(
                        Convert.ToString(
                            fila("NombreProducto")
                        ),
                        Convert.ToInt32(
                            fila("iStockActual")
                        ),
                        Convert.ToInt32(
                            fila("iStockMinimo")
                        ),
                        cantidad,
                        nombreEstado,
                        clase
                    )

                End If

            Next

            gvAlertasStock.DataSource = tabla
            gvAlertasStock.DataBind()

            lblAgotados.Text =
                totalAgotados.ToString()

            lblCriticos.Text =
                totalCriticos.ToString()

            ' El indicador lblBajos mostrará
            ' los productos que requieren reposición.
            lblBajos.Text =
                totalReposicion.ToString()

        End Sub

        ' ==========================================
        ' MOVIMIENTOS RECIENTES
        ' ==========================================

        Private Sub CargarMovimientosRecientes(
            ByVal conexion As WebApplication_Keyove.Data.ConexionBD
        )

            Dim consulta As String =
                "SELECT TOP (6) " &
                "m.dFechaMovimiento AS Fecha, " &
                "m.cTipoMovimiento AS Tipo, " &
                "p.cNombre AS Producto, " &
                "m.iCantidad AS Cantidad, " &
                "'Usuario ' + CAST(m.iCodUsuario AS varchar(20)) AS Usuario " &
                "FROM dbo.Movimientos_Inventario m " &
                "INNER JOIN dbo.Productos p " &
                "ON p.iCodProducto = m.iCodProducto " &
                "ORDER BY m.dFechaMovimiento DESC, " &
                "m.iCodMovimiento DESC"

            gvMovimientos.DataSource =
                conexion.ExecuteDataTable(consulta)

            gvMovimientos.DataBind()

        End Sub

    End Class

End Namespace
