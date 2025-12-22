using System;
using System.Text.Json;


namespace Aplicacion.DTOs.Vistas
{
    public class RrhReporteDto
    {
        public int IdrrhReporte { get; set; }
        public string TipoReporte { get; set; }
        public string Parametros { get; set; }
        public string RutaArchivo { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaGeneracion { get; set; }
    }

    public class TestReporteRequest
    {
        public string TipoReporte { get; set; }
        public string ParametrosJson { get; set; }
        public long IdReporte { get; set; }
    }
}
