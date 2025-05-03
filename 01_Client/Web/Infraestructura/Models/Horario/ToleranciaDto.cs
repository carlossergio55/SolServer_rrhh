using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Horario
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
    public class HorarioCompletoDto
    {
        public int? IdgenClasificadortipo { get; set; }
        public string Descripcion { get; set; }
        public string Abreviatura { get; set; }
        public ToleranciaDto Tolerancia { get; set; }
        public List<TurnoDiaDto> Turnos { get; set; } = new();
    }

    // Para enviarlo tal cual al API usa string en las horas ⇒ “08:00:00”
    public class TurnoDiaDto
    {
        public string DiaSemana { get; set; }
        public string HoraEntrada { get; set; } 
        public string HoraSalida { get; set; }
    }

    
}
