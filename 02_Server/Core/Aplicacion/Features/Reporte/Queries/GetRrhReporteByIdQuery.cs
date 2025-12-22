using Aplicacion.DTOs.Vistas;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Vistas;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Reporte.Queries
{
    public class GetRrhReporteByIdQuery : IRequest<Response<RrhReporteDto>>
    {
        public int Id { get; set; }
    }

    public class GetRrhReporteByIdQueryHandler: IRequestHandler<GetRrhReporteByIdQuery, Response<RrhReporteDto>>
    {
        private readonly IRepositoryAsync<RrhReporte> _repository;
        private readonly IMapper _mapper;

        public GetRrhReporteByIdQueryHandler(IRepositoryAsync<RrhReporte> repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<RrhReporteDto>> Handle(GetRrhReporteByIdQuery request,CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);

            if (entity == null)
            {
                return new Response<RrhReporteDto>
                {
                    Succeeded = false,
                    Message = "Reporte no encontrado"
                };
            }

            var dto = _mapper.Map<RrhReporteDto>(entity);

            return new Response<RrhReporteDto>(dto)
            {
                Succeeded = true
            };
        }
    }
}