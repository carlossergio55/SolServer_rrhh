using Infraestructura.Models.Clasificador;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Persona
{
    public class RrhPersonaDto
    {
        public int IdrrhPersona             { get; set; }
        public string NombreApellido        { get; set; }
        public string Ci                    { get; set; }
        public string Exp                   { get; set; }
        public string Celular               { get; set; }
        public string ApellidoPaterno       { get; set; }
        public string ApellidoMaterno       { get; set; }

        public string Nombre                { get; set; }
        public string Apellido              { get; set; }
        public string Extension             { get; set; }
        public string Contrasena            { get; set; }
        public DateTime? FechaNacimiento    { get; set; }

        public int? Edad                    { get; set; }
        public string Domicilio             { get; set; }
        public string Residencia            { get; set; }
        public DateTime? InicioContrato     { get; set; }
        public string Correo                { get; set; }

        /*id*/
        public int? IdgenUnidad             { get; set; }    //Idgen UNIDAD ...
        public int? IdgenCategoria          { get; set; }
        public int? IdgenClase              { get; set; }
        public int? IdgenNivelsalarial      { get; set; }
        public int? IdgenPuestodenominacion { get; set; }
        public string? Sexo                 { get; set; }
        public int? IdgenProfesion          { get; set; }
        public int? IdgengrupoTrabajo       { get; set; }
        public int? IdgenPuestodescripcion  { get; set; }
        public char? Estado                 { get; set; } = '1';

        

        //Inmediato Superior ...
        public int? InmediatoSuperior      { get; set; }


    }
}
