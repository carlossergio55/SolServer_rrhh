using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Vistas
{
    [Table("vw_asistencia_diaria", Schema = "public")]
    public partial class SVistaAsistencias
    {
        [Key]
        public int IdbioAsistencia { get; set; }
        public int Uid { get; set; }
        public string NombreApellido { get; set; }
        public string Ci { get; set; }
        public string Celular { get; set; }
        public DateTime ShiftDate { get; set; }
        public string HoraEntrada { get; set; }
        public string HoraSalida { get; set; }
        public DateTime FechaRealEntrada { get; set; }
        public DateTime? FechaRealSalida { get; set; } 

    }

}
