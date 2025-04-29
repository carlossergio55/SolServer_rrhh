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
    public class DeleteRrhhTurnodiaCommand : IRequest<Response<int>>
    {
        public int IdrrhhTurnodia { get; set; }

        public class Handler : IRequestHandler<DeleteRrhhTurnodiaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhhTurnodia> _repo;

            public Handler(IRepositoryAsync<RrhhTurnodia> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteRrhhTurnodiaCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhhTurnodia);
                if (entity == null)
                    throw new KeyNotFoundException("Turno no encontrado");

                await _repo.DeleteAsync(entity);
                return new Response<int>(entity.IdrrhhTurnodia);
            }
        }
    }

}
