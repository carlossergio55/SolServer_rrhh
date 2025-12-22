using System;

namespace Aplicacion.DTOs.Permisos
{
    public class SRrhFeriadoDto
    {
        public int IdrrhFeriado { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public bool EsNacional { get; set; }
        public bool AplicaATurnoRotativo { get; set; }
    }

}
