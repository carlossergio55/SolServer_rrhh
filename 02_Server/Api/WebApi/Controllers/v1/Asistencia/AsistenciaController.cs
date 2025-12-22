
using Aplicacion.Features.Horario.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Asistencia
{
    [ApiVersion("1.0")]

    public class AsistenciaController : BaseApiController

    {
        [HttpGet]
        public async Task<IActionResult> GetAsistencia(
     [FromQuery] int? idPersona,
     [FromQuery] DateTime? fechaInicio,
     [FromQuery] DateTime? fechaFin)
        {
            fechaInicio ??= new DateTime(2025, 11, 1);
            fechaFin ??= new DateTime(2025, 12, 15, 23, 59, 59);

            return Ok(await Mediator.Send(new GetAsistenciaQuery
            {
                IdPersona = idPersona,
                FechaInicio = fechaInicio.Value,
                FechaFin = fechaFin.Value
            }));
        }


    }
}
