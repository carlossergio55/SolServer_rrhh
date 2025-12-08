using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Persona
{
    internal class PersonaCiDto
    {
        public int IdrrhPersona { get; set; }
        public string Ci { get; set; }
        public string Exp { get; set; }
        public string Celular { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public int? InmediatoSuperior { get; set; }
    }
}
