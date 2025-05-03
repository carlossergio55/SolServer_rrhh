using Aplicacion.DTOs.Clasificador;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Clasificador.Commands
{
    public class CreateGenClasificadorTipoCommand : IRequest<Response<int>>
    {
        public GenClasificadortipoDto _GenClasificadorTipo { get; set; }
    }

    public class CreateGenClasificadorTipoCommandHandler : IRequestHandler<CreateGenClasificadorTipoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<GenClasificadortipo> _repositoryAsync;
        private readonly IMapper _mapper;
        public CreateGenClasificadorTipoCommandHandler(IRepositoryAsync<GenClasificadortipo> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateGenClasificadorTipoCommand request, CancellationToken cancellationToken)
        {
            var nuevoRegistro = _mapper.Map<GenClasificadortipo>(request._GenClasificadorTipo);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdgenClasificadortipo);
        }
    }

    public class CreateGenClasificadorTipoCommandValidator : AbstractValidator<CreateGenClasificadorTipoCommand>
    {
        public CreateGenClasificadorTipoCommandValidator()
        {

        }
    }
}
