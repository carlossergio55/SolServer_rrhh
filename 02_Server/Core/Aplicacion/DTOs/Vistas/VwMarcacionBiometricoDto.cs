using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Vistas
{
    public class VwMarcacionBiometricoDto
    {
        public long UserId { get; set; }
        public int IdrrhPersona { get; set; }
        public string NombreApellido { get; set; }
        public string Ci { get; set; }
        public string Celular { get; set; }
        public DateTime Timestamp { get; set; }
        public string Hora { get; set; }
        public string IpBiometrico { get; set; }
        public int Punch { get; set; }
        public string TipoRegistro { get; set; }
    }

}
