using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Wrappers;
using Aplicacion.DTOs.Contrato;

namespace Aplicacion.DTOs.Contrato
{
    public class RrhContratoDto
    {
        public int         IdrrhhContrato { get; set; }
        public DateTime?   InicioContrato  { get; set; }
        public DateTime?   FinContrato     { get; set; }
        public int?        NumeroContrato  { get; set; }
        public string?     TipoContrato    { get; set; }
        public int?        IdrrhhPersona   { get; set; }
    }
}
