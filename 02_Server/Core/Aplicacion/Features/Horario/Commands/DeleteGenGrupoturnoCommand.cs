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
    public class DeleteGenGrupoturnoCommand : IRequest<Response<int>>
    {
        public int IdgenGrupoturno { get; set; }

        public class Handler : IRequestHandler<DeleteGenGrupoturnoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<GenGrupoturno> _repo;

            public Handler(IRepositoryAsync<GenGrupoturno> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteGenGrupoturnoCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdgenGrupoturno);
                if (entity == null)
                    throw new KeyNotFoundException("Grupo de turno no encontrado");

                await _repo.DeleteAsync(entity);
                return new Response<int>(entity.IdgenGrupoturno);
            }
        }
    }

}
