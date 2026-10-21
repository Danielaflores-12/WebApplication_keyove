Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports WebApplication_keyove.WebApplication_Keyove.Data
Imports ModeloCompra = WebApplication_keyove.WebApplication_Keyove.Model.Compras
Imports ModeloComprobanteOCR = WebApplication_keyove.WebApplication_Keyove.Model.ComprobantesOCR
Imports ModeloDetalleCompra = WebApplication_keyove.WebApplication_Keyove.Model.DetalleCompra
Imports ModeloProveedor = WebApplication_keyove.WebApplication_Keyove.Model.Proveedores
Imports ModeloProducto = WebApplication_keyove.WebApplication_Keyove.Model.Productos
Imports ModeloUsuario = WebApplication_keyove.WebApplication_Keyove.Model.Usuarios

Namespace WebApplication_Keyove.Views.Operaciones.Compras

    Partial Public Class Compra1
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load
            ExigirAdministradora()
            DescargarArchivoCompra()
        End Sub


        '==================================================
        ' OPCIÓN PARA COMBOBOX
        '==================================================

        Public Class OpcionCombo

            Public v As String
            Public t As String
            Public codigo As String
            Public stock As Integer
            Public marca As String
            Public modelo As String
            Public descripcion As String
            Public categoria As Integer

        End Class


        '==================================================
        ' LISTAR PROVEEDORES PARA COMBOBOX
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarProveedoresCombo() As List(Of OpcionCombo)
            ExigirAdministradora()

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ConexionBD().ExecuteDataTable("SELECT iCodProveedor AS ValueMember, cRazonSocial AS DisplayMember, cRuc FROM Proveedores WHERE bEstado = 1")

            For Each fila As DataRow In tabla.Rows

                Dim opcion As New OpcionCombo()

                opcion.v =
                    Convert.ToString(fila("ValueMember"))

                opcion.t =
                    Convert.ToString(fila("DisplayMember"))
                opcion.codigo = Convert.ToString(fila("cRuc"))

                lista.Add(opcion)

            Next

            Return lista

        End Function


        '==================================================
        ' LISTAR USUARIOS PARA COMBOBOX
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarUsuariosCombo() As List(Of OpcionCombo)
            ExigirAdministradora()

            Dim sesion = System.Web.HttpContext.Current.Session
            Return New List(Of OpcionCombo) From {
                New OpcionCombo With {.v = Convert.ToString(sesion("iCodUsuario")), .t = Convert.ToString(sesion("Usuario"))}
            }

        End Function


        '==================================================
        ' LISTAR PRODUCTOS PARA COMBOBOX
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarProductosCombo() As List(Of OpcionCombo)
            ExigirAdministradora()

            Dim lista As New List(Of OpcionCombo)()

            Dim tabla As DataTable =
                New ModeloProducto().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                If Convert.ToBoolean(fila("bEstado")) Then

                    Dim opcion As New OpcionCombo()

                    opcion.v =
                        Convert.ToString(fila("iCodProducto"))

                    opcion.t =
                        Convert.ToString(fila("cNombre"))
                    opcion.codigo = Convert.ToString(fila("cCodigo"))
                    opcion.stock = Convert.ToInt32(fila("iStockActual"))
                    opcion.marca = Convert.ToString(fila("cMarca"))
                    opcion.modelo = Convert.ToString(fila("cModelo"))
                    opcion.descripcion = Convert.ToString(fila("cDescripcion"))
                    opcion.categoria = Convert.ToInt32(fila("iCodCategoria"))

                    lista.Add(opcion)

                End If

            Next

            Return lista

        End Function


        '==================================================
        ' LISTAR COMPRAS
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarCompras() As List(Of ModeloCompra)
            ExigirAdministradora()

            Dim lista As New List(Of ModeloCompra)()

            Dim tabla As DataTable =
                New ModeloCompra().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloCompra()

                obj.iCodCompra =
                    Convert.ToInt32(fila("iCodCompra"))

                obj.iCodProveedor =
                    Convert.ToInt32(fila("iCodProveedor"))

                obj.iCodUsuario =
                    Convert.ToInt32(fila("iCodUsuario"))

                obj.cNombreProveedor =
                    Convert.ToString(fila("cNombreProveedor"))

                obj.cNombreUsuario =
                    Convert.ToString(fila("cNombreUsuario"))

                obj.cTipoComprobante =
                    Convert.ToString(fila("cTipoComprobante"))

                obj.cNumeroComprobante =
                    Convert.ToString(fila("cNumeroComprobante"))

                obj.dFechaCompra =
                    Convert.ToDateTime(fila("dFechaCompra"))

                obj.nSubTotal =
                    Convert.ToDecimal(fila("nSubTotal"))

                obj.nIgv =
                    Convert.ToDecimal(fila("nIgv"))

                obj.nTotal =
                    Convert.ToDecimal(fila("nTotal"))

                If IsDBNull(fila("cObservacion")) Then
                    obj.cObservacion = Nothing
                Else
                    obj.cObservacion =
                        Convert.ToString(fila("cObservacion"))
                End If

                obj.cEstado =
                    Convert.ToString(fila("cEstado"))

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR COMPRA CON SUS DETALLES
        '==================================================

        '==================================================
        ' MODIFICAR COMPRA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function ModificarCompra(compra As ModeloCompra) As String
            ExigirAdministradora()

            Try

                Using db As New ConexionBD()
                    db.ExecuteQuery("UPDATE Compras SET cObservacion=@observacion WHERE iCodCompra=@id AND cEstado='REGISTRADA'", New List(Of System.Data.SqlClient.SqlParameter) From {
                        New System.Data.SqlClient.SqlParameter("@observacion", If(compra.cObservacion, "")),
                        New System.Data.SqlClient.SqlParameter("@id", compra.iCodCompra)
                    })
                End Using

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' ANULAR COMPRA
        '==================================================

        <WebMethod(EnableSession:=True)>
        Public Shared Function EliminarCompra(idCompra As Integer) As String
            ExigirAdministradora()

            Try

                Dim objCompra As New ModeloCompra()

                objCompra.iCodCompra = idCompra
                AnularCompraConInventario(idCompra)

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER COMPRA POR ID
        '==================================================

        <WebMethod(EnableSession:=True)>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ObtenerCompra(idCompra As Integer) As ModeloCompra
            ExigirAdministradora()

            Dim objCompra As New ModeloCompra()

            objCompra.iCodCompra = idCompra
            objCompra.getRecord()

            Return objCompra

        End Function

    End Class

End Namespace
