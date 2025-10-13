using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Asistencia
{
    [Table("asistencia", Schema = "public")]
    public partial class SAsistencia : AuditableBaseEntity
    {
        [Key]
        public int       IdbioAsistencia { get; set; }
        public int       Uid             { get; set; }
        public long      UserId          { get; set; }
        public DateTime  Timestamp       { get; set; }
        public int       Status          { get; set; }
        public int       Punch           { get; set; }
        public string    IpBiometrico    { get; set; }
        public DateTime? HoraEntrada     { get; set; }
        public DateTime? HoraSalida      { get; set; }
        public DateTime  Fecha           { get; set; }
    }

}
