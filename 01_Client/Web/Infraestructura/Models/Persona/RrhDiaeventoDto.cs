using Infraestructura.Models.Clasificador;
using System;

namespace Infraestructura.Models.Persona
{
    public class RrhDiaeventoDto
    {
        public int IdrrhDiaevento { get; set; }
        public int IdrrhPersona { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Motivo { get; set; }
        public RrhPersonaDto RrhPersona { get; set; }
        public GenClasificadorTipoDto GenClasificadortipo { get; set; }
    }
}
