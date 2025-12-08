using System;


namespace Aplicacion.DTOs.Persona
{
    public class UltimoTurnoDto
    {
        public DateTime? Fecha { get; set; }
        public int? IdgenClasificadortipo { get; set; }
        public string DescripcionTurno { get; set; }
        public int? IdgenGrupoturno { get; set; }
        public string NombreGrupo { get; set; }
        public int? OrdenActual { get; set; }
        public int? OrdenSiguiente { get; set; }
        public bool TieneRegistros { get; set; }
    }
}
