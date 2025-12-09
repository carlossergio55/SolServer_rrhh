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
    [Table("gen_grupoturno_detalle", Schema = "public")]
    public class GenGrupoturnoDetalle : AuditableBaseEntity
    {
        [Key]
        public int IdgenGrupoturnoDetalle { get; set; }

        [ForeignKey("GenGrupoturno")]
        public int IdgenGrupoturno { get; set; }

        [ForeignKey("GenClasificadortipo")]
        public int IdgenClasificadortipo { get; set; }

        public int Orden { get; set; }

        public virtual GenGrupoturno GenGrupoturno { get; set; }
        public virtual GenClasificadortipo GenClasificadortipo { get; set; }
    }
}
