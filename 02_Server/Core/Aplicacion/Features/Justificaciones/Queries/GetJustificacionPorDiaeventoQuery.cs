using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces.Repositories.Horario;
using Aplicacion.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Justificaciones.Queries
{
    public class GetJustificacionPorDiaeventoQuery : IRequest<Response<JustificacionDto>>
    {
        public int IdrrhDiaevento { get; set; }
    }

    public class GetJustificacionPorDiaeventoHandler
        : IRequestHandler<GetJustificacionPorDiaeventoQuery, Response<JustificacionDto>>
    {
        private readonly IRrhJustificacionOmisionRepository _repository;

        public GetJustificacionPorDiaeventoHandler(IRrhJustificacionOmisionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<JustificacionDto>> Handle(
            GetJustificacionPorDiaeventoQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetJustificacionPorDiaevento(request.IdrrhDiaevento);
        }
    }
}