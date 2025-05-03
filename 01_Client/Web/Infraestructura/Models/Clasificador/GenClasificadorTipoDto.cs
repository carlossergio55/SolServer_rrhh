using Infraestructura.Models.Horario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Clasificador
{
    public class GenClasificadorTipoDto
    {
        public int IdgenClasificadortipo { get; set; }
        public string Descripcion { get; set; }
        public string Abreviatura { get; set; }
        public List<RrhhTurnodiaDto> Turnos { get; set; } = new List<RrhhTurnodiaDto>();
    }
}
