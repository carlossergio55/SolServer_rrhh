using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Persona
{
    [Table("rrh_persona", Schema = "public")]
    public partial class RrhPersona : AuditableBaseEntity
    {
        [Key]
        public int    IdrrhPersona   { get; set; }
        public string NombreApellido { get; set; }
        public string Ci             { get; set; }
        public string Exp            { get; set; }
        public string Celular        { get; set; }

        /// <summary>
        //////////////////////////////////////////////////////
        /// </summary>
        public string Nombre             { get; set; }
        public string Apellido           { get; set; }
        public string ApellidoPaterno    { get; set; }
        public string ApellidoMaterno    { get; set; }
        public string Extension          { get; set; }
        public string Contrasena         { get; set; }

        public DateTime? FechaNacimiento { get; set; }
        //TABLA de redesSociales, Vacaciones, compensaciones, Inicio de Contrato con las vacaciones, cantidad de tablas de contratos, adendas, cuantos dias.
        //Salario de cada contrato, ...

        public int?      Edad             { get; set; }
        public string    Domicilio        { get; set; }
        public string    Residencia       { get; set; }
        public DateTime? InicioContrato   { get; set; }
        public string    Correo           { get; set; }

        /*id*/
        public int?     IdgenUnidad             { get; set; }
        public int?     IdgenCategoria          { get; set; }
        public int?     IdgenClase              { get; set; }
        public int?     IdgenNivelsalarial      { get; set; }
        public int?     IdgenPuestodenominacion { get; set; }
        public string?  Sexo                    { get; set; }
        public int?     IdgenProfesion          { get; set; }
        public int?     IdgengrupoTrabajo       { get; set; }
        public int?     IdgenPuestodescripcion  { get; set; }
        public char?    Estado                  { get; set; }
        public int?     InmediatoSuperior       { get; set; }


        //The Full Name ...
        //public string FullName { get; set; } = "";
    }
}
