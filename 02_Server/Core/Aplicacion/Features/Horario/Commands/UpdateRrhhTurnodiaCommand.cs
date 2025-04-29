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
    public class UpdateRrhhTurnodiaCommand : IRequest<Response<int>>
    {
        public int IdrrhhTurnodia { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public string DiaSemana { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan TiempoExtra { get; set; }

        public class Handler : IRequestHandler<UpdateRrhhTurnodiaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhhTurnodia> _repo;

            public Handler(IRepositoryAsync<RrhhTurnodia> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateRrhhTurnodiaCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhhTurnodia);
                if (entity == null)
                    throw new KeyNotFoundException("Turno no encontrado");

                entity.IdgenClasificadortipo = request.IdgenClasificadortipo;
                entity.DiaSemana = request.DiaSemana;
                entity.HoraEntrada = request.HoraEntrada;
                entity.HoraSalida = request.HoraSalida;
                entity.TiempoExtra = request.TiempoExtra;

                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhhTurnodia);
            }
        }
    }

}
