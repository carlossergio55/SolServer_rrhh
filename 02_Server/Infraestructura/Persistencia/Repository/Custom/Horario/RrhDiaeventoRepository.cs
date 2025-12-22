// Persistencia/Repository/Custom/Persona/RrhDiaeventoRepository.cs
using Aplicacion.DTOs.Horario;
using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces.Repositories.Horario;
using Aplicacion.Wrappers;
using Dominio.Entities.Asistencia;
using Microsoft.EntityFrameworkCore;
using Persistencia.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistencia.Repository.Custom.Horario
{
    public class RrhDiaeventoRepository : IRrhDiaeventoRepository
    {
        private readonly AplicationDbContext _context;

        public RrhDiaeventoRepository(AplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<AsistenciaConsultaDto>> GetAsistenciaPorRango(int? idPersona, DateTime fechaInicio, DateTime fechaFin)
        {
            var response = new Response<AsistenciaConsultaDto>();

            try
            {
                // ============================================================================
                // PASO 1: Obtener horarios programados (SIN DESCANSOS)
                // ============================================================================
                var query = from evento in _context.RrhDiaevento
                            join clasificador in _context.GenClasificadortipo
                                on evento.IdgenClasificadortipo equals clasificador.IdgenClasificadortipo
                            join persona in _context.RrhPersona
                                on evento.IdrrhPersona equals persona.IdrrhPersona
                            where evento.IdgenClasificadortipo != 22
                                  && evento.Fecha >= fechaInicio.Date
                                  && evento.Fecha <= fechaFin.Date
                            select new
                            {
                                evento.IdrrhDiaevento,
                                evento.Fecha,
                                evento.IdrrhPersona,
                                persona.NombreApellido,
                                persona.Ci,
                                evento.IdgenClasificadortipo,
                                clasificador.Descripcion,
                                DiaSemana = evento.Fecha.DayOfWeek.ToString()
                            };

                if (idPersona.HasValue)
                    query = query.Where(x => x.IdrrhPersona == idPersona.Value);

                var eventos = await query.ToListAsync();

                if (!eventos.Any())
                {
                    response.Succeeded = false;
                    response.Message = "No se encontraron registros en el rango especificado";
                    return response;
                }

                // ============================================================================
                // PASO 2: Obtener turnos programados
                // ============================================================================
                var idsClasificador = eventos.Select(e => e.IdgenClasificadortipo).Distinct().ToList();
                var turnos = await _context.RrhhTurnodia
                    .Where(t => idsClasificador.Contains(t.IdgenClasificadortipo))
                    .ToListAsync();

                // ============================================================================
                // PASO 3: Obtener TODAS las justificaciones (sin filtrar por estado)
                // ============================================================================
                var idsDiaevento = eventos.Select(e => e.IdrrhDiaevento).ToList();
                var justificaciones = await _context.RrhJustificacionOmision
                    .Include(j => j.RrhPersonaAprueba)
                    .Where(j => idsDiaevento.Contains(j.IdrrhDiaevento))  // ✅ SIN FILTRO DE ESTADO
                    .ToListAsync();

                var justificacionesDict = justificaciones.ToDictionary(j => j.IdrrhDiaevento);

                // ============================================================================
                // PASO 4: Obtener marcaciones del biométrico
                // ============================================================================
                var cis = eventos.Select(e => e.Ci).Distinct().ToList();
                var userIds = cis.Where(ci => long.TryParse(ci, out _))
                                 .Select(ci => long.Parse(ci))
                                 .ToList();

                var marcaciones = await _context.SAsistencia
                    .Where(a => userIds.Contains(a.UserId) &&
                               a.Timestamp >= fechaInicio.AddDays(-1) &&
                               a.Timestamp <= fechaFin.AddDays(1))
                    .OrderBy(a => a.UserId)
                    .ThenBy(a => a.Timestamp)
                    .ToListAsync();

                // ============================================================================
                // PASO 5: Mapear días de la semana
                // ============================================================================
                var diasSemana = new Dictionary<DayOfWeek, string>
                    {
                        { DayOfWeek.Monday, "Lunes" },
                        { DayOfWeek.Tuesday, "Martes" },
                        { DayOfWeek.Wednesday, "Miércoles" },
                        { DayOfWeek.Thursday, "Jueves" },
                        { DayOfWeek.Friday, "Viernes" },
                        { DayOfWeek.Saturday, "Sábado" },
                        { DayOfWeek.Sunday, "Domingo" }
                    };

                // ============================================================================
                // PASO 6: Construir resultados
                // ============================================================================
                var resultados = eventos.Select(e =>
                {
                    var diaTexto = diasSemana[e.Fecha.DayOfWeek];
                    var turno = turnos.FirstOrDefault(t =>
                        t.IdgenClasificadortipo == e.IdgenClasificadortipo &&
                        t.DiaSemana.Equals(diaTexto, StringComparison.OrdinalIgnoreCase)
                    );

                    string estado = DeterminarEstadoPorClasificador(e.IdgenClasificadortipo);
                    DateTime? marcacionEntrada = null;
                    DateTime? marcacionSalida = null;
                    int? minutosAtraso = null;

                    // Procesar marcaciones para turnos laborales
                    if (EsTurnoLaboral(e.IdgenClasificadortipo))
                    {
                        (marcacionEntrada, marcacionSalida) = BuscarMarcaciones(
                            e.Ci, e.Fecha, turno?.HoraEntrada, turno?.HoraSalida, marcaciones);

                        (estado, minutosAtraso) = CalcularEstadoYAtraso(
                            turno?.HoraEntrada, marcacionEntrada, marcacionSalida);
                    }

                    // ✅ Buscar justificación (cualquier estado: SOLICITADO, APROBADO, RECHAZADO)
                    justificacionesDict.TryGetValue(e.IdrrhDiaevento, out var justificacion);

                    // ✅ Solo cambia a JUSTIFICADO si la justificación está APROBADA
                    if (justificacion != null &&
                        justificacion.Estado == "APROBADO" &&
                        (estado == "FALTA" || estado == "OMISION_ENTRADA" || estado == "OMISION_SALIDA"))
                    {
                        estado = "JUSTIFICADO";
                    }

                    return new AsistenciaDiaDto
                    {
                        IdrrhDiaevento = e.IdrrhDiaevento,
                        Fecha = e.Fecha,
                        Dia = diaTexto,
                        IdPersona = e.IdrrhPersona,
                        Ci = e.Ci,
                        NombrePersona = e.NombreApellido,
                        Turno = e.Descripcion,
                        HoraEntradaProgramada = turno?.HoraEntrada,
                        HoraSalidaProgramada = turno?.HoraSalida,
                        MarcacionEntrada = marcacionEntrada,
                        MarcacionSalida = marcacionSalida,
                        MinutosAtraso = minutosAtraso,
                        Estado = estado,
                        // ✅ SIEMPRE llenar estos campos (incluso si está SOLICITADO o RECHAZADO)
                        IdJustificacion = justificacion?.IdrrhJustificacion,
                        TipoJustificacion = justificacion?.TipoOmision,
                        EstadoJustificacion = justificacion?.Estado,
                        JustificacionAprobadaPor = justificacion?.RrhPersonaAprueba?.NombreApellido
                    };
                })
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.IdPersona)
                .ToList();

                // ============================================================================
                // PASO 7: Calcular resúmenes
                // ============================================================================
                var resumenPorPersona = CalcularResumenPorPersona(resultados);

                response.Succeeded = true;
                response.Data = new AsistenciaConsultaDto
                {
                    Parametros = new AsistenciaParametrosDto
                    {
                        IdPersona = idPersona,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin
                    },
                    Resultados = resultados,
                    ResumenPorPersona = resumenPorPersona
                };
                response.Message = $"Se encontraron {resultados.Count} registros para {resumenPorPersona.Count} persona(s)";
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        // ============================================================================
        // MÉTODOS AUXILIARES
        // ============================================================================

        /// <summary>
        /// Determina si es un turno laboral (15-18)
        /// </summary>
        private bool EsTurnoLaboral(int idClasificador)
        {
            return new[] { 15, 16, 17, 18 }.Contains(idClasificador);
        }

        /// <summary>
        /// Determina el estado según el tipo de clasificador
        /// </summary>
        private string DeterminarEstadoPorClasificador(int idClasificador)
        {
            return idClasificador switch
            {
                24 => "VACACIONES",
                25 => "BAJA_MEDICA",
                26 => "COMISION",
                27 => "PERMISO_SIN_GOCE",
                23 => "FALTA",
                28 => "FERIADO",
                _ => "PENDIENTE" // Se calculará después con marcaciones
            };
        }

        /// <summary>
        /// Calcula el estado y minutos de atraso basado en la marcación
        /// </summary>
        private (string estado, int? minutosAtraso) CalcularEstadoYAtraso(
    TimeSpan? horaEntradaProgramada,
    DateTime? marcacionEntrada,
    DateTime? marcacionSalida)  // ✅ NUEVO parámetro
        {
            // Sin turno programado
            if (!horaEntradaProgramada.HasValue)
                return ("SIN_TURNO", null);

            // ✅ NUEVO: No marcó ni entrada ni salida = FALTA
            if (!marcacionEntrada.HasValue && !marcacionSalida.HasValue)
                return ("FALTA", null);

            // No marcó entrada (pero sí salida)
            if (!marcacionEntrada.HasValue)
                return ("OMISION_ENTRADA", null);

            // Calcular diferencia
            var horaRealEntrada = marcacionEntrada.Value.TimeOfDay;
            var diferencia = horaRealEntrada - horaEntradaProgramada.Value;
            var minutosTotal = (int)diferencia.TotalMinutes;

            // Llegó temprano o a tiempo (dentro de 5 min tolerancia)
            if (minutosTotal <= 5)
                return ("A_TIEMPO", null);

            // Restar los 5 minutos de tolerancia
            var minutosAtrasoReal = minutosTotal - 5;

            // Atraso entre 6-29 minutos (1-24 minutos después de tolerancia)
            if (minutosTotal >= 6 && minutosTotal <= 29)
                return ("ATRASO", minutosAtrasoReal);

            // Atraso de 30+ minutos = INASISTENCIA
            return ("INASISTENCIA", null);
        }

        /// <summary>
        /// Busca marcaciones de entrada y salida
        /// </summary>
        private (DateTime? entrada, DateTime? salida) BuscarMarcaciones(
            string ci,
            DateTime fecha,
            TimeSpan? horaEntradaProgramada,
            TimeSpan? horaSalidaProgramada,
            List<SAsistencia> todasLasMarcaciones)
        {
            if (!horaEntradaProgramada.HasValue || !horaSalidaProgramada.HasValue)
                return (null, null);

            if (!long.TryParse(ci, out var userId))
                return (null, null);

            var marcacionesPersona = todasLasMarcaciones
                .Where(m => m.UserId == userId)
                .ToList();

            // ============================================================================
            // TURNO DIURNO (salida > entrada)
            // ============================================================================
            if (horaSalidaProgramada.Value > horaEntradaProgramada.Value)
            {
                var ventanaEntrada = fecha.Date.Add(horaEntradaProgramada.Value);
                var marcacionEntrada = marcacionesPersona
                    .Where(m => m.Timestamp.Date == fecha.Date &&
                               Math.Abs((m.Timestamp - ventanaEntrada).TotalHours) <= 4)
                    .OrderBy(m => Math.Abs((m.Timestamp - ventanaEntrada).Ticks))
                    .FirstOrDefault();

                DateTime? marcacionSalida = null;
                if (marcacionEntrada != null)
                {
                    var ventanaSalida = fecha.Date.Add(horaSalidaProgramada.Value);
                    marcacionSalida = marcacionesPersona
                        .Where(m => m.Timestamp > marcacionEntrada.Timestamp &&
                                   Math.Abs((m.Timestamp - ventanaSalida).TotalHours) <= 4)
                        .OrderBy(m => Math.Abs((m.Timestamp - ventanaSalida).Ticks))
                        .FirstOrDefault()
                        ?.Timestamp;
                }

                return (marcacionEntrada?.Timestamp, marcacionSalida);
            }
            // ============================================================================
            // TURNO NOCTURNO (salida < entrada, cruza medianoche)
            // ============================================================================
            else
            {
                var ventanaEntrada = fecha.Date.Add(horaEntradaProgramada.Value);
                var marcacionEntrada = marcacionesPersona
                    .Where(m => m.Timestamp.Date == fecha.Date &&
                               Math.Abs((m.Timestamp - ventanaEntrada).TotalHours) <= 4)
                    .OrderBy(m => Math.Abs((m.Timestamp - ventanaEntrada).Ticks))
                    .FirstOrDefault();

                DateTime? marcacionSalida = null;
                if (marcacionEntrada != null)
                {
                    var fechaSalida = fecha.AddDays(1);
                    var ventanaSalida = fechaSalida.Date.Add(horaSalidaProgramada.Value);
                    marcacionSalida = marcacionesPersona
                        .Where(m => m.Timestamp.Date == fechaSalida.Date &&
                                   Math.Abs((m.Timestamp - ventanaSalida).TotalHours) <= 4)
                        .OrderBy(m => Math.Abs((m.Timestamp - ventanaSalida).Ticks))
                        .FirstOrDefault()
                        ?.Timestamp;
                }

                return (marcacionEntrada?.Timestamp, marcacionSalida);
            }
        }

        /// <summary>
        /// Calcula el resumen mensual por persona con sanciones
        /// </summary>
        private List<ResumenPersonaDto> CalcularResumenPorPersona(List<AsistenciaDiaDto> resultados)
        {
            var resumen = resultados
                .GroupBy(r => new { r.IdPersona, r.Ci, r.NombrePersona })
                .Select(g =>
                {
                    // Acumular minutos de atraso (solo estado ATRASO)
                    var minutosAtrasoAcumulados = g
                        .Where(r => r.Estado == "ATRASO" && r.MinutosAtraso.HasValue)
                        .Sum(r => r.MinutosAtraso.Value);

                    // ✅ Contar inasistencias (incluye FALTA)
                    var diasInasistencia = g.Count(r => r.Estado == "INASISTENCIA" || r.Estado == "FALTA");

                    // Contar omisiones
                    var omisionesEntrada = g.Count(r => r.Estado == "OMISION_ENTRADA");
                    var omisionesSalida = g.Count(r =>
                        r.MarcacionEntrada.HasValue &&
                        !r.MarcacionSalida.HasValue &&
                        r.Estado != "OMISION_ENTRADA" &&
                        r.Estado != "FALTA");  // ✅ Excluir FALTAs

                    // Calcular sanciones
                    var sancionAtrasos = CalcularSancionPorAtrasos(minutosAtrasoAcumulados);
                    var sancionInasistencias = CalcularSancionPorInasistencias(diasInasistencia);
                    var sancionOmisiones = CalcularSancionPorOmisiones(omisionesEntrada + omisionesSalida);

                    return new ResumenPersonaDto
                    {
                        IdPersona = g.Key.IdPersona,
                        NombrePersona = g.Key.NombrePersona,
                        Ci = g.Key.Ci,
                        MinutosAtrasoAcumulados = minutosAtrasoAcumulados,
                        DiasInasistencia = diasInasistencia,
                        OmisionesEntrada = omisionesEntrada,
                        OmisionesSalida = omisionesSalida,
                        DiasSancionPorAtrasos = sancionAtrasos,
                        DiasSancionPorInasistencias = sancionInasistencias,
                        DiasSancionPorOmisiones = sancionOmisiones,
                        TotalDiasSancion = sancionAtrasos + sancionInasistencias + sancionOmisiones
                    };
                })
                .ToList();

            return resumen;
        }

        /// <summary>
        /// Calcula sanción por atrasos acumulados (ARTÍCULO 45.I)
        /// </summary>
        private decimal CalcularSancionPorAtrasos(int minutosAcumulados)
        {
            return minutosAcumulados switch
            {
                <= 30 => 0m,
                <= 45 => 0.5m,
                <= 60 => 1m,
                <= 90 => 2m,
                <= 120 => 3m,
                _ => 4m
            };
        }

        /// <summary>
        /// Calcula sanción por inasistencias (ARTÍCULO 45.II)
        /// </summary>
        private decimal CalcularSancionPorInasistencias(int diasInasistencia)
        {
            // Cada día de inasistencia = 2 días de sanción
            return diasInasistencia * 2m;
        }

        /// <summary>
        /// Calcula sanción por omisiones (ARTÍCULO 45.III)
        /// </summary>
        private decimal CalcularSancionPorOmisiones(int totalOmisiones)
        {
            return totalOmisiones switch
            {
                0 => 0m,
                1 => 0.5m,  // Primera vez
                2 => 1.5m,  // 0.5 + 1.0
                _ => 1.5m + ((totalOmisiones - 2) * 2m)  // Tercera vez en adelante = 2 días c/u
            };
        }
    }
}