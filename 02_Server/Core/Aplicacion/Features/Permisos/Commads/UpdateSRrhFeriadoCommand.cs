using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Permisos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Permisos.Commads
{
    public class UpdateSRrhFeriadoCommand : IRequest<Response<int>>
    {
        public int IdrrhFeriado { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public bool EsNacional { get; set; }
        public bool AplicaATurnoRotativo { get; set; }
        public string? UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public class Handler : IRequestHandler<UpdateSRrhFeriadoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<SRrhFeriado> _repositoryAsync;

            public Handler(IRepositoryAsync<SRrhFeriado> repositoryAsync)
            {
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<int>> Handle(UpdateSRrhFeriadoCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repositoryAsync.GetByIdAsync(request.IdrrhFeriado);
                if (entity == null)
                    throw new KeyNotFoundException("Feriado no encontrado");

                entity.Fecha = request.Fecha;
                entity.Descripcion = request.Descripcion;
                entity.EsNacional = request.EsNacional;
                entity.AplicaATurnoRotativo = request.AplicaATurnoRotativo;
                entity.UsuarioModificacion = request.UsuarioModificacion;
                entity.FechaModificacion = request.FechaModificacion ?? DateTime.Now;

                await _repositoryAsync.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhFeriado);
            }
        }
    }

}
