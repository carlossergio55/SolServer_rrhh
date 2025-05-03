// Aplicacion/Features/Horario/Commands/UpsertHorarioCompletoCommand.cs
using Aplicacion.DTOs.Horario;
using Aplicacion.Interfaces;                       // ← tus nuevas interfaces
using Aplicacion.Wrappers;
using Ardalis.Specification;
using Dominio.Entities;
using Dominio.Entities.Horario;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Horario.Commands
{
    public record UpsertHorarioCompletoCommand(HorarioCompletoDto Horario)
        : IRequest<Response<int>>;

    public class UpsertHorarioCompletoHandler
    : IRequestHandler<UpsertHorarioCompletoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<GenClasificadortipo> _repoTipo;
        private readonly IRepositoryAsync<RrhhTurnodia> _repoDia;
        private readonly IRepositoryAsync<RrhhTurnotolerancia> _repoTol;

        public UpsertHorarioCompletoHandler(
            IRepositoryAsync<GenClasificadortipo> repoTipo,
            IRepositoryAsync<RrhhTurnodia> repoDia,
            IRepositoryAsync<RrhhTurnotolerancia> repoTol)
        {
            _repoTipo = repoTipo;
            _repoDia = repoDia;
            _repoTol = repoTol;
        }

        public async Task<Response<int>> Handle(
            UpsertHorarioCompletoCommand req, CancellationToken ct)
        {
            var dto = req.Horario;
            GenClasificadortipo tipo;
            bool nuevo = !dto.IdgenClasificadortipo.HasValue;

            // 1) Tipo ────────────────────────────────────────────────
            if (nuevo)
            {
                tipo = new GenClasificadortipo
                {
                    Descripcion = dto.Descripcion,
                    Abreviatura = dto.Abreviatura
                };
                await _repoTipo.AddAsync(tipo, ct);
            }
            else
            {
                tipo = await _repoTipo.GetByIdAsync(dto.IdgenClasificadortipo.Value, ct)
                       ?? throw new KeyNotFoundException("Horario no encontrado");

                tipo.Descripcion = dto.Descripcion;
                tipo.Abreviatura = dto.Abreviatura;
                await _repoTipo.UpdateAsync(tipo, ct);   // ← cambia Update por UpdateAsync

                // --- borrar hijos actuales ---
                var diasOld = (await _repoDia.ListAsync(ct))
                              .Where(d => d.IdgenClasificadortipo == tipo.IdgenClasificadortipo);
                foreach (var d in diasOld)
                    await _repoDia.DeleteAsync(d, ct);

                var tolOld = (await _repoTol.ListAsync(ct))
                             .Where(t => t.IdgenClasificadortipo == tipo.IdgenClasificadortipo);
                foreach (var t in tolOld)
                    await _repoTol.DeleteAsync(t, ct);
            }

            // 2) Tolerancia ──────────────────────────────────────────
            var tol = dto.Tolerancia;
            await _repoTol.AddAsync(new RrhhTurnotolerancia
            {
                IdgenClasificadortipo = tipo.IdgenClasificadortipo,
                ToleranciaAtraso = tol.ToleranciaAtraso,
                ToleranciaFalta = tol.ToleranciaFalta,
                ToleranciaSalida = tol.ToleranciaSalida,
                SalidaAdelantada = tol.SalidaAdelantada,
                Puntualidad = tol.Puntualidad,
                Prioridad = tol.Prioridad
            }, ct);

            // 3) Días (varios) ──────────────────────────────────────
            foreach (var d in dto.Turnos)
            {
                await _repoDia.AddAsync(new RrhhTurnodia
                {
                    IdgenClasificadortipo = tipo.IdgenClasificadortipo,
                    DiaSemana = d.DiaSemana,
                    HoraEntrada = d.HoraEntrada,
                    HoraSalida = d.HoraSalida,
                    TiempoExtra = d.TiempoExtra
                }, ct);
            }

            return new Response<int>(tipo.IdgenClasificadortipo,
                nuevo ? "Horario creado" : "Horario actualizado");
        }
    }

}
