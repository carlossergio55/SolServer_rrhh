using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.BajaMedica;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entities.Persona;
using Aplicacion.Features.Persona.Commands;
using System.Threading;

namespace Aplicacion.Features.BajaMedica.Commands
{
    public class DeleteRrhBajaMedicaCommand : IRequest<Response<int>>
    {
        public int IdrrhBajaMedica { get; set; }

        public class Handler : IRequestHandler<DeleteRrhBajaMedicaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhBajaMedica> _repo;

            public Handler(IRepositoryAsync<RrhBajaMedica> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(DeleteRrhBajaMedicaCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhBajaMedica);
                if (entity == null)
                    throw new KeyNotFoundException("Baja Medica no encontrada");

                await _repo.DeleteAsync(entity);
                return new Response<int>(entity.IdrrhBajaMedica);
            }
        }

    }
}
