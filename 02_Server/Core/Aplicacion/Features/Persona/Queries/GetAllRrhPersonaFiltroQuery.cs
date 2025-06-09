using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Queries
{
    public class GetAllRrhPersonaFiltroQuery : IRequest<Response<List<RrhPersonaDto>>>
    {
        public string Busqueda { get; set; }

        public class GetAllRrhPersonaFiltroQueryHandler : IRequestHandler<GetAllRrhPersonaFiltroQuery, Response<List<RrhPersonaDto>>>
        {
            private readonly IRepositoryAsync<RrhPersona> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllRrhPersonaFiltroQueryHandler(IRepositoryAsync<RrhPersona> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<RrhPersonaDto>>> Handle(GetAllRrhPersonaFiltroQuery request, CancellationToken cancellationToken)
            {
                var personas = await _repositoryAsync.ListAsync(
                    new RrhPersonaFiltroSpecification(request.Busqueda),
                    cancellationToken
                );

                var personasDto = _mapper.Map<List<RrhPersonaDto>>(personas);
                return new Response<List<RrhPersonaDto>>(personasDto);
            }
        }

        public class RrhPersonaFiltroSpecification : Specification<RrhPersona>
        {
            public RrhPersonaFiltroSpecification(string busqueda)
            {
                var search = busqueda?.Trim().ToLower() ?? "";

                Query.Where(p => p.NombreApellido.ToLower().Contains(search)
                               /*|| p.Ci.Contains(search)*/)
                     .OrderBy(p => p.NombreApellido)
                     .Take(10)
                     .AsNoTracking();
            }
        }
    }
}
