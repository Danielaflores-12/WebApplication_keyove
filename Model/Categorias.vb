Imports System.Data
Imports System.Data.SqlClient
Imports WebApplication_Keyove.WebApplication_Keyove.Data

Namespace WebApplication_Keyove.Model

    Public Class Categorias

        '==================================================
        ' VARIABLES
        '==================================================

        Public iCodCategoria As Integer
        Public cNombre As String
        Public cDescripcion As String
        Public bEstado As Boolean = True
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
        ' CREAR PARÁMETROS DE LA CATEGORÍA
        '==================================================

        Private Function CrearParametros(
            incluirId As Boolean
        ) As List(Of SqlParameter)

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@cNombre", SqlDbType.NVarChar, 200) With {
                    .Value = Me.cNombre.Trim()
                }
            )

            parametros.Add(
                New SqlParameter("@cDescripcion", SqlDbType.NVarChar, 500) With {
                    .Value = ValorONull(Me.cDescripcion)
                }
            )

            parametros.Add(
                New SqlParameter("@bEstado", SqlDbType.Bit) With {
                    .Value = Me.bEstado
                }
            )

            If incluirId Then

                parametros.Add(
                    New SqlParameter("@iCodCategoria", SqlDbType.Int) With {
                        .Value = Me.iCodCategoria
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
                "UPDATE Categorias SET " &
                "cNombre = @cNombre, " &
                "cDescripcion = @cDescripcion, " &
                "bEstado = @bEstado " &
                "WHERE iCodCategoria = @iCodCategoria"

            Return Query

        End Function


        '==================================================
        ' LISTAR DATOS SEGÚN qSelect
        '==================================================

        Public Function ListaDatosTable() As DataTable

            Return db.ExecuteDataTable(Me.qSelect)

        End Function


        '==================================================
        ' LISTAR TODAS LAS CATEGORÍAS
        '==================================================

        Public Function ListaDatosShort() As DataTable

            Dim Query As String =
                "SELECT " &
                "iCodCategoria, " &
                "cNombre, " &
                "cDescripcion, " &
                "bEstado, " &
                "dFechaRegistro " &
                "FROM Categorias " &
                "ORDER BY iCodCategoria DESC"

            Return db.ExecuteDataTable(Query)

        End Function


        '==================================================
        ' INSERTAR CATEGORÍA
        '==================================================

        Public Sub Insertar()

            Dim Query As String =
                "INSERT INTO Categorias " &
                "(cNombre, cDescripcion, bEstado) " &
                "VALUES " &
                "(@cNombre, @cDescripcion, @bEstado); " &
                "SELECT CAST(SCOPE_IDENTITY() AS INT);"

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(False)

            Me.iCodCategoria =
                Convert.ToInt32(
                    db.ExecuteScalar(Query, parametros)
                )

        End Sub


        '==================================================
        ' MODIFICAR CATEGORÍA
        '==================================================

        Public Sub Modificar()

            Dim Query As String = ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' MODIFICAR CATEGORÍA EN TRANSACCIÓN
        '==================================================

        Public Sub ModificarTransact()

            Dim Query As String = ConsultaModificar()

            Dim parametros As List(Of SqlParameter) =
                CrearParametros(True)

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' DESACTIVAR CATEGORÍA
        '==================================================

        Public Sub Eliminar()

            Dim Query As String =
                "UPDATE Categorias " &
                "SET bEstado = 0 " &
                "WHERE iCodCategoria = @iCodCategoria"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodCategoria", SqlDbType.Int) With {
                    .Value = Me.iCodCategoria
                }
            )

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' DESACTIVAR CATEGORÍA EN TRANSACCIÓN
        '==================================================

        Public Sub EliminarTransact()

            Dim Query As String =
                "UPDATE Categorias " &
                "SET bEstado = 0 " &
                "WHERE iCodCategoria = @iCodCategoria"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodCategoria", SqlDbType.Int) With {
                    .Value = Me.iCodCategoria
                }
            )

            db.ExecuteQueryTransact(Query, parametros)

        End Sub


        '==================================================
        ' ACTIVAR CATEGORÍA
        '==================================================

        Public Sub Activar()

            Dim Query As String =
                "UPDATE Categorias " &
                "SET bEstado = 1 " &
                "WHERE iCodCategoria = @iCodCategoria"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodCategoria", SqlDbType.Int) With {
                    .Value = Me.iCodCategoria
                }
            )

            db.ExecuteQuery(Query, parametros)

        End Sub


        '==================================================
        ' OBTENER UNA CATEGORÍA POR SU ID
        '==================================================

        Public Function getRecord() As Boolean

            Dim Query As String =
                "SELECT " &
                "iCodCategoria, " &
                "cNombre, " &
                "cDescripcion, " &
                "bEstado, " &
                "dFechaRegistro " &
                "FROM Categorias " &
                "WHERE iCodCategoria = @iCodCategoria"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@iCodCategoria", SqlDbType.Int) With {
                    .Value = Me.iCodCategoria
                }
            )

            Dim readers As IDataReader =
                db.ExecuteGetRecord(Query, parametros)

            Try

                If Not readers.Read() Then
                    Return False
                End If

                Me.iCodCategoria =
                    Convert.ToInt32(readers("iCodCategoria"))

                Me.cNombre =
                    Convert.ToString(readers("cNombre"))

                If IsDBNull(readers("cDescripcion")) Then
                    Me.cDescripcion = ""
                Else
                    Me.cDescripcion =
                        Convert.ToString(readers("cDescripcion"))
                End If

                Me.bEstado =
                    Convert.ToBoolean(readers("bEstado"))

                Me.dFechaRegistro =
                    Convert.ToDateTime(readers("dFechaRegistro"))

                Return True

            Finally

                readers.Close()

            End Try

        End Function


        '==================================================
        ' LISTAR CATEGORÍAS ACTIVAS PARA COMBOBOX
        '==================================================

        Public Function ListaDatosCombo() As DataTable

            Dim miDataTable As New DataTable()

            miDataTable.Columns.Add("ValueMember")
            miDataTable.Columns.Add("DisplayMember")

            Dim Query As String =
                "SELECT " &
                "iCodCategoria, " &
                "cNombre " &
                "FROM Categorias " &
                "WHERE bEstado = 1 " &
                "ORDER BY cNombre"

            Dim readers As IDataReader =
                db.ExecuteReader(Query)

            Try

                While readers.Read()

                    miDataTable.Rows.Add(
                        Convert.ToString(
                            readers("iCodCategoria")
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


        '==================================================
        ' BUSCAR POR NOMBRE O DESCRIPCIÓN
        '==================================================

        Public Function Buscar(
            ByVal texto As String
        ) As DataTable

            Dim Query As String =
                "SELECT " &
                "iCodCategoria, " &
                "cNombre, " &
                "cDescripcion, " &
                "bEstado, " &
                "dFechaRegistro " &
                "FROM Categorias " &
                "WHERE cNombre LIKE @Texto " &
                "OR cDescripcion LIKE @Texto " &
                "ORDER BY cNombre"

            Dim parametros As New List(Of SqlParameter)

            parametros.Add(
                New SqlParameter("@Texto", SqlDbType.NVarChar, 500) With {
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