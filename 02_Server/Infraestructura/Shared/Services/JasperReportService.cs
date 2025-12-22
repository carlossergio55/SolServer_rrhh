using Aplicacion.Interfaces;
using Aplicacion.Interfaces.Jasper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class JasperReportService : IJasperReportService
    {
        private readonly HttpClient _httpClient;
        private readonly ReportesSettings _settings;
        private readonly ILogger<JasperReportService> _logger;

        // Mapeo de TipoReporte -> Ruta en Jasper
        private readonly Dictionary<string, string> _reportePaths = new Dictionary<string, string>
        {
            { "JUSTIFICACIONES_POR_FECHA", "Reports/ENVIBOL/RRHH/Justificaciones.pdf" },
            { "ASISTENCIAS_MENSUAL", "Reports/ENVIBOL/RRHH/AsistenciasMensual.pdf" },
            { "DEPRECIACION", "Reports/ENVIBOL/Depresiacion/Depresiacion.pdf" }
            // 👉 Agregar más reportes aquí conforme los necesites
        };

        public JasperReportService(
            HttpClient httpClient,
            IOptions<ReportesSettings> settings,
            ILogger<JasperReportService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<(bool Success, string RutaRelativa, string ErrorMessage)> GenerarReportePdfAsync(
            string tipoReporte,
            string parametrosJson,
            long idReporte)
        {
            try
            {
                _logger.LogInformation("Iniciando generación de reporte {TipoReporte} con ID {IdReporte}", tipoReporte, idReporte);

                // 1️⃣ Validar que el tipo de reporte existe
                if (!_reportePaths.ContainsKey(tipoReporte))
                {
                    var error = $"Tipo de reporte '{tipoReporte}' no está configurado en el mapeo de rutas.";
                    _logger.LogError(error);
                    return (false, null, error);
                }

                // 2️⃣ Construir URL completa de Jasper
                var jasperPath = _reportePaths[tipoReporte];
                var urlCompleta = ConstruirUrlJasper(jasperPath, parametrosJson);

                _logger.LogDebug("URL Jasper: {Url}", urlCompleta.Replace(_settings.Password, "***"));

                // 3️⃣ Descargar PDF desde Jasper
                var response = await _httpClient.GetAsync(urlCompleta);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = $"Jasper retornó error {response.StatusCode}: {errorContent}";
                    _logger.LogError(error);
                    return (false, null, error);
                }

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    var error = "El PDF generado está vacío";
                    _logger.LogError(error);
                    return (false, null, error);
                }

                // 4️⃣ Guardar archivo en disco
                var rutaRelativa = GuardarArchivo(pdfBytes, tipoReporte, idReporte);

                _logger.LogInformation("Reporte generado exitosamente: {Ruta}", rutaRelativa);

                return (true, rutaRelativa, null);
            }
            catch (HttpRequestException ex)
            {
                var error = $"Error de conexión con Jasper: {ex.Message}";
                _logger.LogError(ex, error);
                return (false, null, error);
            }
            catch (Exception ex)
            {
                var error = $"Error inesperado al generar reporte: {ex.Message}";
                _logger.LogError(ex, error);
                return (false, null, error);
            }
        }

        /// <summary>
        /// Construye la URL completa para llamar a Jasper con credenciales y parámetros
        /// </summary>
        private string ConstruirUrlJasper(string jasperPath, string parametrosJson)
        {
            // URL base: https://jasper.envibol.site:8443/jasperserver/rest_v2/reports/Reports/ENVIBOL/...
            var url = $"{_settings.JasperUrl}/{jasperPath}";
            url += $"?j_username={_settings.Usuario}&j_password={_settings.Password}";

            // Parsear parámetros JSON y agregarlos a la URL
            if (!string.IsNullOrWhiteSpace(parametrosJson))
            {
                var parametros = JsonSerializer.Deserialize<Dictionary<string, object>>(parametrosJson);
                foreach (var param in parametros)
                {
                    url += $"&{param.Key}={param.Value}";
                }
            }

            return url;
        }

        /// <summary>
        /// Guarda el PDF en disco y retorna la ruta relativa
        /// </summary>
        private string GuardarArchivo(byte[] pdfBytes, string tipoReporte, long idReporte)
        {
            // Crear estructura de carpetas: BasePath/YYYY/MM/
            var ahora = DateTime.Now;
            var carpetaMes = Path.Combine(_settings.BasePath, ahora.Year.ToString(), ahora.Month.ToString("D2"));

            if (!Directory.Exists(carpetaMes))
            {
                Directory.CreateDirectory(carpetaMes);
            }

            // Nombre del archivo: reporte_{id}_{tipo}_{timestamp}.pdf
            var nombreArchivo = $"reporte_{idReporte}_{tipoReporte}_{ahora:yyyyMMdd_HHmmss}.pdf";
            var rutaCompleta = Path.Combine(carpetaMes, nombreArchivo);

            // Guardar archivo
            File.WriteAllBytes(rutaCompleta, pdfBytes);

            // Retornar ruta relativa (sin BasePath)
            var rutaRelativa = Path.Combine(ahora.Year.ToString(), ahora.Month.ToString("D2"), nombreArchivo);

            // Normalizar separadores para Linux/Windows
            return rutaRelativa.Replace("\\", "/");
        }
    }
}