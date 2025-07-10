using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Contrato
{
    public class RrhContratoDto
    {
        public int?      IdrrhhContrato  { get; set; }
        public DateTime? InicioContrato  { get; set; }
        public DateTime? FinContrato     { get; set; }
        public int?      NumeroContrato  { get; set; }
        public string    TipoContrato    { get; set; }
        public int?      IdrrhhPersona   { get; set; }
    }
}

