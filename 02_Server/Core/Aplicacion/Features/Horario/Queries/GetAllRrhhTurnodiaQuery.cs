using Aplicacion.DTOs.Horario;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Horario;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Horario.Queries
{
    public class GetAllRrhhTurnodiaQuery : IRequest<Response<List<RrhhTurnodiaDto>>> 
    {
        public int IdgenClasificadortipo { get; set; }
    }

    public class GetAllRrhhTurnodiaQueryHandler : IRequestHandler<GetAllRrhhTurnodiaQuery, Response<List<RrhhTurnodiaDto>>>
    {
        private readonly IRepositoryAsync<RrhhTurnodia> _repo;
        private readonly IMapper _mapper;

        public GetAllRrhhTurnodiaQueryHandler(IRepositoryAsync<RrhhTurnodia> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<List<RrhhTurnodiaDto>>> Handle(GetAllRrhhTurnodiaQuery request, CancellationToken cancellationToken)
        {
            // Cambio importante aquí: pasamos el parámetro al constructor de la especificación
            var spec = new RrhhTurnodiaSpecification(request.IdgenClasificadortipo);
            var list = await _repo.ListAsync(spec, cancellationToken); // Usamos la especificación con filtro
            var dto = _mapper.Map<List<RrhhTurnodiaDto>>(list);
            return new Response<List<RrhhTurnodiaDto>>(dto);
        }
    }

    public class RrhhTurnodiaSpecification : Specification<RrhhTurnodia>
    {
        public RrhhTurnodiaSpecification(int idgenClasificadortipo)
        {
            Query.Include(x => x.GenClasificadortipo)
                 .Where(x => x.IdgenClasificadortipo == idgenClasificadortipo); // Filtro agregado
        }
    }

}
