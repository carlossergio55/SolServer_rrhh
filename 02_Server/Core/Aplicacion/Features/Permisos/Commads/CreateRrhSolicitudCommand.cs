using Aplicacion.DTOs.Permisos;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Permisos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Permisos.Commads
{
    public class CreateRrhSolicitudCommand : IRequest<Response<int>>
    {
        public SRrhSolicitudDto Item { get; set; }
        public class Handler : IRequestHandler<CreateRrhSolicitudCommand, Response<int>>
        {
            private readonly IRepositoryAsync<SRrhSolicitud> _repositoryAsync;
            private readonly IMapper _mapper;
            public Handler(IRepositoryAsync<SRrhSolicitud> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }
            public async Task<Response<int>> Handle(CreateRrhSolicitudCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<SRrhSolicitud>(request.Item);
                var newRecord = await _repositoryAsync.AddAsync(entity);
                return new Response<int>(newRecord.IdrrhSolicitud);
            }
        }
    }
}
