using Aplicacion.DTOs.Horario;
using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces.Repositories.Horario;
using Aplicacion.Wrappers;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Horario.Queries
{
    public class GetAsistenciaQuery : IRequest<Response<AsistenciaConsultaDto>>
    {
        public int? IdPersona { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public class Handler : IRequestHandler<GetAsistenciaQuery, Response<AsistenciaConsultaDto>>
        {
            private readonly IRrhDiaeventoRepository _repository;

            public Handler(IRrhDiaeventoRepository repository)
            {
                _repository = repository;
            }

            public async Task<Response<AsistenciaConsultaDto>> Handle( GetAsistenciaQuery request,CancellationToken cancellationToken)
            {
                // Validaciones
                if (request.FechaInicio > request.FechaFin)
                {
                    return new Response<AsistenciaConsultaDto>
                    {
                        Succeeded = false,
                        Message = "La fecha de inicio no puede ser mayor a la fecha fin"
                    };
                }

                return await _repository.GetAsistenciaPorRango(
                    request.IdPersona,
                    request.FechaInicio,
                    request.FechaFin
                );
            }
        }
    }
}
