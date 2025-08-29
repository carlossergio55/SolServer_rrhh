using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Supervisor
{
    public class RrhSupervisorDto
    {
        public int Id { get; set; }  //maps to idrrh_persona
        public string Descripcion { get; set; } = "";  //example "Juan Perez ..."
    }
}
