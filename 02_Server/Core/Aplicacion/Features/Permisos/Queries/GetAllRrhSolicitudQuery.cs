using Aplicacion.DTOs.Permisos;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Permisos;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Permisos.Queries
{
    public record GetAllRrhSolicitudQuery : IRequest<Response<List<SRrhSolicitudDto>>>;

    public class GetAllRrhSolicitudHandler : IRequestHandler<GetAllRrhSolicitudQuery, Response<List<SRrhSolicitudDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<SRrhSolicitud> _repository;

        public GetAllRrhSolicitudHandler(IMapper mapper, IRepositoryAsync<SRrhSolicitud> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Response<List<SRrhSolicitudDto>>> Handle(GetAllRrhSolicitudQuery request, CancellationToken cancellationToken)
        {
            var solicitud = await _repository.ListAsync(new GetAllRrhSolicitudSpecification(), cancellationToken);
            var solicitudDto = _mapper.Map<List<SRrhSolicitudDto>>(solicitud);
            return new Response<List<SRrhSolicitudDto>>(solicitudDto);
        }
    }
    public sealed class GetAllRrhSolicitudSpecification : Specification<SRrhSolicitud>
    {
        public GetAllRrhSolicitudSpecification()
        {
            Query
                .AsNoTracking()
                .Include(p => p.Persona)  
                .Include(p => p.TipoSolicitudNavigation)    
                .Include(p => p.UsuarioApruebaNavigation);  
        }
    }
}
