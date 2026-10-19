using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WebApplication_keyove;

// Ejecutar con el directorio del proyecto. Sólo detiene el proceso creado por esta prueba.
class OcrStartupIntegration {
    static int Pid() {
        using (var client = new WebClient { Proxy = null }) {
            var data = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(client.DownloadString("http://127.0.0.1:5000/health"));
            if (Convert.ToString(data["service"]) != "keyove-compras-ocr") throw new Exception("Servicio inesperado.");
            return Convert.ToInt32(data["pid"]);
        }
    }
    static void Main(string[] args) {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings.Remove("OcrAutoStart");
        config.AppSettings.Settings.Add("OcrAutoStart", "true");
        config.AppSettings.Settings.Remove("OcrServiceUrl");
        config.AppSettings.Settings.Add("OcrServiceUrl", "http://127.0.0.1:5000/ocr/extraer");
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
        bool existed;
        try { Pid(); existed = true; } catch (WebException) { existed = false; }
        Task.WaitAll(Task.Run(() => OcrServiceHost.EnsureRunning(args[0], true)), Task.Run(() => OcrServiceHost.EnsureRunning(args[0], true)));
        int first = Pid();
        OcrServiceHost.EnsureRunning(args[0], true);
        if (Pid() != first) throw new Exception("Se duplicó el proceso OCR.");
        Console.WriteLine("OK: arranque concurrente y reutilización del servicio OCR.");
        if (!existed) {
            using (var owned = Process.GetProcessById(first)) { owned.Kill(); owned.WaitForExit(); }
            OcrServiceHost.EnsureRunning(args[0], true);
            if (Pid() == first) throw new Exception("No se reinició OCR.");
            Console.WriteLine("OK: recuperación automática después de detener el proceso de prueba.");
        }
        Console.WriteLine("OCR listo para usar desde la aplicación.");
    }
}
