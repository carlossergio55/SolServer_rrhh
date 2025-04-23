﻿using Dominio.Entities;
using Dominio.Entities.Asistencia;
using Dominio.Entities.Persona;
using Dominio.Entities.Seguridad;
using Dominio.Entities.Vistas;
using Microsoft.EntityFrameworkCore;

namespace Persistencia.Contexts
{
    public class GenericContexDb : DbContext
    {
        public GenericContexDb(DbContextOptions options) : base(options)
        {
        }

        //TODO: Agregar aqui DbSets de las entidades de dominio correspondiente al contexto de conexcion general.
        #region DbSets
        public DbSet<SegUsuario> SegUsuario { get; set; }
        public DbSet<GenClasificador> Clasificador { get; set; }
        public DbSet<GenClasificadortipo> ClasificadorTipo { get; set; }
        public DbSet<GenClasificador> GenClasificador { get; set; }
        public DbSet<GenClasificadortipo> GenClasificadortipo { get; set; }
        public DbSet<SAsistencia> SAsistencia { get; set; }
        public DbSet<SVistaAsistencias> SVistaAsistencias { get; set; }
        public DbSet<RrhPersona> RrhPersona { get; set; }
        public DbSet<VwMarcacionBiometrico> VwMarcacionBiometrico { get; set; }

        #endregion

    }
}
