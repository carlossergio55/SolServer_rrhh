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
    public class DeleteRrhSolicitudCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public class Handler : IRequestHandler<DeleteRrhSolicitudCommand, Response<int>>
        {
            private readonly IRepositoryAsync<SRrhSolicitud> _repositoryAsync;
            public Handler(IRepositoryAsync<SRrhSolicitud> repositoryAsync)
            {
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<int>> Handle(DeleteRrhSolicitudCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repositoryAsync.GetByIdAsync(request.Id);
                if (entity == null)
                    throw new KeyNotFoundException("Registro no encontrado");
                await _repositoryAsync.DeleteAsync(entity);
                return new Response<int>(entity.IdrrhSolicitud);
            }
        }
    }
}
