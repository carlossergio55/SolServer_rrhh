using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Persona
{
    public class PersonaMinDto
    {
        public int IdrrhPersona { get; set; }
        public string NombreApellido { get; set; } = string.Empty;
    }
}
