using Infraestructura.Models.Clasificador;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.BajaMedica
{
    public class RrhBajaMedicaDto
    {
        public int      IdrrhBajaMedica    { get; set; }

        public DateTime? FechaInicioReposo { get; set; }

        public DateTime? FechaFinReposo    { get; set; }

        public string   Diagnostico        { get; set; }
        
        public int      DiasReposo         { get; set; }

        public int      IdrrhPersona       { get; set; }




    }
}
