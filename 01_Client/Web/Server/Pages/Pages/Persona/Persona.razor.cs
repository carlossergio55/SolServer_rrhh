using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Infraestructura.Abstract;
using Infraestructura.Models.Clasificador;
using Infraestructura.Models.Persona;
using Microsoft.JSInterop;
using System.Text.Json;

using MudBlazor;
using Microsoft.AspNetCore.Http.Connections;
using DocumentFormat.OpenXml.Office2013.Word;
using Aplicacion.DTOs.Contrato;
using Aplicacion.DTOs.Charts;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Inputs;

using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace Server.Pages.Pages.Persona
{
    public partial class Persona
    {
        private bool expande = false;
        public bool popupAdmView = false;
        private string searchString = "";  //added recently ...
        private string TotalText;
        private bool formVisible = true; // para mostrar/ocultar el formulario


        protected List<RrhPersonaDto>       Tablapersona            { get; set; } = new();
        protected List<GenClasificadorDto> _ListaPrfesion           { get; set; } = new();
        protected List<GenClasificadorDto> _ListaSalario            { get; set; } = new();
        protected List<GenClasificadorDto> _ListaPuestoDenominacion { get; set; } = new();
        protected List<GenClasificadorDto> _ListaGrupoTrabajo       { get; set; } = new();
        protected List<GenClasificadorDto> _ListaPuestoDescripcion  { get; set; } = new();
        protected List<GenClasificadorDto> _ListaCategoria          { get; set; } = new();
        protected List<GenClasificadorDto> _ListaClase              { get; set; } = new();
        protected List<GenClasificadorDto> _ListaUnidad             { get; set; } = new(); 

        private RrhPersonaDto _Persona   = new RrhPersonaDto();
        private RrhContratoDto _Contrato = new RrhContratoDto();

        private   List<RrhPersonaDto> _RrhPersona           = new List<RrhPersonaDto>();
        protected List<RrhContratoDto> TablaContrato        = new List<RrhContratoDto>();
        protected List<RrhContratoDto> TablaContratoPersona = new List<RrhContratoDto>();

        private List<GenClasificadorDto> _AllClasificadores = new();

        //CHARTS, CHARTS, CHARTS, CHARTS, CHARTS ...
        private List<RrhChartsDto> DonutDataTotalPersonal = new();
        private List<RrhChartsDto> DonutAdministrativo    = new();
        private List<RrhChartsDto> DonutOperativo         = new();

        private List<RrhChartsDto> DonutDepartamento = new();
        private List<RrhChartsDto> BarDataChuquisaca = new();
        private List<RrhChartsDto> BarDataEducacion  = new();
        private List<RrhChartsDto> BarDataEdad       = new();

        private async Task LoadClasificadores()
        {
            var result = await _Rest.GetAsync<List<GenClasificadorDto>>("Clasificador/Clasificador");
            if (result.State == State.Success)
            {
                _AllClasificadores = result.Data;
                _ListaUnidad             = FilterClasificadorByTipo(new[] { 1, 2, 3, 4, 5 });
                _ListaClase              = FilterClasificadorByTipo(7);
                _ListaCategoria          = FilterClasificadorByTipo(6);
                _ListaPuestoDescripcion  = FilterClasificadorByTipo(10);
                _ListaGrupoTrabajo       = FilterClasificadorByTipo(12);
                _ListaPuestoDenominacion = FilterClasificadorByTipo(9);
                _ListaPrfesion           = FilterClasificadorByTipo(11);
                _ListaSalario            = FilterClasificadorByTipo(8);
            }
            else
            {
                _MessageShow("Error al cargar clasificadores: " + result.Message, State.Warning);
            }

        }
        private List<GenClasificadorDto> FilterClasificadorByTipo(params int[] tipos)
        {
            return _AllClasificadores.Where(x => tipos.Contains(x.IdgenClasificadortipo)).ToList();
        }

        //The Principal ...
        private async Task GetPersonaTurnos()
        {
            try
            {
                _Loading.Show();
                var result = await _Rest.GetAsync<List<RrhPersonaDto>>("RrhPersona/GetAll");
                _Loading.Hide();
                if (result.State == State.Success)
                    //Tablapersona = result.Data;
                    Tablapersona = result.Data.OrderByDescending(p => p.IdrrhPersona).ToList(); // <-- sorting added here
                else
                    _MessageShow($"Error111122: {result.Message}", State.Error);
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow($"ExcepciónAA: {ex.Message}", State.Error);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await GetPersonaTurnos      ();
            await LoadClasificadores    ();
            await GetPersonaContrato    ();
            LoadDonutCharts             ();
        }

        ////////////////////////////////UNIDADES////////////////////////////////////////////////////////////    
        private string GetDescripcionById(List<GenClasificadorDto> lista, int? id)
        {
            return lista?.FirstOrDefault(x => x.IdgenClasificador == id)?.Descripcion ?? "No definido";
        }

        private string GetUnidadDescripcion(int? id)
        {
            return GetDescripcionById(_ListaUnidad, id);
        }

        private string GetCategoriaDescripcion(int? id)
        {
            return GetDescripcionById(_ListaCategoria, id);
        }

        private string GetPuestoDenominacionDescripcion(int? id)
        {
            return GetDescripcionById(_ListaPuestoDenominacion, id);
        }

        private string GetGrupoTrabajoDescripcion(int? id)
        {
            return GetDescripcionById(_ListaGrupoTrabajo, id);
        }

        private string GetPuestoDescripcion(int? id)
        {
            return GetDescripcionById(_ListaPuestoDescripcion, id);
        }

        private string GetProfesionDescripcion(int? id)
        {
            return GetDescripcionById(_ListaPrfesion, id);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        //Crear/api/{version}/RrhPersona
        private async Task SavePersona(RrhPersonaDto persona)
        {
            try
            {
                _Loading.Show();

                //var response = await _Rest.PostAsync<int?>("RrhPersona", new { _RrhPersonapost = persona });
                _MessageShow("Contenido enviado: " + JsonSerializer.Serialize(persona), State.Warning);

                var response = await _Rest.PostAsync<int?>("RrhPersona", persona);
                _Loading.Hide();

                _MessageShow(response.Message, response.State);

                if (response.Errors != null)
                    response.Errors.ForEach(e => _MessageShow(e, State.Warning));
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow(ex.Message, State.Error);
                await GetPersonaTurnos();
            }
        }

        // Actualizar la persona de Recursos Humanos ...
        private async Task UpdatePersona(RrhPersonaDto persona)
        {
            try
            {
                _Loading.Show();
                var response = await _Rest.PutAsync<int>("RrhPersona", persona, persona.IdrrhPersona);
                _Loading.Hide();
                _MessageShow(response.Message, response.State);
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow(ex.Message, State.Error);
            }
        }

        // Eliminar la persona de Recursos Humanos ...
        protected async Task EliminarPersona(int id)
        {
            //await _MessageConfirm("¿Seguro de eliminar el registro de esta persona?", async () =>
            //{
                var response = await _Rest.DeleteAsync<int>("RrhPersona", id);
                if (!response.Succeeded)
                {
                    _MessageShow(response.Message, State.Error);
                }
                else
                {
                    _MessageShow(response.Message, response.State);
                    await GetPersonaTurnos();
                    StateHasChanged();
                }
           // });
        }

        //Validar y guardar/actualizar el formulario
        private async Task OnValidPerfil(EditContext ctx)
        {
            if (_Persona.IdrrhPersona > 0)
            {
                await UpdatePersona(_Persona);
            }
            else
            {
                await SavePersona(_Persona);
            }
            _Persona = new RrhPersonaDto();
            await GetPersonaTurnos();
            ToggleExpand();
            StateHasChanged();
        }

        //Validar y guardar/actualizar ...
        private async Task GuardarContrato(EditContext ctx)
        {
            try
            {
                if (_Contrato.IdrrhhPersona == 0)
                {
                    _MessageShow("Seleccione una persona primero.", State.Warning);
                    return;
                }

                var response = await _Rest.PostAsync<int?>("RrhContrato", _Contrato);
                _MessageShow(response.Message, response.State);

                if (response.Succeeded)
                {
                    await GetPersonaContrato(); // Recargar todos los contratos
                    TablaContratoPersona = TablaContrato.Where(x => x.IdrrhhPersona == _Contrato.IdrrhhPersona).ToList();
                    _Contrato = new(); // limpiar formulario
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                _MessageShow("Error al guardar contrato: " + ex.Message, State.Error);
            }
        }

        /* ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// */
        private async Task GetPersonaContrato()
        {
            try
            {
                _Loading.Show();
                var result = await _Rest.GetAsync<List<RrhContratoDto>>("RrhContrato/GetAll");
                _Loading.Hide();
                if (result.State == State.Success)
                    TablaContrato = result.Data;
                else
                    _MessageShow($"Error111122: {result.Message}", State.Error);
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow($"ExcepciónAA: {ex.Message}", State.Error);
            }
        }

        protected async Task ShowPopu(int id)
        {
            try
            {
                TablaContratoPersona = TablaContrato.Where(x => x.IdrrhhPersona == id).ToList();
                if (TablaContratoPersona.Any())
                {
                    popupAdmView = true;
                }
                else
                {
                    popupAdmView = false;
                    _MessageShow("No hay contrato parav la paersona", State.Warning);

                }
            }
            catch (Exception ex) {
                _MessageShow("Error al filtrar datos de contrato"+ex,State.Error);
            }
        }

        private async Task CambiarEstado(RrhPersonaDto persona)
        {
            persona.Estado = persona.Estado == '1' ? '2' : '1';

            await UpdatePersona(persona);
            await GetPersonaTurnos(); //Opcional si necesitas refrescar la lista
        }


        // Cargar para editar la informacion ...
        private void FormEditarPersona(RrhPersonaDto persona)
        {
            _Persona = persona;
            ToggleExpand();
        }
        private void ResetPerfil() => _Persona = new RrhPersonaDto();
        private void ToggleExpand() => expande = !expande;

        private void btnCancelPop()
        {
            popupAdmView= false;
        }

        private bool FilterFunc1(RrhPersonaDto element) => FilterFunc(element, searchString);


        /*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
        private bool FilterFunc(RrhPersonaDto element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;

            searchString = searchString.Trim();

            return (element.Nombre?.Contains          (searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                || (element.ApellidoPaterno?.Contains (searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                || (element.ApellidoMaterno?.Contains (searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                || (element.Ci?.Contains              (searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                || (element.Celular?.Contains         (searchString, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        private string GetCategoriaPorId(int id)
        {
            if (id == 172 || id == 173 || id == 176 || id == 178 || id == 180 || id == 183 || id == 185 || id  == 186)
                return "TECNICO";
            else if ((id >= 144 && id <= 158) || id == 131 || id == 132 || id == 177 || id == 182 || id == 184 || id == 187)
                return "PROFESIONAL";
            else if (id >= 135 && id <= 143)
                return "EGRESADO";
            else if (id == 178 || id == 179)
                return "SECUNDARIA";
            else if (id == 175)
                return "PRIMARIA";
            else if (id == 134)
                return "BACHILLER";
            else
                return "OTRO";
        }
        
        private string GetEdadPorId(int id)
        {
            if (id >= 18 && id <= 28)
                return "18 A 28 AÑOS";
            else if (id >= 29 && id <= 39)
                return "29 A 39 AÑOS";
            else if (id >= 40 && id <= 59)
                return "40 A 59 AÑOS";
            else if (id >= 60)
                return "60 O MAS";
            else
                return "OTRO";
        }


        //GRAPHICS, GRAPHICS, GRAPHICS, GRAPHICS, GRAPHICS, GRAPHICS, GRAPHICS, GRAPHICS ...
        private void LoadDonutCharts()
        {
            DonutDataTotalPersonal = GenerarDonutPorSexo(
                Tablapersona,
                "TOTAL",
                out int totalPersonal);

            DonutAdministrativo = GenerarDonutPorSexo(
                Tablapersona.Where(p => p.IdgengrupoTrabajo == 167),
                "ADMINISTRATIVOS",
                out int totalAdmin);

            DonutOperativo = GenerarDonutPorSexo(
                Tablapersona.Where(p => p.IdgengrupoTrabajo == 165),
                "OPERATIVOS",
                out int totalOperativo);

            //Bar Chart ...
            DonutDepartamento = GenerarDonutDepartamento(
                Tablapersona,
                "DEPARTAMENTAL",
                out int totalDepartamento);

            BarDataChuquisaca = GenerarChuquisaca(
                Tablapersona,
                "CHUQUISACA",
                out int totalChuquisaca);

            //Bar for Educaction ...
            BarDataEducacion = GenerarBarEducacion( 
                Tablapersona,
                "EDUCACION",
                out int totalEducacion);

            //Bar for Age ...
            BarDataEdad = GenerarBarEdad(
                Tablapersona,
                "EDAD",
                out int totalEdad);

        }

        //Bar Char Formacion Edad ...
        private List<RrhChartsDto> GenerarBarEdad(IEnumerable<RrhPersonaDto> personas, string grupoLabel, out int totalPersonas)
        {
            string[] colores = { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0" };

            var grouped = personas
                .Where(p => p.Edad.HasValue)
                .GroupBy(p => GetEdadPorId(p.Edad.Value))
                .Select((g, i) => new RrhChartsDto
                {
                    Category = g.Key,
                    Value    = g.Count(),
                    Color    = colores[i % colores.Length]
                })
                .ToList();

            totalPersonas = grouped.Sum(x => (int)x.Value);

            return grouped;
        }

        //Bar Char Formacion Empleados ... 
        private List<RrhChartsDto> GenerarBarEducacion(IEnumerable<RrhPersonaDto> personas, string grupoLabel, out int totalPersonas)
        {

            string[] colores = {
                "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF", "#FF9F40", "#66BB6A"
            };

            var grouped = personas
                .Where(p => p.IdgenProfesion.HasValue)
                .GroupBy(p => GetCategoriaPorId(p.IdgenProfesion.Value))
                .Select((g, i) => new RrhChartsDto
                {
                    Category = g.Key,
                    Value = g.Count(),
                    Color = colores[i % colores.Length]
                })
                .ToList();

            totalPersonas = grouped.Sum(x => (int)x.Value);

            /*foreach (var item in grouped)
            {
                double porcentaje = (item.Value / (double)totalPersonas) * 100;
                item.Category += $" ({porcentaje:F1}%)";
            }*/
            return grouped;
        }

        private List<RrhChartsDto> GenerarDonutPorSexo(IEnumerable<RrhPersonaDto> personas, string grupoLabel, out int totalPersonas)
        {
            var grouped = personas
                .Where(p => !string.IsNullOrWhiteSpace(p.Sexo))
                .GroupBy(p => p.Sexo.ToUpper())
                .Select(g => new RrhChartsDto
                {
                    Category = (g.Key == "M" ? "HOMBRES" : "MUJERES") + ": " + g.Count(),
                    Value = g.Count(),
                    Color = g.Key == "M" ? "#03B0F0" : "#F75A95"
                }).ToList();

            totalPersonas = (int)grouped.Sum(x => x.Value);

            foreach (var item in grouped)
            {
                double porcentaje = (item.Value / (double)totalPersonas) * 100;
                item.Category += $" ({porcentaje:F1}%)";
            }

            Console.WriteLine($"{grupoLabel} - Total personas: {totalPersonas}");
            return grouped;
        }


        private List<RrhChartsDto> GenerarChuquisaca(IEnumerable<RrhPersonaDto> personas, string grupoLabel, out int totalPersonas)
        {
            string[] colores = {
                "#FF6384",  // SUCRE
                "#36A2EB",  // ZUDAÑEZ
                "#FFCE56"   // OTROS LUGARES
            };

            // Inicializa los contadores
            int sucreCount = 0;
            int zudanezCount = 0;
            int otrosCount = 0;

            foreach (var persona in personas)
            {
                if (string.IsNullOrWhiteSpace(persona.Residencia))
                    continue;

                var residencia = persona.Residencia.Trim().ToUpper();

                if (residencia == "SUCRE")
                    sucreCount++;
                else if (residencia == "ZUDAÑEZ")
                    zudanezCount++;
                else
                    otrosCount++;
            }

            var resultado = new List<RrhChartsDto>
                {
                    new RrhChartsDto { Category = "SUCRE", Value = sucreCount, Color = colores[0] },
                    new RrhChartsDto { Category = "ZUDAÑEZ", Value = zudanezCount, Color = colores[1] },
                    new RrhChartsDto { Category = "OTROS", Value = otrosCount, Color = colores[2] }
                };

            totalPersonas = resultado.Sum(x => (int)x.Value);
            return resultado;
        }

        //THIS IS FOR THE BAR CHART, BAR CHART, BAR CHART, BAR CHART, BAR CHART ...
        private List<RrhChartsDto> GenerarDonutDepartamento(IEnumerable<RrhPersonaDto> personas, string grupoLabel, out int totalPersonas)
        {
            var departamentoMap = new Dictionary<string, string>
            {
                { "CH", "CHUQUISACA" },
                { "LP", "LA PAZ" },
                { "CB", "COCHABAMBA" },
                { "OR", "ORURO" },
                { "PT", "POTOSI" },
                { "TJ", "TARIJA" },
                { "SC", "SANTA CRUZ" },
                { "BN", "BENI" },
                { "PD", "PANDO" }
            };

            string[] colores = {
                "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF",
                "#FF9F40", "#66BB6A", "#BA68C8", "#FFA726"
            };

            var grouped = personas
                .Where(p => !string.IsNullOrWhiteSpace(p.Exp))
                .GroupBy(p => p.Exp.ToUpper())
                .Select((g, index) => new RrhChartsDto
                {
                    Category = g.Key, // Abbreviation
                    FullName = departamentoMap.ContainsKey(g.Key) ? departamentoMap[g.Key] : g.Key,
                    Value = g.Count(),
                    Color = colores[index % colores.Length]
                })
                .ToList();

            totalPersonas = Convert.ToInt32(grouped.Sum(x => x.Value));

            foreach (var item in grouped)
            {
                double porcentaje = (item.Value / (double)totalPersonas) * 100;

                // Category solo con abreviación
                // FullName muestra todo (nombre completo y porcentaje)
                item.FullName += $" ({porcentaje:F1}%)";
            }
            return grouped;
        }
    }
}
