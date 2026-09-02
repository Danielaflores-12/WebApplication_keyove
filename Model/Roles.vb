Imports System.Data
Imports System.Data.SqlClient

Public Class Roles

    '==================================================
    ' PROPIEDADES
    '==================================================

    Public idRol As Integer
    Public Nombre As String
    Public Descripcion As String
    Public Estado As Boolean
    Public Fecha_Registro As DateTime

    'Consulta personalizada
    Public qSelect As String

    'Conexión
    Public db As New WebApplication_Keyove.Data.ConexionBD


    '==================================================
    ' LISTAR DATOS
    '==================================================

    Public Function ListaDatosTable() As DataTable

        Dim Query As String

        Query = Me.qSelect

        Return db.ExecuteDataTable(Query)

    End Function


    '==================================================
    ' LISTAR TODOS LOS ROLES
    '==================================================

    Public Function ListaDatosShort() As DataTable

        Dim Query As String

        Query =
            "SELECT " &
            "id_Rol, " &
            "Nombre, " &
            "Descripcion, " &
            "Estado, " &
            "Fecha_Registro " &
            "FROM Roles " &
            "ORDER BY id_Rol DESC"

        Return db.ExecuteDataTable(Query)

    End Function


    '==================================================
    ' INSERTAR
    '==================================================

    Public Sub Insertar()

        Dim Query As String

        Query =
            "INSERT INTO Roles " &
            "(Nombre, Descripcion, Estado) " &
            "VALUES " &
            "(@Nombre, @Descripcion, @Estado); " &
            "SELECT CAST(SCOPE_IDENTITY() AS INT);"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@Nombre", SqlDbType.NVarChar, 50) With {
                .Value = Me.Nombre.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Descripcion", SqlDbType.NVarChar, 200) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Descripcion),
                    CType(DBNull.Value, Object),
                    Me.Descripcion.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Estado", SqlDbType.Bit) With {
                .Value = Me.Estado
            }
        )

        Dim resultado As Object =
            db.ExecuteScalar(Query, parametros)

        If resultado IsNot Nothing AndAlso
           Not IsDBNull(resultado) Then

            Me.idRol = Convert.ToInt32(resultado)

        End If

    End Sub


    '==================================================
    ' MODIFICAR
    '==================================================

    Public Sub Modificar()

        Dim Query As String

        Query =
            "UPDATE Roles SET " &
            "Nombre = @Nombre, " &
            "Descripcion = @Descripcion, " &
            "Estado = @Estado " &
            "WHERE id_Rol = @idRol"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@Nombre", SqlDbType.NVarChar, 50) With {
                .Value = Me.Nombre.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Descripcion", SqlDbType.NVarChar, 200) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Descripcion),
                    CType(DBNull.Value, Object),
                    Me.Descripcion.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Estado", SqlDbType.Bit) With {
                .Value = Me.Estado
            }
        )

        parametros.Add(
            New SqlParameter("@idRol", SqlDbType.Int) With {
                .Value = Me.idRol
            }
        )

        db.ExecuteQuery(Query, parametros)

    End Sub


    '==================================================
    ' MODIFICAR EN TRANSACCIÓN
    '==================================================

    Public Sub ModificarTransact()

        Dim Query As String

        Query =
            "UPDATE Roles SET " &
            "Nombre = @Nombre, " &
            "Descripcion = @Descripcion, " &
            "Estado = @Estado " &
            "WHERE id_Rol = @idRol"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@Nombre", SqlDbType.NVarChar, 50) With {
                .Value = Me.Nombre.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Descripcion", SqlDbType.NVarChar, 200) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Descripcion),
                    CType(DBNull.Value, Object),
                    Me.Descripcion.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Estado", SqlDbType.Bit) With {
                .Value = Me.Estado
            }
        )

        parametros.Add(
            New SqlParameter("@idRol", SqlDbType.Int) With {
                .Value = Me.idRol
            }
        )

        db.ExecuteQueryTransact(Query, parametros)

    End Sub


    '==================================================
    ' ELIMINAR
    '==================================================

    Public Sub Eliminar()

        Dim Query As String

        Query =
            "DELETE FROM Roles " &
            "WHERE id_Rol = @idRol"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@idRol", SqlDbType.Int) With {
                .Value = Me.idRol
            }
        )

        db.ExecuteQuery(Query, parametros)

    End Sub


    '==================================================
    ' ELIMINAR EN TRANSACCIÓN
    '==================================================

    Public Sub EliminarTransact()

        Dim Query As String

        Query =
            "DELETE FROM Roles " &
            "WHERE id_Rol = @idRol"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@idRol", SqlDbType.Int) With {
                .Value = Me.idRol
            }
        )

        db.ExecuteQueryTransact(Query, parametros)

    End Sub


    '==================================================
    ' OBTENER UN ROL
    '==================================================

    Public Function getRecord() As Boolean

        Dim Query As String

        Query =
            "SELECT " &
            "id_Rol, " &
            "Nombre, " &
            "Descripcion, " &
            "Estado, " &
            "Fecha_Registro " &
            "FROM Roles " &
            "WHERE id_Rol = @idRol"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@idRol", SqlDbType.Int) With {
                .Value = Me.idRol
            }
        )

        Dim readers As IDataReader =
            db.ExecuteGetRecord(Query, parametros)

        Try

            If Not readers.Read() Then
                Return False
            End If

            Me.idRol =
                Convert.ToInt32(
                    readers("id_Rol")
                )

            Me.Nombre =
                Convert.ToString(
                    readers("Nombre")
                )

            If IsDBNull(readers("Descripcion")) Then
                Me.Descripcion = ""
            Else
                Me.Descripcion =
                    Convert.ToString(
                        readers("Descripcion")
                    )
            End If

            Me.Estado =
                Convert.ToBoolean(
                    readers("Estado")
                )

            Me.Fecha_Registro =
                Convert.ToDateTime(
                    readers("Fecha_Registro")
                )

            Return True

        Finally

            readers.Close()

        End Try

    End Function


    '==================================================
    ' LISTAR PARA COMBOBOX
    '==================================================

    Public Function ListaDatosCombo() As DataTable

        Dim miDataTable As New DataTable()

        miDataTable.Columns.Add("ValueMember")
        miDataTable.Columns.Add("DisplayMember")

        Dim Query As String =
            "SELECT " &
            "id_Rol, " &
            "Nombre " &
            "FROM Roles " &
            "WHERE Estado = 1 " &
            "ORDER BY Nombre"

        Dim readers As IDataReader =
            db.ExecuteReader(Query)

        Try

            While readers.Read()

                miDataTable.Rows.Add(
                    Convert.ToString(
                        readers("id_Rol")
                    ),
                    Convert.ToString(
                        readers("Nombre")
                    )
                )

            End While

        Finally

            readers.Close()

        End Try

        Return miDataTable

    End Function


    '==================================================
    ' BUSCAR POR NOMBRE O DESCRIPCIÓN
    '==================================================

    Public Function Buscar(
        ByVal texto As String
    ) As DataTable

        Dim Query As String =
            "SELECT " &
            "id_Rol, " &
            "Nombre, " &
            "Descripcion, " &
            "Estado, " &
            "Fecha_Registro " &
            "FROM Roles " &
            "WHERE Nombre LIKE @Texto " &
            "OR Descripcion LIKE @Texto " &
            "ORDER BY Nombre"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@Texto", SqlDbType.NVarChar, 200) With {
                .Value = "%" & texto.Trim() & "%"
            }
        )

        Return db.ExecuteDataTable(
            Query,
            parametros
        )

    End Function

End Class