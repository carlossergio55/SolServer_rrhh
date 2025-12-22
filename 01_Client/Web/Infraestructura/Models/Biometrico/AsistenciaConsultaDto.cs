using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Biometrico
{
    public class AsistenciaConsultaDto
    {
        public AsistenciaParametrosDto Parametros { get; set; }
        public List<AsistenciaDiaDto> Resultados { get; set; }
        public List<ResumenPersonaDto> ResumenPorPersona { get; set; }
    }

    public class AsistenciaParametrosDto
    {
        public int? IdPersona { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class AsistenciaDiaDto
    {
        public DateTime Fecha { get; set; }
        public string Dia { get; set; }
        public int IdPersona { get; set; }
        public int IdrrhDiaevento { get; set; }
        public string Ci { get; set; }
        public string NombrePersona { get; set; }
        public string Turno { get; set; }
        public TimeSpan? HoraEntradaProgramada { get; set; }
        public TimeSpan? HoraSalidaProgramada { get; set; }
        public DateTime? MarcacionEntrada { get; set; }
        public DateTime? MarcacionSalida { get; set; }
        public int? MinutosAtraso { get; set; }
        public string Estado { get; set; }
        public int? IdJustificacion { get; set; }
        public string TipoJustificacion { get; set; }
        public string EstadoJustificacion { get; set; }
        public string JustificacionAprobadaPor { get; set; }
    }

    public class ResumenPersonaDto
    {
        public int IdPersona { get; set; }
        public string NombrePersona { get; set; }
        public string Ci { get; set; }
        public int MinutosAtrasoAcumulados { get; set; }
        public int DiasInasistencia { get; set; }
        public int OmisionesEntrada { get; set; }
        public int OmisionesSalida { get; set; }
        public decimal DiasSancionPorAtrasos { get; set; }
        public decimal DiasSancionPorInasistencias { get; set; }
        public decimal DiasSancionPorOmisiones { get; set; }
        public decimal TotalDiasSancion { get; set; }
    }
}
