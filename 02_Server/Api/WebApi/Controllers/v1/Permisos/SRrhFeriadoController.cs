using Aplicacion.Features.Permisos.Commads;
using Aplicacion.Features.Permisos.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Permisos
{
    [ApiVersion("1.0")]
    [ApiController]
    public class SRrhFeriadoController : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllSRrhFeriadoQuery()));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateSRrhFeriadoCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateSRrhFeriadoCommand command)
        {
            if (id != command.IdrrhFeriado)
                return BadRequest();

            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteSRrhFeriadoCommand { Id = id }));
        }
    }
}
