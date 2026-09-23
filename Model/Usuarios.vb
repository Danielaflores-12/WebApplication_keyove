Imports System.Data.SqlClient
Imports WebApplication_keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Usuarios

        '==================================================
        ' VARIABLES
        '==================================================

        Public iCodUsuario As Integer
        Public iCodPersona As Integer?
        Public iCodRol As Integer
        Public cNombreRol As String
        Public cNombrePersona As String
        Public cNombreUsuario As String
        Public cContrasenaHash As String
        Public dFechaRegistro As DateTime
        Public bEstado As Boolean = True

        Public qSelect As String
        Public db As New ConexionBD()


        '==================================================
        ' CREAR PARÁMETROS DEL USUARIO
        '==================================================

        Private Function CrearParametros(
            incluirId As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

            ' Persona puede ser NULL
            parametros.Add(
                New SqlParameter("@iCodPersona", SqlDbType.Int) With {
                    .Value = If(
                        Me.iCodPersona.HasValue,
                        CType(Me.iCodPersona.Value, Object),
                        DBNull.Value
                    )
                }
            )

            ' Rol
            parametros.Add(
                New SqlParameter("@iCodRol", SqlDbType.Int) With {
                    .Value = Me.iCodRol
                }
            )

            ' Nombre de usuario
            parametros.Add(
                New SqlParameter("@cNombreUsuario", SqlDbType.NVarChar, 100) With {
                    .Value = Me.cNombreUsuario.Trim()
                }
            )

            ' Contraseña Hash
            parametros.Add(
                New SqlParameter("@cContrasenaHash", SqlDbType.NVarChar, 255) With {
                    .Value = Me.cContrasenaHash
                }
            )

            ' Estado
            parametros.Add(
                New SqlParameter("@bEstado", SqlDbType.Bit) With {
                    .Value = Me.bEstado
                }
            )

            ' ID del usuario solamente para modificar
            If incluirId Then

                parametros.Add(
                    New SqlParameter("@iCodUsuario", SqlDbType.Int) With {
                        .Value = Me.iCodUsuario
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
                "UPDATE Usuarios SET " &
                "iCodPersona = @iCodPersona, " &
                "iCodRol = @iCodRol, " &
                "cNombreUsuario = @cNombreUsuario, " &
                "cContrasenaHash = @cContrasenaHash, " &
                "bEstado = @bEstado " &
                "WHERE iCodUsuario = @iCodUsuario"

            Return Query

        End Function


        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function


        '==================================================
        ' LISTAR TODOS LOS USUARIOS
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "U.iCodUsuario, " &
                "U.iCodPersona, " &
                "LTRIM(RTRIM(COALESCE(P.cNombres, '') + ' ' + COALESCE(P.cApellidos, ''))) AS cNombrePersona, " &
                "U.iCodRol, " &
                "R.cNombre AS Rol, " &
                "U.cNombreUsuario, " &
                "U.dFechaRegistro, " &
                "U.bEstado " &
                "FROM Usuarios U " &
                "LEFT JOIN Personas P ON U.iCodPersona = P.iCodPersona " &
                "INNER JOIN Roles R " &
                "ON U.iCodRol = R.iCodRol " &
                "ORDER BY U.iCodUsuario DESC"

            Return db.ExecuteDataTable(Query)

        End Function


        '==================================================
        ' INSERTAR USUARIO
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Usuarios " &
                "(iCodPersona, iCodRol, cNombreUsuario, " &
                "cContrasenaHash, bEstado) " &
                "VALUES " &
                "(@iCodPersona, @iCodRol, @cNombreUsuario, " &
                "@cContrasenaHash, @bEstado); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodUsuario =
                Convert.ToInt32(
                    db.ExecuteScalar(Query, parametros)
                )

        End Sub


        '==================================================
        ' MODIFICAR USUARIO
        '==================================================

        Public Sub Modificar()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' MODIFICAR USUARIO EN TRANSACCIÓN
        '==================================================

        Public Sub ModificarTransact()

            Dim Query As String =
                ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' DESACTIVAR USUARIO
        '==================================================

        Public Sub Eliminar()

            Dim Query As String =
                "UPDATE Usuarios " &
                "SET bEstado = 0 " &
                "WHERE iCodUsuario = @iCodUsuario"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodUsuario", SqlDbType.Int) With {
                    .Value = Me.iCodUsuario
                }
            )

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' DESACTIVAR USUARIO EN TRANSACCIÓN
        '==================================================

        Public Sub EliminarTransact()

            Dim Query As String =
                "UPDATE Usuarios " &
                "SET bEstado = 0 " &
                "WHERE iCodUsuario = @iCodUsuario"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodUsuario", SqlDbType.Int) With {
                    .Value = Me.iCodUsuario
                }
            )

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' ACTIVAR USUARIO
        '==================================================

        Public Sub Activar()

            Dim Query As String =
                "UPDATE Usuarios " &
                "SET bEstado = 1 " &
                "WHERE iCodUsuario = @iCodUsuario"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodUsuario", SqlDbType.Int) With {
                    .Value = Me.iCodUsuario
                }
            )

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' OBTENER UN USUARIO POR SU ID
        '==================================================

        Public Sub getRecord()

            Dim Query As String =
                "SELECT " &
                "iCodUsuario, " &
                "iCodPersona, " &
                "iCodRol, " &
                "cNombreUsuario, " &
                "cContrasenaHash, " &
                "dFechaRegistro, " &
                "bEstado " &
                "FROM Usuarios " &
                "WHERE iCodUsuario = @iCodUsuario"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodUsuario", SqlDbType.Int) With {
                    .Value = Me.iCodUsuario
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(Query, parametros)

            Try

                If readers.Read() Then

                    Me.iCodUsuario =
                        Convert.ToInt32(
                            readers("iCodUsuario")
                        )

                    If IsDBNull(
                        readers("iCodPersona")
                    ) Then

                        Me.iCodPersona = Nothing

                    Else

                        Me.iCodPersona =
                            Convert.ToInt32(
                                readers("iCodPersona")
                            )

                    End If

                    Me.iCodRol =
                        Convert.ToInt32(
                            readers("iCodRol")
                        )

                    Me.cNombreUsuario =
                        Convert.ToString(
                            readers("cNombreUsuario")
                        )

                    Me.cContrasenaHash =
                        Convert.ToString(
                            readers("cContrasenaHash")
                        )

                    Me.dFechaRegistro =
                        Convert.ToDateTime(
                            readers("dFechaRegistro")
                        )

                    Me.bEstado =
                        Convert.ToBoolean(
                            readers("bEstado")
                        )

                End If

            Finally

                readers.Close()

            End Try

        End Sub


        '==================================================
        ' BUSCAR USUARIO POR NOMBRE
        '==================================================

        Public Function BuscarPorNombre(
            nombreUsuario As String
        ) As DataTable

            Dim Query As String =
                "SELECT " &
                "U.iCodUsuario, " &
                "U.iCodPersona, " &
                "U.iCodRol, " &
                "R.cNombre AS Rol, " &
                "U.cNombreUsuario, " &
                "U.dFechaRegistro, " &
                "U.bEstado " &
                "FROM Usuarios U " &
                "INNER JOIN Roles R " &
                "ON U.iCodRol = R.iCodRol " &
                "WHERE U.cNombreUsuario LIKE @NombreUsuario " &
                "ORDER BY U.cNombreUsuario"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter(
                    "@NombreUsuario",
                    SqlDbType.NVarChar,
                    100
                ) With {
                    .Value = "%" & nombreUsuario.Trim() & "%"
                }
            )

            Return db.ExecuteDataTable(
                Query,
                parametros
            )

        End Function


        '==================================================
        ' LISTAR USUARIOS ACTIVOS PARA COMBOBOX
        '==================================================

        Public Function ListaDatosCombo() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Dim Query As String =
                "SELECT " &
                "iCodUsuario, " &
                "cNombreUsuario " &
                "FROM Usuarios " &
                "WHERE bEstado = 1 " &
                "ORDER BY cNombreUsuario"

            Dim readers As IDataReader =
                db.ExecuteReader(Query)

            Try

                While readers.Read()

                    miDataTable.Rows.Add(
                        Convert.ToString(
                            readers("iCodUsuario")
                        ),
                        Convert.ToString(
                            readers("cNombreUsuario")
                        )
                    )

                End While

            Finally

                readers.Close()

            End Try

            Return miDataTable

        End Function


        '==================================================
        ' LISTAR ROLES ACTIVOS PARA COMBOBOX
        '==================================================

        Public Function ListaRolesCombo() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Dim Query As String =
                "SELECT " &
                "iCodRol, " &
                "cNombre " &
                "FROM Roles " &
                "WHERE bEstado = 1 " &
                "ORDER BY cNombre"

            Dim readers As IDataReader =
                db.ExecuteReader(Query)

            Try

                While readers.Read()

                    miDataTable.Rows.Add(
                        Convert.ToString(
                            readers("iCodRol")
                        ),
                        Convert.ToString(
                            readers("cNombre")
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
