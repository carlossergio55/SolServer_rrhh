using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entities.Falta;
using Aplicacion.DTOs.Falta;
using MediatR;
using Aplicacion.Wrappers;
using Aplicacion.Features.BajaMedica.Commands;
using Aplicacion.Interfaces;
using AutoMapper;
using Dominio.Entities.BajaMedica;
using System.Threading;



namespace Aplicacion.Features.Falta.Commands
{
    public class CreateRrhFaltaCommand : RrhFaltaDto , IRequest<Response<int>>
    {

        public class Handler : IRequestHandler<CreateRrhFaltaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhFalta> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhFalta> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateRrhFaltaCommand request, CancellationToken cancellationToken)
            {

                try
                {
                    var entity = _mapper.Map<RrhFalta>(request);
                    var created = await _repo.AddAsync(entity, cancellationToken);
                    return new Response<int>(created.IdrrhFalta);
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? ex.Message;

                    // Incluir los valores que llegan
                    var debugInfo = $"FechaInicioFalta: {request.FechaInicioFalta}, FechaFinReposo: {request.FechaFinFalta},    DiasFalta: {request.DiasFalta}, IdrrhPersona: {request.IdrrhPersona}";

                    return new Response<int>(0, $"Error al guardar Falta: {innerMessage}. Datos enviados: {debugInfo}");

                    //var innerMessage = ex.InnerException?.Message ?? ex.Message;
                    //return new Response<int>(0, $"Error: {innerMessage}");
                }
            }

        }
    }
}
