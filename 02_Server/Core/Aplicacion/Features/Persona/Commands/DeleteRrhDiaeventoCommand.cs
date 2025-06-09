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
    public class DeleteRrhDiaeventoCommand : IRequest<Response<int>>
    {
        public int IdrrhDiaevento { get; set; }

        public class Handler : IRequestHandler<DeleteRrhDiaeventoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhDiaevento> _repo;

            public Handler(IRepositoryAsync<RrhDiaevento> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteRrhDiaeventoCommand req, CancellationToken ct)
            {
                var entity = await _repo.GetByIdAsync(req.IdrrhDiaevento, ct);
                if (entity == null) throw new KeyNotFoundException("Registro no encontrado");

                await _repo.DeleteAsync(entity, ct);
                return new Response<int>(entity.IdrrhDiaevento);
            }
        }
    }

}
