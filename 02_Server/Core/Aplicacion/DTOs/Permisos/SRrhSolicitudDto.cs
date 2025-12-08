using Aplicacion.DTOs.Clasificador;
using Aplicacion.DTOs.Persona;
using Dominio.Entities;
using Dominio.Entities.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Aplicacion.DTOs.Permisos
{
    public class SRrhSolicitudDto
    {
        public int IdrrhSolicitud { get; set; }
        public int IdrrhPersona { get; set; }
        public int TipoSolicitud { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Motivo { get; set; }
        public string Estado { get; set; } = "SOLICITADO";
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public int? UsuarioAprueba { get; set; }
        public string? ObservacionesAprobacion { get; set; }
        public virtual RrhPersonaDto Persona { get; set; }
        public virtual GenClasificadortipoDto TipoSolicitudNavigation { get; set; }
        public virtual RrhPersonaDto UsuarioApruebaNavigation { get; set; }
    }
}
