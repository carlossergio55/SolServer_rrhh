using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Horario
{
    [Table("rrhh_turnotolerancia", Schema = "public")]
    public partial class RrhhTurnotolerancia : AuditableBaseEntity
    {
        [Key]
        public int IdrrhhTurnotolerancia { get; set; }

        [ForeignKey("GenClasificadortipo")]
        public int IdgenClasificadortipo { get; set; }

        public int ToleranciaAtraso { get; set; }
        public int ToleranciaFalta { get; set; }
        public int ToleranciaSalida { get; set; }
        public int SalidaAdelantada { get; set; }
        public int Puntualidad { get; set; }
        public string Prioridad { get; set; }

        public virtual GenClasificadortipo GenClasificadortipo { get; set; }
    }

}
