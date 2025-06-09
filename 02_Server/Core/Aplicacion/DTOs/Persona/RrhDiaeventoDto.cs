using Aplicacion.DTOs.Clasificador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Persona
{
    public class RrhDiaeventoDto
    {
        public int IdrrhDiaevento { get; set; }
        public int IdrrhPersona { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public DateTime Fecha { get; set; }
        public string? Motivo { get; set; }

        public RrhPersonaDto RrhPersona { get; set; }
        public GenClasificadortipoDto GenClasificadortipo { get; set; }
    }

}
