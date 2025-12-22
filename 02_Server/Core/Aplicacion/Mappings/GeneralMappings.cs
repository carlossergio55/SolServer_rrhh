using Aplicacion.DTOs.Asistencia;
using Aplicacion.DTOs.BajaMedica;
using Aplicacion.DTOs.Clasificador;
using Aplicacion.DTOs.Comision;
using Aplicacion.DTOs.Contrato;
using Aplicacion.DTOs.Falta;
using Aplicacion.DTOs.Horario;
using Aplicacion.DTOs.Permisos;
using Aplicacion.DTOs.Persona;
using Aplicacion.DTOs.Segurity;
using Aplicacion.DTOs.Vacacion;
using Aplicacion.DTOs.Vistas;
using Aplicacion.Features.Contrato.Commands;
using AutoMapper;
using Dominio.Entities;
using Dominio.Entities.Asistencia;
using Dominio.Entities.BajaMedica;
using Dominio.Entities.Comision;
using Dominio.Entities.Contrato;
using Dominio.Entities.Falta;
using Dominio.Entities.Horario;
using Dominio.Entities.Permisos;
using Dominio.Entities.Persona;
using Dominio.Entities.Seguridad;
using Dominio.Entities.Vacacion;
using Dominio.Entities.Vistas;

namespace Aplicacion.Mappings
{
    public class GeneralMappings : Profile
    {
        public GeneralMappings()
        {
            //TODO: Agregar aqui el registro de mapeo para obtenion de consultas  direccion  EntidadDominio --> ModeloDto
            #region QueryDto
            CreateMap<SegvUsuario,           SegUsuarioDto>();
            CreateMap<SegUsuario,            SeUsuarioDto>();


            CreateMap<SegvMenuobjetos,       UserMenuDto>();
            CreateMap<GenClasificador,       GenClasificadorDto>();
            CreateMap<GenClasificadortipo,   GenClasificadortipoDto>();
            CreateMap<SAsistencia,           SAsistenciaDto>();
            CreateMap<SVistaAsistencias,     SVistaAsistenciasDto>();
            CreateMap<RrhPersona,            RrhPersonaDto>();
            CreateMap<VwMarcacionBiometrico, VwMarcacionBiometricoDto>();
            CreateMap<RrhhTurnodia,          RrhhTurnodiaDto>();
            CreateMap<RrhhTurnotolerancia,   RrhhTurnotoleranciaDto>();
            CreateMap<RrhhTurnotolerancia,   ToleranciaDto>();
            CreateMap<RrhhTurnodia,          TurnoDiaDto>();
            CreateMap<GenClasificadortipo,   HorarioCompletoDto>();
            CreateMap<RrhDiaevento,          RrhDiaeventoDto>();
            CreateMap<RrhPersona,            RrhPersonaCumpleanieroDto>();
            CreateMap<GenGrupoturno,         GenGrupoturnoDto>();
            CreateMap<GenGrupoturnoDetalle,  GenGrupoturnoDetalleDto>();
            CreateMap<RrhContrato,           RrhContratoDto>();     //RrhContrato RrhContrato RrhContrato ...
            CreateMap<RrhBajaMedica,         RrhBajaMedicaDto>();
            CreateMap<RrhFalta,              RrhFaltaDto>();  //Falta ...
            CreateMap<RrhVacacion,           RrhVacacionDto>();
            CreateMap<RrhComision,           RrhComisionDto>();
            CreateMap<RrhPersona,            PersonaMinDto>();

            CreateMap<SRrhFeriado, SRrhFeriadoDto>();
            CreateMap<SRrhSolicitud, SRrhSolicitudDto>();
            CreateMap<CrearJustificacionDto, RrhJustificacionOmision>();
            CreateMap<RrhReporte, RrhReporteDto>();



            #endregion
            //TODO: Agregar aqui el registro de mapeo para ejecucion de comandos  direccion  ModeloDto --> EntidadDominio Ej. : CreateMap<ProductoDto, CapProducto>();
            //This would be the "Backend" ...

            #region Commands
            CreateMap<SeUsuarioDto,             SegUsuario>();
            CreateMap<GenClasificadorDto,       GenClasificador>();
            CreateMap<GenClasificadortipoDto,   GenClasificadortipo>();
            CreateMap<SAsistenciaDto,           SAsistencia>();
            CreateMap<RrhPersonaDto,            RrhPersona>();     //RrhPersona RrhPersona RrhPersona ...
            CreateMap<RrhhTurnodiaDto,          RrhhTurnodia>();
            CreateMap<RrhhTurnotoleranciaDto,   RrhhTurnotolerancia>();
            CreateMap<ToleranciaDto,            RrhhTurnotolerancia>();
            CreateMap<TurnoDiaDto,              RrhhTurnodia>();
            CreateMap<HorarioCompletoDto,       GenClasificadortipo>();
            CreateMap<RrhDiaeventoDto,          RrhDiaevento>();
            CreateMap<GenGrupoturnoDto,         GenGrupoturno>();
            CreateMap<GenGrupoturnoDetalleDto,  GenGrupoturnoDetalle>();


            CreateMap<RrhContratoDto,           RrhContrato>();  //RrhContrato RrhContrato RrhContrato ...
            CreateMap<RrhBajaMedicaDto,         RrhBajaMedica>();
            CreateMap<RrhFaltaDto,              RrhFalta>();  //Falta ...
            CreateMap<RrhVacacionDto,           RrhVacacion>();
            CreateMap<RrhComisionDto,           RrhComision>();
            CreateMap<PersonaMinDto,            RrhPersona>();
            CreateMap<SRrhFeriadoDto,           SRrhFeriado>();
            CreateMap<SRrhSolicitudDto,         SRrhSolicitud>();
            CreateMap<RrhJustificacionOmision, JustificacionDto>();
            CreateMap<RrhReporteDto, RrhReporte>();


            #endregion
        }
    }
}
