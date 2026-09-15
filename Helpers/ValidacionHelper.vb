Public Class ValidacionHelper

    Public Shared Function TextoVacio(valor As String) As Boolean
        Return String.IsNullOrWhiteSpace(valor)
    End Function

    Public Shared Function TelefonoValido(telefono As String) As Boolean
        Return telefono IsNot Nothing AndAlso
               telefono.Length = 9 AndAlso
               IsNumeric(telefono)
    End Function

End Class