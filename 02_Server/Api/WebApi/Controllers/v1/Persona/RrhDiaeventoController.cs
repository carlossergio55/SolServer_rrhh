using Aplicacion.Features.Diaevento.Commands;
using Aplicacion.Features.Diaevento.Queries;
using Aplicacion.Features.Persona.Commands;
using Aplicacion.Features.Persona.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Persona
{
    [ApiVersion("1.0")]
    [ApiController]
    public class RrhDiaeventoController : BaseApiController
    {
        [HttpGet("Horaser")]
        [Authorize]
        public async Task<IActionResult> Gethoraser()
        {
            return Ok(DateTime.Now.Date);
        }
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
            => Ok(await Mediator.Send(new GetAllRrhDiaeventoQuery()));

        [HttpGet("GetByMes")]
        [Authorize]
        public async Task<IActionResult> GetByMes([FromQuery] int mes, [FromQuery] int anio)
        {
            // Validación del mes
            if (mes < 1 || mes > 12)
                return BadRequest(new { message = "El mes debe estar entre 1 y 12" });

            // Validación del año
            if (anio < 1900 || anio > 2100)
                return BadRequest(new { message = "El año no es válido" });

            var result = await Mediator.Send(new GetRrhDiaeventoByMesQuery
            {
                Mes = mes,
                Anio = anio
            });

            return Ok(result);
        }


        [HttpGet("GetUltimoTurno/{idPersona}")]
        [Authorize]
        public async Task<IActionResult> GetUltimoTurno(int idPersona)
        {
            if (idPersona <= 0)
                return BadRequest(new { message = "El ID de persona debe ser mayor a 0" });

            var result = await Mediator.Send(new GetUltimoTurnoQuery { IdPersona = idPersona });
            return Ok(result);
        }
        [HttpPost("VerificarRango")]
        [Authorize]
        public async Task<IActionResult> VerificarRango([FromBody] VerificarRangoCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("ResumenMensual/{idPersona}/{mes}/{anio}")]
        [Authorize]
        public async Task<IActionResult> ResumenMensual(int idPersona, int mes, int anio)
        {
            if (idPersona <= 0)
                return BadRequest(new { message = "El ID de persona debe ser mayor a 0" });

            if (mes < 1 || mes > 12)
                return BadRequest(new { message = "El mes debe estar entre 1 y 12" });

            if (anio < 2000 || anio > 2100)
                return BadRequest(new { message = "El año debe estar entre 2000 y 2100" });

            var result = await Mediator.Send(new ResumenMensualQuery
            {
                IdPersona = idPersona,
                Mes = mes,
                Anio = anio
            });

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateRrhDiaeventoCommand cmd)
            => Ok(await Mediator.Send(cmd));

        [HttpPost("bulk")]
        [Authorize]
        public async Task<IActionResult> PostBulk(CreateBulkRrhDiaeventoCommand command)
    => Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRrhDiaeventoCommand cmd)
        {
            if (id != cmd.IdrrhDiaevento) return BadRequest();
            return Ok(await Mediator.Send(cmd));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
            => Ok(await Mediator.Send(new DeleteRrhDiaeventoCommand { IdrrhDiaevento = id }));
    }
}
