Imports System.Data

Namespace Views.Dashboard

    Public Class Dashboard
        Inherits System.Web.UI.Page


        '==========================================================
        ' CARGA DE LA PAGINA
        '==========================================================

        Protected Sub Page_Load(
            ByVal sender As Object,
            ByVal e As EventArgs
        ) Handles Me.Load

            If Not IsPostBack Then

                CargarDashboard()

            End If

        End Sub


        '==========================================================
        ' CARGAR TODO EL DASHBOARD
        '==========================================================

        Private Sub CargarDashboard()

            Using conexion As New WebApplication_Keyove.Data.ConexionBD()


                '==================================================
                ' VENTAS DE HOY
                '==================================================

                Dim consultaVentasHoy As String =
                    "SELECT COUNT(*) " &
                    "FROM dbo.Ventas " &
                    "WHERE dFechaVenta >= CONVERT(date, GETDATE()) " &
                    "AND dFechaVenta < DATEADD(day, 1, CONVERT(date, GETDATE())) " &
                    "AND cEstado <> 'ANULADA'"

                Dim resultadoVentasHoy As Object =
                    conexion.ExecuteScalar(consultaVentasHoy)

                If resultadoVentasHoy Is Nothing OrElse
                   resultadoVentasHoy Is DBNull.Value Then

                    lblVentasHoy.Text = "0"

                Else

                    lblVentasHoy.Text =
                        Convert.ToString(resultadoVentasHoy)

                End If


                '==================================================
                ' PRODUCTOS REGISTRADOS
                '==================================================

                Dim consultaProductos As String =
                    "SELECT COUNT(*) " &
                    "FROM dbo.Productos"

                Dim resultadoProductos As Object =
                    conexion.ExecuteScalar(consultaProductos)

                If resultadoProductos Is Nothing OrElse
                   resultadoProductos Is DBNull.Value Then

                    lblProductosRegistrados.Text = "0"

                Else

                    lblProductosRegistrados.Text =
                        Convert.ToString(resultadoProductos)

                End If


                '==================================================
                ' PRODUCTOS CON STOCK BAJO
                '==================================================

                Dim consultaStockBajo As String =
                    "SELECT COUNT(*) " &
                    "FROM dbo.Productos " &
                    "WHERE iStockActual <= iStockMinimo " &
                    "AND bEstado = 1"

                Dim resultadoStockBajo As Object =
                    conexion.ExecuteScalar(consultaStockBajo)

                If resultadoStockBajo Is Nothing OrElse
                   resultadoStockBajo Is DBNull.Value Then

                    lblStockBajo.Text = "0"

                Else

                    lblStockBajo.Text =
                        Convert.ToString(resultadoStockBajo)

                End If


                '==================================================
                ' COMPRAS REALIZADAS
                '==================================================

                Dim consultaCompras As String =
                    "SELECT COUNT(*) " &
                    "FROM dbo.Compras " &
                    "WHERE cEstado <> 'ANULADA'"

                Dim resultadoCompras As Object =
                    conexion.ExecuteScalar(consultaCompras)

                If resultadoCompras Is Nothing OrElse
                   resultadoCompras Is DBNull.Value Then

                    lblComprasRealizadas.Text = "0"

                Else

                    lblComprasRealizadas.Text =
                        Convert.ToString(resultadoCompras)

                End If


                '==================================================
                ' PRODUCTOS MAS VENDIDOS
                '==================================================

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
                    conexion.ExecuteDataTable(
                        consultaMasVendidos
                    )

                gvMasVendidos.DataBind()


                '==================================================
                ' GRAFICO DE VENTAS
                '==================================================

                CargarVentasMensuales(conexion)

            End Using

        End Sub


        '==========================================================
        ' CARGAR VENTAS DE LOS ULTIMOS 12 MESES
        '==========================================================

        Private Sub CargarVentasMensuales(
            ByVal conexion As WebApplication_Keyove.Data.ConexionBD
        )

            Dim consulta As String =
                "WITH Meses AS (" &
                "    SELECT DATEFROMPARTS(" &
                "        YEAR(DATEADD(month, -11, GETDATE()))," &
                "        MONTH(DATEADD(month, -11, GETDATE()))," &
                "        1" &
                "    ) AS FechaMes " &
                "    UNION ALL " &
                "    SELECT DATEADD(month, 1, FechaMes) " &
                "    FROM Meses " &
                "    WHERE FechaMes < DATEFROMPARTS(" &
                "        YEAR(GETDATE())," &
                "        MONTH(GETDATE())," &
                "        1" &
                "    )" &
                "), VentasMensuales AS (" &
                "    SELECT " &
                "        DATEFROMPARTS(" &
                "            YEAR(v.dFechaVenta)," &
                "            MONTH(v.dFechaVenta)," &
                "            1" &
                "        ) AS FechaMes, " &
                "        SUM(d.nSubTotal) AS TotalVentas " &
                "    FROM dbo.Ventas AS v " &
                "    INNER JOIN dbo.Detalle_Venta AS d " &
                "        ON d.iCodVenta = v.iCodVenta " &
                "    WHERE v.cEstado <> 'ANULADA' " &
                "    AND v.dFechaVenta >= DATEADD(" &
                "        month, -11, " &
                "        DATEFROMPARTS(" &
                "            YEAR(GETDATE())," &
                "            MONTH(GETDATE())," &
                "            1" &
                "        )" &
                "    ) " &
                "    GROUP BY " &
                "        YEAR(v.dFechaVenta), " &
                "        MONTH(v.dFechaVenta) " &
                ") " &
                "SELECT " &
                "    m.FechaMes, " &
                "    ISNULL(v.TotalVentas, 0) AS TotalVentas " &
                "FROM Meses AS m " &
                "LEFT JOIN VentasMensuales AS v " &
                "    ON v.FechaMes = m.FechaMes " &
                "ORDER BY m.FechaMes " &
                "OPTION (MAXRECURSION 12)"


            Dim tabla As DataTable =
                conexion.ExecuteDataTable(consulta)


            '======================================================
            ' AGREGAR COLUMNAS PARA EL GRAFICO
            '======================================================

            tabla.Columns.Add(
                "MesNombre",
                GetType(String)
            )

            tabla.Columns.Add(
                "AlturaBarra",
                GetType(Integer)
            )


            '======================================================
            ' BUSCAR EL VALOR MAXIMO
            '======================================================

            Dim maximo As Decimal = 0D


            For Each fila As DataRow In tabla.Rows

                Dim total As Decimal =
                    Convert.ToDecimal(
                        fila("TotalVentas")
                    )

                If total > maximo Then

                    maximo = total

                End If

            Next


            '======================================================
            ' PREPARAR DATOS DEL GRAFICO
            '======================================================

            For Each fila As DataRow In tabla.Rows

                Dim fecha As DateTime =
                    Convert.ToDateTime(
                        fila("FechaMes")
                    )


                Dim total As Decimal =
                    Convert.ToDecimal(
                        fila("TotalVentas")
                    )


                fila("MesNombre") =
                    ObtenerNombreMes(
                        fecha.Month
                    )


                Dim altura As Integer = 4


                If maximo > 0D AndAlso total > 0D Then

                    altura =
                        CInt(
                            (total / maximo) * 220D
                        )


                    If altura < 4 Then

                        altura = 4

                    End If


                    If altura > 220 Then

                        altura = 220

                    End If

                End If


                fila("AlturaBarra") = altura

            Next


            '======================================================
            ' COMPROBAR SI HAY VENTAS
            '======================================================

            rptGraficoVentas.DataSource = tabla
            rptGraficoVentas.DataBind()

            pnlGraficoVentas.Visible = True
            pnlSinVentas.Visible = False

        End Sub


        '==========================================================
        ' OBTENER NOMBRE DEL MES
        '==========================================================

        Private Function ObtenerNombreMes(
            ByVal numeroMes As Integer
        ) As String

            Select Case numeroMes

                Case 1
                    Return "Ene"

                Case 2
                    Return "Feb"

                Case 3
                    Return "Mar"

                Case 4
                    Return "Abr"

                Case 5
                    Return "May"

                Case 6
                    Return "Jun"

                Case 7
                    Return "Jul"

                Case 8
                    Return "Ago"

                Case 9
                    Return "Sep"

                Case 10
                    Return "Oct"

                Case 11
                    Return "Nov"

                Case 12
                    Return "Dic"

                Case Else
                    Return ""

            End Select

        End Function

    End Class

End Namespace