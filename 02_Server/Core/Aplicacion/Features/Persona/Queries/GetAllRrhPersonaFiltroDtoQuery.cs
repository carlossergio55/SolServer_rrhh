using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Aplicacion.Features.Persona.Queries.GetAllRrhPersonaFiltroQuery;

namespace Aplicacion.Features.Persona.Queries
{
    public class GetAllRrhPersonaFiltroDtoQuery : IRequest<Response<List<PersonaMinDto>>>
    {
        public string Busqueda { get; set; } = string.Empty;

        public class Handler : IRequestHandler<GetAllRrhPersonaFiltroDtoQuery, Response<List<PersonaMinDto>>>
        {
            private readonly IRepositoryAsync<RrhPersona> _repository;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhPersona> repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<Response<List<PersonaMinDto>>> Handle(GetAllRrhPersonaFiltroDtoQuery request, CancellationToken ct)
            {
                var personas = await _repository.ListAsync(
                    new RrhPersonaFiltroSpecification(request.Busqueda),
                    ct);

                var dto = _mapper.Map<List<PersonaMinDto>>(personas);
                return new Response<List<PersonaMinDto>>(dto);
            }
        }
    }
}
