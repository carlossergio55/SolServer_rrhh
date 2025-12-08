using Infraestructura.Abstract;
using Infraestructura.Models.Authentication;
using Infraestructura.Models.Clasificador;
using Infraestructura.Models.Permisos;
using Infraestructura.Models.Persona;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Persona
{
    public partial class Permiso
    {
        // ===============================================
        // VARIABLES Y PROPIEDADES
        // ===============================================

        // Listas de datos
        private List<GenClasificadorTipoDto> _permisos = new();
        private List<SRrhSolicitudDto> _solicitudes = new();

        // Persona actual (usuario logueado)
        private RrhPersonaDto _personaActual;

        // Usuario desde localStorage
        private ObjectEntity _usuarioSeg;

        // Variables para formularios
        private GenClasificadorTipoDto _permisoSeleccionado;
        private SRrhSolicitudDto _solicitudActual = new();
        private SRrhSolicitudDto _solicitudDetalle;

        // Variables auxiliares para el formulario (nullable para MudDatePicker)
        private DateTime? _fechaInicio;
        private DateTime? _fechaFin;

        // Control de diálogos
        private bool _mostrarFormulario = false;
        private bool _mostrarDetalles = false;
        private bool _cargandoDatos = true;

        // Opciones del diálogo
        private DialogOptions _dialogOptions = new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            CloseButton = true,
            DisableBackdropClick = true
        };

        // ===============================================
        // MÉTODOS DE INICIALIZACIÓN
        // ===============================================

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _cargandoDatos = true;

                // 1. Obtener usuario logueado desde localStorage
                await ObtenerUsuarioLogueado();

                // 2. Si tenemos usuario, obtener su información de persona
                if (_usuarioSeg != null && !string.IsNullOrEmpty(_usuarioSeg.loginUsuario))
                {
                    await ObtenerPersonaPorCi(_usuarioSeg.loginUsuario);
                }

                // 3. Cargar permisos disponibles
                await ObtenerPermisos();

                // 4. Cargar solicitudes del usuario
                if (_personaActual != null)
                {
                    await ObtenerMisSolicitudes();
                }
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al inicializar: {ex.Message}", State.Error);
                Console.WriteLine($"Error en OnInitializedAsync: {ex.Message}");
            }
            finally
            {
                _cargandoDatos = false;
            }
        }

        // ===============================================
        // MÉTODOS PARA OBTENER DATOS
        // ===============================================

        /// <summary>
        /// Obtiene el usuario logueado desde localStorage
        /// </summary>
        private async Task ObtenerUsuarioLogueado()
        {
            try
            {
                var localStorageValue = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "USER");
                if (!string.IsNullOrEmpty(localStorageValue))
                {
                    _usuarioSeg = JsonSerializer.Deserialize<ObjectEntity>(localStorageValue);
                    Console.WriteLine($"Usuario logueado: {_usuarioSeg?.nombreCompleto} - CI: {_usuarioSeg?.loginUsuario}");
                }
                else
                {
                    _MessageShow("No se pudo obtener la información del usuario. Por favor, inicie sesión nuevamente.", State.Warning);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario desde localStorage: {ex.Message}");
                _MessageShow("Error al obtener información del usuario", State.Error);
            }
        }

        /// <summary>
        /// Obtiene los datos de la persona por su CI
        /// </summary>
        private async Task ObtenerPersonaPorCi(string ci)
        {
            try
            {
                var res = await _Rest.GetAsyncFromPath<List<RrhPersonaDto>>("RrhPersona/GetPersona", ci);

                if (res.State == State.Success && res.Data != null && res.Data.Any())
                {
                    _personaActual = res.Data.First();
                    Console.WriteLine($"Persona cargada: ID={_personaActual.IdrrhPersona}, Nombre={_personaActual.NombreApellido}");
                }
                else
                {
                    _MessageShow("No se encontró información de la persona en el sistema", State.Warning);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener persona por CI: {ex.Message}");
                _MessageShow("Error al obtener información de la persona", State.Error);
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de permisos disponibles
        /// </summary>
        private async Task ObtenerPermisos()
        {
            try
            {
                var res = await _Rest.GetAsync<List<GenClasificadorTipoDto>>("Clasificador/Permiso");
                if (res.State == State.Success && res.Data != null)
                {
                    _permisos = res.Data;
                    Console.WriteLine($"Permisos cargados: {_permisos.Count}");
                }
                else
                {
                    _MessageShow("Error al cargar tipos de permisos: " + res.Message, State.Warning);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener permisos: {ex.Message}");
                _MessageShow("Error al cargar los tipos de permisos", State.Error);
            }
        }

        /// <summary>
        /// Obtiene las solicitudes del usuario actual
        /// </summary>
        private async Task ObtenerMisSolicitudes()
        {
            try
            {
                var res = await _Rest.GetAsync<List<SRrhSolicitudDto>>("RrhSolicitud/GetAll");

                if (res.State == State.Success && res.Data != null)
                {
                    // Filtrar por persona actual
                    _solicitudes = res.Data
                        .Where(s => s.IdrrhPersona == _personaActual.IdrrhPersona)
                        .OrderByDescending(s => s.FechaSolicitud)
                        .ToList();

                    Console.WriteLine($"Solicitudes cargadas: {_solicitudes.Count}");
                }
                else
                {
                    _solicitudes = new List<SRrhSolicitudDto>();
                }
            }
            catch (Exception ex)
            {
                _solicitudes = new List<SRrhSolicitudDto>();
                Console.WriteLine($"Error al obtener solicitudes: {ex.Message}");
                // No mostrar mensaje de error en la primera carga
            }
        }

        // ===============================================
        // MÉTODOS PARA ABRIR/CERRAR FORMULARIOS
        // ===============================================

        /// <summary>
        /// Abre el formulario para solicitar un nuevo permiso
        /// </summary>
        private void AbrirFormularioPermiso(GenClasificadorTipoDto permiso)
        {
            // Verificar que tengamos la persona cargada
            if (_personaActual == null)
            {
                _MessageShow("No se pudo cargar la información del usuario. Por favor, recargue la página.", State.Warning);
                return;
            }

            _permisoSeleccionado = permiso;
            _solicitudActual = new SRrhSolicitudDto
            {
                IdrrhPersona = _personaActual.IdrrhPersona,
                TipoSolicitud = permiso.IdgenClasificadortipo,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(1),
                FechaSolicitud = DateTime.Now,
                Estado = "SOLICITADO"
            };

            // Inicializar variables auxiliares para el formulario
            _fechaInicio = _solicitudActual.FechaInicio;
            _fechaFin = _solicitudActual.FechaFin;

            Console.WriteLine($"Abriendo formulario - Permiso: {permiso.Descripcion}, Persona ID: {_personaActual.IdrrhPersona}");
            _mostrarFormulario = true;
        }

        private void CerrarFormulario()
        {
            _mostrarFormulario = false;
            _solicitudActual = new SRrhSolicitudDto();
            _permisoSeleccionado = null;
            _fechaInicio = null;
            _fechaFin = null;
        }

        /// <summary>
        /// Prepara el formulario para editar una solicitud existente
        /// </summary>
        private void EditarSolicitud(SRrhSolicitudDto solicitud)
        {
            _solicitudActual = new SRrhSolicitudDto
            {
                IdrrhSolicitud = solicitud.IdrrhSolicitud,
                IdrrhPersona = solicitud.IdrrhPersona,
                TipoSolicitud = solicitud.TipoSolicitud,
                FechaInicio = solicitud.FechaInicio,
                FechaFin = solicitud.FechaFin,
                Motivo = solicitud.Motivo,
                Estado = solicitud.Estado,
                FechaSolicitud = solicitud.FechaSolicitud
            };

            // Inicializar variables auxiliares
            _fechaInicio = _solicitudActual.FechaInicio;
            _fechaFin = _solicitudActual.FechaFin;

            _permisoSeleccionado = _permisos.FirstOrDefault(p => p.IdgenClasificadortipo == solicitud.TipoSolicitud);
            _mostrarFormulario = true;
        }

        /// <summary>
        /// Muestra los detalles completos de una solicitud
        /// </summary>
        private void VerDetalleSolicitud(SRrhSolicitudDto solicitud)
        {
            _solicitudDetalle = solicitud;
            _mostrarDetalles = true;
        }

        // ===============================================
        // MÉTODOS PARA GUARDAR/ELIMINAR
        // ===============================================

        /// <summary>
        /// Guarda una nueva solicitud o actualiza una existente
        /// </summary>
        private async Task GuardarSolicitud()
        {
            try
            {
                // ===== VALIDACIONES =====

                // Validar que se hayan seleccionado las fechas
                if (!_fechaInicio.HasValue || !_fechaFin.HasValue)
                {
                    _MessageShow("Debes seleccionar ambas fechas", State.Warning);
                    return;
                }

                // Asignar las fechas al objeto solicitud
                _solicitudActual.FechaInicio = _fechaInicio.Value;
                _solicitudActual.FechaFin = _fechaFin.Value;

                if (_solicitudActual.FechaFin < _solicitudActual.FechaInicio)
                {
                    _MessageShow("La fecha de fin no puede ser anterior a la fecha de inicio", State.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_solicitudActual.Motivo))
                {
                    _MessageShow("Debes indicar el motivo de tu solicitud", State.Warning);
                    return;
                }

                if (_solicitudActual.Motivo.Length < 10)
                {
                    _MessageShow("El motivo debe tener al menos 10 caracteres", State.Warning);
                    return;
                }

                // Verificar que tenga IdrrhPersona
                if (_solicitudActual.IdrrhPersona <= 0)
                {
                    _MessageShow("Error: No se pudo identificar al usuario", State.Error);
                    return;
                }

                Console.WriteLine($"Guardando solicitud - Persona ID: {_solicitudActual.IdrrhPersona}, Tipo: {_solicitudActual.TipoSolicitud}");

                _Loading.Show();

                ResponseEntity<int?> response;

                if (_solicitudActual.IdrrhSolicitud > 0)
                {
                    // ===== ACTUALIZAR SOLICITUD EXISTENTE =====
                    Console.WriteLine($"Actualizando solicitud ID: {_solicitudActual.IdrrhSolicitud}");
                    response = await _Rest.PutAsync<int?>("RrhSolicitud", _solicitudActual, _solicitudActual.IdrrhSolicitud);
                }
                else
                {
                    // ===== CREAR NUEVA SOLICITUD =====
                    Console.WriteLine("Creando nueva solicitud");
                    response = await _Rest.PostAsync<int?>("RrhSolicitud", new { Item = _solicitudActual });
                }

                _Loading.Hide();

                if (response.State == State.Success)
                {
                    _MessageShow("¡Solicitud guardada correctamente!", State.Success);
                    await ObtenerMisSolicitudes();
                    CerrarFormulario();
                    StateHasChanged();
                }
                else
                {
                    _MessageShow(response.Message ?? "Error al guardar la solicitud", State.Warning);
                    response.Errors?.ForEach(x => _MessageShow(x, State.Warning));
                }
            }
            catch (Exception e)
            {
                _Loading.Hide();
                _MessageShow("Error al guardar la solicitud: " + e.Message, State.Error);
                Console.WriteLine($"Excepción en GuardarSolicitud: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Cancela/elimina una solicitud pendiente
        /// </summary>
        private async Task CancelarSolicitud(int idSolicitud)
        {
            await _MessageConfirm("¿Estás seguro de cancelar esta solicitud de permiso?", async () =>
            {
                try
                {
                    _Loading.Show();
                    var response = await _Rest.DeleteAsync<int>("RrhSolicitud", idSolicitud);
                    _Loading.Hide();

                    if (response.Succeeded)
                    {
                        _MessageShow("Solicitud cancelada correctamente", State.Success);
                        await ObtenerMisSolicitudes();
                        StateHasChanged();
                    }
                    else
                    {
                        _MessageShow(response.Message, State.Error);
                    }
                }
                catch (Exception e)
                {
                    _Loading.Hide();
                    _MessageShow("Error al cancelar la solicitud: " + e.Message, State.Error);
                }
            });
        }

        // ===============================================
        // MÉTODOS AUXILIARES PARA UI
        // ===============================================

        private string ObtenerIconoPermiso(string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion)) return Icons.Material.Filled.Description;

            return descripcion.ToUpper() switch
            {
                "VACACIONES" => Icons.Material.Filled.BeachAccess,
                "BAJA MEDICA" or "BAJA MÉDICA" or "MÉDICO" => Icons.Material.Filled.MedicalServices,
                "COMISION" or "COMISIÓN" => Icons.Material.Filled.Work,
                "PERMISO SIN GOCE" => Icons.Material.Filled.MoneyOff,
                _ => Icons.Material.Filled.Description
            };
        }

        private Color ObtenerColorPermiso(string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion)) return Color.Default;

            return descripcion.ToUpper() switch
            {
                "VACACIONES" => Color.Info,
                "BAJA MEDICA" or "BAJA MÉDICA" or "MÉDICO" => Color.Error,
                "COMISION" or "COMISIÓN" => Color.Tertiary,
                "PERMISO SIN GOCE" => Color.Warning,
                _ => Color.Default
            };
        }

        private Color ObtenerColorEstado(string estado)
        {
            return estado?.ToUpper() switch
            {
                "SOLICITADO" or "PENDIENTE" => Color.Warning,
                "APROBADO" => Color.Success,
                "RECHAZADO" => Color.Error,
                _ => Color.Default
            };
        }

        private string ObtenerIconoEstado(string estado)
        {
            return estado?.ToUpper() switch
            {
                "SOLICITADO" or "PENDIENTE" => Icons.Material.Filled.HourglassEmpty,
                "APROBADO" => Icons.Material.Filled.CheckCircle,
                "RECHAZADO" => Icons.Material.Filled.Cancel,
                _ => Icons.Material.Filled.Help
            };
        }

        private string ObtenerTextoEstado(string estado)
        {
            return estado?.ToUpper() switch
            {
                "SOLICITADO" => "En espera",
                "PENDIENTE" => "En espera",
                "APROBADO" => "Aprobado",
                "RECHAZADO" => "No aprobado",
                _ => estado
            };
        }

        private string CalcularDias(DateTime fechaInicio, DateTime fechaFin)
        {
            var dias = (fechaFin.Date - fechaInicio.Date).Days + 1;
            return dias == 1 ? "1 día" : $"{dias} días";
        }
    }
}