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
    public class GetAllRrhDiaeventoQuery : IRequest<Response<List<RrhDiaeventoDto>>> { }

    public class GetAllRrhDiaeventoQueryHandler :
        IRequestHandler<GetAllRrhDiaeventoQuery, Response<List<RrhDiaeventoDto>>>
    {
        private readonly IRepositoryAsync<RrhDiaevento> _repo;
        private readonly IMapper _mapper;

        public GetAllRrhDiaeventoQueryHandler(IRepositoryAsync<RrhDiaevento> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<List<RrhDiaeventoDto>>> Handle(
            GetAllRrhDiaeventoQuery request, CancellationToken ct)
        {
            var list = await _repo.ListAsync(new RrhDiaeventoSpecification(), ct);
            var dto = _mapper.Map<List<RrhDiaeventoDto>>(list);
            return new Response<List<RrhDiaeventoDto>>(dto);
        }
    }
    public class RrhDiaeventoSpecification : Specification<RrhDiaevento>
    {
        public RrhDiaeventoSpecification()
        {
            Query
                .Include(x => x.RrhPersona)
                .Include(x => x.GenClasificadortipo);
        }
    }

}
