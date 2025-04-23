using Aplicacion.DTOs.Vistas;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Vistas;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Asistencia.Queries
{
    // 1. Query que recibe los parámetros por query-string
    public class GetAllVwMarcacionByCiQuery : IRequest<Response<List<VwMarcacionBiometricoDto>>>
    {
        public string Ci { get; set; } = "";
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    // 2. Especificación que aplica los filtros sobre la entidad keyless VwMarcacionBiometrico
    public class GetAllVwMarcacionByCiSpecification : Specification<VwMarcacionBiometrico>
    {
        public GetAllVwMarcacionByCiSpecification(string ci, DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!string.IsNullOrEmpty(ci))
            {
                Query.Where(x => x.Ci.ToLower().Contains(ci.ToLower()));
            }
            if (fechaInicio.HasValue)
            {
                Query.Where(x => x.Timestamp >= fechaInicio.Value);
            }
            if (fechaFin.HasValue)
            {
                Query.Where(x => x.Timestamp <= fechaFin.Value);
            }
            Query.OrderBy(x => x.Timestamp)
                 .AsNoTracking();
        }
    }

    // 3. Handler que ejecuta la especificación y mapea al DTO
    public class GetAllVwMarcacionByCiQueryHandler
        : IRequestHandler<GetAllVwMarcacionByCiQuery, Response<List<VwMarcacionBiometricoDto>>>
    {
        private readonly IRepositoryAsync<VwMarcacionBiometrico> _repo;
        private readonly IMapper _mapper;

        public GetAllVwMarcacionByCiQueryHandler(IRepositoryAsync<VwMarcacionBiometrico> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<List<VwMarcacionBiometricoDto>>> Handle(
            GetAllVwMarcacionByCiQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new GetAllVwMarcacionByCiSpecification(request.Ci, request.FechaInicio, request.FechaFin);
            var list = await _repo.ListAsync(spec, cancellationToken);
            var dto = _mapper.Map<List<VwMarcacionBiometricoDto>>(list);
            return new Response<List<VwMarcacionBiometricoDto>>(dto);
        }
    }
}
