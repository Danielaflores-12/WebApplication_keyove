Imports System.Data
Imports System.Data.SqlClient
Imports WebApplication_Keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Personas

        '==================================================
        ' VARIABLES
        '==================================================

        Public iCodPersona As Integer
        Public cNombres As String
        Public cApellidos As String
        Public cGenero As String
        Public cCorreo As String
        Public cTelefono As String
        Public dFechaNacimiento As Date?
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
        ' CREAR PARÁMETROS DE LA PERSONA
        '==================================================

        Private Function CrearParametros(
            incluirId As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@cNombres", SqlDbType.NVarChar, 100) With {
                    .Value = Me.cNombres.Trim()
                }
            )

            parametros.Add(
                New SqlParameter("@cApellidos", SqlDbType.NVarChar, 100) With {
                    .Value = Me.cApellidos.Trim()
                }
            )

            parametros.Add(
                New SqlParameter("@cGenero", SqlDbType.Char, 1) With {
                    .Value = Me.cGenero
                }
            )

            parametros.Add(
                New SqlParameter("@cCorreo", SqlDbType.NVarChar, 100) With {
                    .Value = ValorONull(Me.cCorreo)
                }
            )

            parametros.Add(
                New SqlParameter("@cTelefono", SqlDbType.VarChar, 9) With {
                    .Value = ValorONull(Me.cTelefono)
                }
            )

            parametros.Add(
                New SqlParameter("@dFechaNacimiento", SqlDbType.Date) With {
                    .Value = If(
                        Me.dFechaNacimiento.HasValue,
                        CType(Me.dFechaNacimiento.Value, Object),
                        DBNull.Value
                    )
                }
            )

            If incluirId Then

                parametros.Add(
                    New SqlParameter("@iCodPersona", SqlDbType.Int) With {
                        .Value = Me.iCodPersona
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
                "UPDATE Personas SET " &
                "cNombres = @cNombres, " &
                "cApellidos = @cApellidos, " &
                "cGenero = @cGenero, " &
                "cCorreo = @cCorreo, " &
                "cTelefono = @cTelefono, " &
                "dFechaNacimiento = @dFechaNacimiento " &
                "WHERE iCodPersona = @iCodPersona"

            Return Query

        End Function


        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function


        '==================================================
        ' LISTAR TODAS LAS PERSONAS
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "iCodPersona, " &
                "cNombres, " &
                "cApellidos, " &
                "cGenero, " &
                "cCorreo, " &
                "cTelefono, " &
                "dFechaNacimiento, " &
                "dFechaRegistro " &
                "FROM Personas " &
                "ORDER BY iCodPersona DESC"

            Return db.ExecuteDataTable(Query)

        End Function


        '==================================================
        ' INSERTAR PERSONA
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Personas " &
                "(cNombres, cApellidos, cGenero, " &
                "cCorreo, cTelefono, dFechaNacimiento) " &
                "VALUES " &
                "(@cNombres, @cApellidos, @cGenero, " &
                "@cCorreo, @cTelefono, @dFechaNacimiento); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodPersona =
                Convert.ToInt32(
                    db.ExecuteScalar(Query, parametros)
                )

        End Sub


        '==================================================
        ' MODIFICAR PERSONA
        '==================================================

        Public Sub Modificar()

            Dim Query As String = ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' MODIFICAR PERSONA EN TRANSACCIÓN
        '==================================================

        Public Sub ModificarTransact()

            Dim Query As String = ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' ELIMINAR PERSONA
        '==================================================

        Public Sub Eliminar()

            Dim Query As String =
                "DELETE FROM Personas " &
                "WHERE iCodPersona = @iCodPersona"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodPersona", SqlDbType.Int) With {
                    .Value = Me.iCodPersona
                }
            )

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' ELIMINAR PERSONA EN TRANSACCIÓN
        '==================================================

        Public Sub EliminarTransact()

            Dim Query As String =
                "DELETE FROM Personas " &
                "WHERE iCodPersona = @iCodPersona"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodPersona", SqlDbType.Int) With {
                    .Value = Me.iCodPersona
                }
            )

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' OBTENER UNA PERSONA POR SU ID
        '==================================================

        Public Function getRecord() As Boolean

            Dim Query As String =
                "SELECT " &
                "iCodPersona, " &
                "cNombres, " &
                "cApellidos, " &
                "cGenero, " &
                "cCorreo, " &
                "cTelefono, " &
                "dFechaNacimiento, " &
                "dFechaRegistro " &
                "FROM Personas " &
                "WHERE iCodPersona = @iCodPersona"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodPersona", SqlDbType.Int) With {
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
                    Convert.ToInt32(readers("iCodPersona"))

                Me.cNombres =
                    Convert.ToString(readers("cNombres"))

                Me.cApellidos =
                    Convert.ToString(readers("cApellidos"))

                Me.cGenero =
                    Convert.ToString(readers("cGenero"))

                If IsDBNull(readers("cCorreo")) Then
                    Me.cCorreo = ""
                Else
                    Me.cCorreo =
                        Convert.ToString(readers("cCorreo"))
                End If

                If IsDBNull(readers("cTelefono")) Then
                    Me.cTelefono = ""
                Else
                    Me.cTelefono =
                        Convert.ToString(readers("cTelefono"))
                End If

                If IsDBNull(readers("dFechaNacimiento")) Then
                    Me.dFechaNacimiento = Nothing
                Else
                    Me.dFechaNacimiento =
                        Convert.ToDateTime(
                            readers("dFechaNacimiento")
                        )
                End If

                Me.dFechaRegistro =
                    Convert.ToDateTime(
                        readers("dFechaRegistro")
                    )

                Return True

            Finally

                readers.Close()

            End Try

        End Function


        '==================================================
        ' LISTAR PERSONAS PARA COMBOBOX
        '==================================================

        Public Function ListaDatosCombo() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Dim Query As String =
                "SELECT " &
                "iCodPersona, " &
                "cNombres + ' ' + cApellidos AS NombreCompleto " &
                "FROM Personas " &
                "ORDER BY cNombres, cApellidos"

            Dim readers As IDataReader =
                db.ExecuteReader(Query)

            Try

                While readers.Read()

                    miDataTable.Rows.Add(
                        Convert.ToString(
                            readers("iCodPersona")
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
                "iCodPersona, " &
                "cNombres, " &
                "cApellidos, " &
                "cGenero, " &
                "cCorreo, " &
                "cTelefono, " &
                "dFechaNacimiento, " &
                "dFechaRegistro " &
                "FROM Personas " &
                "WHERE cNombres LIKE @Texto " &
                "OR cApellidos LIKE @Texto " &
                "ORDER BY cNombres, cApellidos"

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

End Namespace