using Identity.Services.Biometrico;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Webapi.Controllers.v1;
using Aplicacion.Features.Asistencia;
using Microsoft.AspNetCore.Authorization;
using Aplicacion.Features.Asistencia.Queries;

namespace WebApi.Controllers.v1.Biometrico
{
    [ApiVersion("1.0")]
    [ApiController]
    public class BiometricoController : BaseApiController
    {
        private readonly IBiometricoService _biometricoService;

        public BiometricoController(IBiometricoService biometricoService)
        {
            _biometricoService = biometricoService;
        }
        [HttpGet("ExtraerAsistencia")]
        public async Task<IActionResult> ExtraerAsistencia([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var response = await _biometricoService.ExtraerAsistenciaAsync(fechaInicio, fechaFin);
            return Ok(response);
        }

        [HttpGet("ExtraerUsuarios")]
        public async Task<IActionResult> ExtraerUsuarios()
        {
            var response = await _biometricoService.ExtraerUsuariosAsync();
            return Ok(response);
        }
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] GetAllSVistaAsistenciasQuery query)
        {
            return Ok(await Mediator.Send(query));
        } 
        
        [HttpGet("GetAllCi")]
        [Authorize]
        public async Task<IActionResult> GetAllCi([FromQuery] GetAllSVistaAsistenciasByCiQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("GetAllbioCi")]
        [Authorize]
        public async Task<IActionResult> GetAllCi([FromQuery] GetAllVwMarcacionByCiQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        // Nuevo endpoint para sugerencias
        [HttpGet("SuggestNombres")]
        [Authorize]
        public async Task<IActionResult> SuggestNombres( string searchText)
        {
            var result = await Mediator.Send(new GetSugerenciasNombresQuery
            {
                SearchText = searchText
            });

            return Ok(result);
        }
    }
}
