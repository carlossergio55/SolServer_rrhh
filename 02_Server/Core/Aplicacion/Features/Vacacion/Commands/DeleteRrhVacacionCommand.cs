using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Wrappers;
using MediatR;
using Dominio.Entities.Vacacion ;
using Aplicacion.Features.BajaMedica.Commands;
using Aplicacion.Interfaces;
using Dominio.Entities.BajaMedica;
using System.Threading;

namespace Aplicacion.Features.Vacacion.Commands
{
    public class DeleteRrhVacacionCommand : IRequest<Response<int>>
    {
        public int IdrrhVacacion { get; set; }

        public class Handler : IRequestHandler<DeleteRrhVacacionCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhVacacion> _repo;

            public Handler(IRepositoryAsync<RrhVacacion> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteRrhVacacionCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhVacacion);
                if (entity == null)
                    throw new KeyNotFoundException("Vacacion no encontrada DeleteCommand");

                await _repo.DeleteAsync(entity);
                return new Response<int>(entity.IdrrhVacacion);
            }
        }
    }
}
