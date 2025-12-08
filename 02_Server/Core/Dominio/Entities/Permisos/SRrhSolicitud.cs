using Dominio.Common;
using Dominio.Entities.Persona;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Permisos
{
    [Table("rrh_solicitud", Schema = "public")]
    public class SRrhSolicitud : AuditableBaseEntity
    {
        [Key]
        public int IdrrhSolicitud { get; set; }

        [ForeignKey("Persona")]
        public int IdrrhPersona { get; set; }
        [ForeignKey("TipoSolicitudNavigation")]
        public int TipoSolicitud { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Motivo { get; set; }
        public string Estado { get; set; } = "SOLICITADO";
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        [ForeignKey("UsuarioApruebaNavigation")]
        public int? UsuarioAprueba { get; set; }
        public string? ObservacionesAprobacion { get; set; }
        public virtual RrhPersona? Persona { get; set; }
        public virtual GenClasificadortipo? TipoSolicitudNavigation { get; set; }
        public virtual RrhPersona? UsuarioApruebaNavigation { get; set; }
    }
}
