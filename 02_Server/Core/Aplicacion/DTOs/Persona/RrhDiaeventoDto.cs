using Aplicacion.DTOs.Clasificador;
using Aplicacion.DTOs.Horario;
using System;
using System.Collections.Generic;


namespace Aplicacion.DTOs.Persona
{
    public class RrhDiaeventoDto
    {
        public int IdrrhDiaevento { get; set; }
        public int IdrrhPersona { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public DateTime Fecha { get; set; }
        public string? Motivo { get; set; }

        public PersonaMinDto RrhPersona { get; set; }
        public GenClasificadortipoDto GenClasificadortipo { get; set; }
        public List<RrhhTurnodiaDto> TurnosDia { get; set; }
    }


}
