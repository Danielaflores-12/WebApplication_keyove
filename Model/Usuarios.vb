Public Class Usuarios
    Public idUsuario As String
    Public Nombres As String
    Public Apellidos As String
    Public Nombre_Usuario As String
    Public Contraseña As String
    Public Correo As String
    Public Telefono As String
    Public Fecha_Registro As String
    Public qSelect As String
    Public db As New Data.ConexionBD
    Public Function ListaDatosTable() As DataTable
        Dim Query As String
        Dim readers As DataTable
        Query = Me.qSelect
        readers = db.ExecuteDataTable(Query)
        Return readers
    End Function
    Public Function ListaDatosShort() As DataTable
        Dim Query As String
        Dim readers As DataTable
        Query = "SELECT * FROM Usuarios"
        readers = db.ExecuteDataTable(Query)
        Return readers
    End Function
    Public Sub Insertar()
        Dim Query As String
        Query = String.Format("INSERT into Usuarios VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", Me.Nombres, Me.Apellidos, Me.Nombre_Usuario, Me.Contraseña, Me.Correo, Me.Telefono, Me.Fecha_Registro)
        If db.ExecuteQuery(Query) Then
            Me.idUsuario = Me.LastID()
        End If
    End Sub
    Public Sub Modificar()
        Dim Query As String
        Query = String.Format("UPDATE  Usuarios SET cNomPerfilPuesto='{1}',cNomCortoPerfilPuesto='{2}',cDesPerfilPuesto='{3}',cMOCMONC='{4}',dFechaCreacion='{5}',iCodUsuario='{6}',dFechaSistema='{7}' WHERE iCodPerfilPuesto={0}", Me.Nombres, Me.Apellidos, Me.Nombre_Usuario, Me.Contraseña, Me.Correo, Me.Telefono, Me.Fecha_Registro)
        db.ExecuteQuery(Query)
    End Sub
    Public Sub ModificarTransact()
        Dim Query As String
        Query = String.Format("UPDATE  Usuarios SET cNomPerfilPuesto='{1}',cNomCortoPerfilPuesto='{2}',cDesPerfilPuesto='{3}',cMOCMONC='{4}',dFechaCreacion='{5}',iCodUsuario='{6}',dFechaSistema='{7}' WHERE iCodPerfilPuesto={0}", Me.Nombres, Me.Apellidos, Me.Nombre_Usuario, Me.Contraseña, Me.Correo, Me.Telefono, Me.Fecha_Registro)
        db.ExecuteQueryTransact(Query)
    End Sub
    Public Sub Eliminar()
        Dim Query As String
        Query = String.Format("DELETE from Usuarios WHERE idusuario={0}", Me.idUsuario)
        db.ExecuteQuery(Query)
    End Sub
    Public Sub EliminarTransact()
        Dim Query As String
        Query = String.Format("DELETE from Usuarios WHERE idusuario={0}", Me.idUsuario)
        db.ExecuteQueryTransact(Query)
    End Sub
    Public Sub getRecord()
        Dim Query As String
        Dim readers As IDataReader
        Query = String.Format("SELECT * FROM Perfilpuesto WHERE iCodPerfilPuesto={0}", Me.idUsuario)
        readers = db.ExecuteGetRecord(Query)
        readers.Read()
        Me.idUsuario = CStr(readers.GetValue(0))
        Me.Nombres = CStr(readers.GetValue(1))
        Me.Apellidos = CStr(readers.GetValue(2))
        Me.Nombre_Usuario = CStr(readers.GetValue(3))
        Me.Contraseña = CStr(readers.GetValue(4))
        Me.Correo = CStr(readers.GetValue(5))
        Me.Telefono = CStr(readers.GetValue(6))
        Me.Fecha_Registro = CStr(readers.GetValue(7))
        readers.Close()
    End Sub
    Public Function ListaDatosCombo() As DataTable
        Dim miDataTable As New DataTable
        Dim Query As String
        Dim readers As IDataReader
        miDataTable.Columns.Add("ValueMember")
        miDataTable.Columns.Add("DisplayMember")
        Query = String.Format("SELECT * FROM Perfilpuesto")
        readers = db.ExecuteReader(Query)
        While readers.Read()
            miDataTable.Rows.Add(CStr(readers.GetValue(0)), CStr(readers.GetValue(1)))
        End While
        readers.Close()
        Return miDataTable
    End Function
    Public Function LastID() As Integer
        Const query As String = "SELECT SCOPE_IDENTITY()"
        Return db.ExecuteScalar(query)
    End Function
    'Public Function LastIDT() As Integer
    'Const query As String = "SELECT SCOPE_IDENTITY()"
    'Return db.ExecuteScalarTransact(query)
    'End Function
End Class
