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
using Microsoft.AspNetCore.Components;


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
using Aplicacion.DTOs.Supervisor;
using ClosedXML.Excel;

using Aplicacion.Features.Asistencia;    //Vacaciones ...
using Infraestructura.Models.Biometrico; //Vacaciones ...



namespace Server.Pages.Pages.Persona
{
    public partial class Persona
    {
        private bool expande = false;
        public bool popupAdmView = false;
        private string searchString = "";  //added recently ...

        protected List<RrhPersonaDto>       Tablapersona            { get; set; } = new(); //Each employee is represented by a DTO object of type RrhPersonaDto like Nombre, Celular, Sexo, Edad ... 
        protected List<RrhContratoDto>      TablaContrato           { get; set; } = new();

        protected List<GenClasificadorDto> _ListaPrfesion           { get; set; } = new(); //Foreign key
        protected List<GenClasificadorDto> _ListaSalario            { get; set; } = new(); //Foreign Key
        protected List<GenClasificadorDto> _ListaPuestoDenominacion { get; set; } = new(); //Foreign key
        protected List<GenClasificadorDto> _ListaGrupoTrabajo       { get; set; } = new(); //Foreign key
        protected List<GenClasificadorDto> _ListaPuestoDescripcion  { get; set; } = new(); //Foreign key
        protected List<GenClasificadorDto> _ListaCategoria          { get; set; } = new(); //Foreign key
        protected List<GenClasificadorDto> _ListaClase              { get; set; } = new(); //Foreign key
        protected List<GenClasificadorDto> _ListaUnidad             { get; set; } = new(); //Foreign Key


        //Vacaciones ...



        protected List<VwMarcacionBiometricoDto> MarcacionPersonal { get; set; } = new();




        private RrhPersonaDto  _Persona   = new RrhPersonaDto();
        private RrhContratoDto _Contrato  = new RrhContratoDto();

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

        //The Principal Persona ...
        private async Task GetPersonaTurnos()
        {
            try
            {
                _Loading.Show();
                var result = await _Rest.GetAsync<List<RrhPersonaDto>>("RrhPersona/GetAll");
                _Loading.Hide();


                if (result.State == State.Success)
                {
                    //Tablapersona = result.Data;
                    Tablapersona = result.Data.OrderByDescending(p => p.IdrrhPersona).ToList(); // Sorting added here ...
                }
                else
                {
                    _MessageShow($"Error_Persona: {result.Message}", State.Error);
                }
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow($"Excepción_Persona: {ex.Message}", State.Error);
            }
        }

