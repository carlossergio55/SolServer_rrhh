using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Persona;
using MediatR;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Queries
{
    public class GetPersonasByCiQuery : IRequest<Response<List<RrhPersonaDto>>>
    {
        public string Ci { get; set; }

        public GetPersonasByCiQuery(string ci)
        {
            Ci = ci;
        }
    }


    public class GetPersonasByCiQueryHandler : IRequestHandler<GetPersonasByCiQuery, Response<List<RrhPersonaDto>>>
    {
        private readonly IRepositoryAsync<RrhPersona> _repo;
        private readonly IMapper _mapper;


        public GetPersonasByCiQueryHandler(IRepositoryAsync<RrhPersona> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        public async Task<Response<List<RrhPersonaDto>>> Handle(GetPersonasByCiQuery request, CancellationToken cancellationToken)
        {
            var lista = await _repo.ListAsync(new PersonaPorUnidadSpecification(request.Ci), cancellationToken);
            var dto = _mapper.Map<List<RrhPersonaDto>>(lista);
            return new Response<List<RrhPersonaDto>>(dto);
        }
    }


    public class PersonaPorUnidadSpecification : Specification<RrhPersona>
    {
        public PersonaPorUnidadSpecification(string ci)
        {
            Query.Where(x => x.Ci == ci);
        }
    }


}
