using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infraestructura.Abstract;
using Infraestructura.Models.Clasificador;
using Infraestructura.Models.Horario;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Server.Pages.Pages.Horario
{
    public partial class GenClasificadorTipo
    {
        /* ────────────  CAMPOS  ──────────── */

        private bool expande = false;

        private GenClasificadorTipoDto _tipo = new();
        private ToleranciaDto _tolerancia = new();
        private readonly string[] _diasSemana =
            { "Lunes","Martes","Miércoles","Jueves","Viernes","Sábado","Domingo" };
        private List<TurnoDiaDto> _turnos = new();

        private readonly List<string> prioridades = new() { "BAJA", "NORMAL", "ALTA" };
        private List<GenClasificadorTipoDto> _listaTipos = new();
        /* ────────────  INICIALIZACIÓN  ──────────── */

        protected override async Task OnInitializedAsync()
        {
            await CargarTipos();
            InicializarDias();
        }


        private void InicializarDias()
        {
            _turnos = _diasSemana.Select(d => new TurnoDiaDto
            {
                DiaSemana = d,
                HoraEntrada = "",
                HoraSalida = ""
            }).ToList();
        }

        /* ────────────  UTILIDADES UI  ──────────── */

        private void ToggleExpand() => expande = !expande;

        private void LimpiarTodo()
        {
            _tipo = new();
            _tolerancia = new();
            InicializarDias();
        }

        /* ────────────  BINDING TimePicker  ──────────── */

        private static TimeSpan? Parse(string s) =>
            TimeSpan.TryParse(s, out var ts) ? ts : (TimeSpan?)null;
        private static string F(TimeSpan? t) =>
            t.HasValue ? t.Value.ToString(@"hh\:mm\:ss") : "";

        private TimeSpan? GetEntrada(TurnoDiaDto r) => Parse(r.HoraEntrada);
        private TimeSpan? GetSalida(TurnoDiaDto r) => Parse(r.HoraSalida);
        private bool _propagarPrimerDia = true;

        private void SetEntrada(TurnoDiaDto r, TimeSpan? t)
        {
            r.HoraEntrada = F(t);
            if (_propagarPrimerDia && r == _turnos.First()) PropagarDesde(r);
        }
        private void SetSalida(TurnoDiaDto r, TimeSpan? t)
        {
            r.HoraSalida = F(t);
            if (_propagarPrimerDia && r == _turnos.First()) PropagarDesde(r);
        }

        /* ────────────  PROPAGACIÓN  ──────────── */

        private void PropagarDesde(TurnoDiaDto origen)
        {
            foreach (var d in _turnos.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(d.HoraEntrada))
                    d.HoraEntrada = origen.HoraEntrada;
                if (string.IsNullOrWhiteSpace(d.HoraSalida))
                    d.HoraSalida = origen.HoraSalida;
            }
            StateHasChanged();
        }

        /* ────────────  GUARDAR HORARIO COMPLETO  ──────────── */


        private async Task GuardarHorarioCompleto()
        {
            // 1) Validar datos básicos y tolerancias
            if (!ValidarCampos())
                return;   // sale si hay errores

            // 2) Construir DTO y llamar al API
            var dto = new HorarioCompletoDto
            {
                IdgenClasificadortipo = _tipo.IdgenClasificadortipo == 0 ? null : _tipo.IdgenClasificadortipo,
                Descripcion = _tipo.Descripcion,
                Abreviatura = _tipo.Abreviatura,
                Tolerancia = _tolerancia,
                Turnos = _turnos
            };

            _Loading.Show();
            var resp = await _Rest.PostAsync<int?>("Clasificador/completo", new { horario = dto });
            _Loading.Hide();

            _MessageShow(resp.Message, resp.State);
            if (resp.State == State.Success)
                LimpiarTodo();
        }

        private void QuitarDia(TurnoDiaDto dia)
        {
            _turnos.Remove(dia);
            if (!_turnos.Any())
            {
                _propagarPrimerDia = false;  
            }

            StateHasChanged();
        }

        private bool ValidarCampos()
        {
            bool ok = true;

            // Descripción obligatoria
            if (string.IsNullOrWhiteSpace(_tipo.Descripcion))
            {
                _MessageShow("La descripción es obligatoria.", State.Warning);
                ok = false;
            }

            // Abreviatura obligatoria
            if (string.IsNullOrWhiteSpace(_tipo.Abreviatura))
            {
                _MessageShow("La abreviatura es obligatoria.", State.Warning);
                ok = false;
            }

            // Prioridad obligatoria
            if (string.IsNullOrWhiteSpace(_tolerancia.Prioridad))
            {
                _MessageShow("Debe seleccionar una prioridad.", State.Warning);
                ok = false;
            }

            // Valores de tolerancia no negativos (sólo como ejemplo)
            if (_tolerancia.ToleranciaAtraso < 0 ||
                _tolerancia.ToleranciaFalta < 0 ||
                _tolerancia.ToleranciaSalida < 0)
            {
                _MessageShow("Los minutos de tolerancia no pueden ser negativos.", State.Warning);
                ok = false;
            }

            return ok;
        }

        /* ─────  Sugerencia automática de abreviatura  ───── */

        private bool _abreviaturaEditada = false;

        private void DescripcionChanged(string nuevaDescripcion)
        {
            _tipo.Descripcion = nuevaDescripcion;
            if (!_abreviaturaEditada)
                _tipo.Abreviatura = GenerarAbreviatura(nuevaDescripcion);
            StateHasChanged();  // <— fuerza actualización
        }

        private void AbreviaturaChanged(string nuevoValor)
        {
            _tipo.Abreviatura = nuevoValor;
            _abreviaturaEditada = true;
            StateHasChanged();
        }
        private static string GenerarAbreviatura(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return string.Empty;

            // 1) rompe en palabras
            var palabras = descripcion
                           .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2) Si es una sola palabra → primeros 3 caracteres
            if (palabras.Length == 1)
                return palabras[0].Length <= 3
                       ? palabras[0].ToUpperInvariant()
                       : palabras[0].Substring(0, 3).ToUpperInvariant();

            // 3) Varias palabras → toma la inicial de cada una (máx. 4 letras)
            var sigla = string.Concat(palabras.Select(p => char.ToUpperInvariant(p[0])));
            return sigla.Length > 4 ? sigla[..4] : sigla;
        }
        private async Task CargarTipos()
        {
            _Loading.Show();
            var resp = await _Rest.GetAsync<List<GenClasificadorTipoDto>>("Clasificador/Turno");
            _Loading.Hide();

            if (resp.State == State.Success)
                _listaTipos = resp.Data;
            else
                _MessageShow(resp.Message, State.Warning);
        }

        private Dictionary<int, List<RrhhTurnodiaDto>> _turnosCache = new Dictionary<int, List<RrhhTurnodiaDto>>();
        private Dictionary<int, List<RrhhTurnotoleranciaDto>> _toleranciaCache = new Dictionary<int, List<RrhhTurnotoleranciaDto>>();
        private Dictionary<int, bool> _popoverStates = new();
        private bool IsPopoverOpen(int id)
               => _popoverStates.TryGetValue(id, out var open) && open;

        private async Task LoadTurnos(int idgenClasificadortipo)
        {
            _Loading.Show();

            var result = await _Rest.GetAsyncFromQuery<List<RrhhTurnodiaDto>>(
                "RrhhTurnodia/GetAll",
                new { IdgenClasificadortipo = idgenClasificadortipo }
            );

            _Loading.Hide();

            if (result.State == State.Success)
            {
                _turnosCache[idgenClasificadortipo] = result.Data; // ✅ Guarda en el caché
                StateHasChanged(); // Forzar actualización
            }
            else
                _MessageShow($"Error: {result.Message}", State.Warning);
        }

        private async Task LoadTolerancia(int idgenClasificadortipo)
        {
            _Loading.Show();

            var result = await _Rest.GetAsyncFromQuery<List<RrhhTurnotoleranciaDto>>(
                "RrhhTurnotolerancia/GetAll",
                new { IdgenClasificadortipo = idgenClasificadortipo }
            );

            _Loading.Hide();

            if (result.State == State.Success)
            {
                _toleranciaCache[idgenClasificadortipo] = result.Data; // ✅ Guarda en el caché
                StateHasChanged(); // Forzar actualización
            }
            else
                _MessageShow($"Error: {result.Message}", State.Warning);
        }
        private async Task ShowDetails(int id)
        {
            var loadTasks = new List<Task>();

            if (!_turnosCache.ContainsKey(id))
                loadTasks.Add(LoadTurnos(id));

            if (!_toleranciaCache.ContainsKey(id))
                loadTasks.Add(LoadTolerancia(id));

            if (loadTasks.Any())
                await Task.WhenAll(loadTasks);

            _popoverStates[id] = true;
        }

        private Color GetPriorityColor(string prioridad)
        {
            return prioridad?.ToUpper() switch
            {
                "ALTA" => Color.Error,
                "MEDIA" => Color.Warning,
                "BAJA" => Color.Success,
                _ => Color.Default
            };
        }

    }
}
