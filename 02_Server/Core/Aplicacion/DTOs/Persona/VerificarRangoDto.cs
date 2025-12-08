using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Persona
{
    public class VerificarRangoDto
    {
        public bool ExistenTurnos { get; set; }
        public int CantidadRegistros { get; set; }
        public DateTime? PrimeraFechaExistente { get; set; }
        public DateTime? UltimaFechaExistente { get; set; }
        public DateTime? FechaSugerida { get; set; }

        // ✅ NUEVO: Para rotativos a medio completar
        public bool CicloIncompleto { get; set; }
        public int? DiasCompletados { get; set; }
        public int? DiasFaltantes { get; set; }
        public int? IdgenClasificadortipoActual { get; set; }
        public string DescripcionTurnoActual { get; set; }
        public int? OrdenActualEnCiclo { get; set; }
    }
}
