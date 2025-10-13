using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.DTOs.Persona;
using Dominio.Entities.Persona;

namespace Aplicacion.DTOs.Comision
{
    public class RrhComisionDto
    {
        public int       IdrrhComision             { get; set; }
        public DateTime? FechaSolicitudComision    { get; set; }
        public DateTime? FechaSalidaComision       { get; set; }
        public int?       IdgenMotivoComision       { get; set; }
        public string    JustificacionComision     { get; set; }
        public int       IdrrhPersonaComision      { get; set; }
        public int?      IdgenHorarioTurnoComision { get; set; }

        public virtual   PersonaMinDto Persona     { get; set; }
    }
}