        //The Principal Contrato ...
        private async Task GetPersonaContrato()
        {
            try
            {
                _Loading.Show();
                var result = await _Rest.GetAsync<List<RrhContratoDto>>("RrhContrato/GetAll");
                _Loading.Hide();

                if (result.State == State.Success)
                {
                    TablaContrato = result.Data.OrderBy(p => p.NumeroContrato).ToList(); // <-- sorting added here
                }
                else
                {
                    _MessageShow($"Error_Contrato: {result.Message}", State.Error);
                }
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow($"Excepción_Contrato: {ex.Message}", State.Error);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await GetPersonaTurnos     ();
            await LoadClasificadores   ();
            await GetPersonaContrato   ();

            LoadDonutCharts();
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

        private string GetClaseDescripcion(int? id)
        {
            return GetDescripcionById(_ListaClase, id);
        }

        private string GetNivelSalarialDescripcion(int? id)
        {
            return GetDescripcionById(_ListaSalario, id);
        }
            
        private string SupervisorNombre(int? id)
        {
            // if no ID, return default
            if (id == null)
                return "No definido";

            // try to find the supervisor in the list
            var supervisor = Tablapersona.FirstOrDefault(p => p.IdrrhPersona == id);

            // if found, return the name
            if (supervisor != null)
                return supervisor.NombreApellido;

            // if not found, just return the ID number as text
            return $"ID {id}";
        }        


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //Crear/api/{version}/RrhPersona, RrhPersona, RrhPersona ...
        private async Task SavePersona(RrhPersonaDto persona)
        {
            try
            {
                _MessageShow($"==> Save Persona ...", State.Success);

                // Verificar si el CI ya existe en la lista de personas
                var personaExistente = Tablapersona.FirstOrDefault(p =>
                    !string.IsNullOrWhiteSpace(p.Ci) &&
                    p.Ci.Trim().Equals(persona.Ci?.Trim(), StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(_Persona.Celular))
                {
                    // Si ya tiene 591, no hagas nada
                    if (!_Persona.Celular.StartsWith("591"))
                    {
                        _Persona.Celular = "591" + _Persona.Celular.Trim();
                    }
                }

                var codigo = GeneratePassword();                         //Añadir contraseña ....   
                _Persona.Contrasena = codigo;                               
                _Persona.Edad = CalcularEdad(_Persona.FechaNacimiento);  //Añadir la edad del trabajador ...

                //_MessageShow($"_Persona.Contrasena: {_Persona.Contrasena}", State.Success);

                if (personaExistente != null)
                {
                    _MessageShow($"Ya existe una persona registrada con este CI: {personaExistente.Ci}, Nombre: {personaExistente.NombreApellido}", State.Warning);
                    _Loading.Hide();
                    return;
                }

                _Loading.Show();
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


        //Crear/api/{version}/RrhContrato, RrhContrato, RrhContrato, RrhContrato, RrhContrato ...
        private async Task SaveContrato(RrhContratoDto contrato)
        {
            try
            {
                _Loading.Show();

                //_MessageShow("Contenido enviado tercero: " + JsonSerializer.Serialize(contrato), State.Warning);

                var response = await _Rest.PostAsync<int?>("RrhContrato", contrato);

                _Loading.Hide();
                _MessageShow(response.Message, response.State);

                //_MessageShow("Estado111: " + response.State.ToString(), State.Warning);
                //_MessageShow("Mensaje222: " + (response?.Message ?? "Sin mensaje"), State.Warning);

                if (response.Errors != null)
                    response.Errors.ForEach(e => _MessageShow(e, State.Warning));
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow(ex.Message, State.Error);
                await GetPersonaContrato();
            }
        }


        // Actualizar Contrato de Empleado de la Persona ...
        private async Task UpdateContrato(RrhContratoDto contrato)
        {
            try
            {

                _Loading.Show();
                var response = await _Rest.PutAsync<int>("RrhContrato", contrato, contrato.IdrrhhContrato);
                _Loading.Hide();
                _MessageShow(response.Message, response.State);
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow(ex.Message, State.Error);
            }
        }


        // Actualizar la persona de Recursos Humanos ...
        private async Task UpdatePersona(RrhPersonaDto persona)
        {
            try
            {
                _MessageShow($"==> Update_Persona ...", State.Success);

                //_MessageShow($"Up IdrrhPersona      ==> {persona.IdrrhPersona}", State.Success);
                //_MessageShow($"Up Nombre            ==> {persona.Nombre}", State.Success);
                //_MessageShow($"Up ApellidoPaterno   ==> {persona.ApellidoPaterno}", State.Success);
                //_MessageShow($"Up ApellidoMaterno   ==> {persona.ApellidoMaterno}", State.Success);
                //_MessageShow($"Up InmediatoSuperior ==> {persona.InmediatoSuperior}", State.Success);
                //_MessageShow($"Up InmediatoSuperior Nombre: {SupervisorNombre(_Persona.InmediatoSuperior)}", State.Success);


                _Loading.Show();
                var response = await _Rest.PutAsync<int>("RrhPersona", persona, persona.IdrrhPersona);
                _Loading.Hide();

                _MessageShow($"=====>: {response.Message}", response.State);
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


        protected async Task EliminarContrato(int id)
        {
           //await _MessageConfirm("¿Seguro de eliminar el registro de esta persona?", async () => {

                var response = await _Rest.DeleteAsync<int>("RrhContrato", id);
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

                await GetPersonaContrato();
                TablaContratoPersona = TablaContrato.Where(x => x.IdrrhhPersona == _Contrato.IdrrhhPersona).ToList();
                StateHasChanged();
            //});
        }


        //Validar y guardar/actualizar el formulario
        private async Task OnValidPerfil(EditContext ctx) 
        {
            //_MessageShow($"_Persona.Nombre            ==> {_Persona.Nombre}", State.Success);
            //_MessageShow($"_Persona.IdrrhPersona      ==> {_Persona.IdrrhPersona}", State.Success);
            //_MessageShow($"_Persona.ApellidoPaterno   ==> {_Persona.ApellidoPaterno}", State.Success);
            //_MessageShow($"_Persona.ApellidoMaterno   ==> {_Persona.ApellidoMaterno}", State.Success);
            //_MessageShow($"_Persona.InmediatoSuperior ==> {_Persona.InmediatoSuperior}", State.Success);
            //_MessageShow($"[SAVE] InmediatoSuperior Nombre: {SupervisorNombre(_Persona.InmediatoSuperior)}", State.Success);

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
        private async Task OnValidContrato(EditContext ctx)
        {
            //_MessageShow("¡Formulario válido primero! ", State.Success);
            //_MessageShow($"¡Formulario válido! ID del contrato: {_Contrato.IdrrhhContrato}", State.Success);

            if (_Contrato.IdrrhhContrato > 0)
            {
                await UpdateContrato(_Contrato);
                _MessageShow($"Contrato actualizado correctamente (ID: {_Contrato.IdrrhhContrato})", State.Success);
            }
            else
            {
                await SaveContrato(_Contrato);
                _MessageShow($"Contrato registrado exitosamente (ID: {_Contrato.IdrrhhContrato})", State.Success);
            }

            await GetPersonaContrato(); // 🔄 Vuelve a cargar todos los contratos o al menos los del empleado actual

            //"TablaContratoPersona" guarda en una nueva lista ...
            TablaContratoPersona = TablaContrato.Where(x => x.IdrrhhPersona == _Contrato.IdrrhhPersona).ToList();  //.ToList() Lo convierte en una nueva tabla ...

            //_MessageShow($"_Contrato.IdrrhhContrato ==> {_Contrato.IdrrhhContrato}", State.Success);
            //_MessageShow($"_Contrato.InicioContrato ==> {_Contrato.InicioContrato}", State.Success);
            //_MessageShow($"_Contrato.FinContrato    ==> {_Contrato.FinContrato}", State.Success);
            //_MessageShow($"_Contrato.NumeroContrato ==> {_Contrato.NumeroContrato}", State.Success);
            //_MessageShow($"_Contrato.TipoContrato   ==> {_Contrato.TipoContrato}", State.Success);
            //_MessageShow($"_Contrato.IdrrhhPersona  ==> {_Contrato.IdrrhhPersona}", State.Success);

            _Contrato.InicioContrato = null;
            _Contrato.FinContrato = null;
            _Contrato.NumeroContrato = 0;
            _Contrato.TipoContrato = null;

            StateHasChanged();  // 🔃 Forzar el renderizado de la vista actualizada
        }

        /* ///////////////////////////////////////////////////////////////////////////////// */
        protected async Task ShowPopu(int id)
        {
            try
            {
                TablaContratoPersona = TablaContrato.Where(x => x.IdrrhhPersona == id).ToList();

                if (TablaContratoPersona.Any())
                {
                    popupAdmView = true;
                    _Contrato.IdrrhhPersona = id;

                }
                else
                {
                    popupAdmView = true;
                    _Contrato.IdrrhhPersona = id;
                    //popupAdmView = false;
                    //_MessageShow("No hay contrato para la persona", State.Warning);

                }
            }
            catch (Exception ex) {
                _MessageShow("Error al filtrar datos de contrato" + ex, State.Error);
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

        private void FormEditarContrato(RrhContratoDto contrato)
        {
            _Contrato = contrato;
        }

        private void ResetPerfil()
        {
            _Persona = new RrhPersonaDto();
        }


        private void ResetContrato() => _Contrato = new RrhContratoDto();


        private void ToggleExpand() => expande = !expande;

        private void btnCancelPop()
        {
            popupAdmView= false;
        }

        private bool FilterFunc1(RrhPersonaDto element) => FilterFunc(element, searchString);


        /*////////////////////////////////////////////////////////////////////////////////////////////////////////*/

        private int CalcularEdad(DateTime? BornDate)
        {
            if (BornDate == null)
            {
                return 0;
            }

            var today = DateTime.Today;
            var edad = today.Year - BornDate.Value.Year;

            

            if (BornDate.Value.Date > today.AddYears(-edad))
                edad = edad - 1;

            return edad;
        }

        private String GeneratePassword()
        {
            Random aleatorio = new Random();
            int numero = aleatorio.Next(1000,9999);
            return numero.ToString();
        }

        private bool FilterFunc(RrhPersonaDto element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;

            searchString = searchString.Trim();

            return (element.Nombre?.Contains          (searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                || (element.ApellidoPaterno?.Contains (searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                || (element.ApellidoMaterno?.Contains (searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                || (element.Ci?.Contains              (searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                || (element.Celular?.Contains         (searchString, StringComparison.OrdinalIgnoreCase) ?? false)
;
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

        //THIS IS FOR THE BAR CHART, BAR CHART, BAR CHART, BAR CHART, BAR CHART, BAR CHART ...
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

        // METTODO DE DESCARGA EN EXCEL
        public async Task ExportarPersonas()
        {
            try
            {
                if (Tablapersona == null || !Tablapersona.Any())
                {
                    _MessageShow("No hay datos para exportar a Excel.", State.Warning);
                    return;  //Shows warning if it doesnt work.
                }

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Personal_Envibol_" + DateTime.Now.ToString("MM-yyyy"));

                int filaActual = 1;

                // ==== Estilo cabecera base ====
                var estiloCabecera = workbook.Style;
                estiloCabecera.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                estiloCabecera.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
                estiloCabecera.Font.Bold            = true;
                estiloCabecera.Fill.BackgroundColor = XLColor.LightGreen;
                estiloCabecera.Font.FontColor       = XLColor.Black;

                // === Cabeceras (idénticas a la tabla) ===
                string[] cabeceras = {  //Creating headers ...
                    //"OPCIONES",            // (sólo marcador, quedará vacío)
                    "ID PERSONA",
                    "NOMBRE",
                    "APELLIDO PATERNO",
                    "APELLIDO MATERNO",
                    "CI",
                    "EXT",
                    "EXP",
                    "CELULAR",
                    "CONTRASEÑA",
                    "UNIDAD ORGANIZACIONAL",
                    "CATEGORIA",
                    "CLASE",
                    "N. SALARIAL",
                    "PUESTO DENOMINACION",
                    "PROFESION",
                    "GRUPO TRABAJO",
                    "PUESTO DESCRIPCION",
                    "SEXO",
                    "FECHA NACIMIENTO",
                    "DOMICILIO",
                    "RESIDENCIA",
                    "EDAD",
                    "INMEDIATO SUPERIOR",
                    "CORREO"
                };


                _MessageShow($"Cantidad Cabeceras ==> {cabeceras.Length}", State.Success);


                for (int i = 0; i < cabeceras.Length; i++)
                {
                    worksheet.Cell(filaActual, i + 1).Value = cabeceras[i];
                    worksheet.Cell(filaActual, i + 1).Style = estiloCabecera;
                }

                // Congelar fila de cabecera y activar autofiltro
                worksheet.SheetView.FreezeRows(1);
                worksheet.Range(filaActual, 1, filaActual, cabeceras.Length).SetAutoFilter();

                // === Anchos de columnas (ajústalos si deseas) ===
                //worksheet.Column(1).Width = 12;   // OPCIONES
                worksheet.Column(1).Width = 12;   // ID PERSONA
                worksheet.Column(2).Width = 22;   // NOMBRE
                worksheet.Column(3).Width = 20;   // APELLIDO PATERNO
                worksheet.Column(4).Width = 20;   // APELLIDO MATERNO
                worksheet.Column(5).Width = 14;   // CI
                worksheet.Column(6).Width = 8;    // EXT
                worksheet.Column(7).Width = 8;    // EXP
                worksheet.Column(8).Width = 16;   // CELULAR
                worksheet.Column(9).Width = 18;  // CONTRASEÑA
                worksheet.Column(10).Width = 38;  // UNIDAD ORGANIZACIONAL
                worksheet.Column(11).Width = 18;  // CATEGORIA
                worksheet.Column(12).Width = 14;  // CLASE
                worksheet.Column(13).Width = 16;  // N. SALARIAL
                worksheet.Column(14).Width = 26;  // PUESTO DENOMINACION
                worksheet.Column(15).Width = 22;  // PROFESION
                worksheet.Column(16).Width = 20;  // GRUPO TRABAJO
                worksheet.Column(17).Width = 26;  // PUESTO DESCRIPCION
                worksheet.Column(18).Width = 10;  // SEXO
                worksheet.Column(19).Width = 16;  // FECHA NACIMIENTO
                worksheet.Column(20).Width = 30;  // DOMICILIO
                worksheet.Column(21).Width = 24;  // RESIDENCIA
                worksheet.Column(22).Width = 10;  // EDAD
                worksheet.Column(23).Width = 28;  // INMEDIATO SUPERIOR
                worksheet.Column(24).Width = 30;  // CORREO

                filaActual++;

                // === Datos ===
                foreach (var p in Tablapersona)
                {
                    int c = 1;

                    //worksheet.Cell(filaActual, c++).Value = ""; // OPCIONES (vacío)
                    worksheet.Cell(filaActual, c++).Value = p.IdrrhPersona;
                    worksheet.Cell(filaActual, c++).Value = p.Nombre;
                    worksheet.Cell(filaActual, c++).Value = p.ApellidoPaterno;
                    worksheet.Cell(filaActual, c++).Value = p.ApellidoMaterno;
                    worksheet.Cell(filaActual, c++).Value = p.Ci;
                    worksheet.Cell(filaActual, c++).Value = p.Extension;
                    worksheet.Cell(filaActual, c++).Value = p.Exp;
                    worksheet.Cell(filaActual, c++).Value = p.Celular;
                    worksheet.Cell(filaActual, c++).Value = p.Contrasena; // Si prefieres, enmascara aquí
                    worksheet.Cell(filaActual, c++).Value = GetUnidadDescripcion(p.IdgenUnidad);
                    worksheet.Cell(filaActual, c++).Value = GetCategoriaDescripcion(p.IdgenCategoria);
                    worksheet.Cell(filaActual, c++).Value = GetClaseDescripcion(p.IdgenClase);
                    worksheet.Cell(filaActual, c++).Value = GetNivelSalarialDescripcion(p.IdgenNivelsalarial);
                    worksheet.Cell(filaActual, c++).Value = GetPuestoDenominacionDescripcion(p.IdgenPuestodenominacion);
                    worksheet.Cell(filaActual, c++).Value = GetProfesionDescripcion(p.IdgenProfesion);
                    worksheet.Cell(filaActual, c++).Value = GetGrupoTrabajoDescripcion(p.IdgengrupoTrabajo);
                    worksheet.Cell(filaActual, c++).Value = GetPuestoDescripcion(p.IdgenPuestodescripcion);
                    worksheet.Cell(filaActual, c++).Value = p.Sexo;

                    var celdaFecha = worksheet.Cell(filaActual, c++);
                    if (p.FechaNacimiento.HasValue)
                    {
                        celdaFecha.Value = p.FechaNacimiento.Value;
                        celdaFecha.Style.DateFormat.Format = "dd/MM/yyyy";
                    }
                    else
                    {
                        celdaFecha.Value = "";
                    }

                    worksheet.Cell(filaActual, c++).Value = p.Domicilio;
                    worksheet.Cell(filaActual, c++).Value = p.Residencia;
                    worksheet.Cell(filaActual, c++).Value = p.Edad;
                    worksheet.Cell(filaActual, c++).Value = SupervisorNombre(p.InmediatoSuperior);
                    worksheet.Cell(filaActual, c++).Value = p.Correo;

                    // Ajustes de alineación
                    worksheet.Range(filaActual, 1, filaActual, cabeceras.Length)
                             .Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    filaActual++;
                }

                // === Bordes para toda el área usada ===
                var rangoUsado = worksheet.RangeUsed();
                if (rangoUsado != null)
                {
                    rangoUsado.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    rangoUsado.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                // Ajuste de texto en celdas de texto largo
                worksheet.Columns(11, 25).Style.Alignment.WrapText = true;

                // === Guardar y descargar ===
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                var base64 = Convert.ToBase64String(stream.ToArray());
                await JSRuntime.InvokeVoidAsync("downloadFile",
                    "personas_" + DateTime.Now.ToString("MM-yy") + ".xlsx",
                    base64);
            }
            catch (Exception ex)
            {
                _MessageShow("Error al exportar personas: " + ex.Message, State.Error);
            }
        }

    }
}









