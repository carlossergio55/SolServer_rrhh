using Aplicacion.DTOs.Clasificador;
using Aplicacion.DTOs.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Horario
{
    public class RrhPersonaProgramacionDto
    {
        public PersonaMinDto Persona { get; set; }

        // Catálogo del turno (una sola vez)
        public GenClasificadortipoDto Turno { get; set; }
        public List<RrhhTurnodiaDto> Horarios { get; set; }

        // Programación real (fechas)
        public List<RrhDiaProgramadoDto> DiasProgramados { get; set; }
    }

}
