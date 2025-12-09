using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Persona;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Queries
{
    public class GetPersonalACargoQuery : IRequest<Response<List<PersonaMinDto>>>
    {
        public string CiSuperior { get; set; } = string.Empty;

        public class Handler : IRequestHandler<GetPersonalACargoQuery, Response<List<PersonaMinDto>>>
        {
            private readonly IRepositoryAsync<RrhPersona> _repository;
            private readonly IMapper _mapper;

            public Handler(IRepositoryAsync<RrhPersona> repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<Response<List<PersonaMinDto>>> Handle(
                GetPersonalACargoQuery request,
                CancellationToken ct)
            {
                // ✅ 1️⃣ Buscar superior por CI (UN SOLO REGISTRO)
                var superior = await _repository.GetBySpecAsync(
                    new RrhPersonaPorCiSpecification(request.CiSuperior),
                    ct);

                if (superior == null)
                {
                    return new Response<List<PersonaMinDto>>
                    {
                        Succeeded = false,
                        Message = "Supervisor no encontrado"
                    };
                }

                // ✅ 2️⃣ Obtener personal a cargo usando el ID del superior
                var personal = await _repository.ListAsync(
                    new RrhPersonaPorSuperiorSpecification(superior.IdrrhPersona),
                    ct);

                // ✅ 3️⃣ Construir lista final (superior + personal)
                var personas = new List<RrhPersona> { superior };
                personas.AddRange(personal);

                // ✅ 4️⃣ Mapear a DTO
                var dto = _mapper.Map<List<PersonaMinDto>>(personas);

                return new Response<List<PersonaMinDto>>(dto);
            }
        }
    }

    // =========================================================================
    // SPECIFICATION: PERSONAL A CARGO
    // =========================================================================
    public class RrhPersonaPorSuperiorSpecification : Specification<RrhPersona>
    {
        public RrhPersonaPorSuperiorSpecification(int idSuperior)
        {
            Query
                .Where(p => p.InmediatoSuperior == idSuperior)
                .OrderBy(p => p.ApellidoPaterno)
                .ThenBy(p => p.ApellidoMaterno)
                .ThenBy(p => p.Nombre);
        }
    }

    // =========================================================================
    // SPECIFICATION: PERSONA POR CI (UN SOLO RESULTADO)
    // =========================================================================
    public class RrhPersonaPorCiSpecification
        : Specification<RrhPersona>, ISingleResultSpecification
    {
        public RrhPersonaPorCiSpecification(string ci)
        {
            Query.Where(p => p.Ci == ci);
        }
    }
}
