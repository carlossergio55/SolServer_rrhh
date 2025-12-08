using System;


namespace Infraestructura.Models.Horario
{
    public class GenGrupoturnoDto
    {
        public int IdgenGrupoturno { get; set; }
        public string Nombre { get; set; }
        public string ModoGeneracion { get; set; }
        public int DiasLaborables { get; set; }
        public int DiasDescanso { get; set; }
        public bool ExcluirFinesSemana { get; set; }

    }
}
