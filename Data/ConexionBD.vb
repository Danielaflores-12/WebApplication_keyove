Imports System.Data.SqlClient
Imports System.Data
Namespace WebApplication_Keyove.Data

    Public Class ConexionBD
        Implements IDisposable

        Private linkSQL As SqlConnection
        Private miTransact As SqlTransaction

        '==================================================
        ' CONFIGURACIÓN DE LA BASE DE DATOS
        '==================================================

        Private ReadOnly Servidor As String =
        ".\SQLEXPRESS"

        Private ReadOnly BaseDatos As String =
        "DB_keyove_inventario"

        ' Autenticación de Windows
        Private ReadOnly cadenaConexion As String =
        "Data Source=" & Servidor & ";" &
        "Initial Catalog=" & BaseDatos & ";" &
        "Integrated Security=True;" &
        "MultipleActiveResultSets=True;" &
        "TrustServerCertificate=True;"


        '==================================================
        ' CONSTRUCTOR
        '==================================================

        Public Sub New()

            linkSQL = New SqlConnection(cadenaConexion)

        End Sub


        '==================================================
        ' ABRIR CONEXIÓN
        '==================================================

        Public Function OpenConnection() As Boolean

            Try

                If linkSQL Is Nothing Then
                    linkSQL = New SqlConnection(cadenaConexion)
                End If

                If linkSQL.State <> ConnectionState.Open Then
                    linkSQL.Open()
                End If

                Return True

            Catch ex As SqlException

                Throw New Exception(
                "No se pudo conectar con la base de datos." &
                Environment.NewLine &
                ex.Message,
                ex
            )

            End Try

        End Function


        '==================================================
        ' CERRAR CONEXIÓN
        '==================================================

        Public Sub CloseConnection()

            If linkSQL IsNot Nothing Then

                If linkSQL.State <> ConnectionState.Closed Then
                    linkSQL.Close()
                End If

            End If

        End Sub


        '==================================================
        ' SELECT - SQLDATAREADER
        '==================================================

        Public Function ExecuteReader(
        ByVal consulta As String
    ) As SqlDataReader

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Dim comando As New SqlCommand(
            consulta,
            linkSQL
        )

            Return comando.ExecuteReader()

        End Function


        '==================================================
        ' SELECT PARAMETRIZADO - SQLDATAREADER
        '==================================================

        Public Function ExecuteReader(
        ByVal consulta As String,
        ByVal parametros As List(Of SqlParameter)
    ) As SqlDataReader

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Dim comando As New SqlCommand(
            consulta,
            linkSQL
        )

            If parametros IsNot Nothing Then

                comando.Parameters.AddRange(
                parametros.ToArray()
            )

            End If

            Return comando.ExecuteReader()

        End Function


        '==================================================
        ' INSERT / UPDATE / DELETE
        '==================================================

        Public Function ExecuteQuery(
        ByVal consulta As String
    ) As Boolean

            Try

                If linkSQL.State <> ConnectionState.Open Then
                    linkSQL.Open()
                End If

                Using comando As New SqlCommand(
                consulta,
                linkSQL
            )

                    comando.ExecuteNonQuery()

                End Using

                Return True

            Catch ex As SqlException

                Throw New Exception(
                "Error al ejecutar la consulta." &
                Environment.NewLine &
                ex.Message,
                ex
            )

            End Try

        End Function


        '==================================================
        ' INSERT / UPDATE / DELETE PARAMETRIZADO
        '==================================================

        Public Function ExecuteQuery(
        ByVal consulta As String,
        ByVal parametros As List(Of SqlParameter)
    ) As Boolean

            Try

                If linkSQL.State <> ConnectionState.Open Then
                    linkSQL.Open()
                End If

                Using comando As New SqlCommand(
                consulta,
                linkSQL
            )

                    If parametros IsNot Nothing Then

                        comando.Parameters.AddRange(
                        parametros.ToArray()
                    )

                    End If

                    comando.ExecuteNonQuery()

                End Using

                Return True

            Catch ex As SqlException

                Throw New Exception(
                "Error al ejecutar la consulta." &
                Environment.NewLine &
                ex.Message,
                ex
            )

            End Try

        End Function


        '==================================================
        ' INSERT / UPDATE / DELETE
        ' DEVUELVE FILAS AFECTADAS
        '==================================================

        Public Function ExecuteQueryGetRows(
        ByVal consulta As String
    ) As Integer

            Try

                If linkSQL.State <> ConnectionState.Open Then
                    linkSQL.Open()
                End If

                Using comando As New SqlCommand(
                consulta,
                linkSQL
            )

                    Return comando.ExecuteNonQuery()

                End Using

            Catch ex As SqlException

                Throw New Exception(
                "Error al ejecutar la consulta." &
                Environment.NewLine &
                ex.Message,
                ex
            )

            End Try

        End Function


        '==================================================
        ' INSERT / UPDATE / DELETE PARAMETRIZADO
        ' DEVUELVE FILAS AFECTADAS
        '==================================================

        Public Function ExecuteQueryGetRows(
        ByVal consulta As String,
        ByVal parametros As List(Of SqlParameter)
    ) As Integer

            Try

                If linkSQL.State <> ConnectionState.Open Then
                    linkSQL.Open()
                End If

                Using comando As New SqlCommand(
                consulta,
                linkSQL
            )

                    If parametros IsNot Nothing Then

                        comando.Parameters.AddRange(
                        parametros.ToArray()
                    )

                    End If

                    Return comando.ExecuteNonQuery()

                End Using

            Catch ex As SqlException

                Throw New Exception(
                "Error al ejecutar la consulta." &
                Environment.NewLine &
                ex.Message,
                ex
            )

            End Try

        End Function


        '==================================================
        ' OBTENER UN SOLO REGISTRO
        '==================================================

        Public Function ExecuteGetRecord(
        ByVal consulta As String
    ) As SqlDataReader

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Dim comando As New SqlCommand(
            consulta,
            linkSQL
        )

            Return comando.ExecuteReader(
            CommandBehavior.SingleRow
        )

        End Function


        '==================================================
        ' OBTENER UN SOLO REGISTRO PARAMETRIZADO
        '==================================================

        Public Function ExecuteGetRecord(
        ByVal consulta As String,
        ByVal parametros As List(Of SqlParameter)
    ) As SqlDataReader

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Dim comando As New SqlCommand(
            consulta,
            linkSQL
        )

            If parametros IsNot Nothing Then

                comando.Parameters.AddRange(
                parametros.ToArray()
            )

            End If

            Return comando.ExecuteReader(
            CommandBehavior.SingleRow
        )

        End Function


        '==================================================
        ' DEVUELVE DATATABLE
        '==================================================

        Public Function ExecuteDataTable(
        ByVal consulta As String
    ) As DataTable

            Dim tabla As New DataTable()

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Using comando As New SqlCommand(
            consulta,
            linkSQL
        )

                Using adaptador As New SqlDataAdapter(comando)

                    adaptador.Fill(tabla)

                End Using

            End Using

            Return tabla

        End Function


        '==================================================
        ' DATATABLE PARAMETRIZADO
        '==================================================

        Public Function ExecuteDataTable(
        ByVal consulta As String,
        ByVal parametros As List(Of SqlParameter)
    ) As DataTable

            Dim tabla As New DataTable()

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Using comando As New SqlCommand(
            consulta,
            linkSQL
        )

                If parametros IsNot Nothing Then

                    comando.Parameters.AddRange(
                    parametros.ToArray()
                )

                End If

                Using adaptador As New SqlDataAdapter(comando)

                    adaptador.Fill(tabla)

                End Using

            End Using

            Return tabla

        End Function


        '==================================================
        ' DEVUELVE DATASET
        '==================================================

        Public Function ExecuteDataSet(
        ByVal consulta As String
    ) As DataSet

            Dim datos As New DataSet()

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Using comando As New SqlCommand(
            consulta,
            linkSQL
        )

                Using adaptador As New SqlDataAdapter(comando)

                    adaptador.Fill(datos)

                End Using

            End Using

            Return datos

        End Function


        '==================================================
        ' DATASET PARAMETRIZADO
        '==================================================

        Public Function ExecuteDataSet(
        ByVal consulta As String,
        ByVal parametros As List(Of SqlParameter)
    ) As DataSet

            Dim datos As New DataSet()

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Using comando As New SqlCommand(
            consulta,
            linkSQL
        )

                If parametros IsNot Nothing Then

                    comando.Parameters.AddRange(
                    parametros.ToArray()
                )

                End If

                Using adaptador As New SqlDataAdapter(comando)

                    adaptador.Fill(datos)

                End Using

            End Using

            Return datos

        End Function


        '==================================================
        ' EXECUTE SCALAR
        ' COUNT, SUM, MAX, SCOPE_IDENTITY, ETC.
        '==================================================

        Public Function ExecuteScalar(
        ByVal consulta As String
    ) As Object

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Using comando As New SqlCommand(
            consulta,
            linkSQL
        )

                Return comando.ExecuteScalar()

            End Using

        End Function


        '==================================================
        ' EXECUTE SCALAR PARAMETRIZADO
        '==================================================

        Public Function ExecuteScalar(
        ByVal consulta As String,
        ByVal parametros As List(Of SqlParameter)
    ) As Object

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            Using comando As New SqlCommand(
            consulta,
            linkSQL
        )

                If parametros IsNot Nothing Then

                    comando.Parameters.AddRange(
                    parametros.ToArray()
                )

                End If

                Return comando.ExecuteScalar()

            End Using

        End Function


        '==================================================
        ' INICIAR TRANSACCIÓN
        '==================================================

        Public Sub BeginTransaction()

            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            If miTransact IsNot Nothing Then
                Throw New InvalidOperationException(
                "Ya existe una transacción activa."
            )
            End If

            miTransact = linkSQL.BeginTransaction()

        End Sub


        '==================================================
        ' CONFIRMAR TRANSACCIÓN
        '==================================================

        Public Sub CommitTransaction()

            If miTransact IsNot Nothing Then

                Try

                    miTransact.Commit()

                Finally

                    miTransact.Dispose()
                    miTransact = Nothing

                End Try

            End If

        End Sub


        '==================================================
        ' CANCELAR TRANSACCIÓN
        '==================================================

        Public Sub RollbackTransaction()

            If miTransact IsNot Nothing Then

                Try

                    miTransact.Rollback()

                Finally

                    miTransact.Dispose()
                    miTransact = Nothing

                End Try

            End If

        End Sub


        '==================================================
        ' EXECUTE QUERY EN TRANSACCIÓN
        '==================================================

        Public Function ExecuteQueryTransact(
        ByVal consulta As String
    ) As Boolean

            If miTransact Is Nothing Then

                Throw New InvalidOperationException(
                "No existe una transacción activa."
            )

            End If

            Using comando As New SqlCommand(
            consulta,
            linkSQL,
            miTransact
        )

                comando.ExecuteNonQuery()

            End Using

            Return True

        End Function


        '==================================================
        ' EXECUTE QUERY PARAMETRIZADO EN TRANSACCIÓN
        '==================================================

        Public Function ExecuteQueryTransact(
        ByVal consulta As String,
        ByVal parametros As List(Of SqlParameter)
    ) As Boolean

            If miTransact Is Nothing Then

                Throw New InvalidOperationException(
                "No existe una transacción activa."
            )

            End If

            Using comando As New SqlCommand(
            consulta,
            linkSQL,
            miTransact
        )

                If parametros IsNot Nothing Then

                    comando.Parameters.AddRange(
                    parametros.ToArray()
                )

                End If

                comando.ExecuteNonQuery()

            End Using

            Return True

        End Function


        '==================================================
        ' DATATABLE EN TRANSACCIÓN
        '==================================================

        Public Function ExecuteDataTableTransact(
        ByVal consulta As String
    ) As DataTable

            If miTransact Is Nothing Then

                Throw New InvalidOperationException(
                "No existe una transacción activa."
            )

            End If

            Dim tabla As New DataTable()

            Using comando As New SqlCommand(
            consulta,
            linkSQL,
            miTransact
        )

                Using lector As SqlDataReader =
                comando.ExecuteReader()

                    tabla.Load(lector)

                End Using

            End Using

            Return tabla

        End Function


        '==================================================
        ' DATATABLE PARAMETRIZADO EN TRANSACCIÓN
        '==================================================

        Public Function ExecuteDataTableTransact(
        ByVal consulta As String,
        ByVal parametros As List(Of SqlParameter)
    ) As DataTable

            If miTransact Is Nothing Then

                Throw New InvalidOperationException(
                "No existe una transacción activa."
            )

            End If

            Dim tabla As New DataTable()

            Using comando As New SqlCommand(
            consulta,
            linkSQL,
            miTransact
        )

                If parametros IsNot Nothing Then

                    comando.Parameters.AddRange(
                    parametros.ToArray()
                )

                End If

                Using lector As SqlDataReader =
                comando.ExecuteReader()

                    tabla.Load(lector)

                End Using

            End Using

            Return tabla

        End Function


        '==================================================
        ' LIBERAR RECURSOS
        '==================================================

        Public Sub Dispose() Implements IDisposable.Dispose

            Try

                If miTransact IsNot Nothing Then

                    Try
                        miTransact.Rollback()
                    Catch
                    End Try

                    miTransact.Dispose()
                    miTransact = Nothing

                End If

            Finally

                CloseConnection()

                If linkSQL IsNot Nothing Then

                    linkSQL.Dispose()
                    linkSQL = Nothing

                End If

            End Try

        End Sub

    End Class
End Namespace