Imports System.Data
Imports WebApplication_keyove.Data
Imports WebApplication_keyove.WebApplication_Keyove.Data

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


        Using conexion As New ConexionBD()


            Dim sql As String = "

                SELECT
                    cNombre AS NombreProducto,
                    iStockActual,
                    iStockMinimo,

                    CASE
                        WHEN iStockActual = 0
                            THEN 'AGOTADO'

                        WHEN iStockActual <= iStockMinimo
                            THEN 'STOCK BAJO'

                    END AS EstadoStock

                FROM Productos

                WHERE bEstado = 1
                AND iStockActual <= iStockMinimo

                ORDER BY iStockActual ASC

            "


            Dim dt As DataTable =
                conexion.ExecuteDataTable(sql)


            gvAlertasStock.DataSource = dt

            gvAlertasStock.DataBind()


        End Using


    End Sub


End Class