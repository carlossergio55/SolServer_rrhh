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
    public class UpdateRrhFaltaCommand : IRequest<Response<int>>
    {
        public int      IdrrhFalta        { get; set; }
        public DateTime FechaInicioFalta  { get; set; }
        public DateTime FechaFinFalta     { get; set; }
        public int      DiasFalta         { get; set; }
        public int      IdrrhPersona      { get; set; }


        public class Handler : IRequestHandler<UpdateRrhFaltaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhFalta> _repo;

            public Handler(IRepositoryAsync<RrhFalta> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateRrhFaltaCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhFalta);
                if (entity == null)
                    throw new KeyNotFoundException("Falta Update Command ...");

                entity.IdrrhFalta = request.IdrrhFalta;
                entity.FechaInicioFalta = request.FechaInicioFalta;
                entity.FechaFinFalta = request.FechaFinFalta;
                entity.DiasFalta = request.DiasFalta;


                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhFalta, "Actualizado correctamente Falta Update Command...");
            }
        }





    }
}
