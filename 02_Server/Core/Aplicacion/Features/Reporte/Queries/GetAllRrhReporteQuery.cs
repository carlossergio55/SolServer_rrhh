using Aplicacion.DTOs.Vistas;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Vistas;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Reporte.Queries
{
    public class GetRrhReporteQuery : IRequest<Response<List<RrhReporteDto>>>
    {
        public string TipoReporte { get; set; }
        public string Estado { get; set; }
    }

    public class GetRrhReporteQueryHandler
        : IRequestHandler<GetRrhReporteQuery, Response<List<RrhReporteDto>>>
    {
        private readonly IRepositoryAsync<RrhReporte> _repository;
        private readonly IMapper _mapper;

        public GetRrhReporteQueryHandler(
            IRepositoryAsync<RrhReporte> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<List<RrhReporteDto>>> Handle(
            GetRrhReporteQuery request,
            CancellationToken cancellationToken)
        {
            var query = await _repository.ListAsync(cancellationToken);

            if (!string.IsNullOrEmpty(request.TipoReporte))
                query = query.Where(x => x.TipoReporte == request.TipoReporte).ToList();

            if (!string.IsNullOrEmpty(request.Estado))
                query = query.Where(x => x.Estado == request.Estado).ToList();

            var dto = _mapper.Map<List<RrhReporteDto>>(query);
            return new Response<List<RrhReporteDto>>(dto);
        }
    }
}
