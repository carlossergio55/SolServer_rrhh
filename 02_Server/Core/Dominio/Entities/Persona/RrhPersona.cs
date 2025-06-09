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
    [Table("rrh_persona", Schema = "public")]
    public partial class RrhPersona : AuditableBaseEntity
    {
        [Key]
        public int IdrrhPersona { get; set; }
        public int? IdgengrupoTrabajo { get; set; }
        public string NombreApellido { get; set; }
        public string Ci { get; set; }
        public string Exp { get; set; }
        public string Celular { get; set; }
       
    }

}
