using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Persona
{
    public class VerificarRangoDto
    {
        public bool ExistenTurnos { get; set; }
        public int CantidadRegistros { get; set; }
        public DateTime? PrimeraFechaExistente { get; set; }
        public DateTime? UltimaFechaExistente { get; set; }
        public DateTime? FechaSugerida { get; set; }
        public bool CicloIncompleto { get; set; }
        public int? DiasCompletados { get; set; }
        public int? DiasFaltantes { get; set; }
        public int? IdgenClasificadortipoActual { get; set; }
        public string DescripcionTurnoActual { get; set; }
        public int? OrdenActualEnCiclo { get; set; }
    }
}
