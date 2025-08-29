using Aplicacion.DTOs.Asistencia;
using Aplicacion.DTOs.Clasificador;
using Aplicacion.DTOs.Contrato;
using Aplicacion.DTOs.Horario;
using Aplicacion.DTOs.Persona;
using Aplicacion.DTOs.Segurity;
using Aplicacion.DTOs.Vistas;
using Aplicacion.Features.Contrato.Commands;
using AutoMapper;
using Dominio.Entities;
using Dominio.Entities.Asistencia;
using Dominio.Entities.Contrato;
using Dominio.Entities.Horario;
using Dominio.Entities.Persona;
using Dominio.Entities.Seguridad;
using Dominio.Entities.Vistas;

namespace Aplicacion.Mappings
{
    public class GeneralMappings : Profile
    {
        public GeneralMappings()
        {
            //TODO: Agregar aqui el registro de mapeo para obtenion de consultas  direccion  EntidadDominio --> ModeloDto
            #region QueryDto
            CreateMap<SegvUsuario, SegUsuarioDto>();
            CreateMap<SegUsuario, SeUsuarioDto>();
            CreateMap<SegvMenuobjetos, UserMenuDto>();
            CreateMap<GenClasificador, GenClasificadorDto>();
            CreateMap<GenClasificadortipo, GenClasificadortipoDto>();
            CreateMap<SAsistencia, SAsistenciaDto>();
            CreateMap<SVistaAsistencias, SVistaAsistenciasDto>();
            CreateMap<RrhPersona, RrhPersonaDto>();
            CreateMap<VwMarcacionBiometrico, VwMarcacionBiometricoDto>();
            CreateMap<RrhhTurnodia, RrhhTurnodiaDto>();
            CreateMap<RrhhTurnotolerancia, RrhhTurnotoleranciaDto>();

            CreateMap<RrhhTurnotolerancia, ToleranciaDto>();
            CreateMap<RrhhTurnodia, TurnoDiaDto>();
            CreateMap<GenClasificadortipo, HorarioCompletoDto>();
            CreateMap<RrhDiaevento, RrhDiaeventoDto>();
            CreateMap<RrhPersona, PersonaMinDto>();
            CreateMap<RrhPersona, RrhPersonaCumpleanieroDto>();
            CreateMap<GenGrupoturno, GenGrupoturnoDto>();
            CreateMap<GenGrupoturnoDetalle, GenGrupoturnoDetalleDto>();
            CreateMap<RrhContrato, RrhContratoDto>();     //RrhContrato RrhContrato RrhContrato ...

            #endregion
            //TODO: Agregar aqui el registro de mapeo para ejecucion de comandos  direccion  ModeloDto --> EntidadDominio Ej. : CreateMap<ProductoDto, CapProducto>();

            #region Commands
            CreateMap<SeUsuarioDto, SegUsuario>();
            CreateMap<GenClasificadorDto, GenClasificador>();
            CreateMap<GenClasificadortipoDto, GenClasificadortipo>();
            CreateMap<SAsistenciaDto, SAsistencia>();
            CreateMap<RrhPersonaDto, RrhPersona>();     //RrhPersona RrhPersona RrhPersona ...
            CreateMap<RrhhTurnodiaDto, RrhhTurnodia>();
            CreateMap<RrhhTurnotoleranciaDto, RrhhTurnotolerancia>();

            CreateMap<ToleranciaDto, RrhhTurnotolerancia>();
            CreateMap<TurnoDiaDto, RrhhTurnodia>();
            CreateMap<HorarioCompletoDto, GenClasificadortipo>();
            CreateMap<RrhDiaeventoDto, RrhDiaevento>();
            CreateMap<GenGrupoturnoDto, GenGrupoturno>();
            CreateMap<GenGrupoturnoDetalleDto, GenGrupoturnoDetalle>();
            CreateMap<RrhContratoDto, RrhContrato>();  //RrhContrato RrhContrato RrhContrato ...
            #endregion
        }
    }
}
