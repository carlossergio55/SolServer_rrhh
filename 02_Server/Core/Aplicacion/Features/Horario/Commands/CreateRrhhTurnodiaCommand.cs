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
    public class CreateRrhhTurnodiaCommand : IRequest<Response<int>>
    {
        public RrhhTurnodiaDto _RrhhTurnodia { get; set; }

        public class Handler : IRequestHandler<CreateRrhhTurnodiaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhhTurnodia> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhhTurnodia> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateRrhhTurnodiaCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<RrhhTurnodia>(request._RrhhTurnodia);
                var created = await _repo.AddAsync(entity);
                return new Response<int>(created.IdrrhhTurnodia);
            }
        }
    }

}
