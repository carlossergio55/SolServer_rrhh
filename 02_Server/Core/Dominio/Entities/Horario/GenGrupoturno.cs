using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Horario
{
    [Table("gen_grupoturno", Schema = "public")]
    public partial class GenGrupoturno : AuditableBaseEntity
    {
        [Key]
        public int IdgenGrupoturno { get; set; }

        public string Nombre { get; set; }
        public string ModoGeneracion { get; set; } // fijo, rotativo, alternado
        public int DiasLaborables { get; set; }
        public int DiasDescanso { get; set; }
    }

}
