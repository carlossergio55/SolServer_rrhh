using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entities.Vacacion;
using Aplicacion.DTOs.Vacacion;
using MediatR;
using Aplicacion.Wrappers;
using Aplicacion.Features.BajaMedica.Commands;
using Aplicacion.Interfaces;
using AutoMapper;
using Dominio.Entities.BajaMedica;
using System.Threading;


namespace Aplicacion.Features.Vacacion.Commands
{
    public class CreateRrhVacacionCommand : RrhVacacionDto, IRequest<Response<int>>
    {
        public class Handler : IRequestHandler<CreateRrhVacacionCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhVacacion> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhVacacion> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateRrhVacacionCommand request, CancellationToken cancellationToken)
            {

                try
                {
                    var entity = _mapper.Map<RrhVacacion>(request);
                    var created = await _repo.AddAsync(entity, cancellationToken);
                    return new Response<int>(created.IdrrhVacacion);
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? ex.Message;

                    // Incluir los valores que llegan
                    var debugInfo = $"FechaSolicitudVacacion: {request.FechaSolicitudVacacion}, IdgenHorarioturno: {request.IdgenHorarioturno}, FechaInicioVacacion: {request.FechaInicioVacacion}, FechaFinVacacion: {request.FechaFinVacacion}, AutorizacionLugar: {request.AutorizacionLugar}, AutorizacionFecha: {request.AutorizacionFecha}, IdrrhPersonaVac: {request.IdrrhPersonaVac}";

                    return new Response<int>(0, $"Error al guardar Vacacion: {innerMessage}. Datos enviados: {debugInfo}");

                    //var innerMessage = ex.InnerException?.Message ?? ex.Message;
                    //return new Response<int>(0, $"Error: {innerMessage}");
                }
            }
        }
    }
}
