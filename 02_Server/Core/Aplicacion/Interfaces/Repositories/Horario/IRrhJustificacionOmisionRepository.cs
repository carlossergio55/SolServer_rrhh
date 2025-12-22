using Aplicacion.DTOs.Persona;
using Aplicacion.Wrappers;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces.Repositories.Horario
{
    public interface IRrhJustificacionOmisionRepository
    {
        /// <summary>
        /// Obtiene una justificación por ID de día evento
        /// </summary>
        Task<Response<JustificacionDto>> GetJustificacionPorDiaevento(int idrrhDiaevento);

        /// <summary>
        /// Obtiene una justificación por su ID
        /// </summary>
        Task<Response<JustificacionDto>> GetJustificacionPorId(int idrrhJustificacion);
    }
}