using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Common;
using Dominio.Entities.Persona;

namespace Dominio.Entities.Comision
{
    [Table("rrh_comision", Schema = "public")]
    public partial class RrhComision :  AuditableBaseEntity
    {
        [Key]
        public int         IdrrhComision             { get; set; }
        public DateTime?   FechaSolicitudComision    { get; set; }
        public DateTime?   FechaSalidaComision       { get; set; }
        public int         IdgenMotivoComision       { get; set; }
        public string      JustificacionComision     { get; set; }
        public int         IdrrhPersonaComision      { get; set; }
        public int         IdgenHorarioTurnoComision { get; set; }
        
        [ForeignKey("IdrrhPersonaComision")]
        public virtual RrhPersona Persona            { get; set; }

    }
}
