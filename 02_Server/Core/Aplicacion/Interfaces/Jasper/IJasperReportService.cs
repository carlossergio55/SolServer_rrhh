using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces.Jasper
{
    public interface IJasperReportService
    {
        /// <summary>
        /// Genera un reporte PDF llamando al servidor Jasper
        /// </summary>
        /// <param name="tipoReporte">Tipo de reporte a generar (ejemplo: JUSTIFICACIONES_POR_FECHA)</param>
        /// <param name="parametrosJson">Parámetros en formato JSON</param>
        /// <param name="idReporte">ID del registro rrh_reporte para nombrar el archivo</param>
        /// <returns>Tupla con: Success, RutaRelativa del archivo, ErrorMessage</returns>
        Task<(bool Success, string RutaRelativa, string ErrorMessage)> GenerarReportePdfAsync(
            string tipoReporte,
            string parametrosJson,
            long idReporte
        );
    }
}
