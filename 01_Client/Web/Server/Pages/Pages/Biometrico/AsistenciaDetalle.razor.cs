using System.Collections.Generic;
using System.Threading.Tasks;
using Infraestructura.Abstract;
using System;
using Infraestructura.Models.Biometrico;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor;
using Infraestructura.Models.Authentication;
using System.Text.Json;

namespace Server.Pages.Pages.Biometrico
{
    public partial class AsistenciaDetalle
    {
        protected string    BusquedaCi    { get; set; } = string.Empty;
        protected DateTime? FechaInicio   { get; set; } = null;
        protected DateTime? FechaFin      { get; set; } = null;



        public ObjectEntity _usuarioSeg;  //A  field
        protected List<VwMarcacionBiometricoDto> Marcaciones { get; set; } = new();  //A property

        protected override async Task OnInitializedAsync()
        {
            //Obtener el usuario desde localStorage
            await ObtenerNombreUsuarioDesdeLocalStorage();
            if (_usuarioSeg != null && !string.IsNullOrEmpty(_usuarioSeg.loginUsuario))
            {
                BusquedaCi = _usuarioSeg.loginUsuario;
            }
            FechaFin = DateTime.Today.AddDays(1);
            //Fijar la fecha inicio como el día 20 del mes anterior
            FechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 20).AddMonths(-1);
            //Ejecutar la consulta automáticamente con los filtros por defecto
            await GetMarcaciones();
            // ❌ ELIMINADO: await MostrarDialogoBienvenida();
        }


        protected async Task ObtenerNombreUsuarioDesdeLocalStorage()
        {
            try
            {
                var localStorageValue = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "USER");
                if (!string.IsNullOrEmpty(localStorageValue))
                {
                    _usuarioSeg = JsonSerializer.Deserialize<ObjectEntity>(localStorageValue);

                    // Solo en desarrollo - eliminar en producción
#if DEBUG
                    _MessageShow($"Usuario: {_usuarioSeg?.nombreCompleto}", State.Success);
#endif
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario desde localStorage: {ex.Message}");
                _usuarioSeg = null;
            }
        }

        private async Task GetMarcaciones()
        {
            try
            {
                var queryParams = new Dictionary<string, string>
        {
            { "Ci", BusquedaCi ?? string.Empty },
            { "FechaInicio", FechaInicio?.ToString("yyyy-MM-dd") ?? string.Empty },
            { "FechaFin", FechaFin?.ToString("yyyy-MM-dd") ?? string.Empty }
        };

                var url = QueryHelpers.AddQueryString("Biometrico/GetAllbioCi", queryParams);
                var response = await _Rest.GetAsync<List<VwMarcacionBiometricoDto>>(url);

                if (response.State == State.Success && response.Data != null)
                {
                    Marcaciones = response.Data;

                    // Solo en desarrollo - eliminar en producción
#if DEBUG
                    foreach (var m in Marcaciones)
                    {
                        Console.WriteLine($"{m.NombreApellido} - {m.Timestamp} - {m.TipoRegistro}");
                    }
#endif
                }
                else
                {
                    Marcaciones = new(); // Lista vacía si no hay datos
                }
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al obtener marcaciones: {ex.Message}", State.Error);
                Marcaciones = new(); // Lista vacía en caso de error
            }
        }
        private async Task MostrarDialogoBienvenida()
        {
            var options = new DialogOptions
            {
                CloseButton = false,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseOnEscapeKey = false,
                DisableBackdropClick = true
            };



            var parameters = new DialogParameters
            {
                { "ContentText", "Configuración Inicial" } // Mantenemos el parámetro aunque no se use
            };

            var dialog = DialogService.Show<DialogoSimple>("", parameters, options);
            await Task.Delay(10000);
            dialog.Close();
        }
    }
}
