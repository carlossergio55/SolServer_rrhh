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
    [Table("rrhh_turnodia", Schema = "public")]
    public partial class RrhhTurnodia : AuditableBaseEntity
    {
        [Key]
        public int IdrrhhTurnodia { get; set; }

        [ForeignKey("GenClasificadortipo")]
        public int IdgenClasificadortipo { get; set; }

        public string DiaSemana { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan TiempoExtra { get; set; } = TimeSpan.Zero;

        public virtual GenClasificadortipo GenClasificadortipo { get; set; }
    }

}
