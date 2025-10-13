using Aplicacion.Features.Vacacion.Commands;
using Aplicacion.Features.Vacacion.Queries;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webapi.Controllers.v1;
using Aplicacion.Features.Falta.Queries;
using Aplicacion.Features.Falta.Commands;


namespace WebApi.Controllers.v1.Vacacion
{

    [ApiVersion("1.0")]
    [ApiController]
    public class RrhVacacion : BaseApiController
    {

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        => Ok(await Mediator.Send(new GetAllRrhVacacionQuery()));


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateRrhVacacionCommand cmd)
        => Ok(await Mediator.Send(cmd));


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRrhVacacionCommand cmd)
        {
            if (id != cmd.IdrrhVacacion) return BadRequest();
            return Ok(await Mediator.Send(cmd));
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        => Ok(await Mediator.Send(new DeleteRrhVacacionCommand { IdrrhVacacion = id }));
    }
}
