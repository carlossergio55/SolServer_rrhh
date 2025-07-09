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
    public class UpdateGenGrupoturnoDetalleCommand : IRequest<Response<int>>
    {
        public int IdgenGrupoturnoDetalle { get; set; }
        public int IdgenGrupoturno { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public int Orden { get; set; }

        public class Handler : IRequestHandler<UpdateGenGrupoturnoDetalleCommand, Response<int>>
        {
            private readonly IRepositoryAsync<GenGrupoturnoDetalle> _repo;

            public Handler(IRepositoryAsync<GenGrupoturnoDetalle> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateGenGrupoturnoDetalleCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdgenGrupoturnoDetalle);
                if (entity == null)
                    throw new KeyNotFoundException("Registro no encontrado");

                entity.IdgenGrupoturno = request.IdgenGrupoturno;
                entity.IdgenClasificadortipo = request.IdgenClasificadortipo;
                entity.Orden = request.Orden;

                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdgenGrupoturnoDetalle);
            }
        }
    }

}
