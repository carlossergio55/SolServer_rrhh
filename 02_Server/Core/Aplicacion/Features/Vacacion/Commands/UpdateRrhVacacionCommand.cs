using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Features.BajaMedica.Commands;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.BajaMedica;
using Dominio.Entities.Vacacion;
using MediatR;

namespace Aplicacion.Features.Vacacion.Commands
{
    public class UpdateRrhVacacionCommand : IRequest<Response<int>>
    {
        public int      IdrrhVacacion           { get; set; }
        public DateTime FechaSolicitudVacacion  { get; set; }
        public DateTime FechaInicioVacacion     { get; set; }
        public DateTime FechaFinVacacion        { get; set; }
        public string   AutorizacionLugar       { get; set; }
        public DateTime AutorizacionFecha       { get; set; }

        //public char     EstadoVacacion        { get; set; }

        /*id*/
        public int      IdrrhPersonaVac         { get; set; }
        public int      IdgenHorarioturno       { get; set; }

        public class Handler : IRequestHandler<UpdateRrhVacacionCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhVacacion> _repo;

            public Handler(IRepositoryAsync<RrhVacacion> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateRrhVacacionCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhVacacion);
                if (entity == null)
                    throw new KeyNotFoundException("Vacacion Command ...");

                entity.IdrrhVacacion          = request.IdrrhVacacion;
                entity.FechaSolicitudVacacion = request.FechaSolicitudVacacion;
                entity.FechaInicioVacacion    = request.FechaInicioVacacion;
                entity.FechaFinVacacion       = request.FechaFinVacacion;
                entity.AutorizacionFecha      = request.AutorizacionFecha;
                entity.AutorizacionLugar      = request.AutorizacionLugar;
                //entity.EstadoVacacion       = request.EstadoVacacion;

                /*id*/
                entity.IdrrhPersonaVac = request.IdrrhPersonaVac;
                entity.IdgenHorarioturno      = request.IdgenHorarioturno;

                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhVacacion, "Actualizado correctamente Vacacion Command...");
            }
        }
    }
}
