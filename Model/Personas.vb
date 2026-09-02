Imports System.Data
Imports System.Data.SqlClient

Public Class Personas

    '==================================================
    ' PROPIEDADES
    '==================================================

    Public idPersonas As Integer
    Public Nombres As String
    Public Apellidos As String
    Public Genero As String
    Public Correo As String
    Public Telefono As String
    Public Fecha_Nac As Date?
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
    ' LISTAR TODAS LAS PERSONAS
    '==================================================

    Public Function ListaDatosShort() As DataTable

        Dim Query As String

        Query =
            "SELECT " &
            "id_Personas, " &
            "Nombres, " &
            "Apellidos, " &
            "Genero, " &
            "Correo, " &
            "Teléfono, " &
            "Fecha_Nac, " &
            "Fecha_Registro " &
            "FROM Personas " &
            "ORDER BY id_Personas DESC"

        Return db.ExecuteDataTable(Query)

    End Function


    '==================================================
    ' INSERTAR
    '==================================================

    Public Sub Insertar()

        Dim Query As String

        Query =
            "INSERT INTO Personas " &
            "(Nombres, Apellidos, Genero, Correo, Teléfono, Fecha_Nac) " &
            "VALUES " &
            "(@Nombres, @Apellidos, @Genero, @Correo, @Telefono, @Fecha_Nac); " &
            "SELECT CAST(SCOPE_IDENTITY() AS INT);"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@Nombres", SqlDbType.NVarChar, 100) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Nombres),
                    CType(DBNull.Value, Object),
                    Me.Nombres.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Apellidos),
                    CType(DBNull.Value, Object),
                    Me.Apellidos.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Genero", SqlDbType.Char, 1) With {
                .Value = Me.Genero
            }
        )

        parametros.Add(
            New SqlParameter("@Correo", SqlDbType.NVarChar, 100) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Correo),
                    CType(DBNull.Value, Object),
                    Me.Correo.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Telefono),
                    CType(DBNull.Value, Object),
                    Me.Telefono.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Fecha_Nac", SqlDbType.Date) With {
                .Value = If(
                    Me.Fecha_Nac.HasValue,
                    CType(Me.Fecha_Nac.Value, Object),
                    CType(DBNull.Value, Object)
                )
            }
        )

        Dim resultado As Object =
            db.ExecuteScalar(Query, parametros)

        If resultado IsNot Nothing AndAlso
           Not IsDBNull(resultado) Then

            Me.idPersonas = Convert.ToInt32(resultado)

        End If

    End Sub


    '==================================================
    ' MODIFICAR
    '==================================================

    Public Sub Modificar()

        Dim Query As String

        Query =
            "UPDATE Personas SET " &
            "Nombres = @Nombres, " &
            "Apellidos = @Apellidos, " &
            "Genero = @Genero, " &
            "Correo = @Correo, " &
            "Teléfono = @Telefono, " &
            "Fecha_Nac = @Fecha_Nac " &
            "WHERE id_Personas = @idPersonas"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@Nombres", SqlDbType.NVarChar, 100) With {
                .Value = Me.Nombres.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) With {
                .Value = Me.Apellidos.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Genero", SqlDbType.Char, 1) With {
                .Value = Me.Genero
            }
        )

        parametros.Add(
            New SqlParameter("@Correo", SqlDbType.NVarChar, 100) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Correo),
                    CType(DBNull.Value, Object),
                    Me.Correo.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Telefono),
                    CType(DBNull.Value, Object),
                    Me.Telefono.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Fecha_Nac", SqlDbType.Date) With {
                .Value = If(
                    Me.Fecha_Nac.HasValue,
                    CType(Me.Fecha_Nac.Value, Object),
                    CType(DBNull.Value, Object)
                )
            }
        )

        parametros.Add(
            New SqlParameter("@idPersonas", SqlDbType.Int) With {
                .Value = Me.idPersonas
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
            "UPDATE Personas SET " &
            "Nombres = @Nombres, " &
            "Apellidos = @Apellidos, " &
            "Genero = @Genero, " &
            "Correo = @Correo, " &
            "Teléfono = @Telefono, " &
            "Fecha_Nac = @Fecha_Nac " &
            "WHERE id_Personas = @idPersonas"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@Nombres", SqlDbType.NVarChar, 100) With {
                .Value = Me.Nombres.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) With {
                .Value = Me.Apellidos.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Genero", SqlDbType.Char, 1) With {
                .Value = Me.Genero
            }
        )

        parametros.Add(
            New SqlParameter("@Correo", SqlDbType.NVarChar, 100) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Correo),
                    CType(DBNull.Value, Object),
                    Me.Correo.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Telefono),
                    CType(DBNull.Value, Object),
                    Me.Telefono.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Fecha_Nac", SqlDbType.Date) With {
                .Value = If(
                    Me.Fecha_Nac.HasValue,
                    CType(Me.Fecha_Nac.Value, Object),
                    CType(DBNull.Value, Object)
                )
            }
        )

        parametros.Add(
            New SqlParameter("@idPersonas", SqlDbType.Int) With {
                .Value = Me.idPersonas
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
            "DELETE FROM Personas " &
            "WHERE id_Personas = @idPersonas"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@idPersonas", SqlDbType.Int) With {
                .Value = Me.idPersonas
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
            "DELETE FROM Personas " &
            "WHERE id_Personas = @idPersonas"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@idPersonas", SqlDbType.Int) With {
                .Value = Me.idPersonas
            }
        )

        db.ExecuteQueryTransact(Query, parametros)

    End Sub


    '==================================================
    ' OBTENER UNA PERSONA
    '==================================================

    Public Function getRecord() As Boolean

        Dim Query As String

        Query =
            "SELECT " &
            "id_Personas, " &
            "Nombres, " &
            "Apellidos, " &
            "Genero, " &
            "Correo, " &
            "Teléfono, " &
            "Fecha_Nac, " &
            "Fecha_Registro " &
            "FROM Personas " &
            "WHERE id_Personas = @idPersonas"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@idPersonas", SqlDbType.Int) With {
                .Value = Me.idPersonas
            }
        )

        Dim readers As IDataReader =
            db.ExecuteGetRecord(Query, parametros)

        Try

            If Not readers.Read() Then
                Return False
            End If

            Me.idPersonas =
                Convert.ToInt32(readers("id_Personas"))

            Me.Nombres =
                Convert.ToString(readers("Nombres"))

            Me.Apellidos =
                Convert.ToString(readers("Apellidos"))

            Me.Genero =
                Convert.ToString(readers("Genero"))

            If IsDBNull(readers("Correo")) Then
                Me.Correo = ""
            Else
                Me.Correo =
                    Convert.ToString(readers("Correo"))
            End If

            If IsDBNull(readers("Teléfono")) Then
                Me.Telefono = ""
            Else
                Me.Telefono =
                    Convert.ToString(readers("Teléfono"))
            End If

            If IsDBNull(readers("Fecha_Nac")) Then
                Me.Fecha_Nac = Nothing
            Else
                Me.Fecha_Nac =
                    Convert.ToDateTime(readers("Fecha_Nac"))
            End If

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
            "id_Personas, " &
            "Nombres + ' ' + Apellidos AS NombreCompleto " &
            "FROM Personas " &
            "ORDER BY Nombres, Apellidos"

        Dim readers As IDataReader =
            db.ExecuteReader(Query)

        Try

            While readers.Read()

                miDataTable.Rows.Add(
                    Convert.ToString(
                        readers("id_Personas")
                    ),
                    Convert.ToString(
                        readers("NombreCompleto")
                    )
                )

            End While

        Finally

            readers.Close()

        End Try

        Return miDataTable

    End Function


    '==================================================
    ' BUSCAR POR NOMBRE O APELLIDO
    '==================================================

    Public Function Buscar(
        ByVal texto As String
    ) As DataTable

        Dim Query As String =
            "SELECT " &
            "id_Personas, " &
            "Nombres, " &
            "Apellidos, " &
            "Genero, " &
            "Correo, " &
            "Teléfono, " &
            "Fecha_Nac, " &
            "Fecha_Registro " &
            "FROM Personas " &
            "WHERE Nombres LIKE @Texto " &
            "OR Apellidos LIKE @Texto " &
            "ORDER BY Nombres, Apellidos"

        Dim parametros As New List(Of SqlParameter)

        parametros.Add(
            New SqlParameter("@Texto", SqlDbType.NVarChar, 100) With {
                .Value = "%" & texto.Trim() & "%"
            }
        )

        Return db.ExecuteDataTable(
            Query,
            parametros
        )

    End Function


End Class