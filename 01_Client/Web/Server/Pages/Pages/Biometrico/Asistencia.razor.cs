using Infraestructura.Abstract;
using Infraestructura.Models.Authentication;
using Infraestructura.Models.Biometrico;
using Infraestructura.Models.Persona;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Biometrico
{
    public partial class Asistencia
    {
        // ============================================================================
        // VARIABLES
        // ============================================================================
        private bool _cargando = false;
        private DateTime? _fechaInicio = DateTime.Now.AddDays(-15);
        private DateTime? _fechaFin = DateTime.Now;
        private PersonaMinDto _personaSeleccionada;
        private AsistenciaConsultaDto _datosAsistencia;
        private List<PersonaMinDto> _personalACargo = new();

        private bool _dialogJustificacionVisible = false;
        private bool _dialogDetalleVisible = false;
        private AsistenciaDiaDto _registroSeleccionado;
        private JustificacionDto _justificacionDetalle;
        private string _observacionesAprobacion;
        private RrhPersonaDto _personaActual;
        private bool _modoEdicion = false;

        // Variables para archivos
        private IBrowserFile _archivoArea;
        private IBrowserFile _archivoGarita;
        private string _tipoOmision;
        private string _observaciones;
        private string? _previewAreaBase64;
        private string? _previewGaritaBase64;

        private DialogOptions _dialogOptions = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true };

        // ============================================================================
        // INICIALIZACIÓN
        // ============================================================================
        protected override async Task OnInitializedAsync()
        {
            await CargarPersonaActual();
            await CargarPersonalACargo();
        }

        private async Task CargarPersonaActual()
        {
            try
            {
                var userJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "USER");
                if (string.IsNullOrEmpty(userJson))
                    return;

                var usuario = System.Text.Json.JsonSerializer.Deserialize<ObjectEntity>(userJson);
                if (usuario == null || string.IsNullOrEmpty(usuario.nroCi))
                    return;

                var url = $"RrhPersona/GetPersona/{usuario.nroCi}";
                var response = await _Rest.GetAsync<List<RrhPersonaDto>>(url);

                if (response.State == State.Success && response.Data != null && response.Data.Any())
                {
                    _personaActual = response.Data.First();
                    Console.WriteLine($"Persona actual cargada: {_personaActual.NombreApellido}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando persona actual: {ex.Message}");
            }
        }

        private async Task CargarPersonalACargo()
        {
            try
            {
                var userJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "USER");
                if (string.IsNullOrEmpty(userJson))
                    return;

                var usuario = System.Text.Json.JsonSerializer.Deserialize<ObjectEntity>(userJson);
                if (usuario == null || string.IsNullOrEmpty(usuario.nroCi))
                    return;

                var url = $"RrhPersona/PersonalACargo/{usuario.nroCi}";
                var response = await _Rest.GetPlainAsync<List<PersonaMinDto>>(url);
                _personalACargo = response ?? new List<PersonaMinDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando personal: {ex.Message}");
            }
        }

        // ============================================================================
        // BÚSQUEDA Y CONSULTAS
        // ============================================================================
        private async Task<IEnumerable<PersonaMinDto>> BuscarPersonas(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 3)
                return Enumerable.Empty<PersonaMinDto>();

            return await Task.FromResult(
                _personalACargo
                    .Where(p => p.FullName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                               p.Ci.Contains(value))
                    .ToList()
            );
        }

        private async Task BuscarAsistencia()
        {
            if (!_fechaInicio.HasValue || !_fechaFin.HasValue)
            {
                _MessageShow("Debe seleccionar ambas fechas", State.Warning);
                return;
            }

            if (_fechaInicio > _fechaFin)
            {
                _MessageShow("La fecha de inicio no puede ser mayor a la fecha fin", State.Warning);
                return;
            }

            try
            {
                _cargando = true;
                StateHasChanged();

                var idPersona = _personaSeleccionada?.IdrrhPersona;
                var url = $"Asistencia?fechaInicio={_fechaInicio:yyyy-MM-dd}&fechaFin={_fechaFin:yyyy-MM-dd}";

                if (idPersona.HasValue)
                    url += $"&idPersona={idPersona}";

                var response = await _Rest.GetAsync<AsistenciaConsultaDto>(url);

                if (response.State == State.Success && response.Data != null)
                {
                    _datosAsistencia = response.Data;
                    _MessageShow(response.Message, State.Success);
                }
                else
                {
                    _MessageShow(response.Message ?? "No se encontraron datos", State.Warning);
                    _datosAsistencia = null;
                }
            }
            catch (Exception ex)
            {
                _MessageShow($"Error: {ex.Message}", State.Error);
            }
            finally
            {
                _cargando = false;
                StateHasChanged();
            }
        }

        // ============================================================================
        // MÉTODOS DE UI Y ESTADOS
        // ============================================================================
        private (Color color, string icon, string texto) GetEstadoProps(string estado)
        {
            return estado switch
            {
                "A_TIEMPO" => (Color.Success, Icons.Material.Filled.CheckCircle, "A Tiempo"),
                "ATRASO" => (Color.Warning, Icons.Material.Filled.Schedule, "Atraso"),
                "INASISTENCIA" => (Color.Error, Icons.Material.Filled.EventBusy, "Inasistencia"),
                "OMISION_ENTRADA" => (Color.Secondary, Icons.Material.Filled.Login, "Sin Entrada"),
                "OMISION_SALIDA" => (Color.Info, Icons.Material.Filled.Logout, "Sin Salida"),
                "JUSTIFICADO" => (Color.Success, Icons.Material.Filled.CheckCircleOutline, "Justificado"),
                "VACACIONES" => (Color.Info, Icons.Material.Filled.BeachAccess, "Vacaciones"),
                "BAJA_MEDICA" => (Color.Error, Icons.Material.Filled.MedicalServices, "Baja Médica"),
                "COMISION" => (Color.Tertiary, Icons.Material.Filled.Work, "Comisión"),
                "PERMISO_SIN_GOCE" => (Color.Warning, Icons.Material.Filled.MoneyOff, "Permiso S/G"),
                "FALTA" => (Color.Dark, Icons.Material.Filled.Cancel, "Falta"),
                "FERIADO" => (Color.Default, Icons.Material.Filled.Event, "Feriado"),
                _ => (Color.Default, Icons.Material.Filled.Help, estado)
            };
        }

        private Color ObtenerColorEstadoJustificacion(string estado)
        {
            return estado switch
            {
                "SOLICITADO" => Color.Warning,
                "APROBADO" => Color.Success,
                "RECHAZADO" => Color.Error,
                _ => Color.Default
            };
        }

        // ============================================================================
        // REGLAS DE NEGOCIO
        // ============================================================================
        private bool PuedeJustificar(AsistenciaDiaDto registro)
        {
            if (registro.IdJustificacion.HasValue)
                return false;

            var estadosJustificables = new[] { "OMISION_ENTRADA", "OMISION_SALIDA", "FALTA" };
            return estadosJustificables.Contains(registro.Estado);
        }

        private bool EsPropiaJustificacion(AsistenciaDiaDto registro)
        {
            return _personaActual != null && registro.IdPersona == _personaActual.IdrrhPersona;
        }

        private bool EsSuperiorInmediato(AsistenciaDiaDto registro)
        {
            if (_personaActual == null || registro == null)
                return false;

            var persona = _personalACargo.FirstOrDefault(p => p.IdrrhPersona == registro.IdPersona);
            return persona?.InmediatoSuperior == _personaActual.IdrrhPersona;
        }

        private bool PuedeEditarJustificacion(JustificacionDto justificacion)
        {
            return justificacion != null &&
                   justificacion.Estado == "SOLICITADO" &&
                   _personaActual != null &&
                   justificacion.IdrrhPersona == _personaActual.IdrrhPersona;
        }

        private bool PuedeCancelarJustificacion(JustificacionDto justificacion)
        {
            return PuedeEditarJustificacion(justificacion);
        }

        private bool PuedeAprobarRechazar()
        {
            return _justificacionDetalle != null &&
                   _justificacionDetalle.Estado == "SOLICITADO" &&
                   _registroSeleccionado != null &&
                   EsSuperiorInmediato(_registroSeleccionado);
        }

        // ============================================================================
        // MÉTODOS DE JUSTIFICACIÓN
        // ============================================================================
        private void AbrirDialogJustificacion(AsistenciaDiaDto registro)
        {
            _registroSeleccionado = registro;
            _modoEdicion = false;

            // Limpiar archivos y campos
            _archivoArea = null;
            _archivoGarita = null;
            _observaciones = "";

            // Pre-seleccionar tipo según omisiones
            if (!registro.MarcacionEntrada.HasValue && !registro.MarcacionSalida.HasValue)
                _tipoOmision = "AMBAS";
            else if (!registro.MarcacionEntrada.HasValue)
                _tipoOmision = "ENTRADA";
            else if (!registro.MarcacionSalida.HasValue)
                _tipoOmision = "SALIDA";

            _dialogJustificacionVisible = true;
        }

        private void AbrirDialogEdicion(JustificacionDto justificacion)
        {
            if (!PuedeEditarJustificacion(justificacion))
            {
                _MessageShow("No puede editar esta justificación", State.Warning);
                return;
            }

            _modoEdicion = true;
            _registroSeleccionado = _datosAsistencia?.Resultados
                .FirstOrDefault(r => r.IdrrhDiaevento == justificacion.IdrrhDiaevento);

            _tipoOmision = justificacion.TipoOmision;
            _observaciones = justificacion.Observaciones;
            _archivoArea = null;  // En modo edición, los archivos son opcionales
            _archivoGarita = null;

            _dialogJustificacionVisible = true;
        }

        /// <summary>
        /// Guarda la justificación con archivos (OPTIMIZADO)
        /// </summary>
        private async Task GuardarJustificacion()
        {
            // Validaciones
            if (string.IsNullOrEmpty(_tipoOmision))
            {
                _MessageShow("Debe seleccionar el tipo de omisión", State.Warning);
                return;
            }

            if (_archivoArea == null || _archivoGarita == null)
            {
                _MessageShow("Debe adjuntar ambas fotos", State.Warning);
                return;
            }

            try
            {
                _Loading.Show();

                using var content = new MultipartFormDataContent();

                // Agregar campos de texto
                content.Add(new StringContent(_registroSeleccionado.IdrrhDiaevento.ToString()), "IdrrhDiaevento");
                content.Add(new StringContent(_tipoOmision), "TipoOmision");
                content.Add(new StringContent(_observaciones ?? ""), "Observaciones");

                // ✅ AGREGAR ARCHIVO ÁREA EN CALIDAD ORIGINAL
                // allowRemoteStreaming: true = permite archivos grandes
                // disableImageResize: true = NO redimensiona la imagen (parámetro clave)
                var streamArea = _archivoArea.OpenReadStream(
                    maxAllowedSize: 10 * 1024 * 1024  // 10MB máximo
                );

                var memoryStreamArea = new MemoryStream();
                await streamArea.CopyToAsync(memoryStreamArea);
                memoryStreamArea.Position = 0;

                var contentArea = new StreamContent(memoryStreamArea);
                contentArea.Headers.ContentType = new MediaTypeHeaderValue(_archivoArea.ContentType ?? "application/octet-stream");
                content.Add(contentArea, "FotoAreaTrabajo", _archivoArea.Name);

                // ✅ AGREGAR ARCHIVO GARITA EN CALIDAD ORIGINAL
                var streamGarita = _archivoGarita.OpenReadStream(
                    maxAllowedSize: 10 * 1024 * 1024  // 10MB máximo
                );

                var memoryStreamGarita = new MemoryStream();
                await streamGarita.CopyToAsync(memoryStreamGarita);
                memoryStreamGarita.Position = 0;

                var contentGarita = new StreamContent(memoryStreamGarita);
                contentGarita.Headers.ContentType = new MediaTypeHeaderValue(_archivoGarita.ContentType ?? "application/octet-stream");
                content.Add(contentGarita, "FotoGarita", _archivoGarita.Name);

                // Enviar al backend
                var response = await _Rest.PostMultipartAsync<int>("Justificacion", content);

                if (response.State == State.Success)
                {
                    _MessageShow("✅ Justificación enviada correctamente", State.Success);
                    CerrarDialogJustificacion();
                    await BuscarAsistencia();
                }
                else
                {
                    _MessageShow(response.Message ?? "Error al guardar", State.Error);
                }

                // ✅ LIMPIAR STREAMS
                await memoryStreamArea.DisposeAsync();
                await memoryStreamGarita.DisposeAsync();
            }
            catch (TaskCanceledException)
            {
                _MessageShow("⏱️ La carga de archivos tomó demasiado tiempo. Intente con archivos más pequeños.", State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"Error: {ex.Message}", State.Error);
                Console.WriteLine($"Error detallado: {ex}");
            }
            finally
            {
                _Loading.Hide();
            }
        }
        private async Task VerDetalleJustificacion(AsistenciaDiaDto registro)
        {
            try
            {
                _Loading.Show();
                _registroSeleccionado = registro;

                var url = $"Justificacion/PorDiaevento?idrrhDiaevento={registro.IdrrhDiaevento}";
                var response = await _Rest.GetAsync<JustificacionDto>(url);

                if (response.State == State.Success && response.Data != null)
                {
                    _justificacionDetalle = response.Data;
                    _observacionesAprobacion = "";
                    _dialogDetalleVisible = true;
                }
                else
                {
                    _MessageShow("No se pudo cargar el detalle", State.Error);
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

        private async Task CancelarSolicitudJustificacion()
        {
            if (_justificacionDetalle == null || !PuedeCancelarJustificacion(_justificacionDetalle))
            {
                _MessageShow("No puede cancelar esta justificación", State.Warning);
                return;
            }

            await _MessageConfirm(
                "¿Está seguro de cancelar esta justificación?",
                async () =>
                {
                    try
                    {
                        _Loading.Show();

                        var response = await _Rest.DeleteAsync<int>(
                            $"Justificacion/{_justificacionDetalle.IdrrhJustificacion}",
                            _justificacionDetalle.IdrrhJustificacion
                        );

                        if (response.Succeeded)
                        {
                            _MessageShow("Justificación cancelada correctamente", State.Success);
                            CerrarDialogDetalle();
                            await BuscarAsistencia();
                        }
                        else
                        {
                            _MessageShow(response.Message ?? "Error al cancelar", State.Error);
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
            );
        }

        private async Task CambiarEstadoJustificacion(int idJustificacion, string nuevoEstado)
        {
            try
            {
                _Loading.Show();

                var parametros = new
                {
                    idrrhJustificacion = idJustificacion,
                    estado = nuevoEstado,
                    observacionesAprobacion = _observacionesAprobacion,
                    idUsuarioAprueba = _personaActual.IdrrhPersona
                };

                var response = await _Rest.PatchAsync<int>(
                "Justificacion/Estado",
                parametros,
                idJustificacion
                );

                if (response.State == State.Success)
                {
                    var mensaje = nuevoEstado == "APROBADO"
                        ? "✅ Justificación aprobada correctamente"
                        : "❌ Justificación rechazada";

                    _MessageShow(mensaje, State.Success);
                    CerrarDialogDetalle();
                    await BuscarAsistencia();
                }
                else
                {
                    _MessageShow(response.Message ?? "Error al procesar", State.Error);
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
        private async Task RechazarJustificacion()
        {
            await CambiarEstadoJustificacion(_justificacionDetalle.IdrrhJustificacion, "RECHAZADO");
        }

        private async Task AprobarJustificacion()
        {
            await CambiarEstadoJustificacion(_justificacionDetalle.IdrrhJustificacion, "APROBADO");
        }

        private async Task OnFileChangedInput(InputFileChangeEventArgs e, string tipo)
        {
            var file = e.File;
            if (file == null)
                return;

            if (file.Size > 5 * 1024 * 1024)
            {
                _MessageShow("El archivo no puede superar los 5MB", State.Warning);
                return;
            }

            var extension = Path.GetExtension(file.Name).ToLowerInvariant();
            var permitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

            if (!permitidas.Contains(extension))
            {
                _MessageShow("Solo se permiten JPG, PNG o PDF", State.Warning);
                return;
            }

            string? previewBase64 = null;

            // SOLO generar preview si es imagen
            if (extension != ".pdf")
            {
                using var stream = file.OpenReadStream(5 * 1024 * 1024);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                var base64 = Convert.ToBase64String(ms.ToArray());
                previewBase64 = $"data:{file.ContentType};base64,{base64}";
            }

            if (tipo == "area")
            {
                _archivoArea = file;
                _previewAreaBase64 = previewBase64;
            }
            else if (tipo == "garita")
            {
                _archivoGarita = file;
                _previewGaritaBase64 = previewBase64;
            }

            StateHasChanged();
        }


        private string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} KB";
            return $"{bytes / (1024 * 1024):F1} MB";
        }

        // ============================================================================
        // CERRAR DIALOGS
        // ============================================================================
        private void CerrarDialogJustificacion()
        {
            _dialogJustificacionVisible = false;
            _registroSeleccionado = null;
            _archivoArea = null;
            _archivoGarita = null;
            _tipoOmision = "";
            _observaciones = "";
            _modoEdicion = false;
        }

        private void CerrarDialogDetalle()
        {
            _dialogDetalleVisible = false;
            _justificacionDetalle = null;
            _observacionesAprobacion = "";
        }
        [Inject] private IConfiguration Configuration { get; set; } = default!;
        private string ApiBaseUrl => Configuration["EndPoints:Api"];

        private string GetUrlArchivo(string rutaRelativa)
        {
            if (string.IsNullOrWhiteSpace(rutaRelativa))
                return string.Empty;

            rutaRelativa = rutaRelativa.Replace('\\', '/');

            var partes = rutaRelativa.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length < 2)
                return string.Empty;

            var idPersona = partes[^2];
            var nombreArchivo = partes[^1];

            var apiBaseUrl = Configuration["EndPoints:Api"]; // ya tiene /api/v1/

            return $"{apiBaseUrl}Archivos/{idPersona}/{nombreArchivo}";
        }

        /// <summary>
        /// Abre una imagen en una nueva ventana del navegador
        /// </summary>
        private async Task AbrirImagenEnNuevaVentana(string rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa))
                return;

            var url = GetUrlArchivo(rutaRelativa);

            if (string.IsNullOrEmpty(url))
                return;

            await JSRuntime.InvokeVoidAsync("open", url, "_blank");
        }
    }
}