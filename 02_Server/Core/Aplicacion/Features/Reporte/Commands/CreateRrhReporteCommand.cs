using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Vistas;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Reporte.Commands
{
    public class CreateRrhReporteCommand : IRequest<Response<long>>
    {
        public string TipoReporte { get; set; }
        public string Parametros { get; set; }
    }

    public class Handler : IRequestHandler<CreateRrhReporteCommand, Response<long>>
    {
        private readonly IRepositoryAsync<RrhReporte> _repository;

        public Handler(IRepositoryAsync<RrhReporte> repository)
        {
            _repository = repository;
        }

        public async Task<Response<long>> Handle(
            CreateRrhReporteCommand request,
            CancellationToken cancellationToken)
        {
            var entity = new RrhReporte
            {
                TipoReporte = request.TipoReporte,
                Parametros = request.Parametros,
                Estado = "PENDIENTE"
            };


            var result = await _repository.AddAsync(entity, cancellationToken);
            return new Response<long>(result.IdrrhReporte);
        }
    }
}
