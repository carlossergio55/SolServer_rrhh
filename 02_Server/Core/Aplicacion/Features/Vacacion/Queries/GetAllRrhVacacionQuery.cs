using Aplicacion.DTOs.Vacacion;
using System;
using Dominio.Entities.Vacacion;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Wrappers;
using MediatR;
using Aplicacion.Features.BajaMedica.Queries;
using Aplicacion.Interfaces;
using AutoMapper;
using Dominio.Entities.BajaMedica;
using System.Threading;
using Ardalis.Specification;
using System.Collections;

namespace Aplicacion.Features.Vacacion.Queries
{
    public class GetAllRrhVacacionQuery : IRequest<Response<List<RrhVacacionDto>>> { }


    public class GetAllRrhVacacionQueryHandler : IRequestHandler<GetAllRrhVacacionQuery, Response<List<RrhVacacionDto>>>
    {
        private readonly IRepositoryAsync<RrhVacacion> _repo;
        private readonly IMapper _mapper;


        public GetAllRrhVacacionQueryHandler(IRepositoryAsync<RrhVacacion> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        public async Task<Response<List<RrhVacacionDto>>> Handle(GetAllRrhVacacionQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAsync(new GetAllRrhVacacionSpecification(), cancellationToken);
            var dto = _mapper.Map<List<RrhVacacionDto>>(list);
            return new Response<List<RrhVacacionDto>>(dto);

        }
    }
    public class GetAllRrhVacacionSpecification : Specification<RrhVacacion>
    {
        public GetAllRrhVacacionSpecification()
        {
            // Incluimos la relación Persona en la consulta
            Query.Include(x => x.Persona);
        }
    }
}
