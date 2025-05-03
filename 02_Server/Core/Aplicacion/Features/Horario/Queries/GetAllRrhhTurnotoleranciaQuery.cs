using Aplicacion.DTOs.Horario;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Horario;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Horario.Queries 
{
    public class GetAllRrhhTurnotoleranciaQuery : IRequest<Response<List<RrhhTurnotoleranciaDto>>>
    {
        public int IdgenClasificadortipo { get; set; }
    }

    public class GetAllRrhhTurnotoleranciaQueryHandler : IRequestHandler<GetAllRrhhTurnotoleranciaQuery, Response<List<RrhhTurnotoleranciaDto>>>
    {
        private readonly IRepositoryAsync<RrhhTurnotolerancia> _repo;
        private readonly IMapper _mapper;

        public GetAllRrhhTurnotoleranciaQueryHandler(IRepositoryAsync<RrhhTurnotolerancia> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<List<RrhhTurnotoleranciaDto>>> Handle(GetAllRrhhTurnotoleranciaQuery request, CancellationToken cancellationToken)
        {
            var spec = new RrhhTurnotoleranciaSpecification(request.IdgenClasificadortipo);
            var list = await _repo.ListAsync(spec, cancellationToken);
            var dto = _mapper.Map<List<RrhhTurnotoleranciaDto>>(list);
            return new Response<List<RrhhTurnotoleranciaDto>>(dto);
        }
    }

    public class RrhhTurnotoleranciaSpecification : Specification<RrhhTurnotolerancia>
    {
        public RrhhTurnotoleranciaSpecification(int idgenClasificadortipo)
        {
            Query.Include(x => x.GenClasificadortipo)
                 .Where(x => x.IdgenClasificadortipo == idgenClasificadortipo);
        }
    }

}
