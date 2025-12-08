using System;

namespace Aplicacion.DTOs.Persona
{
    public class ResumenMensualDto
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public int TotalDias { get; set; }
        public int DiasRegistrados { get; set; }
        public int DiasFaltantes { get; set; }
        public UltimoTurnoInfoDto UltimoTurno { get; set; }
        public EstadisticasMensualesDto Estadisticas { get; set; }
    }

    public class UltimoTurnoInfoDto
    {
        public DateTime? Fecha { get; set; }
        public int? IdgenClasificadortipo { get; set; }
        public string Descripcion { get; set; }
        public string Abreviatura { get; set; }
    }

    public class EstadisticasMensualesDto
    {
        public int DiasLaborables { get; set; }
        public int DiasDescanso { get; set; }
        public int DiasVacaciones { get; set; }
        public int DiasBajaMedica { get; set; }
        public int DiasPermisos { get; set; }
        public int DiasFaltas { get; set; }

        // Distribución por tipo de turno
        public int DiasTurnoManana { get; set; }
        public int DiasTurnoTarde { get; set; }
        public int DiasTurnoNoche { get; set; }
        public int DiasAdministrativos { get; set; }
    }
}