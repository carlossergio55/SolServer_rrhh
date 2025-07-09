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
    public class CreateGenGrupoturnoCommand : IRequest<Response<int>>
    {
        public GenGrupoturnoDto _GenGrupoturno { get; set; }


        public class Handler : IRequestHandler<CreateGenGrupoturnoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<GenGrupoturno> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<GenGrupoturno> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateGenGrupoturnoCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<GenGrupoturno>(request._GenGrupoturno);
                var created = await _repo.AddAsync(entity);
                return new Response<int>(created.IdgenGrupoturno);
            }
        }
    }

}
