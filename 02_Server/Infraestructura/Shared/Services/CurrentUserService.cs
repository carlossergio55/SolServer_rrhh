using Aplicacion.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Shared.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor http, ILogger<CurrentUserService> logger)
        {
            var user = http.HttpContext?.User;

            if (user == null)
            {
                // 👉 No hay HttpContext: típico de HostedService / background
                SetAsSystem(logger, reason: "No HttpContext (background)");
                return;
            }

            if (user.Identity?.IsAuthenticated == true)
            {
                LoginUsuario = user.FindFirst("Loguin")?.Value ?? "Anonimo";
                IdgenInstitucionsucursal = ToInt(user.FindFirst("IdSucursal")?.Value);
                NombreCompleto = user.FindFirst("NombreCompleto")?.Value ?? "";
                NroCi = user.FindFirst("NroCI")?.Value ?? "";
                Espedido = user.FindFirst("Expedido")?.Value ?? "";
                IdsegUsuarioSistema = ToInt(user.FindFirst("uid")?.Value);
                IdsegPerfil = ToInt(user.FindFirst("IdPerfil")?.Value);
                Perfil = user.FindFirst("Perfil")?.Value ?? "";
                IdgenInstitucion = ToInt(user.FindFirst("IdInstitucion")?.Value);
                Institucion = user.FindFirst("Institucion")?.Value ?? "";
                Sucursal = user.FindFirst("sucursal")?.Value ?? "";
                Estado = user.FindFirst("Estado")?.Value ?? "";
                Roles = new List<string>(); // mapear si aplican

                logger.LogInformation("Usuario autenticado: {user}", LoginUsuario);
            }
            else
            {
                // 👉 Hay HttpContext pero no autenticado: endpoint público
                SetAsAnonymous(logger);
            }
        }

        private static int ToInt(string v) => int.TryParse(v, out var n) ? n : 0;

        private void SetAsAnonymous(ILogger logger)
        {
            LoginUsuario = "Anonimo";
            IdgenInstitucionsucursal = 0;
            NombreCompleto = NroCi = Espedido = Perfil = Institucion = Sucursal = Estado = "";
            IdsegUsuarioSistema = IdsegPerfil = IdgenInstitucion = 0;
            Roles = new List<string>();
            logger.LogDebug("Solicitud sin autenticación (anónimo).");
        }

        private void SetAsSystem(ILogger logger, string reason)
        {
            // 👉 Marcamos usuario del sistema para background jobs
            LoginUsuario = "SISTEMA_AUTO";
            IdgenInstitucionsucursal = 0;
            NombreCompleto = "SISTEMA";
            NroCi = Espedido = Perfil = Institucion = Sucursal = Estado = "";
            IdsegUsuarioSistema = -1;
            IdsegPerfil = 0;
            IdgenInstitucion = 0;
            Roles = new List<string> { "BackgroundJob" };
            logger.LogInformation("Contexto de sistema para background ({reason}).", reason);
        }

        public string LoginUsuario { get; set; }
        public int IdsegUsuarioSistema { get; set; }
        public string NombreCompleto { get; set; }
        public string NroCi { get; set; }
        public string Espedido { get; set; }
        public int IdsegPerfil { get; set; }
        public string Perfil { get; set; }
        public int IdgenInstitucionsucursal { get; set; }
        public int IdgenInstitucion { get; set; }
        public string Institucion { get; set; }
        public string Sucursal { get; set; }
        public string Estado { get; set; }
        public List<string> Roles { get; set; }

        public bool EstaAutenticado => LoginUsuario != "Anonimo";
    }
}
