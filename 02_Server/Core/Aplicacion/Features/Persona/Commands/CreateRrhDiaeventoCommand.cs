using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Commands
{
    public class CreateRrhDiaeventoCommand : IRequest<Response<int>>
    {
        public RrhDiaeventoDto _RrhDiaevento { get; set; }

        public class Handler : IRequestHandler<CreateRrhDiaeventoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhDiaevento> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhDiaevento> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateRrhDiaeventoCommand req, CancellationToken ct)
            {
                var entity = _mapper.Map<RrhDiaevento>(req._RrhDiaevento);
                var created = await _repo.AddAsync(entity, ct);
                return new Response<int>(created.IdrrhDiaevento);
            }
        }
    }

}
