Imports System.Diagnostics
Imports System.IO
Imports System.Net
Imports System.Threading
Imports System.Web.Script.Serialization

' Arranque local de desarrollo. No instala dependencias ni modifica SQL Server.
Public NotInheritable Class OcrServiceHost
    Private Shared ReadOnly puerta As New Object()
    Private Shared proceso As Process

    Public Shared Sub EnsureRunning(raiz As String, desarrollo As Boolean)
        If Not desarrollo OrElse Not String.Equals(System.Configuration.ConfigurationManager.AppSettings("OcrAutoStart"), "true", StringComparison.OrdinalIgnoreCase) Then Return
        Dim endpoint As New Uri(System.Configuration.ConfigurationManager.AppSettings("OcrServiceUrl"))
        If endpoint.Scheme <> "http" OrElse endpoint.Host <> "127.0.0.1" OrElse endpoint.Port <> 5000 Then
            Throw New InvalidOperationException("El inicio automático OCR requiere http://127.0.0.1:5000/ocr/extraer.")
        End If
        SyncLock puerta
            If Disponible() Then Return
            Dim carpeta = Path.Combine(raiz, "servicio_ocr")
            Dim python = Path.Combine(carpeta, ".venv", "Scripts", "python.exe")
            If Not File.Exists(python) Then Throw New InvalidOperationException("Falta preparar OCR en este equipo. Ejecuta servicio_ocr/setup.ps1 una sola vez e instala Tesseract con español.")
            If proceso Is Nothing OrElse proceso.HasExited Then
                If proceso IsNot Nothing Then proceso.Dispose()
                Dim inicio As New ProcessStartInfo(python, "-u app.py") With {
                    .WorkingDirectory = carpeta, .UseShellExecute = False,
                    .CreateNoWindow = True, .WindowStyle = ProcessWindowStyle.Hidden}
                proceso = Process.Start(inicio)
            End If
            Dim tiempo = Stopwatch.StartNew()
            Do
                If Disponible() Then Return
                If proceso.HasExited Then Throw New InvalidOperationException("OCR no pudo iniciarse. Revisa servicio_ocr/.runtime/auto-start.log y las dependencias de Python y Tesseract.")
                Thread.Sleep(250)
            Loop While tiempo.Elapsed < TimeSpan.FromSeconds(25)
            Throw New InvalidOperationException("OCR sigue iniciándose. Espera unos segundos y vuelve a extraer el comprobante. Revisa servicio_ocr/.runtime/auto-start.log si persiste.")
        End SyncLock
    End Sub

    Private Shared Function Disponible() As Boolean
        Try
            Dim solicitud = DirectCast(WebRequest.Create("http://127.0.0.1:5000/health"), HttpWebRequest)
            solicitud.Proxy = Nothing
            solicitud.Timeout = 750
            solicitud.ReadWriteTimeout = 750
            Using respuesta = solicitud.GetResponse(), lector As New StreamReader(respuesta.GetResponseStream())
                Dim datos = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(lector.ReadToEnd())
                Return datos.ContainsKey("service") AndAlso Convert.ToString(datos("service")) = "keyove-compras-ocr" AndAlso Convert.ToString(datos("status")) = "ok"
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class
