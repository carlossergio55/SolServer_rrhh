using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Falta
{

    [Table("rrh_falta", Schema = "public")]
    public partial class RrhFalta : AuditableBaseEntity
    {
        [Key]
        public int       IdrrhFalta        { get; set; }
        public DateTime? FechaInicioFalta  { get; set; }
        public DateTime? FechaFinFalta     { get; set; }
        public int       DiasFalta         { get; set; }
        public int       IdrrhPersona      { get; set; }
    }
}
