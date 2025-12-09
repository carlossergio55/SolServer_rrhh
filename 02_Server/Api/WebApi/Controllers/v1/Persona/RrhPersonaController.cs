using Aplicacion.DTOs.Persona;
using Aplicacion.Features.Persona.Commands;
using Aplicacion.Features.Persona.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Persona
{
    [ApiVersion("1.0")]
    [ApiController]
    public class RrhPersona : BaseApiController
    {


        [HttpGet("GetCumpleaniosDelMes")]
        [Authorize]
        public async Task<IActionResult> GetCumpleaniosDelMes()
        => Ok(await Mediator.Send(new GetCumpleaniosDelMesSimpleQuery()));


        [HttpGet("FiltroDto")]  
        [Authorize]
        public async Task<IActionResult> GetPersonasFiltroDto([FromQuery] string busqueda)
        {
            var result = await Mediator.Send(new GetAllRrhPersonaFiltroDtoQuery { Busqueda = busqueda });
            return Ok(result.Data);          // devolvemos directamente la lista de DTO
        }


        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        => Ok(await Mediator.Send(new GetAllRrhPersonaQuery()));


        [HttpGet("PersonaPorArea")]
        public async Task<IActionResult> GetPersonasPorUnidadDto([FromQuery] int? idgenUnidad)
        {
            var result = await Mediator.Send(new GetAllRrhPersonaPorUnidadDtoQuery
            {
                IdgenUnidad = idgenUnidad
            });

            return Ok(result.Data);  // devolvemos directamente la lista de DTO
        }


        //To get the CI ...
        [HttpGet("GetPersona/{ci}")]
        [Authorize]
        public async Task<IActionResult> GetByUnidad(string ci)
        {
            return Ok(await Mediator.Send(new GetPersonasByCiQuery(ci)));
        }

        [HttpGet("PersonalACargo/{ciSuperior}")]
        [Authorize]
        public async Task<IActionResult> GetPersonalACargo(string ciSuperior)
        {
            var result = await Mediator.Send(new GetPersonalACargoQuery
            {
                CiSuperior = ciSuperior // ✅ Ahora recibe CI en lugar de ID
            });

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result.Data);
        }

        //By ChatGPT ...
        /*[HttpGet("ByCi/{ci}")]
        [Authorize]
        public async Task<IActionResult> GetByCi([FromRoute] string ci)
        {
            var resp = await Mediator.Send(new GetPersonasByCiQuery(ci));
            return Ok(resp.Data); // return plain List<RrhPersonaDto>
        }*/



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(RrhPersonaDto dto)
        {
            var cmd = new CreateRrhPersonaCommand { _RrhPersonapost = dto };
            return Ok(await Mediator.Send(cmd));
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRrhPersonaCommand cmd)
        {
            if (id != cmd.IdrrhPersona) return BadRequest();
            return Ok(await Mediator.Send(cmd));
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
            => Ok(await Mediator.Send(new DeleteRrhPersonaCommand { IdrrhPersona = id }));

    }
}
