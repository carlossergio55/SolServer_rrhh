using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Features.Comision.Commands;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Comision;
using MediatR;

namespace Aplicacion.Features.Comision.Commands
{
    public class DeleteRrhComisionCommand : IRequest<Response<int>>
    {
        public int IdrrhComision { get; set; }

        public class Handler : IRequestHandler<DeleteRrhComisionCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhComision> _repo;

            public Handler(IRepositoryAsync<RrhComision> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteRrhComisionCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhComision);
                if (entity == null)
                    throw new KeyNotFoundException("Comisión no encontrada DeleteCommand");

                await _repo.DeleteAsync(entity);
                return new Response<int>(entity.IdrrhComision);
            }
        }

    }
}
