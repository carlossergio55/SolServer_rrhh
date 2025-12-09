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
    public class GetClasificador1a13Query : IRequest<Response<List<GenClasificadorDto>>>
    {
        public class Handler : IRequestHandler<GetClasificador1a13Query, Response<List<GenClasificadorDto>>>
        {
            private readonly IRepositoryAsync<GenClasificador> _repositoryAsync;
            private readonly IMapper _mapper;

            public Handler(
                IRepositoryAsync<GenClasificador> repositoryAsync,
                IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<GenClasificadorDto>>> Handle(
                GetClasificador1a13Query request,
                CancellationToken cancellationToken)
            {
                var entities = await _repositoryAsync.ListAsync(
                    new Clasificador1a13Specification(),
                    cancellationToken);

                var dto = _mapper.Map<List<GenClasificadorDto>>(entities);

                return new Response<List<GenClasificadorDto>>(dto);
            }
        }
    }

    public class Clasificador1a13Specification : Specification<GenClasificador>
    {
        public Clasificador1a13Specification()
        {
            Query.Where(x => x.IdgenClasificador >= 1 &&
                             x.IdgenClasificador <= 14);
        }
    }
}
