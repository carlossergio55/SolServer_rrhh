using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Horario
{
    public class ToleranciaDto
    {
        public int ToleranciaAtraso { get; set; }
        public int ToleranciaFalta { get; set; }
        public int ToleranciaSalida { get; set; }
        public int SalidaAdelantada { get; set; }
        public int Puntualidad { get; set; }
        public string Prioridad { get; set; }
    }
    public class TurnoDiaDto
    {
        public string DiaSemana { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan? TiempoExtra { get; set; }
    }

    public class HorarioCompletoDto
    {
        public int? IdgenClasificadortipo { get; set; }
        public string Descripcion { get; set; }
        public string Abreviatura { get; set; }

        public ToleranciaDto Tolerancia { get; set; }  // 1-a-1
        public List<TurnoDiaDto> Turnos { get; set; } = new();
    }
}
