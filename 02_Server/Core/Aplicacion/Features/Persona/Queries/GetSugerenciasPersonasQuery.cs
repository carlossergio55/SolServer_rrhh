using Aplicacion.DTOs.Persona;
using Aplicacion.Features.Asistencia.Queries;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Queries
{
    public class GetSugerenciasPersonasQuery : IRequest<Response<List<RrhPersonaDto>>>
    {
        public string SearchText { get; set; } = string.Empty;
    }

    public class GetSugerenciasPersonasQueryHandler
        : IRequestHandler<GetSugerenciasPersonasQuery, Response<List<RrhPersonaDto>>>
    {
        private readonly IRepositoryAsync<RrhPersona> _repository;

        public GetSugerenciasPersonasQueryHandler(IRepositoryAsync<RrhPersona> repository)
        {
            _repository = repository;
        }

        public async Task<Response<List<RrhPersonaDto>>> Handle(
            GetSugerenciasPersonasQuery request,
            CancellationToken cancellationToken)
        {
            var idsExcluidos = new List<int> { };

            var registros = await _repository.ListAsync(
                new SugerenciasNombresSpecification(request.SearchText, idsExcluidos),
                cancellationToken);

            var sugerencias = registros
                .Select(x => new RrhPersonaDto
                {
                    IdrrhPersona = x.IdrrhPersona,
                    IdgengrupoTrabajo = x.IdgengrupoTrabajo,
                    NombreApellido = x.NombreApellido
                })
                // Evitamos duplicados por Id
                .GroupBy(p => p.IdrrhPersona)
                .Select(g => g.First())
                .OrderBy(p => p.NombreApellido)
                .Take(5)
                .ToList();

            return new Response<List<RrhPersonaDto>>(sugerencias);
        }
    }

}
