using Aplicacion.DTOs.Contrato;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Contrato;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Contrato.Queries
{
    public class GetAllRrhContratoQuery : IRequest<Response<List<RrhContratoDto>>> { }

    public class GetAllRrhContratoQueryHandler : IRequestHandler<GetAllRrhContratoQuery, Response<List<RrhContratoDto>>>
    {
        private readonly IRepositoryAsync<RrhContrato> _repo;
        private readonly IMapper _mapper;

        public GetAllRrhContratoQueryHandler(IRepositoryAsync<RrhContrato> repo, IMapper mapper)
        {
            _repo   = repo;
            _mapper = mapper;
        }

        public async Task<Response<List<RrhContratoDto>>> Handle(GetAllRrhContratoQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAsync(cancellationToken);
            var dto = _mapper.Map<List<RrhContratoDto>>(list);
            return new Response<List<RrhContratoDto>>(dto);
        }
    }

    

}




