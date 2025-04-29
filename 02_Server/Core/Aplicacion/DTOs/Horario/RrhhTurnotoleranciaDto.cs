using Aplicacion.DTOs.Clasificador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Horario
{
    public class RrhhTurnotoleranciaDto
    {
        public int IdrrhhTurnotolerancia { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public int ToleranciaAtraso { get; set; }
        public int ToleranciaFalta { get; set; }
        public int ToleranciaSalida { get; set; }
        public int SalidaAdelantada { get; set; }
        public int Puntualidad { get; set; }
        public string Prioridad { get; set; }

        public GenClasificadortipoDto GenClasificadortipo { get; set; }
    }

}
