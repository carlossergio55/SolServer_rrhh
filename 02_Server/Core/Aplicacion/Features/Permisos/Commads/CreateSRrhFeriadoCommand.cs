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
    public class CreateSRrhFeriadoCommand : IRequest<Response<int>>
    {
        public SRrhFeriadoDto Item { get; set; }

        public class Handler : IRequestHandler<CreateSRrhFeriadoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<SRrhFeriado> _repositoryAsync;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<SRrhFeriado> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateSRrhFeriadoCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<SRrhFeriado>(request.Item);
                entity.FechaCreacion = DateTime.Now;

                var newRecord = await _repositoryAsync.AddAsync(entity);
                return new Response<int>(newRecord.IdrrhFeriado);
            }
        }
    }

}
