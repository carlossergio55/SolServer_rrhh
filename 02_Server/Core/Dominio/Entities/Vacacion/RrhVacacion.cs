using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Common;
using Dominio.Entities.Persona;

namespace Dominio.Entities.Vacacion
{
    [Table("rrh_vacacion", Schema = "public")]
    public partial class RrhVacacion : AuditableBaseEntity
    {
        [Key]
        public int       IdrrhVacacion             { get; set; }
        public DateTime? FechaSolicitudVacacion    { get; set; }     
        public DateTime? FechaInicioVacacion       { get; set; }
        public DateTime? FechaFinVacacion          { get; set; }   
        public string    AutorizacionLugar         { get; set; }
        public DateTime? AutorizacionFecha         { get; set; }

        //public char      EstadoVacacion          { get; set; }
        public int       IdrrhPersonaVac           { get; set; }
        /*id*/
        public int       IdgenHorarioturno         { get; set; }


        // Propiedad de navegación
        [ForeignKey("IdrrhPersonaVac")]
        public virtual RrhPersona Persona          { get; set; }

    }
}
