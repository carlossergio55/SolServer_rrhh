using Aplicacion.DTOs.Persona;
using Aplicacion.Features.Justificacion.Commands;
using Aplicacion.Features.Justificaciones.Commands;
using Aplicacion.Features.Justificaciones.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Permisos
{
    [ApiVersion("1.0")]
    public class JustificacionController : BaseApiController    
    {
        [HttpGet("PorDiaevento")]
        public async Task<IActionResult> GetJustificacionPorDiaevento([FromQuery] int idrrhDiaevento)
             => Ok(await Mediator.Send(new GetJustificacionPorDiaeventoQuery
            {
                IdrrhDiaevento = idrrhDiaevento
            }));
        [HttpPost]
        [Consumes("multipart/form-data")]  // ✅ Importante para archivos
        public async Task<IActionResult> Post([FromForm] CrearJustificacionFormDto form)
        {
            var command = new CreateJustificacionCommand
            {
                Justificacion = new CrearJustificacionDto
                {
                    IdrrhDiaevento = form.IdrrhDiaevento,
                    TipoOmision = form.TipoOmision,
                    FotoAreaTrabajo = form.FotoAreaTrabajo,
                    FotoGarita = form.FotoGarita,
                    Observaciones = form.Observaciones
                }
            };

            return Ok(await Mediator.Send(command));
        }

        [HttpPatch("Estado/{id}")]
        public async Task<IActionResult> PatchEstado(int id, [FromBody] UpdateJustificacionEstadoCommand command)
        {
            if (id != command.IdrrhJustificacion)
                return BadRequest("El id de la ruta no coincide con el id del cuerpo de la solicitud.");

            var result = await Mediator.Send(command);
            return Ok(result);   // devuelve Response<int> con el id actualizado
        }
    }
    public class CrearJustificacionFormDto
    {
        public int IdrrhDiaevento { get; set; }
        public string TipoOmision { get; set; }
        public IFormFile FotoAreaTrabajo { get; set; }
        public IFormFile FotoGarita { get; set; }
        public string Observaciones { get; set; }
    }
}
