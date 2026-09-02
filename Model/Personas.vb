Imports System.Data.SqlClient
Imports WebApplication_keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model
    Public Class Personas
        Public idPersonas As Integer
        Public Nombres As String
        Public Apellidos As String
        Public Genero As String
        Public Correo As String
        Public Telefono As String
        Public Fecha_Nac As Date?
        Public Fecha_Registro As DateTime

        Public qSelect As String

        Public db As New ConexionBD()


        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Dim Query As String
            Dim readers As DataTable

            Query = Me.qSelect

            readers = db.ExecuteDataTable(Query)

            Return readers

        End Function


        '==================================================
        ' LISTAR TODAS LAS PERSONAS
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String
            Dim readers As DataTable

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

            readers = db.ExecuteDataTable(Query)

            Return readers

        End Function


        '==================================================
        ' INSERTAR PERSONA
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
                .Value = Me.Nombres
            }
        )

            parametros.Add(
            New SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) With {
                .Value = Me.Apellidos
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
                    Me.Correo
                )
            }
        )

            parametros.Add(
            New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Telefono),
                    CType(DBNull.Value, Object),
                    Me.Telefono
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

            Me.idPersonas = Convert.ToInt32(
            db.ExecuteScalar(Query, parametros)
        )

        End Sub


        '==================================================
        ' MODIFICAR PERSONA
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
                .Value = Me.Nombres
            }
        )

            parametros.Add(
            New SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) With {
                .Value = Me.Apellidos
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
                    Me.Correo
                )
            }
        )

            parametros.Add(
            New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Telefono),
                    CType(DBNull.Value, Object),
                    Me.Telefono
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
                .Value = Me.Nombres
            }
        )

            parametros.Add(
            New SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) With {
                .Value = Me.Apellidos
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
                    Me.Correo
                )
            }
        )

            parametros.Add(
            New SqlParameter("@Telefono", SqlDbType.VarChar, 9) With {
                .Value = If(
                    String.IsNullOrWhiteSpace(Me.Telefono),
                    CType(DBNull.Value, Object),
                    Me.Telefono
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
        ' ELIMINAR PERSONA
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

        Public Sub getRecord()

            Dim Query As String
            Dim parametros As New List(Of SqlParameter)

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

            parametros.Add(
            New SqlParameter("@idPersonas", SqlDbType.Int) With {
                .Value = Me.idPersonas
            }
        )

            Dim readers As IDataReader =
            db.ExecuteGetRecord(Query, parametros)

            If readers.Read() Then

                Me.idPersonas =
                Convert.ToInt32(readers("id_Personas"))

                Me.Nombres =
                Convert.ToString(readers("Nombres"))

                Me.Apellidos =
                Convert.ToString(readers("Apellidos"))

                Me.Genero =
                Convert.ToString(readers("Genero"))

                If Not IsDBNull(readers("Correo")) Then
                    Me.Correo =
                    Convert.ToString(readers("Correo"))
                Else
                    Me.Correo = ""
                End If

                If Not IsDBNull(readers("Teléfono")) Then
                    Me.Telefono =
                    Convert.ToString(readers("Teléfono"))
                Else
                    Me.Telefono = ""
                End If

                If Not IsDBNull(readers("Fecha_Nac")) Then

                    Me.Fecha_Nac =
                    Convert.ToDateTime(readers("Fecha_Nac"))

                Else

                    Me.Fecha_Nac = Nothing

                End If

                Me.Fecha_Registro =
                Convert.ToDateTime(
                    readers("Fecha_Registro")
                )

            End If

            readers.Close()

        End Sub


        '==================================================
        ' LISTAR PERSONAS PARA COMBOBOX
        '==================================================

        Public Function ListaDatosCombo() As DataTable

            Dim miDataTable As New DataTable()

            Dim Query As String

            Dim readers As IDataReader

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Query =
            "SELECT " &
            "id_Personas, " &
            "Nombres + ' ' + Apellidos AS NombreCompleto " &
            "FROM Personas " &
            "ORDER BY Nombres, Apellidos"

            readers = db.ExecuteReader(Query)

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

            readers.Close()

            Return miDataTable

        End Function


        '==================================================
        ' OBTENER ÚLTIMO ID
        '==================================================

        Public Function LastID() As Integer

            Const Query As String =
            "SELECT CAST(SCOPE_IDENTITY() AS INT)"

            Return Convert.ToInt32(
            db.ExecuteScalar(Query)
        )

        End Function
    End Class
End Namespace