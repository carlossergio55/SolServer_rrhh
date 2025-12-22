using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces.Repositories.Horario;
using Aplicacion.Wrappers;
using Microsoft.EntityFrameworkCore;
using Persistencia.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Persistencia.Repository.Custom.Horario
{
    public class RrhJustificacionOmisionRepository : IRrhJustificacionOmisionRepository
    {
        private readonly AplicationDbContext _context;

        public RrhJustificacionOmisionRepository(AplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene una justificación por ID de día evento
        /// </summary>
        public async Task<Response<JustificacionDto>> GetJustificacionPorDiaevento(int idrrhDiaevento)
        {
            var response = new Response<JustificacionDto>();

            try
            {
                var justificacion = await (
                    from j in _context.RrhJustificacionOmision
                    join d in _context.RrhDiaevento on j.IdrrhDiaevento equals d.IdrrhDiaevento
                    join p in _context.RrhPersona on d.IdrrhPersona equals p.IdrrhPersona
                    join aprobador in _context.RrhPersona on j.UsuarioAprueba equals aprobador.IdrrhPersona into aprobadores
                    from a in aprobadores.DefaultIfEmpty()
                    where j.IdrrhDiaevento == idrrhDiaevento
                    select new JustificacionDto
                    {
                        IdrrhJustificacion = j.IdrrhJustificacion,
                        IdrrhDiaevento = j.IdrrhDiaevento,
                        IdrrhPersona = p.IdrrhPersona,
                        NombrePersona = p.NombreApellido,
                        Ci = p.Ci,
                        Fecha = d.Fecha,
                        TipoOmision = j.TipoOmision,
                        FotoAreaTrabajo = j.FotoAreaTrabajo,
                        FotoGarita = j.FotoGarita,
                        Observaciones = j.Observaciones,
                        Estado = j.Estado,
                        FechaSolicitud = j.FechaSolicitud,
                        FechaAprobacion = j.FechaAprobacion,
                        UsuarioAprueba = j.UsuarioAprueba,
                        NombreAprobador = a != null ? a.NombreApellido : null,
                        ObservacionesAprobacion = j.ObservacionesAprobacion
                    }
                ).FirstOrDefaultAsync();

                if (justificacion == null)
                {
                    response.Succeeded = false;
                    response.Message = "No se encontró justificación para el día/evento especificado";
                    return response;
                }

                response.Succeeded = true;
                response.Data = justificacion;
                response.Message = "Justificación obtenida correctamente";
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        /// <summary>
        /// Obtiene una justificación por su ID
        /// </summary>
        public async Task<Response<JustificacionDto>> GetJustificacionPorId(int idrrhJustificacion)
        {
            var response = new Response<JustificacionDto>();

            try
            {
                var justificacion = await (
                    from j in _context.RrhJustificacionOmision
                    join d in _context.RrhDiaevento on j.IdrrhDiaevento equals d.IdrrhDiaevento
                    join p in _context.RrhPersona on d.IdrrhPersona equals p.IdrrhPersona
                    join aprobador in _context.RrhPersona on j.UsuarioAprueba equals aprobador.IdrrhPersona into aprobadores
                    from a in aprobadores.DefaultIfEmpty()
                    where j.IdrrhJustificacion == idrrhJustificacion
                    select new JustificacionDto
                    {
                        IdrrhJustificacion = j.IdrrhJustificacion,
                        IdrrhDiaevento = j.IdrrhDiaevento,
                        IdrrhPersona = p.IdrrhPersona,
                        NombrePersona = p.NombreApellido,
                        Ci = p.Ci,
                        Fecha = d.Fecha,
                        TipoOmision = j.TipoOmision,
                        FotoAreaTrabajo = j.FotoAreaTrabajo,
                        FotoGarita = j.FotoGarita,
                        Observaciones = j.Observaciones,
                        Estado = j.Estado,
                        FechaSolicitud = j.FechaSolicitud,
                        FechaAprobacion = j.FechaAprobacion,
                        UsuarioAprueba = j.UsuarioAprueba,
                        NombreAprobador = a != null ? a.NombreApellido : null,
                        ObservacionesAprobacion = j.ObservacionesAprobacion
                    }
                ).FirstOrDefaultAsync();

                if (justificacion == null)
                {
                    response.Succeeded = false;
                    response.Message = "No se encontró la justificación especificada";
                    return response;
                }

                response.Succeeded = true;
                response.Data = justificacion;
                response.Message = "Justificación obtenida correctamente";
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
    }
}