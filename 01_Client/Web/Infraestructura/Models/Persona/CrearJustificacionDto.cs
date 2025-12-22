using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Persona
{
    public class CrearJustificacionDto
    {
        public int IdrrhDiaevento { get; set; }
        public string TipoOmision { get; set; }  // ENTRADA, SALIDA, AMBAS
        public string FotoAreaTrabajo { get; set; }  // Ruta
        public string FotoGarita { get; set; }  // Ruta
        public string Observaciones { get; set; }
    }

    /// <summary>
    /// DTO para listar justificaciones
    /// </summary>
    public class JustificacionDto
    {
        public int IdrrhJustificacion { get; set; }
        public int IdrrhDiaevento { get; set; }
        public int IdrrhPersona { get; set; }
        public string NombrePersona { get; set; }
        public string Ci { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoOmision { get; set; }
        public string FotoAreaTrabajo { get; set; }
        public string FotoGarita { get; set; }
        public string Observaciones { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public int? UsuarioAprueba { get; set; }
        public string NombreAprobador { get; set; }
        public string ObservacionesAprobacion { get; set; }
    }
    /// <summary>
    /// DTO para cambiar el estado de una justificación
    /// </summary>
    public class CambiarEstadoJustificacionDto
    {
        public int IdrrhJustificacion { get; set; }
        public string Estado { get; set; }  // APROBADO, RECHAZADO
        public string ObservacionesAprobacion { get; set; }
    }
}
