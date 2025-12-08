using Aplicacion.DTOs.Clasificador;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Clasificador
{
    public class GetAllPermisosQuery : IRequest<Response<List<GenClasificadortipoDto>>>
    {
        public class GetAllPermisosQueryHandler : IRequestHandler<GetAllPermisosQuery, Response<List<GenClasificadortipoDto>>>
        {
            private readonly IRepositoryAsync<GenClasificadortipo> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllPermisosQueryHandler(IRepositoryAsync<GenClasificadortipo> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<GenClasificadortipoDto>>> Handle(GetAllPermisosQuery request, CancellationToken cancellationToken)
            {
                var genClasificadortipoList = await _repositoryAsync.ListAsync(new PermisosSpecification());
                var genClasificadortipoDtoList = _mapper.Map<List<GenClasificadortipoDto>>(genClasificadortipoList);

                return new Response<List<GenClasificadortipoDto>>(genClasificadortipoDtoList);
            }
        }
    }
    public class PermisosSpecification : Specification<GenClasificadortipo>
    {
        public PermisosSpecification()
        {
            Query.Where(t => t.Valor1 == "1");
        }
    }
}
