using Aplicacion.Features.Permisos.Commads;
using Aplicacion.Features.Permisos.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Permisos
{
    [ApiVersion("1.0")]
    [Authorize]
    public class RrhSolicitudController : BaseApiController
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllRrhSolicitudQuery()));
        }
 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateRrhSolicitudCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRrhSolicitudCommand command)
        {
            if (id != command.IdrrhSolicitud)
                return BadRequest();
            return Ok(await Mediator.Send(command));
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteRrhSolicitudCommand { Id = id }));
        }
        
    }
    }
