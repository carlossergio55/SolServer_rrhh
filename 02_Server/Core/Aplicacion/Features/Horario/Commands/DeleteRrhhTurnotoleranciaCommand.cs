using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
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
    public class DeleteRrhhTurnotoleranciaCommand : IRequest<Response<int>>
    {
        public int IdrrhhTurnotolerancia { get; set; }

        public class Handler : IRequestHandler<DeleteRrhhTurnotoleranciaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhhTurnotolerancia> _repo;

            public Handler(IRepositoryAsync<RrhhTurnotolerancia> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteRrhhTurnotoleranciaCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhhTurnotolerancia);
                if (entity == null)
                    throw new KeyNotFoundException("Registro no encontrado");

                await _repo.DeleteAsync(entity);
                return new Response<int>(entity.IdrrhhTurnotolerancia);
            }
        }
    }

}
