using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Comision;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Features.BajaMedica.Commands;
using Dominio.Entities.BajaMedica;

namespace Aplicacion.Features.Comision.Commands
{
    public class UpdateRrhComisionCommand : IRequest <Response<int>>
    {
        public int       IdrrhComision              { get; set; }
        public DateTime? FechaSolicitudComision     { get; set; }
        public DateTime? FechaSalidaComision        { get; set; }
        public int       IdgenMotivoComision        { get; set; }
        public string    JustificacionComision      { get; set; }
        public int       IdrrhPersonaComision       { get; set; }
        public int       IdgenHorarioTurnoComision  { get; set; }


        public class Handler : IRequestHandler<UpdateRrhComisionCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhComision> _repo;

            public Handler(IRepositoryAsync<RrhComision> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateRrhComisionCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhComision);
                if (entity == null)
                    throw new KeyNotFoundException("Comision UpdateCommand ...");

                entity.IdrrhComision             =  request.IdrrhComision;
                entity.FechaSolicitudComision    =  request.FechaSolicitudComision;
                entity.FechaSalidaComision       =  request.FechaSalidaComision;
                entity.IdgenMotivoComision       =  request.IdgenMotivoComision;
                entity.JustificacionComision     =  request.JustificacionComision;
                entity.IdrrhPersonaComision      =  request.IdrrhPersonaComision;
                entity.IdgenHorarioTurnoComision =  request.IdgenHorarioTurnoComision;

                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhComision, "Actualizado correctamente Comision UpdateCommand ...");
            }
        }


    }
}
