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
    public class GetAllSVistaAsistenciasByCiQuery : IRequest<Response<List<SVistaAsistenciasDto>>>
    {
        public string Busqueda { get; set; } = "";
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    // 2. Especificación para la consulta, aplicando filtros por Ci y rango de fechas en ShiftDate
    public class GetAllSVistaAsistenciasByCiQuerySpecification : Specification<SVistaAsistencias>
    {
        public GetAllSVistaAsistenciasByCiQuerySpecification(string busqueda, DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!string.IsNullOrEmpty(busqueda))
            {
                Query.Where(x => x.Ci.ToLower().Contains(busqueda.ToLower()));
            }
            if (fechaInicio.HasValue)
            {
                Query.Where(x => x.ShiftDate >= fechaInicio.Value);
            }
            if (fechaFin.HasValue)
            {
                Query.Where(x => x.ShiftDate <= fechaFin.Value);
            }
            // Ordenamos por ShiftDate, puedes ajustar el criterio de ordenación si lo requieres.
            Query.OrderBy(x => x.ShiftDate)
                 .AsNoTracking();
        }
    }

    // 3. Handler de la Query que utiliza la especificación para obtener los registros filtrados y mapea al DTO
    public class GetAllSVistaAsistenciasByCiQueryHandler : IRequestHandler<GetAllSVistaAsistenciasByCiQuery, Response<List<SVistaAsistenciasDto>>>
    {
        private readonly IRepositoryAsync<SVistaAsistencias> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllSVistaAsistenciasByCiQueryHandler(IRepositoryAsync<SVistaAsistencias> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<SVistaAsistenciasDto>>> Handle(GetAllSVistaAsistenciasByCiQuery request, CancellationToken cancellationToken)
        {
            var registros = await _repositoryAsync.ListAsync(
                new GetAllSVistaAsistenciasByCiQuerySpecification(request.Busqueda, request.FechaInicio, request.FechaFin),
                cancellationToken);
            var registrosDto = _mapper.Map<List<SVistaAsistenciasDto>>(registros);
            return new Response<List<SVistaAsistenciasDto>>(registrosDto);
        }
    }
}
