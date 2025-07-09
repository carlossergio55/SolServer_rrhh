using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Persona
{
    public class RrhPersonaCumpleanieroDto
    {
        public string NombreApellido { get; set; }
        public int EdadQueCumple { get; set; }
        public string FechaCumpleFormateada { get; set; }
        public string? Sexo { get; set; }
    }
}
