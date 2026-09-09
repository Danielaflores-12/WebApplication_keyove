Imports System.Data
Imports System.Data.SqlClient

Public Class Personas

    '==================================================
    ' PROPIEDADES
    '==================================================

    Public iCodPersona As Integer
    Public cNombres As String
    Public cApellidos As String
    Public cGenero As String
    Public cCorreo As String
    Public cTelefono As String
    Public dFecha_Nac As Date?
    Public dFecha_Registro As DateTime

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
                    String.IsNullOrWhiteSpace(Me.cNombres),
                    CType(DBNull.Value, Object),
                    Me.cNombres.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.cApellidos),
                    CType(DBNull.Value, Object),
                    Me.cApellidos.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Genero", SqlDbType.Char, 1) With {
                .Value = Me.cGenero
            }
        )

        parametros.Add(
            New SqlParameter("@Correo", SqlDbType.NVarChar, 100) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.cCorreo),
                    CType(DBNull.Value, Object),
                    Me.cCorreo.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.cTelefono),
                    CType(DBNull.Value, Object),
                    Me.cTelefono.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Fecha_Nac", SqlDbType.Date) With {
                .Value = If(
                    Me.dFecha_Nac.HasValue,
                    CType(Me.dFecha_Nac.Value, Object),
                    CType(DBNull.Value, Object)
                )
            }
        )

        Dim resultado As Object =
            db.ExecuteScalar(Query, parametros)

        If resultado IsNot Nothing AndAlso
           Not IsDBNull(resultado) Then

            Me.iCodPersona = Convert.ToInt32(resultado)

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
                .Value = Me.cNombres.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) With {
                .Value = Me.cApellidos.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Genero", SqlDbType.Char, 1) With {
                .Value = Me.cGenero
            }
        )

        parametros.Add(
            New SqlParameter("@Correo", SqlDbType.NVarChar, 100) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.cCorreo),
                    CType(DBNull.Value, Object),
                    Me.cCorreo.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.cTelefono),
                    CType(DBNull.Value, Object),
                    Me.cTelefono.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Fecha_Nac", SqlDbType.Date) With {
                .Value = If(
                    Me.dFecha_Nac.HasValue,
                    CType(Me.dFecha_Nac.Value, Object),
                    CType(DBNull.Value, Object)
                )
            }
        )

        parametros.Add(
            New SqlParameter("@idPersonas", SqlDbType.Int) With {
                .Value = Me.iCodPersona
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
                .Value = Me.cNombres.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) With {
                .Value = Me.cApellidos.Trim()
            }
        )

        parametros.Add(
            New SqlParameter("@Genero", SqlDbType.Char, 1) With {
                .Value = Me.cGenero
            }
        )

        parametros.Add(
            New SqlParameter("@Correo", SqlDbType.NVarChar, 100) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.cCorreo),
                    CType(DBNull.Value, Object),
                    Me.cCorreo.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.cTelefono),
                    CType(DBNull.Value, Object),
                    Me.cTelefono.Trim()
                )
            }
        )

        parametros.Add(
            New SqlParameter("@Fecha_Nac", SqlDbType.Date) With {
                .Value = If(
                    Me.dFecha_Nac.HasValue,
                    CType(Me.dFecha_Nac.Value, Object),
                    CType(DBNull.Value, Object)
                )
            }
        )

        parametros.Add(
            New SqlParameter("@idPersonas", SqlDbType.Int) With {
                .Value = Me.iCodPersona
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
                .Value = Me.iCodPersona
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
                .Value = Me.iCodPersona
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
                .Value = Me.iCodPersona
            }
        )

        Dim readers As IDataReader =
            db.ExecuteGetRecord(Query, parametros)

        Try

            If Not readers.Read() Then
                Return False
            End If

            Me.iCodPersona =
                Convert.ToInt32(readers("id_Personas"))

            Me.cNombres =
                Convert.ToString(readers("Nombres"))

            Me.cApellidos =
                Convert.ToString(readers("Apellidos"))

            Me.cGenero =
                Convert.ToString(readers("Genero"))

            If IsDBNull(readers("Correo")) Then
                Me.cCorreo = ""
            Else
                Me.cCorreo =
                    Convert.ToString(readers("Correo"))
            End If

            If IsDBNull(readers("Teléfono")) Then
                Me.cTelefono = ""
            Else
                Me.cTelefono =
                    Convert.ToString(readers("Teléfono"))
            End If

            If IsDBNull(readers("Fecha_Nac")) Then
                Me.dFecha_Nac = Nothing
            Else
                Me.dFecha_Nac =
                    Convert.ToDateTime(readers("Fecha_Nac"))
            End If

            Me.dFecha_Registro =
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