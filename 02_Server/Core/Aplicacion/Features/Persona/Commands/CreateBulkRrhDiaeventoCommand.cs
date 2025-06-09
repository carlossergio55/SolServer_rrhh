using Aplicacion.DTOs.Persona;
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
    public class CreateBulkRrhDiaeventoCommand : IRequest<Response<int>>
    {
        public List<RrhDiaeventoDto> _RrhDiaeventos { get; set; }
    }

    public class CreateBulkRrhDiaeventoHandler
        : IRequestHandler<CreateBulkRrhDiaeventoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<RrhDiaevento> _repo;

        public CreateBulkRrhDiaeventoHandler(IRepositoryAsync<RrhDiaevento> repo)
        {
            _repo = repo;
        }

        public async Task<Response<int>> Handle(
            CreateBulkRrhDiaeventoCommand request,
            CancellationToken cancellationToken)
        {
            int registrosInsertados = 0;

            foreach (var diaDto in request._RrhDiaeventos)
            {
                var entity = new RrhDiaevento
                {
                    IdrrhPersona = diaDto.IdrrhPersona,
                    IdgenClasificadortipo = diaDto.IdgenClasificadortipo,
                    Fecha = diaDto.Fecha,
                    Motivo = diaDto.Motivo
                };

                await _repo.AddAsync(entity, cancellationToken);
                registrosInsertados++;
            }

            return new Response<int>(registrosInsertados, $"{registrosInsertados} días creados");
        }
    }
}
