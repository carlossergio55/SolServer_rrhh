using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Vistas;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Reporte.Commands
{
    public class UpdateRrhReporteCommand : IRequest<Response<int>>
    {
        public int IdrrhReporte { get; set; }
        public string Estado { get; set; }
        public class Handler : IRequestHandler<UpdateRrhReporteCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhReporte> _repositoryAsync;
            public Handler(IRepositoryAsync<RrhReporte> repositoryAsync)
            {
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<int>> Handle(UpdateRrhReporteCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repositoryAsync.GetByIdAsync(request.IdrrhReporte);
                if (entity == null)
                    throw new KeyNotFoundException("Registro no encontrado");
                entity.Estado = request.Estado;
                await _repositoryAsync.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhReporte);
            }
        }
    }
}
