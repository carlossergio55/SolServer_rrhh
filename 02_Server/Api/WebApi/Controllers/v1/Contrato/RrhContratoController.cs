using Aplicacion.Features.Contrato.Commands;
using Aplicacion.Features.Contrato.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Contrato
{
    [ApiVersion("1.0")]
    [ApiController]

    public class RrhContrato : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        => Ok(await Mediator.Send(new GetAllRrhContratoQuery()));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateRrhContratoCommand cmd)
        => Ok(await Mediator.Send(cmd));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRrhContratoCommand cmd)
        {
            if (id != cmd.IdrrhhContrato) return BadRequest();
            return Ok(await Mediator.Send(cmd));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        => Ok(await Mediator.Send(new DeleteRrhContratoCommand { IdrrhhContrato = id }));
    }
}
