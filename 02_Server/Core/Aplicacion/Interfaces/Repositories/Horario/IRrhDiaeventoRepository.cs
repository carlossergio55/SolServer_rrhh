using Aplicacion.DTOs.Horario;
using Aplicacion.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces.Repositories.Horario
{
    public interface IRrhDiaeventoRepository
    {
        Task<Response<AsistenciaConsultaDto>> GetAsistenciaPorRango(
            int? idPersona,
            DateTime fechaInicio,
            DateTime fechaFin
        );
    }
}
