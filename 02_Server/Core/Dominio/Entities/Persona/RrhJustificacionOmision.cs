using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Persona
{
    [Table("rrh_justificacion_omision", Schema = "public")]
    public partial class RrhJustificacionOmision : AuditableBaseEntity
    {
        [Key]
        public int IdrrhJustificacion { get; set; }
        [ForeignKey("RrhDiaevento")]
        public int IdrrhDiaevento { get; set; }
        public string TipoOmision { get; set; }
        public string FotoAreaTrabajo { get; set; }
        public string FotoGarita { get; set; }
        public string Observaciones { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        [ForeignKey("RrhPersonaAprueba")]
        public int? UsuarioAprueba { get; set; }
        public string ObservacionesAprobacion { get; set; }
        public virtual RrhDiaevento RrhDiaevento { get; set; }
        public virtual RrhPersona RrhPersonaAprueba { get; set; }
    }
}