using Aplicacion.Features.Persona.Commands;
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
    public class UpdateRrhContratoCommand : IRequest<Response<int>>
    {

        public int       IdrrhhContrato   { get; set; }
        public DateTime  InicioContrato   { get; set; }
        public DateTime  FinContrato      { get; set; }
        public int       NumeroContrato   { get; set; }
        public string    TipoContrato     { get; set; }


        public class Handler : IRequestHandler<UpdateRrhContratoCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhContrato> _repo;

            public Handler(IRepositoryAsync<RrhContrato> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateRrhContratoCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhhContrato);
                if (entity == null)
                    throw new KeyNotFoundException("Contrato no encontrado Command ...");

                entity.InicioContrato  =  request.InicioContrato;
                entity.FinContrato     =  request.FinContrato;
                entity.NumeroContrato  =  request.NumeroContrato;
                entity.TipoContrato    =  request.TipoContrato;

                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhhContrato);
            }
        }
    }

}




