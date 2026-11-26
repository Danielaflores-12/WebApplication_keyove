
Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI

Public Class Alertas_Stock
    Inherits System.Web.UI.Page

    ' ==========================================
    ' CARGAR PÁGINA
    ' ==========================================

    Protected Sub Page_Load(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles Me.Load

        If Session("iCodUsuario") Is Nothing Then

            Response.Redirect(
                ResolveUrl("~/Views/Login.aspx"),
                False
            )

            Context.ApplicationInstance.CompleteRequest()
            Return

        End If

        ' Mostrar acciones solo al Administrador.
        If gvAlertasStock.Columns.Count > 8 Then

            gvAlertasStock.Columns(8).Visible =
                EsAdministrador()

        End If

        If Not IsPostBack Then
            CargarAlertasStock()
        End If

    End Sub

    ' ==========================================
    ' VERIFICAR ADMINISTRADOR
    ' ==========================================

    Private Function EsAdministrador() As Boolean

        Dim rol As String =
            Convert.ToString(Session("Rol")).Trim()

        Return String.Equals(
            rol,
            "Administrador",
            StringComparison.OrdinalIgnoreCase
        )

    End Function

    ' ==========================================
    ' CARGAR ALERTAS INTELIGENTES
    ' ==========================================

    Private Sub CargarAlertasStock()

        Try

            Dim tabla As DataTable = Nothing

            Using conexion As New WebApplication_Keyove.Data.ConexionBD()

                tabla = conexion.ExecuteDataTable(
                    "EXEC dbo.sp_AnalizarReposicionStock"
                )

            End Using

            If tabla Is Nothing Then

                Throw New Exception(
                    "No se recibieron datos del algoritmo."
                )

            End If

            ' ==================================
            ' CONTADORES
            ' ==================================

            Dim totalAgotados As Integer = 0
            Dim totalCriticos As Integer = 0
            Dim totalReponer As Integer = 0
            Dim totalUnidades As Long = 0

            For Each fila As DataRow In tabla.Rows

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

                    totalReponer += 1
                    totalUnidades += cantidad

                End If

            Next

            ' ==================================
            ' MOSTRAR INDICADORES
            ' ==================================

            lblAgotados.Text =
                totalAgotados.ToString()

            lblUrgentes.Text =
                totalCriticos.ToString()

            lblReponer.Text =
                totalReponer.ToString()

            lblUnidades.Text =
                totalUnidades.ToString("N0")

            ' ==================================
            ' MOSTRAR PRODUCTOS
            ' ==================================

            gvAlertasStock.DataSource = tabla
            gvAlertasStock.DataBind()

        Catch ex As Exception

            System.Diagnostics.Trace.TraceError(
                "Error al cargar alertas: " &
                ex.ToString()
            )

            gvAlertasStock.DataSource = Nothing
            gvAlertasStock.DataBind()

            lblAgotados.Text = "-"
            lblUrgentes.Text = "-"
            lblReponer.Text = "-"
            lblUnidades.Text = "-"

            MostrarMensaje(
                "No fue posible cargar las alertas de stock."
            )

        End Try

    End Sub

    ' ==========================================
    ' GUARDAR CONFIGURACIÓN DE REPOSICIÓN
    ' ==========================================

    Protected Sub btnGuardarConfiguracionServidor_Click(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles btnGuardarConfiguracionServidor.Click

        ' Verificar sesión.
        If Session("iCodUsuario") Is Nothing Then

            MostrarMensaje(
                "La sesión ha expirado. Inicia sesión nuevamente."
            )

            Return

        End If

        ' Solo Administrador puede guardar.
        If Not EsAdministrador() Then

            MostrarMensaje(
                "No tienes permiso para modificar la configuración."
            )

            Return

        End If

        ' ==================================
        ' VARIABLES
        ' ==================================

        Dim idProducto As Integer = 0
        Dim diasEntrega As Integer = 0
        Dim diasCobertura As Integer = 0
        Dim stockSeguridad As Integer = 0

        ' ==================================
        ' VALIDAR PRODUCTO
        ' ==================================

        If Not Integer.TryParse(
            hfConfigProductoId.Value,
            idProducto
        ) Then

            MostrarMensaje(
                "El producto seleccionado no es válido."
            )

            Return

        End If

        If idProducto <= 0 Then

            MostrarMensaje(
                "Seleccione un producto válido."
            )

            Return

        End If

        ' ==================================
        ' VALIDAR DÍAS DE ENTREGA
        ' ==================================

        If Not Integer.TryParse(
            hfConfigDiasEntrega.Value,
            diasEntrega
        ) Then

            MostrarMensaje(
                "Ingrese días de entrega válidos."
            )

            Return

        End If

        If diasEntrega < 1 OrElse diasEntrega > 365 Then

            MostrarMensaje(
                "Los días de entrega deben estar entre 1 y 365."
            )

            Return

        End If

        ' ==================================
        ' VALIDAR DÍAS DE COBERTURA
        ' ==================================

        If Not Integer.TryParse(
            hfConfigDiasCobertura.Value,
            diasCobertura
        ) Then

            MostrarMensaje(
                "Ingrese días de cobertura válidos."
            )

            Return

        End If

        If diasCobertura < 0 OrElse diasCobertura > 365 Then

            MostrarMensaje(
                "Los días de cobertura deben estar entre 0 y 365."
            )

            Return

        End If

        ' ==================================
        ' VALIDAR STOCK DE SEGURIDAD
        ' ==================================

        If Not Integer.TryParse(
            hfConfigStockSeguridad.Value,
            stockSeguridad
        ) Then

            MostrarMensaje(
                "Ingrese un stock de seguridad válido."
            )

            Return

        End If

        If stockSeguridad < 0 OrElse
           stockSeguridad > 1000000 Then

            MostrarMensaje(
                "El stock de seguridad debe estar entre 0 y 1000000."
            )

            Return

        End If

        ' ==================================
        ' GUARDAR EN SQL SERVER
        ' ==================================

        Try

            Dim parametros As New List(Of SqlParameter)()

            Dim parametroProducto As New SqlParameter(
                "@iCodProducto",
                SqlDbType.Int
            )

            parametroProducto.Value = idProducto
            parametros.Add(parametroProducto)

            Dim parametroEntrega As New SqlParameter(
                "@iDiasEntrega",
                SqlDbType.Int
            )

            parametroEntrega.Value = diasEntrega
            parametros.Add(parametroEntrega)

            Dim parametroCobertura As New SqlParameter(
                "@iDiasCobertura",
                SqlDbType.Int
            )

            parametroCobertura.Value = diasCobertura
            parametros.Add(parametroCobertura)

            Dim parametroSeguridad As New SqlParameter(
                "@iStockSeguridad",
                SqlDbType.Int
            )

            parametroSeguridad.Value = stockSeguridad
            parametros.Add(parametroSeguridad)

            ' ==================================
            ' EJECUTAR PROCEDIMIENTO
            ' ==================================

            Dim resultado As DataTable = Nothing

            Using conexion As New WebApplication_Keyove.Data.ConexionBD()

                resultado = conexion.ExecuteDataTable(
                    "EXEC dbo.sp_GuardarConfiguracionReposicion " &
                    "@iCodProducto, " &
                    "@iDiasEntrega, " &
                    "@iDiasCobertura, " &
                    "@iStockSeguridad",
                    parametros
                )

            End Using

            ' ==================================
            ' VALIDAR RESULTADO
            ' ==================================

            If resultado Is Nothing Then

                Throw New Exception(
                    "No se recibió respuesta del procedimiento."
                )

            End If

            If resultado.Rows.Count = 0 Then

                Throw New Exception(
                    "El procedimiento no confirmó el guardado."
                )

            End If

            Dim exito As Integer =
                Convert.ToInt32(
                    resultado.Rows(0)("Exito")
                )

            If exito <> 1 Then

                Throw New Exception(
                    "No se pudo guardar la configuración."
                )

            End If

            ' ==================================
            ' ACTUALIZAR ALERTAS
            ' ==================================

            CargarAlertasStock()

            MostrarMensaje(
                "Configuración guardada correctamente."
            )

        Catch ex As Exception

            System.Diagnostics.Trace.TraceError(
                "Error al guardar configuración: " &
                ex.ToString()
            )

            MostrarMensaje(
                "No fue posible guardar la configuración."
            )

        End Try

    End Sub

    ' ==========================================
    ' MOSTRAR MENSAJES
    ' ==========================================

    Private Sub MostrarMensaje(
        ByVal mensaje As String
    )

        Dim textoSeguro As String =
            HttpUtility.JavaScriptStringEncode(
                mensaje
            )

        Dim script As String =
            "alert('" & textoSeguro & "');"

        ScriptManager.RegisterStartupScript(
            Me,
            Me.GetType(),
            "mensajeConfiguracionStock",
            script,
            True
        )

    End Sub

End Class
