Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloProducto = WebApplication_keyove.WebApplication_Keyove.Model.Productos
Imports ModeloCategoria = WebApplication_keyove.WebApplication_Keyove.Model.Categorias

Namespace WebApplication_Keyove.Views.Mantenimiento.Productos

    Partial Public Class Producto1
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

        End Sub


        '==================================================
        ' OPCIÓN PARA COMBOBOX
        '==================================================

        Public Class OpcionCombo

            Public v As String
            Public t As String

        End Class


        '==================================================
        ' LISTAR CATEGORÍAS PARA COMBOBOX
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarCategoriasCombo() As List(Of OpcionCombo)

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloCategoria().ListaDatosCombo()

            For Each fila As DataRow In tabla.Rows

                Dim opcion As New OpcionCombo()

                opcion.v =
                    Convert.ToString(fila("ValueMember"))

                opcion.t =
                    Convert.ToString(fila("DisplayMember"))

                lista.Add(opcion)

            Next

            Return lista

        End Function


        '==================================================
        ' LISTAR PRODUCTOS
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarProductos() As List(Of ModeloProducto)

            Dim lista As New List(Of ModeloProducto)()

            Dim tabla As DataTable =
                New ModeloProducto().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloProducto()

                obj.iCodProducto =
                    Convert.ToInt32(fila("iCodProducto"))

                obj.iCodCategoria =
                    Convert.ToInt32(fila("iCodCategoria"))

                obj.cNombreCategoria =
                    Convert.ToString(fila("cNombreCategoria"))

                obj.cCodigo =
                    Convert.ToString(fila("cCodigo"))

                obj.cNombre =
                    Convert.ToString(fila("cNombre"))

                If IsDBNull(fila("cDescripcion")) Then
                    obj.cDescripcion = Nothing
                Else
                    obj.cDescripcion =
                        Convert.ToString(fila("cDescripcion"))
                End If

                If IsDBNull(fila("cMarca")) Then
                    obj.cMarca = Nothing
                Else
                    obj.cMarca =
                        Convert.ToString(fila("cMarca"))
                End If

                If IsDBNull(fila("cModelo")) Then
                    obj.cModelo = Nothing
                Else
                    obj.cModelo =
                        Convert.ToString(fila("cModelo"))
                End If

                obj.nPrecioCompra =
                    Convert.ToDecimal(fila("nPrecioCompra"))

                obj.nPrecioVenta =
                    Convert.ToDecimal(fila("nPrecioVenta"))

                obj.iStockActual =
                    Convert.ToInt32(fila("iStockActual"))

                obj.iStockMinimo =
                    Convert.ToInt32(fila("iStockMinimo"))

                obj.cUnidadMedida =
                    Convert.ToString(fila("cUnidadMedida"))

                obj.bEstado =
                    Convert.ToBoolean(fila("bEstado"))

                obj.dFechaRegistro =
                    Convert.ToDateTime(fila("dFechaRegistro"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR PRODUCTO
        '==================================================

        <WebMethod()>
        Public Shared Function GuardarProducto(producto As ModeloProducto) As String

            Try

                producto.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR PRODUCTO
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarProducto(producto As ModeloProducto) As String

            Try

                producto.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' DESACTIVAR PRODUCTO
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarProducto(idProducto As Integer) As String

            Try

                Dim objProducto As New ModeloProducto()

                objProducto.iCodProducto = idProducto
                objProducto.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER PRODUCTO POR ID
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerProducto(idProducto As Integer) As ModeloProducto

            Dim objProducto As New ModeloProducto()

            objProducto.iCodProducto = idProducto
            objProducto.getRecord()

            Return objProducto

        End Function

    End Class

End Namespace