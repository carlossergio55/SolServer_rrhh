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

namespace Aplicacion.Features.Horario.Commands
{
    public class CreateGenGrupoturnoDetalleCommand : IRequest<Response<int>>
    {
        public GenGrupoturnoDetalleDto _GenGrupoturnoDetalle { get; set; }
        public class Handler : IRequestHandler<CreateGenGrupoturnoDetalleCommand, Response<int>>
        {
            private readonly IRepositoryAsync<GenGrupoturnoDetalle> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<GenGrupoturnoDetalle> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateGenGrupoturnoDetalleCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<GenGrupoturnoDetalle>(request._GenGrupoturnoDetalle);
                var result = await _repo.AddAsync(entity);
                return new Response<int>(result.IdgenGrupoturnoDetalle);
            }
        }
    }

}
