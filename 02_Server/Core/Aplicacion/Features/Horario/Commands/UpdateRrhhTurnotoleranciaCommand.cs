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
    public class UpdateRrhhTurnotoleranciaCommand : IRequest<Response<int>>
    {
        public int IdrrhhTurnotolerancia { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public int ToleranciaAtraso { get; set; }
        public int ToleranciaFalta { get; set; }
        public int ToleranciaSalida { get; set; }
        public int SalidaAdelantada { get; set; }
        public int Puntualidad { get; set; }
        public string Prioridad { get; set; }

        public class Handler : IRequestHandler<UpdateRrhhTurnotoleranciaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhhTurnotolerancia> _repo;

            public Handler(IRepositoryAsync<RrhhTurnotolerancia> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateRrhhTurnotoleranciaCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhhTurnotolerancia);
                if (entity == null)
                    throw new KeyNotFoundException("Registro no encontrado");

                entity.IdgenClasificadortipo = request.IdgenClasificadortipo;
                entity.ToleranciaAtraso = request.ToleranciaAtraso;
                entity.ToleranciaFalta = request.ToleranciaFalta;
                entity.ToleranciaSalida = request.ToleranciaSalida;
                entity.SalidaAdelantada = request.SalidaAdelantada;
                entity.Puntualidad = request.Puntualidad;
                entity.Prioridad = request.Prioridad;

                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhhTurnotolerancia);
            }
        }
    }

}
