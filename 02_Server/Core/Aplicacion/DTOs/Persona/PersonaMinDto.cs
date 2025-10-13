using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Persona
{
    public class PersonaMinDto
    {
        public string    Nombre                 { get; set; }
        public string    ApellidoPaterno        { get; set; }
        public string    ApellidoMaterno        { get; set; }
        public int       IdgenPuestodescripcion { get; set; }
        public int?      InmediatoSuperior      { get; set; }



        public string FullName =>
            string.Join(" ", new[] { Nombre, ApellidoPaterno, ApellidoMaterno }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
