using Aplicacion.DTOs.Contrato;      // Data Transfer Object (DTO) for person
using Aplicacion.Interfaces;         // Custom interfaces like repository abstraction
using Aplicacion.Wrappers;           // Used to wrap API responses (standardized format)
using AutoMapper;                    // Used for object mapping (DTO -> Entity)
using Dominio.Entities.Contrato;     // Your domain entity (likely matches database table)
using MediatR;                       // Mediator library for CQRS pattern
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Contrato.Commands
{
    public class CreateRrhContratoCommand : RrhContratoDto , IRequest<Response<int>>
    {
        //New added
        //epublic RrhContratoDto _RrhContratopost {  get; set; }


        public class Handler : IRequestHandler<CreateRrhContratoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhContrato> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhContrato> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;


            }

            public async Task<Response<int>> Handle(CreateRrhContratoCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = _mapper.Map<RrhContrato>(request);
                    var created = await _repo.AddAsync(entity, cancellationToken);
                    return new Response<int>(created.IdrrhhContrato);
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? ex.Message;

                    // Incluir los valores que llegan
                    var debugInfo = $"Inicio: {request.InicioContrato}, Fin: {request.FinContrato}, Número: {request.NumeroContrato}, Tipo: {request.TipoContrato}, Persona: {request.IdrrhhPersona}";

                    return new Response<int>(0, $"Error al guardar contrato: {innerMessage}. Datos enviados: {debugInfo}");

                    //var innerMessage = ex.InnerException?.Message ?? ex.Message;
                    //return new Response<int>(0, $"Error: {innerMessage}");
                }
            }
        }
    }
}
