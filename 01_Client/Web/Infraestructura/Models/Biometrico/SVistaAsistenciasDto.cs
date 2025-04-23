using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Biometrico
{
    public class SVistaAsistenciasDto
    {
        public int IdbioAsistencia { get; set; }
        public int Uid { get; set; }
        public string NombreApellido { get; set; }
        public string Ci { get; set; }
        public string Celular { get; set; }
        public DateTime? ShiftDate { get; set; }
        public string HoraEntrada { get; set; }
        public string? HoraSalida { get; set; }

    }
}
