Imports System.Data.SqlClient
Imports WebApplication_keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Proveedores

        Public iCodProveedor As Integer
        Public cRuc As String
        Public cRazon_Social As String
        Public cRepresentante As String
        Public cTelefono As String
        Public cCorreo As String
        Public cDireccion As String
        Public bEstado As Boolean = True
        Public dFecha_Registro As DateTime

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
                    .Value = Me.cRuc
                }
            )

            parametros.Add(
                New SqlParameter("@Razon_Social", SqlDbType.NVarChar, 150) With {
                    .Value = Me.cRazon_Social
                }
            )

            parametros.Add(
                New SqlParameter("@Representante", SqlDbType.NVarChar, 100) With {
                    .Value = ValorONull(Me.cRepresentante)
                }
            )

            parametros.Add(
                New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                    .Value = ValorONull(Me.cTelefono)
                }
            )

            parametros.Add(
                New SqlParameter("@Correo", SqlDbType.NVarChar, 100) With {
                    .Value = ValorONull(Me.cCorreo)
                }
            )

            parametros.Add(
                New SqlParameter("@Direccion", SqlDbType.NVarChar, 150) With {
                    .Value = ValorONull(Me.cDireccion)
                }
            )

            parametros.Add(
                New SqlParameter("@Estado", SqlDbType.Bit) With {
                    .Value = Me.bEstado
                }
            )

            If incluirId Then

                parametros.Add(
                    New SqlParameter("@idProveedor", SqlDbType.Int) With {
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
                "SET Estado = 0 " &
                "WHERE id_Proveedor = @idProveedor"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@idProveedor", SqlDbType.Int) With {
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
                "SET Estado = 0 " &
                "WHERE id_Proveedor = @idProveedor"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@idProveedor", SqlDbType.Int) With {
                    .Value = Me.iCodProveedor
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
                    .Value = Me.iCodProveedor
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(Query, parametros)

            Try

                If readers.Read() Then

                    Me.iCodProveedor =
                        Convert.ToInt32(readers("id_Proveedor"))

                    Me.cRuc =
                        Convert.ToString(readers("Ruc"))

                    Me.cRazon_Social =
                        Convert.ToString(readers("Razon_Social"))

                    Me.cRepresentante =
                        Convert.ToString(readers("Representante"))

                    Me.cTelefono =
                        Convert.ToString(readers("Telefono"))

                    Me.cCorreo =
                        Convert.ToString(readers("Correo"))

                    Me.cDireccion =
                        Convert.ToString(readers("Direccion"))

                    Me.bEstado =
                        Convert.ToBoolean(readers("Estado"))

                    Me.dFecha_Registro =
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