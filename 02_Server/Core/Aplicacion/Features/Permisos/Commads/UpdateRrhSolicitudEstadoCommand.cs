using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Permisos;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Permisos.Commads
{
    public class UpdateRrhSolicitudEstadoCommand : IRequest<Response<int>>
    {
        public int IdrrhSolicitud { get; set; }
        public string Estado { get; set; } = null!;

        public class Handler : IRequestHandler<UpdateRrhSolicitudEstadoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<SRrhSolicitud> _repository;

            public Handler(IRepositoryAsync<SRrhSolicitud> repository)
            {
                _repository = repository;
            }

            public async Task<Response<int>> Handle(
                UpdateRrhSolicitudEstadoCommand request,
                CancellationToken cancellationToken)
            {
                var entity = await _repository.GetByIdAsync(request.IdrrhSolicitud);

                if (entity == null)
                    throw new KeyNotFoundException("Registro no encontrado");

                entity.Estado = request.Estado;

                await _repository.UpdateAsync(entity);

                return new Response<int>(entity.IdrrhSolicitud);
            }
        }
    }
}
