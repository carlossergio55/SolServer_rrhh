using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Dominio.Entities.BajaMedica
{
    [Table("rrh_baja_medica", Schema = "public")]
    public partial class RrhBajaMedica : AuditableBaseEntity
    {
        [Key]
        public int        IdrrhBajaMedica    { get; set; }
        public DateTime?  FechaInicioReposo  { get; set; }
        public DateTime?  FechaFinReposo     { get; set; }
        public string     Diagnostico        { get; set; }
        public int        DiasReposo         { get; set; }
        public int        IdrrhPersona       { get; set; }
    }
}
