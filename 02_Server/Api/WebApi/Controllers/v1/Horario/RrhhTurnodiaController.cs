using Aplicacion.Features.Horario.Commands;
using Aplicacion.Features.Horario.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Horario
{
    [ApiVersion("1.0")]
    [ApiController]
    public class RrhhTurnodiaController : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll(int idgenClasificadortipo) // Parámetro agregado
        {
            var query = new GetAllRrhhTurnodiaQuery
            {
                IdgenClasificadortipo = idgenClasificadortipo
            };
            return Ok(await Mediator.Send(query));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateRrhhTurnodiaCommand cmd)
            => Ok(await Mediator.Send(cmd));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRrhhTurnodiaCommand cmd)
        {
            if (id != cmd.IdrrhhTurnodia) return BadRequest();
            return Ok(await Mediator.Send(cmd));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
            => Ok(await Mediator.Send(new DeleteRrhhTurnodiaCommand { IdrrhhTurnodia = id }));
    }
}
