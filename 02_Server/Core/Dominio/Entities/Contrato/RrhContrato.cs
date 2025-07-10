
using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Numerics;

namespace Dominio.Entities.Contrato
{
    [Table("rrhh_contrato", Schema = "public")]
    public partial class RrhContrato : AuditableBaseEntity
    {
        [Key]
        public int       IdrrhhContrato  { get; set; }
        public DateTime? InicioContrato  { get; set; }
        public DateTime? FinContrato     { get; set; }
        public int?      NumeroContrato  { get; set; }
        public string    TipoContrato    { get; set; }
        public int?      IdrrhhPersona   { get; set; }

    }
}


