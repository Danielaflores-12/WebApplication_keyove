Imports System.Data.SqlClient
Imports WebApplication_keyove.WebApplication_Keyove.Data

Public Class ComprobanteHelper

    ' Debe ejecutarse en la misma transacción que inserta la compra o venta.
    Public Shared Function GenerarNumero(db As ConexionBD, tabla As String, tipo As String) As String
        If tabla <> "Compras" AndAlso tabla <> "Ventas" Then
            Throw New ArgumentException("Origen de comprobante no válido.")
        End If

        Dim prefijo As String
        Select Case tipo
            Case "BOLETA"
                prefijo = "BO-"
            Case "FACTURA"
                prefijo = "FC-"
            Case Else
                Throw New ArgumentException("Seleccione boleta o factura.")
        End Select

        ' El bloqueo dura hasta confirmar o revertir la transacción completa.
        ' Incluye los comprobantes anulados para no reutilizar sus números.
        Dim consulta As String =
            "SELECT COALESCE(MAX(TRY_CONVERT(bigint, SUBSTRING(cNumeroComprobante, 4, 47))), 2899999) " &
            "FROM " & tabla & " WITH (TABLOCKX, HOLDLOCK) " &
            "WHERE LEFT(cNumeroComprobante, 3) = @Prefijo " &
            "AND SUBSTRING(cNumeroComprobante, 4, 47) NOT LIKE '%[^0-9]%'"

        Dim parametros As New List(Of SqlParameter) From {
            New SqlParameter("@Prefijo", SqlDbType.NVarChar, 3) With {.Value = prefijo}
        }
        Dim ultimo As Long = Convert.ToInt64(db.ExecuteScalarTransact(consulta, parametros))
        Dim siguiente As Long = Math.Max(ultimo, 2899999L) + 1L
        Return prefijo & siguiente.ToString(Globalization.CultureInfo.InvariantCulture)
    End Function

End Class
