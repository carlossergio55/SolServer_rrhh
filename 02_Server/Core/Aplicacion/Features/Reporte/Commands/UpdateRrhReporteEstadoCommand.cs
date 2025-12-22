using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Vistas;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Reporte.Commands
{
    /// <summary>
    /// Command específico para actualizar el estado del reporte después de su generación
    /// </summary>
    public class UpdateRrhReporteEstadoCommand : IRequest<Response<int>>
    {
        public int IdrrhReporte { get; set; }
        public string Estado { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime? FechaGeneracion { get; set; }

        public class Handler : IRequestHandler<UpdateRrhReporteEstadoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhReporte> _repositoryAsync;

            public Handler(IRepositoryAsync<RrhReporte> repositoryAsync)
            {
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<int>> Handle(UpdateRrhReporteEstadoCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repositoryAsync.GetByIdAsync(request.IdrrhReporte);

                if (entity == null)
                    throw new KeyNotFoundException("Registro no encontrado");

                // Actualizar los campos necesarios
                entity.Estado = request.Estado;

                if (!string.IsNullOrEmpty(request.RutaArchivo))
                    entity.RutaArchivo = request.RutaArchivo;

                if (request.FechaGeneracion.HasValue)
                    entity.FechaGeneracion = request.FechaGeneracion.Value;

                await _repositoryAsync.UpdateAsync(entity);

                return new Response<int>((int)entity.IdrrhReporte);
            }
        }
    }
}