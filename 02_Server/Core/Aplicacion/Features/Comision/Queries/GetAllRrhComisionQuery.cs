
using Aplicacion.DTOs.Comision;
using System;
using Dominio.Entities.Comision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Wrappers;
using MediatR;
using Aplicacion.Interfaces;
using AutoMapper;
using System.Threading;
using Ardalis.Specification;
using System.Collections;
using Aplicacion.Features.Comision.Queries;
using Dominio.Entities.Vacacion;


namespace Aplicacion.Features.Comision.Queries
{
    public class GetAllRrhComisionQuery : IRequest<Response<List<RrhComisionDto>>> { }


    public class GetAllRrhComisionQueryHandler : IRequestHandler<GetAllRrhComisionQuery, Response<List<RrhComisionDto>>>
    {
        private readonly IRepositoryAsync<RrhComision> _repo;
        private readonly IMapper _mapper;



        public GetAllRrhComisionQueryHandler(IRepositoryAsync<RrhComision> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        public async Task<Response<List<RrhComisionDto>>> Handle(GetAllRrhComisionQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAsync(new GetAllRrhComisionSpecification(), cancellationToken);
            var dto = _mapper.Map<List<RrhComisionDto>>(list);
            return new Response<List<RrhComisionDto>>(dto);

        }
    }


    public class GetAllRrhComisionSpecification : Specification<RrhComision>
    {
        public GetAllRrhComisionSpecification()
        {
            // Incluimos la relación Persona en la consulta
            Query.Include(x => x.Persona);
        }
    }



}
