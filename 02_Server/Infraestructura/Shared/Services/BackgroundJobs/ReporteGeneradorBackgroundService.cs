using Aplicacion.Features.Reporte.Commands;
using Aplicacion.Features.Reporte.Queries;
using Aplicacion.Interfaces;
using Aplicacion.Interfaces.Jasper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Services.BackgroundJobs
{
    public class ReporteGeneradorBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReporteGeneradorBackgroundService> _logger;
        private readonly TimeSpan _intervaloEjecucion = TimeSpan.FromSeconds(30);

        public ReporteGeneradorBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<ReporteGeneradorBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Servicio generador de reportes iniciado");

            // Esperar 10 segundos antes de iniciar
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcesarReportesPendientes(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en el servicio generador de reportes");
                }

                await Task.Delay(_intervaloEjecucion, stoppingToken);
            }

            _logger.LogInformation("🛑 Servicio generador de reportes detenido");
        }

        private async Task ProcesarReportesPendientes(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var jasperService = scope.ServiceProvider.GetRequiredService<IJasperReportService>();

            // Obtener reportes pendientes
            var query = new GetRrhReporteQuery
            {
                Estado = "PENDIENTE"
            };

            var response = await mediator.Send(query, stoppingToken);

            if (!response.Succeeded || response.Data == null || !response.Data.Any())
            {
                return;
            }

            // ✅ Convertir a int para evitar problemas con dynamic
            var count = response.Data.Count();
            _logger.LogInformation("📋 Se encontraron {Count} reporte(s) pendiente(s)", count);

            foreach (var reporte in response.Data)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                await ProcesarReporte(mediator, jasperService, reporte, stoppingToken);
            }
        }

        private async Task ProcesarReporte(
     IMediator mediator,
     IJasperReportService jasperService,
     Aplicacion.DTOs.Vistas.RrhReporteDto reporte,
     CancellationToken stoppingToken)
        {
            var idReporte = reporte.IdrrhReporte; // Es int, no long
            var tipoReporte = reporte.TipoReporte;

            try
            {
                _logger.LogInformation("⚙️ Procesando reporte {Id} - {Tipo}", idReporte, tipoReporte);

                // Generar el PDF - ✅ Convertir int a long para el servicio
                var resultado = await jasperService.GenerarReportePdfAsync(
                    tipoReporte,
                    reporte.Parametros,
                    (long)idReporte // ✅ Cast explícito
                );

                if (resultado.Success)
                {
                    var updateCommand = new UpdateRrhReporteEstadoCommand
                    {
                        IdrrhReporte = idReporte, // ✅ Ya es int
                        Estado = "GENERADO",
                        RutaArchivo = resultado.RutaRelativa,
                        FechaGeneracion = DateTime.Now
                    };

                    await mediator.Send(updateCommand, stoppingToken);

                    _logger.LogInformation("✅ Reporte {Id} generado exitosamente: {Ruta}",
                        idReporte, resultado.RutaRelativa);
                }
                else
                {
                    var updateCommand = new UpdateRrhReporteEstadoCommand
                    {
                        IdrrhReporte = idReporte, // ✅ Ya es int
                        Estado = "ERROR",
                        RutaArchivo = $"ERROR: {resultado.ErrorMessage}",
                        FechaGeneracion = DateTime.Now
                    };

                    await mediator.Send(updateCommand, stoppingToken);

                    _logger.LogError("❌ Error generando reporte {Id}: {Error}",
                        idReporte, resultado.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Excepción procesando reporte {Id}", idReporte);

                try
                {
                    var updateCommand = new UpdateRrhReporteEstadoCommand
                    {
                        IdrrhReporte = idReporte, // ✅ Ya es int
                        Estado = "ERROR",
                        RutaArchivo = $"EXCEPTION: {ex.Message}",
                        FechaGeneracion = DateTime.Now
                    };

                    await mediator.Send(updateCommand, stoppingToken);
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "❌ No se pudo actualizar estado de error para reporte {Id}", idReporte);
                }
            }
        }
    }
}