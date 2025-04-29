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
    public class RrhhTurnotoleranciaController : BaseApiController

    {
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
       => Ok(await Mediator.Send(new GetAllRrhhTurnotoleranciaQuery()));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateRrhhTurnotoleranciaCommand cmd)
            => Ok(await Mediator.Send(cmd));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRrhhTurnotoleranciaCommand cmd)
        {
            if (id != cmd.IdrrhhTurnotolerancia) return BadRequest();
            return Ok(await Mediator.Send(cmd));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
            => Ok(await Mediator.Send(new DeleteRrhhTurnotoleranciaCommand { IdrrhhTurnotolerancia = id }));
    }
}
