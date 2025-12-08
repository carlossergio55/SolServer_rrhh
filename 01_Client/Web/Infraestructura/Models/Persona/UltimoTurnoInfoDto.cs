using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Persona
{
    public class UltimoTurnoInfoDto
    {
        public DateTime? Fecha { get; set; }
        public int? IdgenClasificadortipo { get; set; }
        public string Descripcion { get; set; }
        public string Abreviatura { get; set; }
    }
}
