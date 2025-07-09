using Aplicacion.DTOs.Horario;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Horario;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Horario.Queries
{
    public class GetAllGenGrupoturnoQuery : IRequest<Response<List<GenGrupoturnoDto>>> { }

    public class GetAllGenGrupoturnoQueryHandler : IRequestHandler<GetAllGenGrupoturnoQuery, Response<List<GenGrupoturnoDto>>>
    {
        private readonly IRepositoryAsync<GenGrupoturno> _repo;
        private readonly IMapper _mapper;

        public GetAllGenGrupoturnoQueryHandler(IRepositoryAsync<GenGrupoturno> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<List<GenGrupoturnoDto>>> Handle(GetAllGenGrupoturnoQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAsync(cancellationToken);
            var dto = _mapper.Map<List<GenGrupoturnoDto>>(list);
            return new Response<List<GenGrupoturnoDto>>(dto);
        }
    }

}
