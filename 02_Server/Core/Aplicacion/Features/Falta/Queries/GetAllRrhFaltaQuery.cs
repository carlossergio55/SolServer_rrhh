using Aplicacion.DTOs.Falta;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Entities.Falta;

using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Falta.Queries
{
    public class GetAllRrhFaltaQuery : IRequest<Response<List<RrhFaltaDto>>> { }

    public class GetAllRrhFaltaQueryHandler : IRequestHandler<GetAllRrhFaltaQuery, Response<List<RrhFaltaDto>>>
    {
        private readonly IRepositoryAsync<RrhFalta> _repo;
        private readonly IMapper _mapper;



        public GetAllRrhFaltaQueryHandler(IRepositoryAsync<RrhFalta> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        public async Task<Response<List<RrhFaltaDto>>> Handle(GetAllRrhFaltaQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAsync(cancellationToken);
            var dto = _mapper.Map<List<RrhFaltaDto>>(list);
            return new Response<List<RrhFaltaDto>>(dto);

        }
    }



}
