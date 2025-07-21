using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infraestructura.Abstract;
using Infraestructura.Models.Clasificador;
using Infraestructura.Models.Horario;
using Infraestructura.Models.Persona;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Server.Pages.Pages.Persona
{
    public partial class PersonaturnoCopia
    {
        private bool expande = false;
        private RrhDiaeventoDto _DiaEvento = new RrhDiaeventoDto();
        private List<RrhDiaeventoDto> _DiasCache = new();
        private PersonaMinDto? _personaSeleccionada;
        private List<GenClasificadorTipoDto> _listaTurnos = new();
        private List<GenGrupoturnoDto> _listaGrupo = new();
        private List<GenGrupoturnoDetalleDto> _grupoDetalles = new();
        private void ToggleExpand() => expande = !expande;

        private int _grupoSeleccionado;
        private DateTime? _fechaInicio;
        private DateTime? _fechaFin;
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
        private async Task ObtenerTurnos()
        {
            var res = await _Rest.GetAsync<List<GenClasificadorTipoDto>>("Clasificador/Turno");
            if (res.State == State.Success)
                _listaTurnos = res.Data;
            else
                _MessageShow("Error: " + res.Message, State.Warning);
        }
        private async Task ObtenerGrupos()
        {
            var res = await _Rest.GetAsync<List<GenGrupoturnoDto>>("GenGrupoturno/GetAll");
            if (res.State == State.Success)
                _listaGrupo = res.Data;
            else
                _MessageShow("Error: " + res.Message, State.Warning);
        }

        private async Task ObtenerGrupoDetalles()
        {
            var res = await _Rest.GetAsync<List<GenGrupoturnoDetalleDto>>("GenGrupoturnoDetalle/GetAll");
            if (res.State == State.Success)
                _grupoDetalles = res.Data.OrderBy(x => x.Orden).ToList();
            else
                _MessageShow("Error: " + res.Message, State.Warning);
        }
        private void OnPersonaChanged(PersonaMinDto? persona)
        {
            _personaSeleccionada = persona;
            if (persona != null)
                _DiaEvento.IdrrhPersona = persona.IdrrhPersona;
        }
        private async Task OnValidDiaEvento(EditContext ctx)
        {
                await SaveDiaEvento(new List<RrhDiaeventoDto> { _DiaEvento });

            _DiaEvento = new RrhDiaeventoDto();
            ToggleExpand();
            StateHasChanged();
        }
        protected override async Task OnInitializedAsync()
        {
            await ObtenerTurnos();
            await ObtenerGrupos();
            await ObtenerGrupoDetalles();
        }

        private async Task GenerarTurnos()
        {
            if (_personaSeleccionada == null || _grupoSeleccionado == null || _fechaInicio == null || _fechaFin == null)
            {
                _MessageShow("Completa todos los campos requeridos", State.Warning);
                return;
            }

            var grupo = _listaGrupo.FirstOrDefault(x => x.IdgenGrupoturno == _grupoSeleccionado);
            var detalles = _grupoDetalles
                .Where(x => x.IdgenGrupoturno == _grupoSeleccionado)
                .OrderBy(x => x.Orden)
                .ToList();

            if (grupo == null || !detalles.Any())
            {
                _MessageShow("No se encontraron detalles del grupo", State.Warning);
                return;
            }

            var diasLaborales = grupo.DiasLaborables;
            var diasDescanso = grupo.DiasDescanso;
            var excluirFines = grupo.ExcluirFinesSemana;

            var fecha = _fechaInicio.Value;
            var turnIndex = 0;

            while (fecha <= _fechaFin.Value)
            {
                // Saltar fines de semana si corresponde
                if (excluirFines && (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday))
                {
                    fecha = fecha.AddDays(1);
                    continue;
                }

                // Asignar turnos laborales
                for (int i = 0; i < diasLaborales && fecha <= _fechaFin.Value; i++)
                {
                    if (excluirFines && (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday))
                    {
                        fecha = fecha.AddDays(1);
                        i--;
                        continue;
                    }

                    _DiasCache.Add(new RrhDiaeventoDto
                    {
                        IdrrhPersona = _personaSeleccionada.IdrrhPersona,
                        Fecha = fecha,
                        IdgenClasificadortipo = detalles[turnIndex].IdgenClasificadortipo ?? 0
                    });

                    fecha = fecha.AddDays(1);
                }

                // Saltar días de descanso
                fecha = fecha.AddDays(diasDescanso);
                turnIndex = (turnIndex + 1) % detalles.Count;
            }

            _MessageShow($"Turnos generados: {_DiasCache.Count}", State.Success);
        }
        private async Task GuardarTurnos()
        {
            if (_DiasCache == null || !_DiasCache.Any())
            {
                _MessageShow("No hay turnos para guardar", State.Warning);
                return;
            }

            await SaveDiaEvento(_DiasCache.ToList()); // Llama a tu método ya creado
        }

    }
}
