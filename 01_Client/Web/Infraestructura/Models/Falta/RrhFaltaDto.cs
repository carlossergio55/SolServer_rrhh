using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Falta
{
    public class RrhFaltaDto
    {
        public int       IdrrhFalta       { get; set; }
        public DateTime? FechaInicioFalta { get; set; }
        public DateTime? FechaFinFalta    { get; set; }
        public int       DiasFalta        { get; set; }
        public int       IdrrhPersona     { get; set; }
    }
}
