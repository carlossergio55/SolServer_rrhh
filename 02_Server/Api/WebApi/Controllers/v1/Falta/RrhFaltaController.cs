using Aplicacion.Features.Falta.Commands;
using Aplicacion.Features.Falta.Queries;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webapi.Controllers.v1;



namespace WebApi.Controllers.v1.Falta
{

    [ApiVersion("1.0")]
    [ApiController]


    public class RrhFalta : BaseApiController
    {

            [HttpGet("GetAll")]
            [Authorize]
            public async Task<IActionResult> GetAll()
            => Ok(await Mediator.Send(new GetAllRrhFaltaQuery()));


            [HttpPost]
            [Authorize]
            public async Task<IActionResult> Post(CreateRrhFaltaCommand cmd)
            => Ok(await Mediator.Send(cmd));


            [HttpPut("{id}")]
            [Authorize]
            public async Task<IActionResult> Put(int id, UpdateRrhFaltaCommand cmd)
            {
                if (id != cmd.IdrrhFalta) return BadRequest();
                return Ok(await Mediator.Send(cmd));
            }


            [HttpDelete("{id}")]
            [Authorize]
            public async Task<IActionResult> Delete(int id)
            => Ok(await Mediator.Send(new DeleteRrhFaltaCommand { IdrrhFalta = id }));
    }
    
}
