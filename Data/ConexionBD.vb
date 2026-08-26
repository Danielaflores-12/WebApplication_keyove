Imports System.Data
Imports System.Data.SqlClient

Namespace Data

    Public Class ConexionBD
        Implements IDisposable

        Private linkSQL As SqlConnection
        Private miTransact As SqlTransaction

        'CAMBIA ESTOS DOS DATOS.
        Private ReadOnly Servidor As String =
            "DESKTOP-U8H3H3L\SQLEXPRESS"

        Private ReadOnly BaseDatos As String =
            "DB_keyove_inventario"

        'Usaremos autenticación de Windows.
        Private ReadOnly cadenaConexion As String =
            "Data Source=" & Servidor & ";" &
            "Initial Catalog=" & BaseDatos & ";" &
            "Integrated Security=True;" &
            "MultipleActiveResultSets=True;" &
            "TrustServerCertificate=True;"

        Public Sub New()
            linkSQL = New SqlConnection(cadenaConexion)
        End Sub

        'Abrir conexión.
        Public Function OpenConnection() As String
            Try
                If linkSQL Is Nothing Then
                    linkSQL = New SqlConnection(cadenaConexion)
                End If

                If linkSQL.State = ConnectionState.Open Then
                    linkSQL.Close()
                End If

                linkSQL.Open()
                Return "OK"

            Catch ex As SqlException
                Return ex.Message
            End Try
        End Function

        'Cerrar conexión.
        Public Sub CloseConnection()
            If linkSQL IsNot Nothing Then

                If linkSQL.State <> ConnectionState.Closed Then
                    linkSQL.Close()
                End If

            End If
        End Sub

        'SELECT: devuelve registros.
        Public Function ExecuteReader(
            ByVal consulta As String
        ) As SqlDataReader

            Dim comando As New SqlCommand(consulta, linkSQL)
            Return comando.ExecuteReader()
        End Function

        'INSERT, UPDATE o DELETE.
        Public Function ExecuteQuery(
            ByVal consulta As String
        ) As Boolean

            Try
                Using comando As New SqlCommand(consulta, linkSQL)
                    comando.ExecuteNonQuery()
                End Using

                Return True

            Catch
                Return False
            End Try
        End Function

        'INSERT, UPDATE o DELETE: devuelve filas afectadas.
        Public Function ExecuteQueryGetRows(
            ByVal consulta As String
        ) As Integer

            Try
                Using comando As New SqlCommand(consulta, linkSQL)
                    Return comando.ExecuteNonQuery()
                End Using

            Catch
                Return -1
            End Try
        End Function

        'Obtiene solamente el primer registro.
        Public Function ExecuteGetRecord(
            ByVal consulta As String
        ) As SqlDataReader

            Dim comando As New SqlCommand(consulta, linkSQL)

            Return comando.ExecuteReader(
                CommandBehavior.SingleRow
            )
        End Function

        'Devuelve los registros en un DataTable.
        Public Function ExecuteDataTable(
            ByVal consulta As String
        ) As DataTable

            Dim tabla As New DataTable()

            Using comando As New SqlCommand(consulta, linkSQL)
                Using adaptador As New SqlDataAdapter(comando)
                    adaptador.Fill(tabla)
                End Using
            End Using

            Return tabla
        End Function

        'Devuelve información en un DataSet.
        Public Function ExecuteDataSet(
            ByVal consulta As String
        ) As DataSet

            Dim datos As New DataSet()

            Using comando As New SqlCommand(consulta, linkSQL)
                Using adaptador As New SqlDataAdapter(comando)
                    adaptador.Fill(datos)
                End Using
            End Using

            Return datos
        End Function

        'Devuelve un único valor: COUNT, MAX, SUM, etc.
        Public Function ExecuteScalar(
            ByVal consulta As String
        ) As Object

            Using comando As New SqlCommand(consulta, linkSQL)
                Return comando.ExecuteScalar()
            End Using
        End Function

        'Iniciar transacción.
        Public Sub BeginTransaction()
            If linkSQL.State <> ConnectionState.Open Then
                linkSQL.Open()
            End If

            miTransact = linkSQL.BeginTransaction()
        End Sub

        'Confirmar transacción.
        Public Sub CommitTransaction()
            If miTransact IsNot Nothing Then
                miTransact.Commit()
                miTransact.Dispose()
                miTransact = Nothing
            End If
        End Sub

        'Cancelar transacción.
        Public Sub RollbackTransaction()
            If miTransact IsNot Nothing Then
                miTransact.Rollback()
                miTransact.Dispose()
                miTransact = Nothing
            End If
        End Sub

        'Ejecutar INSERT, UPDATE o DELETE en una transacción.
        Public Function ExecuteQueryTransact(
            ByVal consulta As String
        ) As Boolean

            Using comando As New SqlCommand(
                consulta,
                linkSQL,
                miTransact
            )
                comando.ExecuteNonQuery()
            End Using

            Return True
        End Function

        'Ejecutar SELECT en una transacción.
        Public Function ExecuteDataTableTransact(
            ByVal consulta As String
        ) As DataTable

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

        'Liberar la conexión.
        Public Sub Dispose() Implements IDisposable.Dispose
            CloseConnection()

            If linkSQL IsNot Nothing Then
                linkSQL.Dispose()
                linkSQL = Nothing
            End If
        End Sub

    End Class

End Namespace