using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Justificacion.Commands
{
    public class UpdateJustificacionEstadoCommand : IRequest<Response<int>>
    {
        public int IdrrhJustificacion { get; set; }
        public string Estado { get; set; } = null!;              // APROBADO, RECHAZADO
        public string ObservacionesAprobacion { get; set; }
        public int IdUsuarioAprueba { get; set; }                // 👈 NUEVO: id de rrh_persona que aprueba
    }

    public class UpdateJustificacionEstadoCommandHandler
        : IRequestHandler<UpdateJustificacionEstadoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<RrhJustificacionOmision> _repository;

        public UpdateJustificacionEstadoCommandHandler(IRepositoryAsync<RrhJustificacionOmision> repository)
        {
            _repository = repository;
        }

        public async Task<Response<int>> Handle(
            UpdateJustificacionEstadoCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.IdrrhJustificacion, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException("Registro no encontrado");

            // Validar estado
            if (request.Estado != "APROBADO" && request.Estado != "RECHAZADO")
                throw new ArgumentException("Estado inválido. Solo se permite APROBADO o RECHAZADO.");

            if (entity.Estado != "SOLICITADO")
                throw new InvalidOperationException("Solo se pueden cambiar justificaciones en estado SOLICITADO.");

            // Actualizar igual que en tu SQL
            entity.Estado = request.Estado;
            entity.ObservacionesAprobacion = request.ObservacionesAprobacion;
            entity.FechaAprobacion = DateTime.Now;
            entity.UsuarioAprueba = request.IdUsuarioAprueba;

            // Si tu AuditableBaseEntity no lo hace automático:
            // entity.UsuarioModificacion = <usuario actual>;
            // entity.FechaModificacion = DateTime.Now;

            await _repository.UpdateAsync(entity, cancellationToken);

            return new Response<int>(entity.IdrrhJustificacion);
        }
    }
}
