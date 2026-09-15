Imports System.Data.SqlClient
Imports WebApplication_Keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Proveedores

        '==================================================
        ' VARIABLES
        '==================================================

        Public iCodProveedor As Integer
        Public cRuc As String
        Public cRazonSocial As String
        Public cRepresentante As String
        Public cTelefono As String
        Public cCorreo As String
        Public cDireccion As String
        Public bEstado As Boolean = True
        Public dFechaRegistro As DateTime

        Public qSelect As String
        Public db As New ConexionBD()


        '==================================================
        ' CONVERTIR CAMPOS VACÍOS EN NULL PARA SQL SERVER
        '==================================================

        Private Function ValorONull(valor As String) As Object

            If String.IsNullOrWhiteSpace(valor) Then
                Return DBNull.Value
            End If

            Return valor.Trim()

        End Function


        '==================================================
        ' CREAR PARÁMETROS DEL PROVEEDOR
        '==================================================

        Private Function CrearParametros(
            incluirId As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@cRuc", SqlDbType.VarChar, 11) With {
                    .Value = Me.cRuc
                }
            )

            parametros.Add(
                New SqlParameter("@cRazonSocial", SqlDbType.NVarChar, 150) With {
                    .Value = Me.cRazonSocial.Trim()
                }
            )

            parametros.Add(
                New SqlParameter("@cRepresentante", SqlDbType.NVarChar, 100) With {
                    .Value = ValorONull(Me.cRepresentante)
                }
            )

            parametros.Add(
                New SqlParameter("@cTelefono", SqlDbType.VarChar, 9) With {
                    .Value = ValorONull(Me.cTelefono)
                }
            )

            parametros.Add(
                New SqlParameter("@cCorreo", SqlDbType.NVarChar, 100) With {
                    .Value = ValorONull(Me.cCorreo)
                }
            )

            parametros.Add(
                New SqlParameter("@cDireccion", SqlDbType.NVarChar, 150) With {
                    .Value = ValorONull(Me.cDireccion)
                }
            )

            parametros.Add(
                New SqlParameter("@bEstado", SqlDbType.Bit) With {
                    .Value = Me.bEstado
                }
            )

            If incluirId Then

                parametros.Add(
                    New SqlParameter("@iCodProveedor", SqlDbType.Int) With {
                        .Value = Me.iCodProveedor
                    }
                )

            End If

            Return parametros

        End Function


        '==================================================
        ' CONSULTA PARA MODIFICAR
        '==================================================

        Private Function ConsultaModificar() As String

            Dim Query As String =
                "UPDATE Proveedores SET " &
                "cRuc = @cRuc, " &
                "cRazonSocial = @cRazonSocial, " &
                "cRepresentante = @cRepresentante, " &
                "cTelefono = @cTelefono, " &
                "cCorreo = @cCorreo, " &
                "cDireccion = @cDireccion, " &
                "bEstado = @bEstado " &
                "WHERE iCodProveedor = @iCodProveedor"

            Return Query

        End Function


        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function


        '==================================================
        ' LISTAR TODOS LOS PROVEEDORES
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "iCodProveedor, " &
                "cRuc, " &
                "cRazonSocial, " &
                "cRepresentante, " &
                "cTelefono, " &
                "cCorreo, " &
                "cDireccion, " &
                "bEstado, " &
                "dFechaRegistro " &
                "FROM Proveedores " &
                "ORDER BY iCodProveedor DESC"

            Return db.ExecuteDataTable(Query)

        End Function


        '==================================================
        ' INSERTAR PROVEEDOR
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Proveedores " &
                "(cRuc, cRazonSocial, cRepresentante, " &
                "cTelefono, cCorreo, cDireccion, bEstado) " &
                "VALUES " &
                "(@cRuc, @cRazonSocial, @cRepresentante, " &
                "@cTelefono, @cCorreo, @cDireccion, @bEstado); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodProveedor =
                Convert.ToInt32(
                    db.ExecuteScalar(Query, parametros)
                )

        End Sub


        '==================================================
        ' MODIFICAR PROVEEDOR
        '==================================================

        Public Sub Modificar()

            Dim Query As String = ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' MODIFICAR PROVEEDOR EN TRANSACCIÓN
        '==================================================

        Public Sub ModificarTransact()

            Dim Query As String = ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' DESACTIVAR PROVEEDOR
        '==================================================

        Public Sub Eliminar()

            Dim Query As String =
                "UPDATE Proveedores " &
                "SET bEstado = 0 " &
                "WHERE iCodProveedor = @iCodProveedor"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodProveedor", SqlDbType.Int) With {
                    .Value = Me.iCodProveedor
                }
            )

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' DESACTIVAR PROVEEDOR EN TRANSACCIÓN
        '==================================================

        Public Sub EliminarTransact()

            Dim Query As String =
                "UPDATE Proveedores " &
                "SET bEstado = 0 " &
                "WHERE iCodProveedor = @iCodProveedor"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodProveedor", SqlDbType.Int) With {
                    .Value = Me.iCodProveedor
                }
            )

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' ACTIVAR PROVEEDOR
        '==================================================

        Public Sub Activar()

            Dim Query As String =
                "UPDATE Proveedores " &
                "SET bEstado = 1 " &
                "WHERE iCodProveedor = @iCodProveedor"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodProveedor", SqlDbType.Int) With {
                    .Value = Me.iCodProveedor
                }
            )

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' OBTENER UN PROVEEDOR POR SU ID
        '==================================================

        Public Function getRecord() As Boolean

            Dim Query As String =
                "SELECT " &
                "iCodProveedor, " &
                "cRuc, " &
                "cRazonSocial, " &
                "cRepresentante, " &
                "cTelefono, " &
                "cCorreo, " &
                "cDireccion, " &
                "bEstado, " &
                "dFechaRegistro " &
                "FROM Proveedores " &
                "WHERE iCodProveedor = @iCodProveedor"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodProveedor", SqlDbType.Int) With {
                    .Value = Me.iCodProveedor
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(Query, parametros)

            Try

                If Not readers.Read() Then
                    Return False
                End If

                Me.iCodProveedor =
                    Convert.ToInt32(readers("iCodProveedor"))

                Me.cRuc =
                    Convert.ToString(readers("cRuc"))

                Me.cRazonSocial =
                    Convert.ToString(readers("cRazonSocial"))

                Me.cRepresentante =
                    Convert.ToString(readers("cRepresentante"))

                Me.cTelefono =
                    Convert.ToString(readers("cTelefono"))

                Me.cCorreo =
                    Convert.ToString(readers("cCorreo"))

                Me.cDireccion =
                    Convert.ToString(readers("cDireccion"))

                Me.bEstado =
                    Convert.ToBoolean(readers("bEstado"))

                Me.dFechaRegistro =
                    Convert.ToDateTime(readers("dFechaRegistro"))

                Return True

            Finally

                readers.Close()

            End Try

        End Function


        '==================================================
        ' LISTAR PROVEEDORES ACTIVOS PARA COMBOBOX
        '==================================================

        Public Function ListaDatosCombo() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Dim Query As String =
                "SELECT " &
                "iCodProveedor, " &
                "cRazonSocial " &
                "FROM Proveedores " &
                "WHERE bEstado = 1 " &
                "ORDER BY cRazonSocial"

            Dim readers As IDataReader =
                db.ExecuteReader(Query)

            Try

                While readers.Read()

                    miDataTable.Rows.Add(
                        Convert.ToString(
                            readers("iCodProveedor")
                        ),
                        Convert.ToString(
                            readers("cRazonSocial")
                        )
                    )

                End While

            Finally

                readers.Close()

            End Try

            Return miDataTable

        End Function


        '==================================================
        ' BUSCAR POR RUC O RAZÓN SOCIAL
        '==================================================

        Public Function Buscar(
            ByVal texto As String
        ) As DataTable

            Dim Query As String =
                "SELECT " &
                "iCodProveedor, " &
                "cRuc, " &
                "cRazonSocial, " &
                "cRepresentante, " &
                "cTelefono, " &
                "cCorreo, " &
                "cDireccion, " &
                "bEstado, " &
                "dFechaRegistro " &
                "FROM Proveedores " &
                "WHERE cRuc LIKE @Texto " &
                "OR cRazonSocial LIKE @Texto " &
                "ORDER BY cRazonSocial"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@Texto", SqlDbType.NVarChar, 150) With {
                    .Value = "%" & texto.Trim() & "%"
                }
            )

            Return db.ExecuteDataTable(
                Query,
                parametros
            )

        End Function

    End Class

End Namespace