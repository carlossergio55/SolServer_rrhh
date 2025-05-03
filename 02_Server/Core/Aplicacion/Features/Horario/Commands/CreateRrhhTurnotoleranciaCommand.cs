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
    public class CreateRrhhTurnotoleranciaCommand : IRequest<Response<int>>
    {
        public RrhhTurnotoleranciaDto _RrhhTurnotolerancia { get; set; }
        public class Handler : IRequestHandler<CreateRrhhTurnotoleranciaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhhTurnotolerancia> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhhTurnotolerancia> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateRrhhTurnotoleranciaCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<RrhhTurnotolerancia>(request._RrhhTurnotolerancia);
                var created = await _repo.AddAsync(entity);
                return new Response<int>(created.IdrrhhTurnotolerancia);
            }
        }
    }

}
