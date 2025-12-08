using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Permisos
{
    [Table("rrh_feriado", Schema = "public")]
    public partial class SRrhFeriado : AuditableBaseEntity
    {
        [Key]
        public int IdrrhFeriado { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [MaxLength(255)]
        public string Descripcion { get; set; }

        public bool EsNacional { get; set; } = true;

        public bool AplicaATurnoRotativo { get; set; } = false;

    }

}
