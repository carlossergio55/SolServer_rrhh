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
    public class GetAllRrhPersonaPorUnidadDtoQuery : IRequest<Response<List<PersonaMinDto>>>
    {
        public int? IdgenUnidad { get; set; }

        public class Handler : IRequestHandler<GetAllRrhPersonaPorUnidadDtoQuery, Response<List<PersonaMinDto>>>
        {
            private readonly IRepositoryAsync<RrhPersona> _repository;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhPersona> repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<Response<List<PersonaMinDto>>> Handle(
                GetAllRrhPersonaPorUnidadDtoQuery request,
                CancellationToken ct)
            {
                var personas = await _repository.ListAsync(
                    new RrhPersonaPorUnidadSpecification(request.IdgenUnidad),
                    ct);

                var dto = _mapper.Map<List<PersonaMinDto>>(personas);
                return new Response<List<PersonaMinDto>>(dto);
            }
        }
    }
    public class RrhPersonaPorUnidadSpecification : Specification<RrhPersona>
    {
        public RrhPersonaPorUnidadSpecification(int? idgenUnidad)
        {
            if (idgenUnidad.HasValue)
            {
                Query.Where(x => x.IdgenUnidad == idgenUnidad.Value);
            }
        }
    }
}
