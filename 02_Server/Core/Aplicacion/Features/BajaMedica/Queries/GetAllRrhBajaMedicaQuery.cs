using Aplicacion.DTOs.BajaMedica;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using System;
using MediatR;
using AutoMapper;
using Dominio.Entities.BajaMedica;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Aplicacion.Features.BajaMedica.Queries
{
    public class GetAllRrhBajaMedicaQuery : IRequest<Response<List<RrhBajaMedicaDto>>> { }


    public class GetAllRrhBajaMedicaQueryHandler : IRequestHandler<GetAllRrhBajaMedicaQuery, Response<List<RrhBajaMedicaDto>>>
    {
        private readonly IRepositoryAsync<RrhBajaMedica> _repo;
        private readonly IMapper _mapper;



        public GetAllRrhBajaMedicaQueryHandler(IRepositoryAsync<RrhBajaMedica> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        public async Task<Response<List<RrhBajaMedicaDto>>> Handle(GetAllRrhBajaMedicaQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAsync(cancellationToken);
            var dto = _mapper.Map<List<RrhBajaMedicaDto>>(list);
            return new Response<List<RrhBajaMedicaDto>>(dto);

        }
    }
}
