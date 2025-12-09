using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Dominio.Entities.Horario
{
    [Table("rrhh_turnodia", Schema = "public")]
    public partial class RrhhTurnodia : AuditableBaseEntity
    {
        [Key]
        public int          IdrrhhTurnodia { get; set; }

        [ForeignKey("GenClasificadortipo")]
        public int          IdgenClasificadortipo { get; set; }

        public string       DiaSemana { get; set; }
        public TimeSpan     HoraEntrada { get; set; }
        public TimeSpan     HoraSalida { get; set; }
        public virtual GenClasificadortipo GenClasificadortipo { get; set; }
    }

}
