using Infraestructura.Models.Clasificador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Horario
{
    public class GenGrupoturnoDetalleDto
    {
        public int IdgenGrupoturnoDetalle { get; set; }
        public int? IdgenGrupoturno { get; set; }
        public int? IdgenClasificadortipo { get; set; }
        public int? Orden { get; set; }

        public GenGrupoturnoDto GenGrupoturno { get; set; }
        public GenClasificadorTipoDto GenClasificadortipo { get; set; }
    }
}
