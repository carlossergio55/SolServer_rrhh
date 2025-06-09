using Aplicacion.Features.Persona.Commands;
using Aplicacion.Features.Persona.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Persona
{
    [ApiVersion("1.0")]
    [ApiController]
    public class RrhDiaeventoController : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
            => Ok(await Mediator.Send(new GetAllRrhDiaeventoQuery()));

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
