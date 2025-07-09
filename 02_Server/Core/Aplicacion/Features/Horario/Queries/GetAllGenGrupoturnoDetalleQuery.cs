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
    public class GetAllGenGrupoturnoDetalleQuery : IRequest<Response<List<GenGrupoturnoDetalleDto>>> { }

    public class GetAllGenGrupoturnoDetalleQueryHandler : IRequestHandler<GetAllGenGrupoturnoDetalleQuery, Response<List<GenGrupoturnoDetalleDto>>>
    {
        private readonly IRepositoryAsync<GenGrupoturnoDetalle> _repo;
        private readonly IMapper _mapper;

        public GetAllGenGrupoturnoDetalleQueryHandler(IRepositoryAsync<GenGrupoturnoDetalle> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<List<GenGrupoturnoDetalleDto>>> Handle(GetAllGenGrupoturnoDetalleQuery request, CancellationToken cancellationToken)
        {
            var lista = await _repo.ListAsync(new GenGrupoturnoDetalleSpecification(), cancellationToken);
            var dto = _mapper.Map<List<GenGrupoturnoDetalleDto>>(lista);
            return new Response<List<GenGrupoturnoDetalleDto>>(dto);
        }
    }

    public class GenGrupoturnoDetalleSpecification : Specification<GenGrupoturnoDetalle>
    {
        public GenGrupoturnoDetalleSpecification()
        {
            Query
                .Include(x => x.GenGrupoturno)
                .Include(x => x.GenClasificadortipo);
        }
    }


}
