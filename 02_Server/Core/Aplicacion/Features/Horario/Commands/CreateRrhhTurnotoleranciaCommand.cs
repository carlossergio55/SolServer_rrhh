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
        public int IdgenClasificadortipo { get; set; }
        public int ToleranciaAtraso { get; set; }
        public int ToleranciaFalta { get; set; }
        public int ToleranciaSalida { get; set; }
        public int SalidaAdelantada { get; set; }
        public int Puntualidad { get; set; }
        public string Prioridad { get; set; }

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
                var entity = _mapper.Map<RrhhTurnotolerancia>(request);
                var created = await _repo.AddAsync(entity);
                return new Response<int>(created.IdrrhhTurnotolerancia);
            }
        }
    }

}
