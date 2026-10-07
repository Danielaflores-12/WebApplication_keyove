Public Class Alertas_Stock
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles Me.Load

        If Not IsPostBack Then
            CargarAlertasStock()
        End If

    End Sub


    Private Sub CargarAlertasStock()

        Using conexion As New WebApplication_Keyove.Data.ConexionBD()

            Dim consulta As String =
                "SELECT " &
                "p.cNombre AS NombreProducto, " &
                "p.iStockActual, " &
                "p.iStockMinimo, " &
                "CASE " &
                "WHEN p.iStockActual = 0 THEN 'AGOTADO' " &
                "WHEN p.iStockActual <= p.iStockMinimo THEN 'STOCK MÍNIMO' " &
                "ELSE 'NORMAL' " &
                "END AS EstadoStock " &
                "FROM dbo.Productos AS p " &
                "WHERE p.bEstado = 1 " &
                "AND p.iStockActual <= p.iStockMinimo " &
                "ORDER BY p.iStockActual ASC, p.cNombre ASC"

            gvAlertasStock.DataSource =
                conexion.ExecuteDataTable(consulta)

            gvAlertasStock.DataBind()

        End Using

    End Sub

End Class