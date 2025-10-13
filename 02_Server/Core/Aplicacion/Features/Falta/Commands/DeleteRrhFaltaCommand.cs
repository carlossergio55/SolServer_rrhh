using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Features.BajaMedica.Commands;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.BajaMedica;
using Dominio.Entities.Falta;
using MediatR;

namespace Aplicacion.Features.Falta.Commands
{
    public class DeleteRrhFaltaCommand : IRequest<Response<int>>
    {
        public int IdrrhFalta { get; set; }

        public class Handler : IRequestHandler<DeleteRrhFaltaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhFalta> _repo;

            public Handler(IRepositoryAsync<RrhFalta> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteRrhFaltaCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhFalta);
                if (entity == null)
                    throw new KeyNotFoundException("Falta no encontrada Delete Command");

                await _repo.DeleteAsync(entity);
                return new Response<int>(entity.IdrrhFalta);
            }
        }

    }
}
