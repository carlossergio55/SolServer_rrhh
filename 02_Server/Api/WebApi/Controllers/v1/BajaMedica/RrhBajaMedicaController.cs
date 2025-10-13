using Aplicacion.Features.BajaMedica.Commands;
using Aplicacion.Features.BajaMedica.Queries;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webapi.Controllers.v1;


namespace WebApi.Controllers.v1.BajaMedica
{

    [ApiVersion("1.0")]
    [ApiController]

    public class RrhBajaMedica : BaseApiController
    {

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        => Ok(await Mediator.Send(new GetAllRrhBajaMedicaQuery()));

        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateRrhBajaMedicaCommand cmd)
        => Ok(await Mediator.Send(cmd));


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRrhBajaMedicaCommand cmd)
        {
            if (id != cmd.IdrrhBajaMedica) return BadRequest();
            return Ok(await Mediator.Send(cmd));
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
            => Ok(await Mediator.Send(new DeleteRrhBajaMedicaCommand { IdrrhBajaMedica = id }));
    }
}
