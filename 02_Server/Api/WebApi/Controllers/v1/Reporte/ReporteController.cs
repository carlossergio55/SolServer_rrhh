using Aplicacion.DTOs.Vistas;
using Aplicacion.Features.Reporte.Commands;
using Aplicacion.Features.Reporte.Queries;
using Aplicacion.Interfaces;
using Aplicacion.Interfaces.Jasper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using System.IO;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Reporte
{
    [ApiVersion("1.0")]
    [Authorize]
    public class ReporteController : BaseApiController
    {
        private readonly IJasperReportService _jasperService;
        private readonly ReportesSettings _reportesSettings;

        public ReporteController(
            IJasperReportService jasperService,
            IOptions<ReportesSettings> reportesSettings)
        {
            _jasperService = jasperService;
            _reportesSettings = reportesSettings.Value;
        }

        /// <summary>
        /// Listar reportes con filtros opcionales
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string tipoReporte, [FromQuery] string estado)
        {
            return Ok(await Mediator.Send(new GetRrhReporteQuery
            {
                TipoReporte = tipoReporte,
                Estado = estado
            }));
        }

        /// <summary>
        /// Crear solicitud de reporte (queda en estado PENDIENTE)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRrhReporteCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Actualizar estado de un reporte (uso interno del BackgroundService)
        /// </summary>
        [HttpPatch("{id}/Estado")]
        public async Task<IActionResult> UpdateEstado(long id, [FromBody] UpdateRrhReporteCommand command)
        {
            if (id != command.IdrrhReporte)
                return BadRequest("Id no coincide");

            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// 🧪 TEST - Generar reporte manualmente (sin registrar en BD)
        /// </summary>
        [HttpPost("TestJasper")]
        public async Task<IActionResult> TestGenerarReporte([FromBody] TestReporteRequest request)
        {
            var resultado = await _jasperService.GenerarReportePdfAsync(
                request.TipoReporte,
                request.ParametrosJson,
                request.IdReporte
            );

            if (resultado.Success)
            {
                return Ok(new
                {
                    exito = true,
                    mensaje = "Reporte generado correctamente",
                    rutaArchivo = resultado.RutaRelativa,
                    idReporte = request.IdReporte
                });
            }
            else
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Error al generar reporte",
                    error = resultado.ErrorMessage
                });
            }
        }

        /// <summary>
        /// 📥 Descargar PDF de un reporte generado
        /// </summary>
        [HttpGet("{id}/Descargar")]
        public async Task<IActionResult> DescargarReporte(int id)
        {
            // 1️⃣ Obtener el reporte por ID
            var query = new GetRrhReporteByIdQuery { Id = id };
            var response = await Mediator.Send(query);

            if (!response.Succeeded || response.Data == null)
            {
                return NotFound(new
                {
                    mensaje = "Reporte no encontrado",
                    idReporte = id
                });
            }

            var reporte = response.Data;

            // 2️⃣ Validar que el reporte esté generado
            if (reporte.Estado != "GENERADO")
            {
                return BadRequest(new
                {
                    mensaje = "El reporte aún no está disponible para descarga",
                    estado = reporte.Estado,
                    idReporte = id
                });
            }

            // 3️⃣ Validar que tenga ruta de archivo
            if (string.IsNullOrEmpty(reporte.RutaArchivo))
            {
                return NotFound(new
                {
                    mensaje = "El reporte no tiene archivo asociado",
                    idReporte = id
                });
            }

            // 4️⃣ Construir ruta completa del archivo
            var rutaCompleta = Path.Combine(_reportesSettings.BasePath, reporte.RutaArchivo);

            // 5️⃣ Verificar que el archivo existe en disco
            if (!System.IO.File.Exists(rutaCompleta))
            {
                return NotFound(new
                {
                    mensaje = "El archivo del reporte no se encontró en el servidor",
                    rutaEsperada = reporte.RutaArchivo,
                    idReporte = id
                });
            }

            // 6️⃣ Leer el archivo y devolverlo
            var bytes = await System.IO.File.ReadAllBytesAsync(rutaCompleta);
            var nombreArchivo = Path.GetFileName(rutaCompleta);

            // 7️⃣ Retornar el PDF para descarga
            return File(bytes, "application/pdf", nombreArchivo);
        }
    }

}