using Aplicacion.DTOs.Persona;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using Dominio.Entities.Horario;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Diaevento.Queries
{
    public class GetUltimoTurnoQuery : IRequest<Response<UltimoTurnoDto>>
    {
        public int IdPersona { get; set; }
    }
    public class GetUltimoTurnoQueryHandler :
        IRequestHandler<GetUltimoTurnoQuery, Response<UltimoTurnoDto>>
    {
        private readonly IRepositoryAsync<RrhDiaevento> _repoDiaevento;
        private readonly IRepositoryAsync<GenGrupoturnoDetalle> _repoDetalle;

        public GetUltimoTurnoQueryHandler(
            IRepositoryAsync<RrhDiaevento> repoDiaevento,
            IRepositoryAsync<GenGrupoturnoDetalle> repoDetalle)
        {
            _repoDiaevento = repoDiaevento;
            _repoDetalle = repoDetalle;
        }

        public async Task<Response<UltimoTurnoDto>> Handle(
            GetUltimoTurnoQuery request,
            CancellationToken ct)
        {
            // ✅ CORREGIDO: Usar ListAsync y obtener el primero
            var spec = new UltimoTurnoSpecification(request.IdPersona);
            var registros = await _repoDiaevento.ListAsync(spec, ct);
            var ultimoRegistro = registros.FirstOrDefault();

            // Si no hay registros previos
            if (ultimoRegistro == null)
            {
                return new Response<UltimoTurnoDto>(new UltimoTurnoDto
                {
                    TieneRegistros = false,
                    Fecha = null,
                    IdgenClasificadortipo = null,
                    OrdenActual = null,
                    OrdenSiguiente = null
                });
            }

            // Construir DTO con información del último turno
            var dto = new UltimoTurnoDto
            {
                TieneRegistros = true,
                Fecha = ultimoRegistro.Fecha,
                IdgenClasificadortipo = ultimoRegistro.IdgenClasificadortipo,
                DescripcionTurno = ultimoRegistro.GenClasificadortipo?.Descripcion,
                OrdenActual = null,
                OrdenSiguiente = null
            };

            // Si el turno pertenece a un grupo, calcular el siguiente orden
            var detallesActuales = await _repoDetalle.ListAsync(
                new DetalleByClasificadorSpecification(ultimoRegistro.IdgenClasificadortipo),
                ct);

            var detalleActual = detallesActuales.FirstOrDefault();

            if (detalleActual != null)
            {
                dto.IdgenGrupoturno = detalleActual.IdgenGrupoturno;
                dto.NombreGrupo = detalleActual.GenGrupoturno?.Nombre;
                dto.OrdenActual = detalleActual.Orden;

                // Obtener todos los detalles del grupo para calcular el siguiente
                var todosDetalles = await _repoDetalle.ListAsync(
                    new DetallesByGrupoSpecification(detalleActual.IdgenGrupoturno),
                    ct);

                if (todosDetalles.Any())
                {
                    var maxOrden = todosDetalles.Max(d => d.Orden);
                    dto.OrdenSiguiente = (dto.OrdenActual >= maxOrden)
                        ? 1
                        : dto.OrdenActual + 1;
                }
            }

            return new Response<UltimoTurnoDto>(dto);
        }
    }
    public class UltimoTurnoSpecification : Specification<RrhDiaevento>
    {
        public UltimoTurnoSpecification(int idPersona)
        {
            Query
                .Include(x => x.GenClasificadortipo)
                .Where(x => x.IdrrhPersona == idPersona)
                .OrderByDescending(x => x.Fecha)
                .Take(1);
        }
    }

    // Buscar detalle de grupo por clasificador
    public class DetalleByClasificadorSpecification : Specification<GenGrupoturnoDetalle>
    {
        public DetalleByClasificadorSpecification(int idClasificador)
        {
            Query
                .Include(x => x.GenGrupoturno)
                .Where(x => x.IdgenClasificadortipo == idClasificador)
                .OrderBy(x => x.Orden)
                .Take(1);
        }
    }

    // Obtener todos los detalles de un grupo
    public class DetallesByGrupoSpecification : Specification<GenGrupoturnoDetalle>
    {
        public DetallesByGrupoSpecification(int idGrupo)
        {
            Query
                .Where(x => x.IdgenGrupoturno == idGrupo)
                .OrderBy(x => x.Orden);
        }
    }
}
