using Aplicacion.Features.Persona.Commands;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.BajaMedica;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.BajaMedica.Commands
{
    public class UpdateRrhBajaMedicaCommand : IRequest<Response<int>>
    {
        public int      IdrrhBajaMedica   { get; set; }
        public DateTime FechaInicioReposo { get; set; }
        public DateTime FechaFinReposo    { get; set; }
        public string   Diagnostico       { get; set; }

        public int      DiasReposo        { get; set; }

        //public int    IdrrhPersona      { get; set; }


        public class Handler : IRequestHandler<UpdateRrhBajaMedicaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhBajaMedica> _repo;

            public Handler(IRepositoryAsync<RrhBajaMedica> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateRrhBajaMedicaCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhBajaMedica);
                if (entity == null)
                    throw new KeyNotFoundException("Baja Medica Command ...");  

                entity.IdrrhBajaMedica = request.IdrrhBajaMedica;
                entity.FechaInicioReposo = request.FechaInicioReposo;
                entity.FechaFinReposo = request.FechaFinReposo;

                entity.Diagnostico = request.Diagnostico;
                entity.DiasReposo = request.DiasReposo;
                //entity.IdrrhPersona = request.IdrrhPersona;

                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhBajaMedica, "Actualizado correctamente Baja Medica Command...");
            }
        }




    }
}
