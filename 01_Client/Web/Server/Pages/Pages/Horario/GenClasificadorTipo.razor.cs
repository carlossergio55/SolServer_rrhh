using System.Collections.Generic;
using System.Threading.Tasks;
using Infraestructura.Abstract;
using System;
using Microsoft.AspNetCore.Components.Forms;
using Infraestructura.Models.Clasificador;
using Infraestructura.Models.Horario;
using MudBlazor;

namespace Server.Pages.Pages.Horario
{
    public partial class GenClasificadorTipo
    {
        private bool expande = false;
        public GenClasificadorTipoDto _Item = new GenClasificadorTipoDto();
        public List<GenClasificadorTipoDto> _List = new List<GenClasificadorTipoDto>();
        public List<RrhhTurnodiaDto> _TurnosList = new List<RrhhTurnodiaDto>();

        protected override async Task OnInitializedAsync()
        {
            await LoadList();
        }

        private void ToggleExpand() => expande = !expande;

        private void ResetForm()
        {
            _Item = new GenClasificadorTipoDto();
        }

        private void EditItem(GenClasificadorTipoDto dto)
        {
            _Item = new GenClasificadorTipoDto
            {
                IdgenClasificadortipo = dto.IdgenClasificadortipo,
                Descripcion = dto.Descripcion,
                Valor1 = dto.Valor1
            };
            expande = true;
        }

        private async Task DeleteItem(int id)
        {
            await _MessageConfirm("¿Eliminar este registro?", async () =>
            {
                var resp = await _Rest.DeleteAsync<int>("GenClasificadorTipo", id);
                if (resp.Succeeded)
                {
                    await LoadList();
                    StateHasChanged();
                    _MessageShow(resp.Message, resp.State);
                }
                else
                    _MessageShow(resp.Message, State.Error);
            });
        }

        private async Task OnValidSubmit(EditContext ctx)
        {
            if (_Item.IdgenClasificadortipo > 0)
                await _Rest.PutAsync<int>("GenClasificadorTipo", _Item, _Item.IdgenClasificadortipo);
            else
                await _Rest.PostAsync<int?>("GenClasificadorTipo", _Item);

            await LoadList();
            ResetForm();
            expande = false;
        }

        private async Task LoadList()
        {
            _Loading.Show();
            var result = await _Rest.GetAsync<List<GenClasificadorTipoDto>>("Clasificador/Turno");
            _Loading.Hide();

            if (result.State == State.Success)
                _List = result.Data;
            else
                _MessageShow($"Error: {result.Message}", State.Warning);
        }
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
        private MudTable<GenClasificadorTipoDto> _table;
        private Dictionary<int, bool> _expandedItems = new Dictionary<int, bool>();
        private Dictionary<int, List<RrhhTurnodiaDto>> _turnosCache = new Dictionary<int, List<RrhhTurnodiaDto>>();
        private async Task ToggleDetails(int id)
        {
            _expandedItems[id] = !_expandedItems.GetValueOrDefault(id);

            if (_expandedItems[id] && !_turnosCache.ContainsKey(id))
            {
                await LoadTurnos(id);
                StateHasChanged(); // Actualiza la UI
            }
        }
    }
}
