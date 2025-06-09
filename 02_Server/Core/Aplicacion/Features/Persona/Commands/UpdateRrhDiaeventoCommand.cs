using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Commands
{
    public class UpdateRrhDiaeventoCommand : IRequest<Response<int>>
    {
        public int IdrrhDiaevento { get; set; }
        public int IdrrhPersona { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public DateTime Fecha { get; set; }
        public string? Motivo { get; set; }

        public class Handler : IRequestHandler<UpdateRrhDiaeventoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhDiaevento> _repo;

            public Handler(IRepositoryAsync<RrhDiaevento> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateRrhDiaeventoCommand req, CancellationToken ct)
            {
                var entity = await _repo.GetByIdAsync(req.IdrrhDiaevento, ct);
                if (entity == null) throw new KeyNotFoundException("Registro no encontrado");

                entity.IdrrhPersona = req.IdrrhPersona;
                entity.IdgenClasificadortipo = req.IdgenClasificadortipo;
                entity.Fecha = req.Fecha;
                entity.Motivo = req.Motivo;

                await _repo.UpdateAsync(entity, ct);
                return new Response<int>(entity.IdrrhDiaevento);
            }
        }
    }

}
