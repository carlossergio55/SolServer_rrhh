using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infraestructura.Abstract;
using Infraestructura.Models.Persona;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Server.Pages.Pages.Persona
{
    public partial class Personaturno
    {
        private bool expande = false;
        private RrhDiaeventoDto _DiaEvento = new RrhDiaeventoDto();
        private List<RrhDiaeventoDto> _DiasCache = new();
        private bool _MostrarAsignacionMasiva = false;
        private List<RrhDiaeventoDto> _dias = new List<RrhDiaeventoDto>();
        private List<DateTime> _diasDelMes = new List<DateTime>();
        private List<RrhPersonaDto> _personas = new List<RrhPersonaDto>();
        private DateTime? _FechaInicio;
        private DateTime? _FechaFin;
        private PersonaMinDto? _personaSeleccionada;
        // Generar días laborables
        protected string BusquedaNombre { get; set; } = string.Empty;
        // Método para el autocomplete (ahora devuelve strings)
        protected async Task<IEnumerable<PersonaMinDto>> SearchPersonas(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 3)
                return Enumerable.Empty<PersonaMinDto>();
            try
            {
                var url = $"RrhPersona/FiltroDto?busqueda={value}";
                var response = await _Rest.GetPlainAsync<List<PersonaMinDto>>(url);
                return response ?? Enumerable.Empty<PersonaMinDto>();
            }
            catch (Exception e)
            {
                _MessageShow(e.Message, State.Error);
                return Enumerable.Empty<PersonaMinDto>();
            }
        }
        private void OnPersonaChanged(PersonaMinDto? persona)
        {
            _personaSeleccionada = persona;
            if (persona != null)
                _DiaEvento.IdrrhPersona = persona.IdrrhPersona;
        }
        private void GenerarDiasLaborables()
        {
            _DiasCache.Clear();
            if (_FechaInicio == null || _FechaFin == null) return;

            for (var fecha = _FechaInicio.Value.Date; fecha <= _FechaFin.Value.Date; fecha = fecha.AddDays(1))
            {
                if (fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;

                _DiasCache.Add(new RrhDiaeventoDto
                {
                    IdrrhPersona = _DiaEvento.IdrrhPersona,
                    IdgenClasificadortipo = 48,
                    Fecha = fecha,
                    Motivo = "Turno asignado"
                });
            }
            _MostrarAsignacionMasiva = _DiasCache.Any();
        }
        private async Task SaveDiaEvento(List<RrhDiaeventoDto> dias)
        {
            try
            {
                _Loading.Show();
                var response = await _Rest.PostAsync<int?>("RrhDiaevento/bulk", new { _RrhDiaeventos = dias });

                if (response.Succeeded)
                {
                    _DiasCache.Clear();
                    await GetDiaEventos();
                    _MessageShow($"¡{dias.Count} días guardados!", State.Success);
                }
            }
            catch (Exception ex)
            {
                _MessageShow($"Error: {ex.Message}", State.Error);
            }
            finally
            {
                _Loading.Hide();
            } 
        }

        // Guardar registro individual
        private async Task OnValidDiaEvento(EditContext ctx)
        {
            if (_DiaEvento.IdrrhDiaevento > 0)
                await UpdateDiaEvento();
            else
                await SaveDiaEvento(new List<RrhDiaeventoDto> { _DiaEvento });

            _DiaEvento = new RrhDiaeventoDto();
            ToggleExpand();
            StateHasChanged();
        }

        // Cargar datos iniciales
        protected override async Task OnInitializedAsync()
        {
            await GetDiaEventos();
            InitializeDiasDelMes();
            ExtractUniquePersons();
        }
        private void InitializeDiasDelMes()
        {
            var today = DateTime.Today;
            var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            _diasDelMes = Enumerable.Range(1, daysInMonth)
                                   .Select(day => new DateTime(today.Year, today.Month, day))
                                   .ToList();
        }

        private void ExtractUniquePersons()
        {
            _personas = _dias.Select(d => d.RrhPersona)
                             .GroupBy(p => p?.IdrrhPersona)
                             .Select(g => g.First())
                             .Where(p => p != null)
                             .ToList();
        }

        private string GetColorForEventType(int? eventType)
        {
            return eventType switch
            {
                48 => "#c8e6c9", // Mañana
                49 => "#fff9c4", // Tarde
                50 => "#ffcdd2", // Noche
                _ => "transparent"
            };
        }

        private string GetShortEventType(int? eventType)
        {
            return eventType switch
            {
                48 => "M",
                49 => "T",
                50 => "N",
                _ => ""
            };
        }

        private async Task GetDiaEventos()
        {
            try
            {
                _Loading.Show();
                var result = await _Rest.GetAsync<List<RrhDiaeventoDto>>("RrhDiaevento/GetAll");
                _Loading.Hide();
                if (result.State == State.Success)
                    _dias = result.Data;
                else
                    _MessageShow($"Error: {result.Message}", State.Error);
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow($"Excepción: {ex.Message}", State.Error);
            }
        }
        private async Task UpdateDiaEvento()
        { }

        private void ToggleExpand() => expande = !expande;

    }
}
