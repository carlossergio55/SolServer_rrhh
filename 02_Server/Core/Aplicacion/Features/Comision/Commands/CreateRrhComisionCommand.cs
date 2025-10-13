using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.DTOs.Comision;
using Aplicacion.Features.Comision.Commands;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Comision;
using MediatR;

namespace Aplicacion.Features.Comision.Commands
{
    public class CreateRrhComisionCommand : RrhComisionDto, IRequest<Response<int>>
    {
        public class Handler : IRequestHandler<CreateRrhComisionCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhComision> _repo;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhComision> repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateRrhComisionCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = _mapper.Map<RrhComision>(request);
                    var created = await _repo.AddAsync(entity, cancellationToken);
                    return new Response<int>(created.IdrrhComision);
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? ex.Message;

                    // Incluir los valores que llegan
                    var debugInfo = $" IdrrhComision: {request.IdrrhComision}, FechaSolicitudComision: {request.FechaSolicitudComision}, FechaSalidaComision: {request.FechaSalidaComision}, IdgenMotivoComision: {request.IdgenMotivoComision}, JustifiscacionComision: {request.JustificacionComision}, IdrrhPersonaComision:  {request.IdrrhPersonaComision},    IdgenHorarioTurnoComision:  {request.IdgenHorarioTurnoComision}";

                    return new Response<int>(0, $"Error al guardar Comision CreateRrhComision: {innerMessage}. Datos enviados: {debugInfo}");

                    //var innerMessage = ex.InnerException?.Message ?? ex.Message;
                    //return new Response<int>(0, $"Error: {innerMessage}");
                }
            }
        }
    }
}
