using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Persona
{
    [Table("rrh_diaevento", Schema = "public")]
    public partial class RrhDiaevento : AuditableBaseEntity
    {
        [Key]
        public int IdrrhDiaevento { get; set; }

        [ForeignKey(nameof(RrhPersona))]
        public int IdrrhPersona { get; set; }

        [ForeignKey(nameof(GenClasificadortipo))]
        public int IdgenClasificadortipo { get; set; }

        public DateTime Fecha { get; set; }
        public string? Motivo { get; set; }

        // Navegación
        public virtual RrhPersona RrhPersona { get; set; }
        public virtual GenClasificadortipo GenClasificadortipo { get; set; }
    }
}
