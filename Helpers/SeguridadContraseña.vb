Imports System.Security.Cryptography

Namespace WebApplication_Keyove.Helpers

    Public NotInheritable Class SeguridadContrasena

        Private Const Iteraciones As Integer = 100000
        Private Const TamanoSal As Integer = 16
        Private Const TamanoHash As Integer = 32

        Private Sub New()
        End Sub

        Public Shared Function CrearHash(
            contrasena As String
        ) As String

            If String.IsNullOrWhiteSpace(contrasena) Then
                Throw New ArgumentException(
                    "La contraseña es obligatoria."
                )
            End If

            Dim sal(TamanoSal - 1) As Byte

            Using generador As RandomNumberGenerator =
                RandomNumberGenerator.Create()

                generador.GetBytes(sal)

            End Using

            Dim hash As Byte()

            Using derivador As New Rfc2898DeriveBytes(
                contrasena,
                sal,
                Iteraciones
            )

                hash = derivador.GetBytes(TamanoHash)

            End Using

            Return Iteraciones.ToString() & ":" &
                   Convert.ToBase64String(sal) & ":" &
                   Convert.ToBase64String(hash)

        End Function

        Public Shared Function Verificar(
            contrasena As String,
            hashAlmacenado As String
        ) As Boolean

            If String.IsNullOrWhiteSpace(contrasena) OrElse
               String.IsNullOrWhiteSpace(hashAlmacenado) Then

                Return False

            End If

            Try

                Dim partes() As String =
                    hashAlmacenado.Split(":"c)

                If partes.Length <> 3 Then
                    Return False
                End If

                Dim iteraciones As Integer

                If Not Integer.TryParse(
                    partes(0),
                    iteraciones
                ) Then
                    Return False
                End If

                Dim sal As Byte() =
                    Convert.FromBase64String(partes(1))

                Dim hashGuardado As Byte() =
                    Convert.FromBase64String(partes(2))

                Dim hashIngresado As Byte()

                Using derivador As New Rfc2898DeriveBytes(
                    contrasena,
                    sal,
                    iteraciones
                )

                    hashIngresado =
                        derivador.GetBytes(hashGuardado.Length)

                End Using

                Return CompararSeguro(
                    hashGuardado,
                    hashIngresado
                )

            Catch
                Return False
            End Try

        End Function

        Private Shared Function CompararSeguro(
            valor1 As Byte(),
            valor2 As Byte()
        ) As Boolean

            If valor1.Length <> valor2.Length Then
                Return False
            End If

            Dim diferencia As Integer = 0

            For indice As Integer = 0 To valor1.Length - 1
                diferencia =
                    diferencia Or
                    (valor1(indice) Xor valor2(indice))
            Next

            Return diferencia = 0

        End Function

    End Class

End Namespace