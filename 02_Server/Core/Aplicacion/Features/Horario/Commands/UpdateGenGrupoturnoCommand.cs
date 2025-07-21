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
    public class UpdateGenGrupoturnoCommand : IRequest<Response<int>>
    {
        public int IdgenGrupoturno { get; set; }
        public string Nombre { get; set; }
        public string ModoGeneracion { get; set; }
        public int DiasLaborables { get; set; }
        public int DiasDescanso { get; set; }
        public bool? ExcluirFinesSemana { get; set; }

        public class Handler : IRequestHandler<UpdateGenGrupoturnoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<GenGrupoturno> _repo;

            public Handler(IRepositoryAsync<GenGrupoturno> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateGenGrupoturnoCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdgenGrupoturno);
                if (entity == null)
                    throw new KeyNotFoundException("Grupo de turno no encontrado");

                entity.Nombre = request.Nombre;
                entity.ModoGeneracion = request.ModoGeneracion;
                entity.DiasLaborables = request.DiasLaborables;
                entity.DiasDescanso = request.DiasDescanso;
                entity.ExcluirFinesSemana = request.ExcluirFinesSemana;

                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdgenGrupoturno);
            }
        }
    }

}
