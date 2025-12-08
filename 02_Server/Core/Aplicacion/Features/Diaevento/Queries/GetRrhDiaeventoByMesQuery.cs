// Aplicacion/Features/Persona/Queries/GetRrhDiaeventoByMesQuery.cs
using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Persona;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Diaevento.Queries
{
    public class GetRrhDiaeventoByMesQuery : IRequest<Response<List<RrhDiaeventoDto>>>
    {
        public int Mes { get; set; }   // 1–12
        public int Anio { get; set; }  // Año dinámico
    }


    public class GetRrhDiaeventoByMesQueryHandler :
        IRequestHandler<GetRrhDiaeventoByMesQuery, Response<List<RrhDiaeventoDto>>>
    {
        private readonly IRepositoryAsync<RrhDiaevento> _repo;
        private readonly IMapper _mapper;

        public GetRrhDiaeventoByMesQueryHandler(IRepositoryAsync<RrhDiaevento> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<List<RrhDiaeventoDto>>> Handle(
      GetRrhDiaeventoByMesQuery request, CancellationToken ct)
        {
            var spec = new RrhDiaeventoByMesSpecification(request.Mes, request.Anio);

            var list = await _repo.ListAsync(spec, ct);
            var dto = _mapper.Map<List<RrhDiaeventoDto>>(list);

            return new Response<List<RrhDiaeventoDto>>(dto);
        }

    }

    // Specification para filtrar por mes del año 2025
    public class RrhDiaeventoByMesSpecification : Specification<RrhDiaevento>
    {
        public RrhDiaeventoByMesSpecification(int mes, int anio)
        {
            Query
                .Include(x => x.RrhPersona)
                .Include(x => x.GenClasificadortipo)
                .Where(x => x.Fecha.Month == mes && x.Fecha.Year == anio)
                .OrderBy(x => x.Fecha)
                .ThenBy(x => x.RrhPersona.Nombre);
        }
    }

}