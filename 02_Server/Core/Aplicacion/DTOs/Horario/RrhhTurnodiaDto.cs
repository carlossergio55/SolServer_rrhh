using Aplicacion.DTOs.Clasificador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Horario
{
    public class RrhhTurnodiaDto
    {
        public int IdrrhhTurnodia { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public string DiaSemana { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan TiempoExtra { get; set; }

        public GenClasificadortipoDto GenClasificadortipo { get; set; }
    }

}
