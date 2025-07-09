using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Queries
{
    public class GetCumpleaniosDelMesSimpleQuery : IRequest<Response<List<RrhPersonaCumpleanieroDto>>> { }

    public class GetCumpleaniosDelMesSimpleQueryHandler : IRequestHandler<GetCumpleaniosDelMesSimpleQuery, Response<List<RrhPersonaCumpleanieroDto>>>
    {
        private readonly IRepositoryAsync<RrhPersona> _repo;

        public GetCumpleaniosDelMesSimpleQueryHandler(IRepositoryAsync<RrhPersona> repo)
        {
            _repo = repo;
        }

        public async Task<Response<List<RrhPersonaCumpleanieroDto>>> Handle(GetCumpleaniosDelMesSimpleQuery request, CancellationToken cancellationToken)
        {
            var mesActual = DateTime.Now.Month;
            var hoy = DateTime.Now;

            var personas = await _repo.ListAsync(cancellationToken);

            var cumpleanieros = personas
                .Where(p => p.FechaNacimiento.HasValue && p.FechaNacimiento.Value.Month == mesActual)
                .Select(p => new RrhPersonaCumpleanieroDto
                {
                    NombreApellido = p.NombreApellido,
                    Sexo = p.Sexo,
                    EdadQueCumple = p.FechaNacimiento.HasValue
                        ? (hoy.Month > p.FechaNacimiento.Value.Month ||
                          (hoy.Month == p.FechaNacimiento.Value.Month && hoy.Day >= p.FechaNacimiento.Value.Day)
                            ? hoy.Year - p.FechaNacimiento.Value.Year
                            : hoy.Year - p.FechaNacimiento.Value.Year - 1)
                        : 0,
                    FechaCumpleFormateada = p.FechaNacimiento.HasValue
                        ? p.FechaNacimiento.Value.ToString("dd/MM")
                        : null
                })
                .OrderBy(p => p.FechaCumpleFormateada)
                .ToList();

            return new Response<List<RrhPersonaCumpleanieroDto>>(cumpleanieros);
        }
    }
}
