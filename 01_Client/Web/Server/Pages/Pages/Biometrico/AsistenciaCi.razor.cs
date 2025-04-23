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
    public partial class AsistenciaCi
    {

        // No se está usando _sortDirection en este ejemplo de MudTable, pero puedes agregarlo según tu lógica

        // Propiedades para almacenar filtros y datos
        protected string BusquedaCi { get; set; } = string.Empty;
        protected DateTime? FechaInicio { get; set; } = null;
        protected DateTime? FechaFin { get; set; } = null;
        protected List<SVistaAsistenciasDto> AsistenciasList { get; set; } = new List<SVistaAsistenciasDto>();

        // Objeto del usuario autenticado
        public ObjectEntity _usuarioSeg;

        // Método para obtener el usuario desde localStorage
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

        // En OnInitializedAsync se obtienen los filtros por defecto y se ejecuta la consulta automáticamente
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
            await GetAsistenciasCi();
        }

        // Método para obtener las asistencias filtradas por CI y rango de fechas
        protected async Task GetAsistenciasCi()
        {
            try
            {
                var queryParams = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(BusquedaCi))
                    queryParams.Add("Busqueda", BusquedaCi);

                if (FechaInicio.HasValue)
                    queryParams.Add("FechaInicio", FechaInicio.Value.ToString("yyyy-MM-dd"));

                if (FechaFin.HasValue)
                    queryParams.Add("FechaFin", FechaFin.Value.ToString("yyyy-MM-dd"));

                var url = QueryHelpers.AddQueryString("Biometrico/GetAllCi", queryParams);

                var _result = await _Rest.GetAsync<List<SVistaAsistenciasDto>>(url);

                if (_result.State == State.Success && _result.Data != null)
                {
                    AsistenciasList = _result.Data;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al filtrar: {ex.Message}", State.Error);
            }
        }
        protected async Task GenerarReporteMarcaciones()
        {
            try
            {
                // Paso 1: Preparar los parámetros del reporte
                var parametrosReporte = new
                {
                    ruta = "/Reports/ENVIBOL/RRHH/marcaciones", // Ruta del reporte en Jasper
                    fecha_inicio = FechaInicio.HasValue ? FechaInicio.Value.ToString("yyyy-MM-dd") : "",
                    fecha_fin = FechaFin.HasValue ? FechaFin.Value.ToString("yyyy-MM-dd") : "",
                    ci = BusquedaCi   // Se utiliza el CI obtenido (loginUsuario)
                };

                // (Opcional) Puedes depurar los parámetros:
                Console.WriteLine($"ParametrosReporte: ruta={parametrosReporte.ruta}, " +
                                  $"fecha_inicio={parametrosReporte.fecha_inicio}, " +
                                  $"fecha_fin={parametrosReporte.fecha_fin}, ci={parametrosReporte.ci}");

                // Paso 2: Invocar la función JavaScript para generar la URL del reporte
                var urlReporte = await JSRuntime.InvokeAsync<string>(
                    "CargaReportePdfCi",
                    parametrosReporte
                );

                // Depuración: Verificar en la consola la URL generada
                Console.WriteLine("URL del reporte: " + urlReporte);

                // Paso 3: Abrir el diálogo para mostrar el PDF
                var parameters = new DialogParameters { { "UrlReporte", urlReporte } };
                var dialog = DialogService.Show<PdfDialog>(
                    "Reporte de Marcaciones",
                    parameters,
                    new DialogOptions
                    {
                        MaxWidth = MaxWidth.Large,
                        FullWidth = true
                    }
                );

                // Paso 4: (Opcional) Actualizar la tabla después de cerrar el diálogo
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    await GetAsistenciasCi();
                    _MessageShow("Reporte generado y tabla actualizada", State.Success);
                }
            }
            catch (Exception ex)
            {
                _MessageShow($"Error: {ex.Message}", State.Error);
            }
        }



    }
}
