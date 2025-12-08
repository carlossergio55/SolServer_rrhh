using Aplicacion.DTOs.Persona;
using Aplicacion.Features.Diaevento.Queries;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using Dominio.Entities.Horario;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Diaevento.Commands
{
    public class VerificarRangoCommand : IRequest<Response<VerificarRangoDto>>
    {
        public int IdPersona { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
    public class VerificarRangoCommandHandler :
       IRequestHandler<VerificarRangoCommand, Response<VerificarRangoDto>>
    {
        private readonly IRepositoryAsync<RrhDiaevento> _repoDiaevento;
        private readonly IRepositoryAsync<GenGrupoturnoDetalle> _repoDetalle;

        public VerificarRangoCommandHandler(
            IRepositoryAsync<RrhDiaevento> repoDiaevento,
            IRepositoryAsync<GenGrupoturnoDetalle> repoDetalle)
        {
            _repoDiaevento = repoDiaevento;
            _repoDetalle = repoDetalle;
        }

        public async Task<Response<VerificarRangoDto>> Handle(
            VerificarRangoCommand request,
            CancellationToken ct)
        {
            // Buscar registros en el rango
            var spec = new RrhDiaeventoByRangoSpecification(
                request.IdPersona,
                request.FechaInicio,
                request.FechaFin);

            var registros = await _repoDiaevento.ListAsync(spec, ct);

            // Si no hay registros
            if (!registros.Any())
            {
                return new Response<VerificarRangoDto>(new VerificarRangoDto
                {
                    ExistenTurnos = false,
                    CantidadRegistros = 0,
                    PrimeraFechaExistente = null,
                    UltimaFechaExistente = null,
                    FechaSugerida = request.FechaInicio,
                    CicloIncompleto = false
                });
            }

            // Información básica
            var primeraFecha = registros.Min(r => r.Fecha);
            var ultimaFecha = registros.Max(r => r.Fecha);
            var ultimoRegistro = registros.OrderByDescending(r => r.Fecha).First();

            var dto = new VerificarRangoDto
            {
                ExistenTurnos = true,
                CantidadRegistros = registros.Count,
                PrimeraFechaExistente = primeraFecha,
                UltimaFechaExistente = ultimaFecha,
                FechaSugerida = ultimaFecha.AddDays(1),
                IdgenClasificadortipoActual = ultimoRegistro.IdgenClasificadortipo,
                DescripcionTurnoActual = ultimoRegistro.GenClasificadortipo?.Descripcion
            };

            // ✅ VERIFICAR SI ES UN CICLO ROTATIVO INCOMPLETO
            await VerificarCicloIncompleto(ultimoRegistro, dto, ct);

            return new Response<VerificarRangoDto>(dto);
        }

        private async Task VerificarCicloIncompleto(
     RrhDiaevento ultimoRegistro,
     VerificarRangoDto dto,
     CancellationToken ct)
        {
            // ✅ Buscar últimos turnos (hasta 30 días atrás)
            var spec = new UltimosTurnosPersonaSpecification(
                ultimoRegistro.IdrrhPersona,
                ultimoRegistro.Fecha,
                30);

            var ultimosTurnos = await _repoDiaevento.ListAsync(spec, ct);
            var turnosOrdenados = ultimosTurnos.OrderByDescending(t => t.Fecha).ToList();

            // ========================================
            // CASO 1: ÚLTIMO REGISTRO ES DESCANSO
            // ========================================
            if (ultimoRegistro.IdgenClasificadortipo == 22)
            {
                // Buscar el último turno laboral antes del descanso
                var ultimoTurnoLaboral = turnosOrdenados
                    .FirstOrDefault(t => t.IdgenClasificadortipo != 22);

                if (ultimoTurnoLaboral == null)
                {
                    dto.CicloIncompleto = false;
                    return;
                }

                // Buscar grupo del turno laboral
                var detalles = await _repoDetalle.ListAsync(
                    new DetalleByClasificadorSpecification(ultimoTurnoLaboral.IdgenClasificadortipo),
                    ct);

                var detalleActual = detalles.FirstOrDefault();
                if (detalleActual == null || detalleActual.GenGrupoturno == null)
                {
                    dto.CicloIncompleto = false;
                    return;
                }

                var grupo = detalleActual.GenGrupoturno;
                if (grupo.ModoGeneracion != "ROTATIVO")
                {
                    dto.CicloIncompleto = false;
                    return;
                }

                // ✅ Contar días de DESCANSO consecutivos hacia atrás
                int diasDescansoConsecutivos = 0;
                foreach (var turno in turnosOrdenados)
                {
                    if (turno.IdgenClasificadortipo == 22)
                    {
                        diasDescansoConsecutivos++;
                    }
                    else
                    {
                        break; // Parar al encontrar un turno laboral
                    }
                }

                // Verificar si faltan días de descanso
                var diasDescansoRequeridos = grupo.DiasDescanso;
                if (diasDescansoConsecutivos < diasDescansoRequeridos)
                {
                    dto.CicloIncompleto = true;
                    dto.DiasCompletados = diasDescansoConsecutivos;
                    dto.DiasFaltantes = diasDescansoRequeridos - diasDescansoConsecutivos;
                    dto.OrdenActualEnCiclo = detalleActual.Orden;
                    dto.IdgenClasificadortipoActual = 22; // DESCANSO
                    dto.DescripcionTurnoActual = "DESCANSO";
                }
                else
                {
                    dto.CicloIncompleto = false;
                }

                return;
            }

            // ========================================
            // CASO 2: ÚLTIMO REGISTRO ES TURNO LABORAL
            // ========================================
            var ultimoTurnoLaboralDirecto = ultimoRegistro;

            // Buscar grupo del turno
            var detallesLaboral = await _repoDetalle.ListAsync(
                new DetalleByClasificadorSpecification(ultimoTurnoLaboralDirecto.IdgenClasificadortipo),
                ct);

            var detalleLaboralActual = detallesLaboral.FirstOrDefault();
            if (detalleLaboralActual == null || detalleLaboralActual.GenGrupoturno == null)
            {
                dto.CicloIncompleto = false;
                return;
            }

            var grupoLaboral = detalleLaboralActual.GenGrupoturno;
            if (grupoLaboral.ModoGeneracion != "ROTATIVO")
            {
                dto.CicloIncompleto = false;
                return;
            }

            // ✅ Contar días laborables consecutivos del mismo turno
            int diasLaborablesConsecutivos = 0;
            foreach (var turno in turnosOrdenados)
            {
                if (turno.IdgenClasificadortipo == ultimoTurnoLaboralDirecto.IdgenClasificadortipo)
                {
                    diasLaborablesConsecutivos++;
                }
                else if (turno.IdgenClasificadortipo != 22) // Si encuentra otro turno diferente
                {
                    break;
                }
                // Si es descanso, continuar buscando
            }

            // Verificar si faltan días laborables
            var diasLaborablesRequeridos = grupoLaboral.DiasLaborables;
            if (diasLaborablesConsecutivos < diasLaborablesRequeridos)
            {
                dto.CicloIncompleto = true;
                dto.DiasCompletados = diasLaborablesConsecutivos;
                dto.DiasFaltantes = diasLaborablesRequeridos - diasLaborablesConsecutivos;
                dto.OrdenActualEnCiclo = detalleLaboralActual.Orden;
                dto.IdgenClasificadortipoActual = ultimoTurnoLaboralDirecto.IdgenClasificadortipo;
                dto.DescripcionTurnoActual = ultimoTurnoLaboralDirecto.GenClasificadortipo?.Descripcion;
            }
            else
            {
                dto.CicloIncompleto = false;
            }
        }
    }
    public class RrhDiaeventoByRangoSpecification : Specification<RrhDiaevento>
    {
        public RrhDiaeventoByRangoSpecification(int idPersona, DateTime fechaInicio, DateTime fechaFin)
        {
            Query
                .Include(x => x.GenClasificadortipo)
                .Where(x => x.IdrrhPersona == idPersona
                    && x.Fecha >= fechaInicio
                    && x.Fecha <= fechaFin)
                .OrderBy(x => x.Fecha);
        }
    }

    // Buscar últimos N días de una persona desde una fecha hacia atrás
    public class UltimosTurnosPersonaSpecification : Specification<RrhDiaevento>
    {
        public UltimosTurnosPersonaSpecification(int idPersona, DateTime fechaDesde, int dias)
        {
            var fechaHasta = fechaDesde.AddDays(-dias);

            Query
                .Include(x => x.GenClasificadortipo)
                .Where(x => x.IdrrhPersona == idPersona
                    && x.Fecha <= fechaDesde
                    && x.Fecha >= fechaHasta)
                .OrderByDescending(x => x.Fecha);
        }
    }
}
