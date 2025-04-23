using Aplicacion.DTOs.Asistencia;
using Aplicacion.DTOs.Clasificador;
using Aplicacion.DTOs.Persona;
using Aplicacion.DTOs.Segurity;
using Aplicacion.DTOs.Vistas;
using AutoMapper;
using Dominio.Entities;
using Dominio.Entities.Asistencia;
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


            #endregion
            //TODO: Agregar aqui el registro de mapeo para ejecucion de comandos  direccion  ModeloDto --> EntidadDominio Ej. : CreateMap<ProductoDto, CapProducto>();

            #region Commands
            CreateMap<SeUsuarioDto, SegUsuario>();
            CreateMap<GenClasificadorDto, GenClasificador>();
            CreateMap<GenClasificadortipoDto, GenClasificadortipo>();
            CreateMap<SAsistenciaDto, SAsistencia>();
            CreateMap<RrhPersonaDto, RrhPersona>();

            #endregion
        }
    }
}
