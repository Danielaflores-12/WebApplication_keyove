Imports System.Data.SqlClient
Imports WebApplication_keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Proveedores

        Public idProveedor As Integer
        Public Ruc As String
        Public Razon_Social As String
        Public Representante As String
        Public Telefono As String
        Public Correo As String
        Public Direccion As String
        Public Estado As Boolean = True
        Public Fecha_Registro As DateTime

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
                New SqlParameter("@Ruc", SqlDbType.VarChar, 11) With {
                    .Value = Me.Ruc
                }
            )

            parametros.Add(
                New SqlParameter("@Razon_Social", SqlDbType.NVarChar, 150) With {
                    .Value = Me.Razon_Social
                }
            )

            parametros.Add(
                New SqlParameter("@Representante", SqlDbType.NVarChar, 100) With {
                    .Value = ValorONull(Me.Representante)
                }
            )

            parametros.Add(
                New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                    .Value = ValorONull(Me.Telefono)
                }
            )

            parametros.Add(
                New SqlParameter("@Correo", SqlDbType.NVarChar, 100) With {
                    .Value = ValorONull(Me.Correo)
                }
            )

            parametros.Add(
                New SqlParameter("@Direccion", SqlDbType.NVarChar, 150) With {
                    .Value = ValorONull(Me.Direccion)
                }
            )

            parametros.Add(
                New SqlParameter("@Estado", SqlDbType.Bit) With {
                    .Value = Me.Estado
                }
            )

            If incluirId Then

                parametros.Add(
                    New SqlParameter("@idProveedor", SqlDbType.Int) With {
                        .Value = Me.idProveedor
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
        "Ruc = @Ruc, " &
        "Razon_Social = @Razon_Social, " &
        "Representante = @Representante, " &
        "Telefono = @Telefono, " &
        "Correo = @Correo, " &
        "Direccion = @Direccion, " &
        "Estado = @Estado " &
        "WHERE id_Proveedor = @idProveedor"

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
                "id_Proveedor, " &
                "Ruc, " &
                "Razon_Social, " &
                "Representante, " &
                "Telefono, " &
                "Correo, " &
                "Direccion, " &
                "Estado, " &
                "Fecha_Registro " &
                "FROM Proveedores " &
                "ORDER BY id_Proveedor DESC"

            Return db.ExecuteDataTable(Query)

        End Function

        '==================================================
        ' INSERTAR PROVEEDOR
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Proveedores " &
                "(Ruc, Razon_Social, Representante, " &
                "Telefono, Correo, Direccion, Estado) " &
                "VALUES " &
                "(@Ruc, @Razon_Social, @Representante, " &
                "@Telefono, @Correo, @Direccion, @Estado); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.idProveedor =
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
                "SET Estado = 0 " &
                "WHERE id_Proveedor = @idProveedor"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@idProveedor", SqlDbType.Int) With {
                    .Value = Me.idProveedor
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
                "SET Estado = 0 " &
                "WHERE id_Proveedor = @idProveedor"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@idProveedor", SqlDbType.Int) With {
                    .Value = Me.idProveedor
                }
            )

            db.ExecuteQueryTransact(Query, parametros)

        End Sub

        '==================================================
        ' OBTENER UN PROVEEDOR POR SU ID
        '==================================================

        Public Sub getRecord()

            Dim Query As String =
                "SELECT " &
                "id_Proveedor, " &
                "Ruc, " &
                "Razon_Social, " &
                "Representante, " &
                "Telefono, " &
                "Correo, " &
                "Direccion, " &
                "Estado, " &
                "Fecha_Registro " &
                "FROM Proveedores " &
                "WHERE id_Proveedor = @idProveedor"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@idProveedor", SqlDbType.Int) With {
                    .Value = Me.idProveedor
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(Query, parametros)

            Try

                If readers.Read() Then

                    Me.idProveedor =
                        Convert.ToInt32(readers("id_Proveedor"))

                    Me.Ruc =
                        Convert.ToString(readers("Ruc"))

                    Me.Razon_Social =
                        Convert.ToString(readers("Razon_Social"))

                    Me.Representante =
                        Convert.ToString(readers("Representante"))

                    Me.Telefono =
                        Convert.ToString(readers("Telefono"))

                    Me.Correo =
                        Convert.ToString(readers("Correo"))

                    Me.Direccion =
                        Convert.ToString(readers("Direccion"))

                    Me.Estado =
                        Convert.ToBoolean(readers("Estado"))

                    Me.Fecha_Registro =
                        Convert.ToDateTime(readers("Fecha_Registro"))

                End If

            Finally

                readers.Close()

            End Try

        End Sub

        '==================================================
        ' LISTAR PROVEEDORES ACTIVOS PARA COMBOBOX
        '==================================================

        Public Function ListaDatosCombo() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Dim Query As String =
                "SELECT " &
                "id_Proveedor, " &
                "Razon_Social " &
                "FROM Proveedores " &
                "WHERE Estado = 1 " &
                "ORDER BY Razon_Social"

            Dim readers As IDataReader =
                db.ExecuteReader(Query)

            Try

                While readers.Read()

                    miDataTable.Rows.Add(
                        Convert.ToString(
                            readers("id_Proveedor")
                        ),
                        Convert.ToString(
                            readers("Razon_Social")
                        )
                    )

                End While

            Finally

                readers.Close()

            End Try

            Return miDataTable

        End Function

    End Class

End Namespace