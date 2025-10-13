using Aplicacion.Features.Comision.Commands;
using Aplicacion.Features.Comision.Queries;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webapi.Controllers.v1;
using Aplicacion.Features.Vacacion.Queries;
using Aplicacion.Features.Vacacion.Commands;

namespace WebApi.Controllers.v1.Comision
{

    [ApiVersion("1.0")]
    [ApiController]
    public class RrhComision : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        => Ok(await Mediator.Send(new GetAllRrhComisionQuery()));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateRrhComisionCommand cmd)
        => Ok(await Mediator.Send(cmd));


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRrhComisionCommand cmd)
        {
            if (id != cmd.IdrrhComision) return BadRequest();
            return Ok(await Mediator.Send(cmd));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        => Ok(await Mediator.Send(new DeleteRrhComisionCommand { IdrrhComision = id }));


    }
}
