using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Persona
{
    public class EstadisticasMensualesDto
    {
        public int DiasLaborables { get; set; }
        public int DiasDescanso { get; set; }
        public int DiasVacaciones { get; set; }
        public int DiasBajaMedica { get; set; }
        public int DiasPermisos { get; set; }
        public int DiasFaltas { get; set; }
        public int DiasTurnoManana { get; set; }
        public int DiasTurnoTarde { get; set; }
        public int DiasTurnoNoche { get; set; }
        public int DiasAdministrativos { get; set; }
    }
}
