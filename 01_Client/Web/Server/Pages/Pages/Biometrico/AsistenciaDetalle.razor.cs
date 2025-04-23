using System.Collections.Generic;
using System.Threading.Tasks;
using Infraestructura.Abstract;
using System;
using Aplicacion.Features.Asistencia;
using Infraestructura.Models.Biometrico;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor;
using Infraestructura.Models.Authentication;
using System.Text.Json;

namespace Server.Pages.Pages.Biometrico
{
    public partial class AsistenciaDetalle
    {
        protected string BusquedaCi { get; set; } = string.Empty;
        protected DateTime? FechaInicio { get; set; } = null;
        protected DateTime? FechaFin { get; set; } = null;
        protected List<VwMarcacionBiometricoDto> Marcaciones { get; set; } = new();

        public ObjectEntity _usuarioSeg;

        protected override async Task OnInitializedAsync()
        {
            // Obtener el usuario desde localStorage
            await ObtenerNombreUsuarioDesdeLocalStorage();
            if (_usuarioSeg != null && !string.IsNullOrEmpty(_usuarioSeg.loginUsuario))
            {
                BusquedaCi = _usuarioSeg.loginUsuario;
            }
            // Fijar la fecha fin como hoy
            FechaFin = DateTime.Today;
            // Fijar la fecha inicio como el día 20 del mes anterior
            FechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 20).AddMonths(-1);
            // Ejecutar la consulta automáticamente con los filtros por defecto
            await GetMarcaciones();
            await MostrarDialogoBienvenida();
        }
        protected async Task ObtenerNombreUsuarioDesdeLocalStorage()
        {
            try
            {
                var localStorageValue = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "USER");
                if (!string.IsNullOrEmpty(localStorageValue))
                {
                    _usuarioSeg = JsonSerializer.Deserialize<ObjectEntity>(localStorageValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el usuario desde localStorage: " + ex.Message);
            }
        }


        private async Task GetMarcaciones()
        {
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "Ci", BusquedaCi },
                    { "FechaInicio", FechaInicio?.ToString("yyyy-MM-dd") ?? string.Empty },
                    { "FechaFin", FechaFin?.ToString("yyyy-MM-dd") ?? string.Empty }
                };

                var url = QueryHelpers.AddQueryString("Biometrico/GetAllbioCi", queryParams);
                var response = await _Rest.GetAsync<List<VwMarcacionBiometricoDto>>(url);

                if (response.State == State.Success && response.Data != null)
                {
                    Marcaciones = response.Data;
                }
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al obtener marcaciones: {ex.Message}", State.Error);
            }
        }

        private async Task MostrarDialogoBienvenida()
        {
            var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };

            var parameters = new DialogParameters
                {
                       {
                        "ContentText",
                       "📌 Recuerda: Esta app solo te muestra lo que marcaste en el biométrico.<br />No toma en cuenta tu horario asignado."
                    }
                };

            var dialog = DialogService.Show<DialogoSimple>("Bienvenido", parameters, options);

            await Task.Delay(10000); // Espera 10 segundos

            dialog.Close(); // Cierra el diálogo automáticamente
        }

    }
}
