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
        private int? _TurnoInicialId;                 // Selección del usuario
        private readonly int[] _secuencia165 = { 48, 49, 52 };
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
            if (_FechaInicio is null || _FechaFin is null || _personaSeleccionada is null) return;

            switch (_personaSeleccionada.IdgengrupoTrabajo)
            {
                case 167: GenerarAdministrativos(); break;
                case 165: GenerarTurnos165(); break;
                case 166: GenerarEspeciales166(); break;
                default: return;                   // grupo no contemplado
            }
            _MostrarAsignacionMasiva = _DiasCache.Any();
        }


        // === ADMINISTRATIVOS (167) ===
        private void GenerarAdministrativos()
        {
            for (var fecha = _FechaInicio!.Value.Date; fecha <= _FechaFin!.Value.Date; fecha = fecha.AddDays(1))
            {
                if (fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;

                _DiasCache.Add(NuevoDia(fecha, 48)); // día fijo
            }
        }

        // === TURNOS (165): 6x2 con rotación M-N-T ===
        private void GenerarTurnos165()
        {
            if (_TurnoInicialId is null) return;

            var idx = Array.IndexOf(_secuencia165, _TurnoInicialId.Value);
            for (var fecha = _FechaInicio!.Value.Date; fecha <= _FechaFin!.Value.Date;)
            {
                // 6 días laborables
                for (int i = 0; i < 6 && fecha <= _FechaFin; i++, fecha = fecha.AddDays(1))
                    _DiasCache.Add(NuevoDia(fecha, _secuencia165[idx]));

                // 2 días de descanso
                fecha = fecha.AddDays(2);
                // Siguiente turno de la secuencia
                idx = (idx + 1) % _secuencia165.Length;
            }
        }

        // === ESPECIALES (166): 7x7 Día/Noche alterno ===
        private void GenerarEspeciales166()
        {
            if (_TurnoInicialId is null) return;

            var turno = _TurnoInicialId.Value;          // 50 o 51
            for (var fecha = _FechaInicio!.Value.Date; fecha <= _FechaFin!.Value.Date;)
            {
                // 7 días laborables
                for (int i = 0; i < 7 && fecha <= _FechaFin; i++, fecha = fecha.AddDays(1))
                    _DiasCache.Add(NuevoDia(fecha, turno));

                // 7 días de descanso
                fecha = fecha.AddDays(7);
                // Cambia de Día ↔ Noche
                turno = (turno == 50) ? 51 : 50;
            }
        }

        // Factor común para crear el DTO
        private RrhDiaeventoDto NuevoDia(DateTime fecha, int idTurno)
            => new()
            {
                IdrrhPersona = _DiaEvento.IdrrhPersona,
                IdgenClasificadortipo = idTurno,
                Fecha = fecha,
                Motivo = "Turno asignado"
            };

        private async Task SaveDiaEvento(List<RrhDiaeventoDto> dias)
        {
            try
            {
                _Loading.Show();
                var response = await _Rest.PostAsync<int?>("RrhDiaevento/bulk", new { _RrhDiaeventos = dias });

                if (response.Succeeded)
                {
                    _DiasCache.Clear();
                   
                    _MessageShow($"¡{dias.Count} días guardados!", State.Success);
                }
                await GetDiaEventos();
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
                51 => "#d1c4e9", // Noche Especial
                52 => "#b3e5fc", // Tarde Extra
                _ => "transparent"
            };
        }

        private string GetShortEventType(int? eventType)
        {
            return eventType switch
            {
                48 => "M",   // Mañana
                49 => "T",   // Tarde
                50 => "N",   // Noche
                51 => "N*",  // Noche Especial
                52 => "N",  // Tarde Extra
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
