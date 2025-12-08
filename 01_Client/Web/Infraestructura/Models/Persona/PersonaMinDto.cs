namespace Infraestructura.Models.Persona
{
    public class PersonaMinDto
    {
        public int IdrrhPersona { get; set; }
        public int? IdgengrupoTrabajo { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string NombreApellido => FullName;
    }
}