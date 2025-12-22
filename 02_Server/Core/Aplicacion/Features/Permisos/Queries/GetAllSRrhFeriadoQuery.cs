using Aplicacion.DTOs.Permisos;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Permisos;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Permisos.Queries
{
    public class GetAllSRrhFeriadoQuery : IRequest<Response<List<SRrhFeriadoDto>>> { }

    public class GetAllSRrhFeriadoQueryHandler : IRequestHandler<GetAllSRrhFeriadoQuery, Response<List<SRrhFeriadoDto>>>
    {
        private readonly IRepositoryAsync<SRrhFeriado> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllSRrhFeriadoQueryHandler(IRepositoryAsync<SRrhFeriado> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<SRrhFeriadoDto>>> Handle(GetAllSRrhFeriadoQuery request, CancellationToken cancellationToken)
        {
            var feriados = await _repositoryAsync.ListAsync(cancellationToken);
            var feriadosDto = _mapper.Map<List<SRrhFeriadoDto>>(feriados);
            return new Response<List<SRrhFeriadoDto>>(feriadosDto);
        }
    }

}
