using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;


namespace Dominio.Entities.Vistas
{
    [Table("rrh_reporte", Schema = "public")]
    public partial class RrhReporte : AuditableBaseEntity
    {
        [Key]
        public int IdrrhReporte { get; set; }
        public string TipoReporte { get; set; }
        public string Parametros { get; set; }
        public string RutaArchivo { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaGeneracion { get; set; }
    }
}
