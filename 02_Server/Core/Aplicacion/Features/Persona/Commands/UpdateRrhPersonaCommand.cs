using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Persona;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Persona.Commands
{
    public class UpdateRrhPersonaCommand : IRequest<Response<int>>
    {
        public int IdrrhPersona { get; set; }
        public string NombreApellido { get; set; }
        public string Ci { get; set; }
        public string Exp { get; set; }
        public string Celular { get; set; }

        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Extension { get; set; }
        public string Contrasena { get; set; }
        public DateTime? FechaNacimiento { get; set; }


        public int? Edad { get; set; }
        public string Domicilio { get; set; }
        public string Residencia { get; set; }
        public DateTime? InicioContrato { get; set; }
        public string Correo { get; set; }


        /*id*/
        public int? IdgenUnidad { get; set; }    //Idgen UNIDAD ...
        public int? IdgenCategoria { get; set; }
        public int? IdgenClase { get; set; }
        public int? IdgenNivelsalarial { get; set; }
        public int? IdgenPuestodenominacion { get; set; }
        public string? Sexo { get; set; }
        public int? IdgenProfesion { get; set; }
        public int? IdgengrupoTrabajo { get; set; }
        public int? IdgenPuestodescripcion { get; set; }
        public char? Estado { get; set; }



        public class Handler : IRequestHandler<UpdateRrhPersonaCommand, Response<int>>
        {
            private readonly IRepositoryAsync<RrhPersona> _repo;

            public Handler(IRepositoryAsync<RrhPersona> repo)
            {
                _repo = repo;
            }

            public async Task<Response<int>> Handle(UpdateRrhPersonaCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repo.GetByIdAsync(request.IdrrhPersona);
                if (entity == null)
                    throw new KeyNotFoundException("Persona no encontrada");

                //entity.NombreApellido = request.NombreApellido;
                entity.Ci = request.Ci;
                entity.Exp = request.Exp;
                entity.Celular = request.Celular;


                entity.ApellidoPaterno = request.ApellidoPaterno;
                entity.ApellidoMaterno = request.ApellidoMaterno;

                entity.Nombre = request.Nombre;
                entity.Apellido = request.Apellido;
                entity.Extension = request.Extension;
                entity.Contrasena = request.Contrasena;
                entity.FechaNacimiento = request.FechaNacimiento;

                entity.Edad = request.Edad;
                entity.Domicilio = request.Domicilio;
                entity.Residencia = request.Residencia;
                entity.InicioContrato = request.InicioContrato;
                entity.Correo = request.Correo;


                entity.IdgenUnidad = request.IdgenUnidad;
                entity.IdgenCategoria = request.IdgenCategoria;
                entity.IdgenClase = request.IdgenClase;
                entity.IdgenNivelsalarial = request.IdgenNivelsalarial;
                entity.IdgenPuestodenominacion = request.IdgenPuestodenominacion;
                entity.Sexo = request.Sexo;
                entity.IdgenProfesion = request.IdgenProfesion;
                entity.IdgengrupoTrabajo = request.IdgengrupoTrabajo;
                entity.IdgenPuestodescripcion = request.IdgenPuestodescripcion;
                entity.Estado = request.Estado;

                await _repo.UpdateAsync(entity);
                return new Response<int>(entity.IdrrhPersona);
            }
        }
    }

}




