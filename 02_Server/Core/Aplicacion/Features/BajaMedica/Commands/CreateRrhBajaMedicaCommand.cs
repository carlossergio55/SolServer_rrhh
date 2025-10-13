using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.DTOs.BajaMedica;
using Aplicacion.DTOs.Contrato;
using Aplicacion.Features.Persona.Commands;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.BajaMedica;
using Dominio.Entities.Persona;
using MediatR;


namespace Aplicacion.Features.BajaMedica.Commands
{
    public class CreateRrhBajaMedicaCommand : RrhBajaMedicaDto , IRequest<Response<int>>
    {
        //New added
        //public RrhBajaMedicaDto _RrhBajaMedicapost { get; set; }

        public class Handler : IRequestHandler<CreateRrhBajaMedicaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhBajaMedica> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhBajaMedica> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateRrhBajaMedicaCommand request, CancellationToken cancellationToken)
            {

                try
                {
                    var entity = _mapper.Map<RrhBajaMedica>(request);
                    var created = await _repo.AddAsync(entity, cancellationToken);
                    return new Response<int>(created.IdrrhBajaMedica);
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? ex.Message;

                    // Incluir los valores que llegan
                    var debugInfo = $"FechaInicioReposo: {request.FechaInicioReposo}, FechaFinReposo: {request.FechaFinReposo}, Diagnostico: {request.Diagnostico},    DiasReposo: {request.FechaFinReposo}, BajaMedica: {request.IdrrhBajaMedica}";

                    return new Response<int>(0, $"Error al guardar Baja Medica: {innerMessage}. Datos enviados: {debugInfo}");

                    //var innerMessage = ex.InnerException?.Message ?? ex.Message;
                    //return new Response<int>(0, $"Error: {innerMessage}");
                }
            }
        }
    }
}
