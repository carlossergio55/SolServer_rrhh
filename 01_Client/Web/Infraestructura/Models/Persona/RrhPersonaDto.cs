using Infraestructura.Models.Clasificador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Persona
{
    public class RrhPersonaDto
    {
        public int IdrrhPersona { get; set; }
        public int? IdgengrupoTrabajo { get; set; }
        public string NombreApellido { get; set; }
        public string Ci { get; set; }
        public string Exp { get; set; }
        public string Celular { get; set; }
     
    }
}
