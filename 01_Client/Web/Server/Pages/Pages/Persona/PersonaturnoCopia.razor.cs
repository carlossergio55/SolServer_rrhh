using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Infraestructura.Abstract;
using Infraestructura.Models.Clasificador;
using Infraestructura.Models.Horario;
using Infraestructura.Models.Persona;
using Microsoft.AspNetCore.Components.Forms;

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
        private int _anioSeleccionado = DateTime.Now.Year;
        private List<int> _listaAnios = Enumerable.Range(DateTime.Now.Year - 2, 5).ToList();


        // ✅ NUEVAS VARIABLES PARA CONTINUIDAD
        private bool _continuarAutomatico = true; // Por defecto activado
        private VerificarRangoDto? _infoRango;
        private UltimoTurnoDto? _infoUltimoTurno;
        private int? _turnoInicialSeleccionado; // Para rotativos

        // VARIABLES PARA EL CALENDARIO
        private int _mesSeleccionado = DateTime.Now.Month;
        private List<DateTime> _diasDelMes = new();
        private List<RrhDiaeventoDto> _eventosDelMes = new();
        private List<PersonaMinDto> _personasConTurnos = new();

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
                    await ObtenerEventosPorMes(_mesSeleccionado, _anioSeleccionado);

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

        // ============================================================================
        // ✅ NUEVO: MANEJO DE SELECCIÓN DE PERSONA CON VERIFICACIÓN AUTOMÁTICA
        // ============================================================================
        private async Task OnPersonaChanged(PersonaMinDto? persona)
        {
            _personaSeleccionada = persona;
            _infoRango = null;
            _infoUltimoTurno = null;

            if (persona != null)
            {
                _DiaEvento.IdrrhPersona = persona.IdrrhPersona;

                // ✅ Obtener último turno registrado (detecta ciclos incompletos)
                await ObtenerUltimoTurno();

                // ✅ NUEVO: Verificar ciclo incompleto con endpoint especializado
                await VerificarCicloIncompleto();
            }
            else
            {
                _DiaEvento.IdrrhPersona = 0;
            }

            StateHasChanged();
        }
        private async Task VerificarCicloIncompleto()
        {
            if (_personaSeleccionada == null || !_infoUltimoTurno?.TieneRegistros == true)
                return;

            try
            {
                // Usar fecha del último turno como referencia
                var fechaDesde = _infoUltimoTurno.Fecha?.AddDays(-30) ?? DateTime.Now.AddDays(-30);
                var fechaHasta = _infoUltimoTurno.Fecha ?? DateTime.Now;

                var request = new
                {
                    IdPersona = _personaSeleccionada.IdrrhPersona,
                    FechaInicio = fechaDesde,
                    FechaFin = fechaHasta
                };

                var response = await _Rest.PostAsync<VerificarRangoDto>("RrhDiaevento/VerificarRango", request);

                if (response.State == State.Success && response.Data != null)
                {
                    var verificacion = response.Data;

                    // ✅ Si hay ciclo incompleto, mostrar alerta y ajustar generación
                    if (verificacion.CicloIncompleto)
                    {
                        _infoRango = verificacion;

                        // Sugerir fecha inicio como día siguiente
                        if (_infoUltimoTurno.Fecha.HasValue)
                        {
                            _fechaInicio = _infoUltimoTurno.Fecha.Value.AddDays(1);
                        }

                        _MessageShow(
                            $"⚠️ Ciclo incompleto detectado: {verificacion.DiasCompletados} de " +
                            $"{verificacion.DiasCompletados + verificacion.DiasFaltantes} días. " +
                            $"Turno: {verificacion.DescripcionTurnoActual}. " +
                            $"Se continuará automáticamente al generar.",
                            State.Warning
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verificando ciclo incompleto: {ex.Message}");
            }
        }

        // ============================================================================
        // ✅ NUEVO: OBTENER ÚLTIMO TURNO DE LA PERSONA
        // ============================================================================
        private async Task ObtenerUltimoTurno()
        {
            if (_personaSeleccionada == null) return;

            try
            {
                var url = $"RrhDiaevento/GetUltimoTurno/{_personaSeleccionada.IdrrhPersona}";
                var response = await _Rest.GetAsync<UltimoTurnoDto>(url);

                if (response.State == State.Success && response.Data != null)
                {
                    _infoUltimoTurno = response.Data;

                    if (_infoUltimoTurno.TieneRegistros)
                    {
                        // Sugerir fecha inicio como día siguiente al último registro
                        if (_continuarAutomatico && _infoUltimoTurno.Fecha.HasValue)
                        {
                            _fechaInicio = _infoUltimoTurno.Fecha.Value.AddDays(1);
                        }

                        // Si tiene grupo, preseleccionar el mismo grupo
                        if (_infoUltimoTurno.IdgenGrupoturno.HasValue)
                        {
                            _grupoSeleccionado = _infoUltimoTurno.IdgenGrupoturno.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo último turno: {ex.Message}");
            }
        }

        // ============================================================================
        // ✅ NUEVO: VERIFICAR RANGO DE FECHAS
        // ============================================================================
        private async Task VerificarRango()
        {
            if (_personaSeleccionada == null || !_fechaInicio.HasValue || !_fechaFin.HasValue)
                return;

            try
            {
                var request = new
                {
                    IdPersona = _personaSeleccionada.IdrrhPersona,
                    FechaInicio = _fechaInicio.Value,
                    FechaFin = _fechaFin.Value
                };

                var response = await _Rest.PostAsync<VerificarRangoDto>("RrhDiaevento/VerificarRango", request);

                if (response.State == State.Success && response.Data != null)
                {
                    _infoRango = response.Data;

                    // Mostrar alertas según el resultado
                    if (_infoRango.CicloIncompleto)
                    {
                        _MessageShow(
                            $"⚠️ Ciclo incompleto detectado: {_infoRango.DiasCompletados} de " +
                            $"{_infoRango.DiasCompletados + _infoRango.DiasFaltantes} días. " +
                            $"Turno: {_infoRango.DescripcionTurnoActual}",
                            State.Warning
                        );
                    }
                    else if (_infoRango.ExistenTurnos)
                    {
                        _MessageShow(
                            $"ℹ️ Existen {_infoRango.CantidadRegistros} turnos hasta " +
                            $"{_infoRango.UltimaFechaExistente:dd/MM/yyyy}",
                            State.Warning
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verificando rango: {ex.Message}");
            }
        }

        // ============================================================================
        // ✅ NUEVO: OBTENER RESUMEN MENSUAL
        // ============================================================================
        private async Task<ResumenMensualDto?> ObtenerResumenMensual(int idPersona, int mes, int anio)
        {
            try
            {
                var url = $"RrhDiaevento/ResumenMensual/{idPersona}/{mes}/{anio}";
                var response = await _Rest.GetAsync<ResumenMensualDto>(url);

                if (response.State == State.Success)
                {
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo resumen: {ex.Message}");
            }

            return null;
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
            await ObtenerEventosPorMes(_mesSeleccionado, _anioSeleccionado);
        }

        // ============================================================================
        // ✅ MEJORADO: GENERACIÓN CON CONTINUIDAD AUTOMÁTICA
        // ============================================================================
        private async Task GenerarTurnos()
        {
            if (_personaSeleccionada == null || _grupoSeleccionado == 0 || _fechaInicio == null || _fechaFin == null)
            {
                _MessageShow("Completa todos los campos requeridos", State.Warning);
                return;
            }

            var (grupo, detalles) = ObtenerConfiguracionGrupo();
            if (grupo == null || !detalles.Any())
            {
                _MessageShow("No se encontraron detalles del grupo", State.Warning);
                return;
            }

            _DiasCache.Clear();

            // ✅ Determinar índice inicial del turno
            int turnIndexInicial = 0;
            int diasYaCompletados = 0;
            bool esContinuacionDescanso = false;

            // ✅ MEJORADO: Detectar continuación de ciclo incompleto
            if (_continuarAutomatico && _infoRango != null && _infoRango.CicloIncompleto)
            {
                if (_infoRango.IdgenClasificadortipoActual == 22) // Es descanso incompleto
                {
                    esContinuacionDescanso = true;
                    diasYaCompletados = _infoRango.DiasCompletados ?? 0;

                    // ✅ CRÍTICO: Buscar el turno laboral ANTERIOR al descanso
                    // para determinar qué turno viene después de completar los descansos
                    if (_infoRango.OrdenActualEnCiclo.HasValue)
                    {
                        turnIndexInicial = _infoRango.OrdenActualEnCiclo.Value - 1; // Orden es 1-indexed
                    }

                    _MessageShow(
                        $"Completando descansos: {diasYaCompletados} días completados, " +
                        $"faltan {_infoRango.DiasFaltantes} días",
                        State.Warning
                    );
                }
                else // Es turno laboral incompleto
                {
                    var detalleActual = detalles.FirstOrDefault(d =>
                        d.IdgenClasificadortipo == _infoRango.IdgenClasificadortipoActual);

                    if (detalleActual != null)
                    {
                        turnIndexInicial = detalles.IndexOf(detalleActual);
                        diasYaCompletados = _infoRango.DiasCompletados ?? 0;
                    }

                    _MessageShow(
                        $"Continuando turno {_infoRango.DescripcionTurnoActual}: " +
                        $"{diasYaCompletados} días completados, faltan {_infoRango.DiasFaltantes}",
                        State.Warning
                    );
                }
            }
            else if (_turnoInicialSeleccionado.HasValue && grupo.ModoGeneracion == "ROTATIVO")
            {
                turnIndexInicial = _turnoInicialSeleccionado.Value - 1;
            }

            // Generar según el modo
            if (grupo.ExcluirFinesSemana && grupo.ModoGeneracion == "FIJO")
            {
                GenerarTurnosFijos(grupo, detalles);
            }
            else
            {
                GenerarTurnosRotativos(grupo, detalles, turnIndexInicial, diasYaCompletados, esContinuacionDescanso);
            }

            MostrarResumenGeneracion();
        }
        private (GenGrupoturnoDto grupo, List<GenGrupoturnoDetalleDto> detalles) ObtenerConfiguracionGrupo()
        {
            var grupo = _listaGrupo.FirstOrDefault(x => x.IdgenGrupoturno == _grupoSeleccionado);
            var detalles = _grupoDetalles
                .Where(x => x.IdgenGrupoturno == _grupoSeleccionado)
                .OrderBy(x => x.Orden)
                .ToList();

            return (grupo, detalles);
        }

        private void GenerarTurnosFijos(GenGrupoturnoDto grupo, List<GenGrupoturnoDetalleDto> detalles)
        {
            var fecha = _fechaInicio.Value;
            var idTurnoFijo = detalles[0].IdgenClasificadortipo ?? 0;

            while (fecha <= _fechaFin.Value)
            {
                if (EsFinDeSemana(fecha))
                    AgregarDiaDescanso(fecha, "Fin de semana");
                else
                    AgregarDiaLaboral(fecha, idTurnoFijo);

                fecha = fecha.AddDays(1);
            }
        }

        // ============================================================================
        // ✅ MEJORADO: GENERACIÓN ROTATIVA CON CONTINUIDAD
        // ============================================================================
        private void GenerarTurnosRotativos(
    GenGrupoturnoDto grupo,
    List<GenGrupoturnoDetalleDto> detalles,
    int turnIndexInicial = 0,
    int diasYaCompletados = 0,
    bool esContinuacionDescanso = false)
        {
            var fecha = _fechaInicio.Value;
            var turnIndex = turnIndexInicial;
            bool primerCiclo = true;

            while (fecha <= _fechaFin.Value)
            {
                int diasLaborables = grupo.DiasLaborables;
                int diasDescanso = grupo.DiasDescanso;

                // ✅ CASO 1: Continuar con descansos incompletos
                if (primerCiclo && esContinuacionDescanso && diasYaCompletados > 0)
                {
                    // Completar los días de descanso faltantes
                    diasDescanso = grupo.DiasDescanso - diasYaCompletados;
                    fecha = GenerarDiasDescanso(fecha, diasDescanso, grupo.ExcluirFinesSemana);

                    // Después de completar descansos, rotar al siguiente turno
                    turnIndex = (turnIndex + 1) % detalles.Count;
                    primerCiclo = false;
                    continue; // Saltar generación de días laborables en esta iteración
                }

                // ✅ CASO 2: Continuar con turno laboral incompleto
                if (primerCiclo && !esContinuacionDescanso && diasYaCompletados > 0 && _continuarAutomatico && _infoRango?.IdgenClasificadortipoActual != 22)
                {
                    diasLaborables = grupo.DiasLaborables - diasYaCompletados;
                }

                // Fase 1: Días laborables
                fecha = GenerarDiasLaborables(fecha, diasLaborables, grupo.ExcluirFinesSemana, detalles, ref turnIndex);

                // Fase 2: Días de descanso
                fecha = GenerarDiasDescanso(fecha, diasDescanso, grupo.ExcluirFinesSemana);

                // Rotación de turno
                turnIndex = (turnIndex + 1) % detalles.Count;
                primerCiclo = false;
            }
        }

        private DateTime GenerarDiasLaborables(DateTime fechaInicio, int diasLaborales, bool excluirFines,
            List<GenGrupoturnoDetalleDto> detalles, ref int turnIndex)
        {
            var fecha = fechaInicio;

            for (int i = 0; i < diasLaborales && fecha <= _fechaFin.Value; i++)
            {
                if (excluirFines && EsFinDeSemana(fecha))
                {
                    AgregarDiaDescanso(fecha, "Fin de semana");
                    fecha = fecha.AddDays(1);
                    i--;
                    continue;
                }

                var idTurno = detalles[turnIndex].IdgenClasificadortipo ?? 0;
                AgregarDiaLaboral(fecha, idTurno);
                fecha = fecha.AddDays(1);
            }

            return fecha;
        }

        private DateTime GenerarDiasDescanso(DateTime fechaInicio, int diasDescanso, bool excluirFines)
        {
            var fecha = fechaInicio;

            for (int j = 0; j < diasDescanso && fecha <= _fechaFin.Value; j++)
            {
                if (excluirFines && EsFinDeSemana(fecha))
                {
                    AgregarDiaDescanso(fecha, "Fin de semana");
                    fecha = fecha.AddDays(1);
                    j--;
                    continue;
                }

                AgregarDiaDescanso(fecha, null);
                fecha = fecha.AddDays(1);
            }

            return fecha;
        }

        private bool EsFinDeSemana(DateTime fecha)
        {
            return fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday;
        }

        private void AgregarDiaLaboral(DateTime fecha, int idTurno)
        {
            _DiasCache.Add(new RrhDiaeventoDto
            {
                IdrrhPersona = _personaSeleccionada.IdrrhPersona,
                Fecha = fecha,
                IdgenClasificadortipo = idTurno,
                Motivo = null
            });
        }

        private void AgregarDiaDescanso(DateTime fecha, string motivo)
        {
            const int ID_DESCANSO = 22;

            _DiasCache.Add(new RrhDiaeventoDto
            {
                IdrrhPersona = _personaSeleccionada.IdrrhPersona,
                Fecha = fecha,
                IdgenClasificadortipo = ID_DESCANSO,
                Motivo = motivo
            });
        }

        private void MostrarResumenGeneracion()
        {
            const int ID_DESCANSO = 22;
            var cantidadDescansos = _DiasCache.Count(d => d.IdgenClasificadortipo == ID_DESCANSO);
            var cantidadLaborables = _DiasCache.Count - cantidadDescansos;

            _MessageShow(
                $"✅ Turnos generados: {_DiasCache.Count} días " +
                $"({cantidadLaborables} laborables, {cantidadDescansos} descansos)",
                State.Success
            );
        }

        private async Task GuardarTurnos()
        {
            if (_DiasCache == null || !_DiasCache.Any())
            {
                _MessageShow("No hay turnos para guardar", State.Warning);
                return;
            }

            await SaveDiaEvento(_DiasCache.ToList());
        }

        private async Task CambiarMes(int mes)
        {
            _mesSeleccionado = mes;
            GenerarDiasDelMes(mes, _anioSeleccionado);
            await ObtenerEventosPorMes(mes, _anioSeleccionado);
            StateHasChanged();
        }
        private async Task OnAnioChanged(int nuevoAnio)
        {
            _anioSeleccionado = nuevoAnio;

            // Regenerar días y turnos para el mes seleccionado
            GenerarDiasDelMes(_mesSeleccionado, _anioSeleccionado);
            await ObtenerEventosPorMes(_mesSeleccionado, _anioSeleccionado);

            StateHasChanged();
        }


        private void GenerarDiasDelMes(int mes, int anio)
        {
            _diasDelMes.Clear();
            var primerDia = new DateTime(anio, mes, 1);
            var ultimoDia = primerDia.AddMonths(1).AddDays(-1);

            for (var fecha = primerDia; fecha <= ultimoDia; fecha = fecha.AddDays(1))
            {
                _diasDelMes.Add(fecha);
            }
        }

        private async Task ObtenerEventosPorMes(int mes, int anio)
        {
            try
            {
                _Loading.Show();

                var url = $"RrhDiaevento/GetByMes?mes={mes}&anio={anio}";
                var response = await _Rest.GetAsync<List<RrhDiaeventoDto>>(url);

                if (response.State == State.Success)
                {
                    _eventosDelMes = response.Data ?? new List<RrhDiaeventoDto>();

                    _personasConTurnos = _eventosDelMes
                        .Where(e => e.RrhPersona != null)
                        .Select(e => new PersonaMinDto
                        {
                            IdrrhPersona = e.IdrrhPersona,
                            Nombre = e.RrhPersona.NombreApellido ?? "Sin nombre"
                        })
                        .GroupBy(p => p.IdrrhPersona)
                        .Select(g => g.First())
                        .OrderBy(p => p.NombreApellido)
                        .ToList();
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

        private string GetTurnoAbreviado(int idTurno)
        {
            var turno = _listaTurnos.FirstOrDefault(t => t.IdgenClasificadortipo == idTurno);
            if (turno == null) return "?";
            return string.IsNullOrWhiteSpace(turno.Abreviatura)
                ? "?"
                : turno.Abreviatura.ToUpper();
        }

        private string GetChipColorHex(int idTurno)
        {
            Color c = idTurno switch
            {
                15 => Color.SteelBlue,        // 6-2 TURNO MAÑANA
                17 => Color.DarkOrange,       // 6-2 TURNO TARDE
                16 => Color.MidnightBlue,     // 6-2 TURNO NOCHE
                18 => Color.SeaGreen,         // ADMINISTRATIVO
                22 => Color.DarkGray,         // DESCANSO
                23 => Color.DarkRed,          // FALTA
                24 => Color.ForestGreen,      // VACACIONES
                25 => Color.DarkMagenta,      // BAJA MEDICA
                26 => Color.Goldenrod,        // COMISION
                27 => Color.SaddleBrown, // PERMISO SIN GOCE
                28 => Color.RoyalBlue,        // FERIADO
                _ => Color.SlateGray
            };

            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }


        private string GetTooltipText(RrhDiaeventoDto evento)
        {
            var turno = _listaTurnos.FirstOrDefault(t => t.IdgenClasificadortipo == evento.IdgenClasificadortipo);
            var fecha = evento.Fecha?.ToString("dd/MM/yyyy") ?? "Sin fecha";
            var descripcion = turno?.Descripcion ?? "Sin turno";
            var motivo = string.IsNullOrWhiteSpace(evento.Motivo) ? "" : $"\n{evento.Motivo}";
            return $"{fecha}\n{descripcion}{motivo}";
        }

        // ============================================================================
        // ✅ NUEVO: HELPER PARA VERIFICAR SI ES GRUPO ROTATIVO
        // ============================================================================
        private bool EsGrupoRotativo()
        {
            if (_grupoSeleccionado == 0) return false;
            var grupo = _listaGrupo.FirstOrDefault(x => x.IdgenGrupoturno == _grupoSeleccionado);
            return grupo?.ModoGeneracion == "ROTATIVO";
        }
    }

}