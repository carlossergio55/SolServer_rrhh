using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Persona
{
    public class ResumenMensualDto
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public int TotalDias { get; set; }
        public int DiasRegistrados { get; set; }
        public int DiasFaltantes { get; set; }
        public UltimoTurnoInfoDto UltimoTurno { get; set; }
        public EstadisticasMensualesDto Estadisticas { get; set; }
    }
}
