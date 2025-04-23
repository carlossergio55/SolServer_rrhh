using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Vistas
{
    public class SVistaAsistenciasDto
    {
        public int IdbioAsistencia { get; set; }
        public int Uid { get; set; }
        public string NombreApellido { get; set; }
        public string Ci { get; set; }
        public string Celular { get; set; }
        public DateTime ShiftDate { get; set; }
        public string HoraEntrada { get; set; }
        public string HoraSalida { get; set; }

        //public int IdrrhPersona { get; set; }
        //public string NombreApellido { get; set; }
        //public string Ci { get; set; }
        //public DateTime? ShiftDate { get; set; }
        //public DateTime? Entrada { get; set; }
        //public DateTime? Salida { get; set; }
        //public string UsuarioCreacion { get; set; }
        //public DateTime? FechaCreacion { get; set; }
        //public string UsuarioModificacion { get; set; }
        //public DateTime? FechaModificacion { get; set; }
    }

}
