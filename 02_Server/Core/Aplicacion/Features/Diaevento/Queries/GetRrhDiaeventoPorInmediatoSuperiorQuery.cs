using Aplicacion.DTOs.Persona;
using Aplicacion.DTOs.Horario;
using Aplicacion.DTOs.Clasificador;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Persona;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Diaevento.Queries
{
    public class GetRrhDiaeventoPorInmediatoSuperiorQuery : IRequest<Response<List<RrhPersonaCalendarioDto>>>
    {
        public int InmediatoSuperior { get; set; }

        public class Handler
            : IRequestHandler<GetRrhDiaeventoPorInmediatoSuperiorQuery, Response<List<RrhPersonaCalendarioDto>>>
        {
            private readonly IRepositoryAsync<RrhDiaevento> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhDiaevento> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<List<RrhPersonaCalendarioDto>>> Handle(
                GetRrhDiaeventoPorInmediatoSuperiorQuery request,
                CancellationToken ct)
            {
                var entities = await _repo.ListAsync(
                    new RrhDiaeventoPorInmediatoSuperiorSpecification(request.InmediatoSuperior),
                    ct);

                var result = entities
                    .GroupBy(x => x.IdrrhPersona)
                    .Select(g => new RrhPersonaCalendarioDto
                    {
                        Persona = _mapper.Map<PersonaMinDto>(g.First().RrhPersona),

                        Dias = g
                            .OrderBy(d => d.Fecha)
                            .Select(d =>
                            {
                                // buscar el horario que corresponde al día
                                var diaSemana = d.Fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES"));
                                diaSemana = char.ToUpper(diaSemana[0]) + diaSemana.Substring(1);

                                var turnoDia = d.GenClasificadortipo?.Turnos?
                                    .FirstOrDefault(t => t.DiaSemana == diaSemana);

                                return new RrhDiaCalendarioDto
                                {
                                    Fecha = d.Fecha,
                                    DiaSemana = diaSemana,
                                    HoraEntrada = turnoDia?.HoraEntrada,
                                    HoraSalida = turnoDia?.HoraSalida,
                                    Estado = d.GenClasificadortipo?.Abreviatura,
                                    Motivo = d.Motivo
                                };
                            })
                            .ToList()
                    })
                    .ToList();

                return new Response<List<RrhPersonaCalendarioDto>>(result);
            }



        }
    }

    public class RrhDiaeventoPorInmediatoSuperiorSpecification
        : Specification<RrhDiaevento>
    {
        public RrhDiaeventoPorInmediatoSuperiorSpecification(int inmediatoSuperior)
        {
                    Query
             .Include(x => x.RrhPersona)
             .Include(x => x.GenClasificadortipo)
                 .ThenInclude(x => x.Turnos)
             .Where(x => x.RrhPersona.InmediatoSuperior == inmediatoSuperior);

        }
    }
}
