using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.DTOs.Persona;
using Dominio.Entities.Persona;

namespace Aplicacion.DTOs.Vacacion
{
    public class RrhVacacionDto
    {
        public int       IdrrhVacacion          { get; set; }
        public DateTime? FechaSolicitudVacacion { get; set; }
        public DateTime? FechaInicioVacacion    { get; set; }
        public DateTime? FechaFinVacacion       { get; set; }
        public string    AutorizacionLugar      { get; set; }
        public DateTime? AutorizacionFecha      { get; set; }

        //public char      EstadoVacacion       { get; set; }
        public int       IdrrhPersonaVac        { get; set; }
        public int?      IdgenHorarioturno      { get; set; }
        public virtual   PersonaMinDto Persona  { get; set; }
    }
}
