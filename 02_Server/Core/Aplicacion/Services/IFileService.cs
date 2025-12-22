using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Services
{
    public interface IFileService
    {
        Task<string> GuardarArchivoJustificacion(IFormFile archivo, int idPersona, int idDiaevento, string tipoFoto);
        bool EliminarArchivo(string ruta);
    }

    public class FileService : IFileService
    {
        private readonly string _basePath;
        private const int MAX_MB = 10;

        public FileService(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:JustificacionesPath"]
                ?? throw new InvalidOperationException("FileStorage:JustificacionesPath no configurado");

            Directory.CreateDirectory(_basePath);
        }

        public async Task<string> GuardarArchivoJustificacion(
            IFormFile archivo,
            int idPersona,
            int idDiaevento,
            string tipoFoto)
        {
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("Archivo inválido");

            if (archivo.Length > MAX_MB * 1024 * 1024)
                throw new ArgumentException("Archivo mayor a 10MB");

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            var permitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

            if (!permitidas.Contains(extension))
                throw new ArgumentException("Extensión no permitida");

            var carpetaPersona = Path.Combine(_basePath, idPersona.ToString());
            Directory.CreateDirectory(carpetaPersona);

            var nombreArchivo = $"{Guid.NewGuid()}_{tipoFoto}{extension}";
            var rutaFisica = Path.Combine(carpetaPersona, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // ✅ SOLO RUTA LÓGICA (BD / frontend)
            return $"{idPersona}/{nombreArchivo}";
        }

        public bool EliminarArchivo(string rutaRelativa)
        {
            if (string.IsNullOrWhiteSpace(rutaRelativa))
                return false;

            var rutaSegura = rutaRelativa.Replace('\\', '/');
            var rutaFisica = Path.Combine(_basePath, rutaSegura);

            if (!rutaFisica.StartsWith(_basePath))
                return false;

            if (!File.Exists(rutaFisica))
                return false;

            File.Delete(rutaFisica);
            return true;
        }
    }


}
