using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Horario
{
    public class GenGrupoturnoDto
    {
        public int IdgenGrupoturno { get; set; }
        public string Nombre { get; set; }
        public string ModoGeneracion { get; set; }
        public int DiasLaborables { get; set; }
        public int DiasDescanso { get; set; }
        public bool? ExcluirFinesSemana { get; set; }

    }
}
