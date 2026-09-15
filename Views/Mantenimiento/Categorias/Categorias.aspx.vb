Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloCategoria = WebApplication_keyove.WebApplication_Keyove.Model.Categorias

Namespace WebApplication_Keyove.Views.Mantenimiento.Categorias

    Partial Public Class Categoria1
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

        End Sub


        '==================================================
        ' LISTAR CATEGORÍAS
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarCategorias() As List(Of ModeloCategoria)

            Dim lista As New List(Of ModeloCategoria)()

            Dim tabla As DataTable =
                New ModeloCategoria().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloCategoria()

                obj.iCodCategoria =
                    Convert.ToInt32(fila("iCodCategoria"))

                obj.cNombre =
                    Convert.ToString(fila("cNombre"))

                If IsDBNull(fila("cDescripcion")) Then
                    obj.cDescripcion = Nothing
                Else
                    obj.cDescripcion =
                        Convert.ToString(fila("cDescripcion"))
                End If

                obj.bEstado =
                    Convert.ToBoolean(fila("bEstado"))

                obj.dFechaRegistro =
                    Convert.ToDateTime(fila("dFechaRegistro"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR CATEGORÍA
        '==================================================

        <WebMethod()>
        Public Shared Function GuardarCategoria(categoria As ModeloCategoria) As String

            Try

                categoria.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR CATEGORÍA
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarCategoria(categoria As ModeloCategoria) As String

            Try

                categoria.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' DESACTIVAR CATEGORÍA
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarCategoria(idCategoria As Integer) As String

            Try

                Dim objCategoria As New ModeloCategoria()

                objCategoria.iCodCategoria = idCategoria
                objCategoria.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER CATEGORÍA POR ID
        '==================================================

        <WebMethod()>
        Public Shared Function ObtenerCategoria(idCategoria As Integer) As ModeloCategoria

            Dim objCategoria As New ModeloCategoria()

            objCategoria.iCodCategoria = idCategoria
            objCategoria.getRecord()

            Return objCategoria

        End Function

    End Class

End Namespace