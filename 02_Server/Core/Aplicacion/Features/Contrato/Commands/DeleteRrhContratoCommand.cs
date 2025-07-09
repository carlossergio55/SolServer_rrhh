using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Contrato;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Contrato.Commands
{
    public class DeleteRrhContratoCommand : IRequest<Response<int>>
    {
        public int IdrrhhContrato { get; set; }

        public class Handler : IRequestHandler<DeleteRrhContratoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhContrato> _repo;

            public Handler(IRepositoryAsync<RrhContrato> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteRrhContratoCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhhContrato);
                if (entity == null)
                    throw new KeyNotFoundException("Contrato no encontrado");

                await _repo.DeleteAsync(entity);
                return new Response<int>(entity.IdrrhhContrato);
            }
        }
    }

}
