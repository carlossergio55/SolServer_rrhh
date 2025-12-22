using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.v1.Permisos
{
    [ApiVersion("1.0")]
    [Authorize]
    public class ArchivosController : BaseApiController
    {
        private readonly string _basePath;

        public ArchivosController(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:JustificacionesPath"]
                ?? throw new InvalidOperationException(
                    "FileStorage:JustificacionesPath no configurado");
        }

        [AllowAnonymous]
        [HttpGet("{idPersona:int}/{nombreArchivo}")]
        public IActionResult GetArchivo(int idPersona, string nombreArchivo)
        {
            if (string.IsNullOrWhiteSpace(nombreArchivo))
                return BadRequest();

            // Construir ruta física
            var rutaFisica = Path.Combine(_basePath, idPersona.ToString(), nombreArchivo);

            var fullBasePath = Path.GetFullPath(_basePath);
            var fullFilePath = Path.GetFullPath(rutaFisica);

            // Seguridad: evitar path traversal
            if (!fullFilePath.StartsWith(fullBasePath, StringComparison.Ordinal))
                return BadRequest("Ruta inválida");

            if (!System.IO.File.Exists(fullFilePath))
                return NotFound();

            var contentType = Path.GetExtension(nombreArchivo).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            var stream = new FileStream(
                fullFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            );

            return File(stream, contentType);
        }
    }
}
