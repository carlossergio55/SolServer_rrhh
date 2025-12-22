using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Dominio.Entities.Asistencia
{
    [Table("bio_asistencia", Schema = "public")]
    public partial class SAsistencia : AuditableBaseEntity
    {
        [Key]
        [Column("idbio_asistencia")]
        public int IdbioAsistencia { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("uid")]
        public int Uid { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("punch")]
        public int Punch { get; set; }

        [Column("ip_biometrico")]
        public string IpBiometrico { get; set; }


    }
}