using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Horario;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Horario.Commands
{
    public class DeleteGenGrupoturnoDetalleCommand : IRequest<Response<int>>
    {
        public int IdgenGrupoturnoDetalle { get; set; }

        public class Handler : IRequestHandler<DeleteGenGrupoturnoDetalleCommand, Response<int>>
        {
            private readonly IRepositoryAsync<GenGrupoturnoDetalle> _repo;

            public Handler(IRepositoryAsync<GenGrupoturnoDetalle> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteGenGrupoturnoDetalleCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdgenGrupoturnoDetalle);
                if (entity == null)
                    throw new KeyNotFoundException("Registro no encontrado");

                await _repo.DeleteAsync(entity);
                return new Response<int>(entity.IdgenGrupoturnoDetalle);
            }
        }
    }

}
