using Infraestructura.Models.Clasificador;

namespace Infraestructura.Models.Horario
{
    public class GenGrupoturnoDetalleDto
    {
        public int IdgenGrupoturnoDetalle { get; set; }
        public int? IdgenGrupoturno { get; set; }
        public int? IdgenClasificadortipo { get; set; }
        public int? Orden { get; set; }

        public GenGrupoturnoDto GenGrupoturno { get; set; }
        public GenClasificadorTipoDto GenClasificadortipo { get; set; }
    }
}
