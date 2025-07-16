using Aplicacion.DTOs.Persona;      // Data Transfer Object (DTO) for person
using Aplicacion.Interfaces;        // Custom interfaces like repository abstraction
using Aplicacion.Wrappers;          // Used to wrap API responses (standardized format)
using AutoMapper;                   // Used for object mapping (DTO -> Entity)
using Dominio.Entities.Persona;     // Your domain entity (likely matches database table)
using MediatR;                      // Mediator library for CQRS pattern
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Commands
{
    public class CreateRrhPersonaCommand : IRequest<Response<int>>
    {
        //New added
        public RrhPersonaDto _RrhPersonapost { get; set; }


        public class Handler : IRequestHandler<CreateRrhPersonaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhPersona> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhPersona> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateRrhPersonaCommand request, CancellationToken cancellationToke)
            {
                var entity = _mapper.Map<RrhPersona>(request._RrhPersonapost);
                var created = await _repo.AddAsync(entity, cancellationToke);

                return new Response<int>(created.IdrrhPersona);

                //var entity = _mapper.Map<RrhPersona>(request._RrhPersona);
                //var created = await _repo.AddAsync(entity, cancellationToken);
                //return new Response<int>(created.IdrrhPersona);
            }
        }
    }
}
