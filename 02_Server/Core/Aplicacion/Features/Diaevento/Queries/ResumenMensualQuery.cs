using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Diaevento.Queries
{
    public class ResumenMensualQuery : IRequest<Response<ResumenMensualDto>>
    {
        public int IdPersona { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
    }

    public class ResumenMensualQueryHandler :
       IRequestHandler<ResumenMensualQuery, Response<ResumenMensualDto>>
    {
        private readonly IRepositoryAsync<RrhDiaevento> _repoDiaevento;

        public ResumenMensualQueryHandler(IRepositoryAsync<RrhDiaevento> repoDiaevento)
        {
            _repoDiaevento = repoDiaevento;
        }

        public async Task<Response<ResumenMensualDto>> Handle(
            ResumenMensualQuery request,
            CancellationToken ct)
        {
            // Calcular total de días del mes
            var primerDia = new DateTime(request.Anio, request.Mes, 1);
            var ultimoDia = primerDia.AddMonths(1).AddDays(-1);
            var totalDias = ultimoDia.Day;

            // Buscar todos los registros del mes
            var spec = new RrhDiaeventoByMesYPersonaSpecification(
                request.IdPersona,
                request.Mes,
                request.Anio);

            var registros = await _repoDiaevento.ListAsync(spec, ct);

            // Construir DTO base
            var dto = new ResumenMensualDto
            {
                Mes = request.Mes,
                Anio = request.Anio,
                TotalDias = totalDias,
                DiasRegistrados = registros.Count,
                DiasFaltantes = totalDias - registros.Count
            };

            // Si no hay registros
            if (!registros.Any())
            {
                dto.UltimoTurno = null;
                dto.Estadisticas = new EstadisticasMensualesDto();
                return new Response<ResumenMensualDto>(dto);
            }

            // Último turno del mes
            var ultimoRegistro = registros.OrderByDescending(r => r.Fecha).First();
            dto.UltimoTurno = new UltimoTurnoInfoDto
            {
                Fecha = ultimoRegistro.Fecha,
                IdgenClasificadortipo = ultimoRegistro.IdgenClasificadortipo,
                Descripcion = ultimoRegistro.GenClasificadortipo?.Descripcion,
                Abreviatura = ultimoRegistro.GenClasificadortipo?.Abreviatura
            };

            // Calcular estadísticas
            dto.Estadisticas = CalcularEstadisticas(registros);

            return new Response<ResumenMensualDto>(dto);
        }

        private EstadisticasMensualesDto CalcularEstadisticas(System.Collections.Generic.List<RrhDiaevento> registros)
        {
            var stats = new EstadisticasMensualesDto();

            foreach (var registro in registros)
            {
                switch (registro.IdgenClasificadortipo)
                {
                    case 15: // 6-2 TURNO MAÑANA
                        stats.DiasTurnoManana++;
                        stats.DiasLaborables++;
                        break;
                    case 16: // 6-2 TURNO NOCHE
                        stats.DiasTurnoNoche++;
                        stats.DiasLaborables++;
                        break;
                    case 17: // 6-2 TURNO TARDE
                        stats.DiasTurnoTarde++;
                        stats.DiasLaborables++;
                        break;
                    case 18: // ADMINISTRATIVO
                        stats.DiasAdministrativos++;
                        stats.DiasLaborables++;
                        break;
                    case 22: // DESCANSO
                        stats.DiasDescanso++;
                        break;
                    case 23: // FALTA
                        stats.DiasFaltas++;
                        break;
                    case 24: // VACACIONES
                        stats.DiasVacaciones++;
                        break;
                    case 25: // BAJA MEDICA
                        stats.DiasBajaMedica++;
                        break;
                    case 26: // COMISION
                    case 27: // PERMISO SIN GOCE
                        stats.DiasPermisos++;
                        break;
                }
            }

            return stats;
        }
    }
    public class RrhDiaeventoByMesYPersonaSpecification : Specification<RrhDiaevento>
    {
        public RrhDiaeventoByMesYPersonaSpecification(int idPersona, int mes, int anio)
        {
            Query
                .Include(x => x.GenClasificadortipo)
                .Where(x => x.IdrrhPersona == idPersona
                    && x.Fecha.Month == mes
                    && x.Fecha.Year == anio)
                .OrderBy(x => x.Fecha);
        }
    }
}
