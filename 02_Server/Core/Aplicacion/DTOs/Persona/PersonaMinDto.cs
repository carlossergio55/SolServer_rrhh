using System.Linq;


namespace Aplicacion.DTOs.Persona
{
    public class PersonaMinDto
    {
        public int IdrrhPersona { get; set; }
        public string    Nombre                 { get; set; }
        public string    ApellidoPaterno        { get; set; }
        public string    ApellidoMaterno        { get; set; } 
        public int       IdgenPuestodescripcion { get; set; }
        public int?      InmediatoSuperior      { get; set; }
        public int? IdgenUnidad { get; set; }
        public string Ci { get; set; }
        public string FullName =>
            string.Join(" ", new[] { Nombre, ApellidoPaterno, ApellidoMaterno }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
