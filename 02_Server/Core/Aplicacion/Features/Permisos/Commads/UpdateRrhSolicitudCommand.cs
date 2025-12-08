using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Permisos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Permisos.Commads
{
    public class UpdateRrhSolicitudCommand : IRequest<Response<int>>
    {
        public int IdrrhSolicitud { get; set; }
        public int IdrrhPersona { get; set; }
        public int TipoSolicitud { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Motivo { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public int? UsuarioAprueba { get; set; }
        public string? ObservacionesAprobacion { get; set; }
        public class Handler : IRequestHandler<UpdateRrhSolicitudCommand, Response<int>>
        {
            private readonly IRepositoryAsync<SRrhSolicitud> _repositoryAsync;
            public Handler(IRepositoryAsync<SRrhSolicitud> repositoryAsync)
            {
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<int>> Handle(UpdateRrhSolicitudCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repositoryAsync.GetByIdAsync(request.IdrrhSolicitud);
                if (entity == null)
                    throw new KeyNotFoundException("Registro no encontrado");
                entity.IdrrhPersona = request.IdrrhPersona;
                entity.TipoSolicitud = request.TipoSolicitud;
                entity.FechaInicio = request.FechaInicio;
                entity.FechaFin = request.FechaFin;
                entity.Motivo = request.Motivo;
                entity.Estado = request.Estado;
                entity.FechaSolicitud = request.FechaSolicitud;
                entity.FechaAprobacion = request.FechaAprobacion;
                entity.UsuarioAprueba = request.UsuarioAprueba;
                entity.ObservacionesAprobacion = request.ObservacionesAprobacion;
                await _repositoryAsync.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhSolicitud);
            }
        }
    }
}
