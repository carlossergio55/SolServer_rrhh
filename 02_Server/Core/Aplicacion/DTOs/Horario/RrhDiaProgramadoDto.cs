using Aplicacion.DTOs.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Horario
{
    public class RrhDiaProgramadoDto
    {
        public int IdrrhDiaevento { get; set; }
        public DateTime Fecha { get; set; }
        public string? Motivo { get; set; }
        public int IdgenClasificadortipo { get; set; }
    }
    public class RrhDiaCalendarioDto
    {
        public DateTime Fecha { get; set; }
        public string DiaSemana { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public string Estado { get; set; }   // OF, D, etc.
        public string? Motivo { get; set; }
    }
    public class RrhPersonaCalendarioDto
    {
        public PersonaMinDto Persona { get; set; }
        public List<RrhDiaCalendarioDto> Dias { get; set; }
    }

}
