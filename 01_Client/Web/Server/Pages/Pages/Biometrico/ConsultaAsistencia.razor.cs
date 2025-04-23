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

namespace Server.Pages.Pages.Biometrico
{
    public partial class ConsultaAsistencia
    {

        // Instancia del query con parámetros para la búsqueda y el rango de fechas.
        private GetAllSVistaAsistenciasQuery query = new GetAllSVistaAsistenciasQuery();
        private List<SVistaAsistenciasDto> _asistencias = new();

        // Método que realiza la consulta utilizando los parámetros del query.
        protected List<SVistaAsistenciasDto> AsistenciasList { get; set; } = new List<SVistaAsistenciasDto>();

        // Campos que capturamos desde la página para filtrar
        protected string Busqueda { get; set; } = string.Empty;
        protected DateTime? FechaInicio { get; set; } = null;
        protected DateTime? FechaFin { get; set; } = null;

        protected string BusquedaNombre { get; set; } = string.Empty;
        // Método para el autocomplete (ahora devuelve strings)
        protected async Task<IEnumerable<string>> SearchNombres(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 1)
                return Enumerable.Empty<string>();

            try
            {
                var url = $"Biometrico/SuggestNombres?searchText={value}";
                var _result = await _Rest.GetAsync<List<string>>(url);

                if (_result.State == State.Success)
                {
                    return _result.Data;
                }
                else
                {
                    _MessageShow(_result.Message, _result.State);
                    return Enumerable.Empty<string>();
                }
            }
            catch (Exception e)
            {
                _MessageShow(e.Message, State.Error);
                return Enumerable.Empty<string>();
            }
        }
        protected async Task GetAsistencias()
        {
            try
            {
                var queryParams = new Dictionary<string, string>();

                // Filtro principal por nombre (prioritario)
                if (!string.IsNullOrEmpty(BusquedaNombre))
                {
                    queryParams.Add("NombreSearch", BusquedaNombre);
                }
                else if (!string.IsNullOrEmpty(Busqueda)) 
                {
                    queryParams.Add("Busqueda", Busqueda);
                }

                // Filtros de fecha
                if (FechaInicio.HasValue)
                    queryParams.Add("FechaInicio", FechaInicio.Value.ToString("yyyy-MM-dd"));

                if (FechaFin.HasValue)
                    queryParams.Add("FechaFin", FechaFin.Value.ToString("yyyy-MM-dd"));

                var url = QueryHelpers.AddQueryString("Biometrico/GetAll", queryParams);

                var _result = await _Rest.GetAsync<List<SVistaAsistenciasDto>>(url);

                if (_result.State == State.Success && _result.Data != null)
                {
                    AsistenciasList = _result.Data
                        .Where(x => x.NombreApellido == BusquedaNombre || string.IsNullOrEmpty(BusquedaNombre))
                        .ToList();

                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                _MessageShow("Error al filtrar: " + ex.Message, State.Error);
            }
        }
        protected async Task FiltrarAsistenciasYGenerarReporte()
        {
            try
            {
                // ================== VALIDACIONES INICIO ==================
                var errores = new List<string>();

                if (string.IsNullOrWhiteSpace(BusquedaNombre))
                    errores.Add("Debe seleccionar un nombre");

                if (!FechaInicio.HasValue)
                    errores.Add("Fecha de inicio es requerida");

                if (!FechaFin.HasValue)
                    errores.Add("Fecha fin es requerida");

                if (errores.Any())
                {
                    _MessageShow(string.Join("<br>", errores), State.Error);
                    return;
                }

                if (FechaInicio > FechaFin)
                {
                    _MessageShow("La fecha de inicio no puede ser mayor a la fecha fin", State.Error);
                    return;
                }
                // ================== VALIDACIONES FIN ==================

                // Paso 1: Generar reporte PDF
                var parametrosReporte = new
                {
                    ruta = "/Reports/ENVIBOL/RRHH/Asistencia",
                    fecha_inicio = FechaInicio.Value.ToString("yyyy-MM-dd"),
                    fecha_fin = FechaFin.Value.ToString("yyyy-MM-dd"),
                    nombre_apellido = BusquedaNombre
                };

                var urlReporte = await JSRuntime.InvokeAsync<string>(
                    "CargaReportePdf",
                    parametrosReporte
                );

                // Paso 2: Mostrar PDF
                var parameters = new DialogParameters { { "UrlReporte", urlReporte } };
                var dialog = DialogService.Show<PdfDialog>("Reporte de Asistencias", parameters, new DialogOptions
                {
                    MaxWidth = MaxWidth.Large,
                    FullWidth = true
                });

                // Paso 3: Actualizar tabla después de cerrar PDF
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    await GetAsistencias();
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
