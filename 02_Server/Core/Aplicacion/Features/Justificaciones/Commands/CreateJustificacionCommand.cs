using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Services;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Justificaciones.Commands
{
    public class CreateJustificacionCommand : IRequest<Response<int>>
    {
        public CrearJustificacionDto Justificacion { get; set; }

        public class Handler : IRequestHandler<CreateJustificacionCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhJustificacionOmision> _repo;
            private readonly IRepositoryAsync<RrhDiaevento> _diaeventoRepo;
            private readonly IFileService _fileService;
            private readonly IMapper _mapper;

            public Handler(
                IRepositoryAsync<RrhJustificacionOmision> repo,
                IRepositoryAsync<RrhDiaevento> diaeventoRepo,
                IFileService fileService,
                IMapper mapper)
            {
                _repo = repo;
                _diaeventoRepo = diaeventoRepo;
                _fileService = fileService;
                _mapper = mapper;
            }

            public async Task<Response<int>> Handle(CreateJustificacionCommand req, CancellationToken ct)
            {
                try
                {
                    var dto = req.Justificacion;

                    // Validar que existan los archivos
                    if (dto.FotoAreaTrabajo == null || dto.FotoAreaTrabajo.Length == 0)
                        throw new ArgumentException("Debe adjuntar la foto del área de trabajo");

                    if (dto.FotoGarita == null || dto.FotoGarita.Length == 0)
                        throw new ArgumentException("Debe adjuntar la foto de la garita");

                    // Obtener el día evento para saber la persona
                    var diaevento = await _diaeventoRepo.GetByIdAsync(dto.IdrrhDiaevento, ct);
                    if (diaevento == null)
                        throw new ArgumentException("El día evento no existe");

                    var idPersona = diaevento.IdrrhPersona;

                    // Guardar archivos
                    var rutaFotoArea = await _fileService.GuardarArchivoJustificacion(
                        dto.FotoAreaTrabajo,
                        idPersona,
                        dto.IdrrhDiaevento,
                        "area");

                    var rutaFotoGarita = await _fileService.GuardarArchivoJustificacion(
                        dto.FotoGarita,
                        idPersona,
                        dto.IdrrhDiaevento,
                        "garita");

                    // Crear entidad
                    var entity = new RrhJustificacionOmision
                    {
                        IdrrhDiaevento = dto.IdrrhDiaevento,
                        TipoOmision = dto.TipoOmision,
                        FotoAreaTrabajo = rutaFotoArea,      // Ruta relativa
                        FotoGarita = rutaFotoGarita,          // Ruta relativa
                        Observaciones = dto.Observaciones,
                        Estado = "SOLICITADO",
                        FechaSolicitud = DateTime.Now,
                        FechaAprobacion = null,
                        UsuarioAprueba = null,
                        ObservacionesAprobacion = null
                    };

                    var created = await _repo.AddAsync(entity, ct);

                    return new Response<int>(created.IdrrhJustificacion)
                    {
                        Message = "Justificación creada exitosamente"
                    };
                }
                catch (ArgumentException ex)
                {
                    return new Response<int>
                    {
                        Succeeded = false,
                        Message = ex.Message
                    };
                }
                catch (Exception ex)
                {
                    return new Response<int>
                    {
                        Succeeded = false,
                        Message = $"Error al crear justificación: {ex.Message}"
                    };
                }
            }
        }
    }
}