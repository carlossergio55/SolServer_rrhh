using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Persona;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Queries
{
    public class GetAllRrhPersonaQuery : IRequest<Response<List<RrhPersonaDto>>> { }

    public class GetAllRrhPersonaQueryHandler : IRequestHandler<GetAllRrhPersonaQuery, Response<List<RrhPersonaDto>>>
    {
        private readonly IRepositoryAsync<RrhPersona> _repo;
        private readonly IMapper _mapper;

        public GetAllRrhPersonaQueryHandler(IRepositoryAsync<RrhPersona> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<List<RrhPersonaDto>>> Handle(GetAllRrhPersonaQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAsync(cancellationToken);
            var dto = _mapper.Map<List<RrhPersonaDto>>(list);
            return new Response<List<RrhPersonaDto>>(dto);
        }
    }

}
