using SIGH_PracticaHospital.Models;

namespace SIGH_PracticaHospital.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Si ya existen datos, salir
            if (context.Especialidades.Any())
            {
                return;
            }

            // Crear especialidades
            var especialidades = new Especialidad[]
            {
                new Especialidad
                {
                    Nombre = "Medicina General",
                    Descripcion = "Atención médica general y diagnóstico inicial de pacientes",
                    Activa = true
                },
                new Especialidad
                {
                    Nombre = "Cardiología",
                    Descripcion = "Especialidad dedicada al diagnóstico y tratamiento de enfermedades del corazón",
                    Activa = true
                },
                new Especialidad
                {
                    Nombre = "Pediatría",
                    Descripcion = "Especialidad médica dedicada a la salud de los niños",
                    Activa = true
                },
                new Especialidad
                {
                    Nombre = "Dermatología",
                    Descripcion = "Especialidad dedicada al diagnóstico y tratamiento de enfermedades de la piel",
                    Activa = true
                },
                new Especialidad
                {
                    Nombre = "Oftalmología",
                    Descripcion = "Especialidad dedicada a la salud de los ojos y visión",
                    Activa = true
                }
            };

            foreach (Especialidad e in especialidades)
            {
                context.Especialidades.Add(e);
            }
            context.SaveChanges();

            // Crear médicos
            var medicos = new Medico[]
            {
                new Medico
                {
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    CedulaProfesional = "MP001",
                    EspecialidadId = 1,
                    Telefono = "+1234567890",
                    CorreoElectronico = "juan.perez@hospital.com",
                    NumeroConsultorio = "101",
                    Activo = true
                },
                new Medico
                {
                    Nombre = "María",
                    Apellido = "García",
                    CedulaProfesional = "MP002",
                    EspecialidadId = 2,
                    Telefono = "+1234567891",
                    CorreoElectronico = "maria.garcia@hospital.com",
                    NumeroConsultorio = "102",
                    Activo = true
                },
                new Medico
                {
                    Nombre = "Carlos",
                    Apellido = "López",
                    CedulaProfesional = "MP003",
                    EspecialidadId = 3,
                    Telefono = "+1234567892",
                    CorreoElectronico = "carlos.lopez@hospital.com",
                    NumeroConsultorio = "103",
                    Activo = true
                }
            };

            foreach (Medico m in medicos)
            {
                context.Medicos.Add(m);
            }
            context.SaveChanges();

            // Crear pacientes
            var pacientes = new Paciente[]
            {
                new Paciente
                {
                    Nombre = "Roberto",
                    Apellido = "Martínez",
                    Cedula = "C001",
                    FechaNacimiento = new DateTime(1980, 5, 15),
                    Genero = "Masculino",
                    GrupoSanguineo = "O+",
                    Direccion = "Calle Principal 123",
                    Telefono = "+9876543210",
                    CorreoElectronico = "roberto.martinez@email.com",
                    Activo = true
                },
                new Paciente
                {
                    Nombre = "Francisca",
                    Apellido = "Rodríguez",
                    Cedula = "C002",
                    FechaNacimiento = new DateTime(1992, 8, 22),
                    Genero = "Femenino",
                    GrupoSanguineo = "A+",
                    Direccion = "Avenida Central 456",
                    Telefono = "+9876543211",
                    CorreoElectronico = "francisca.rodriguez@email.com",
                    Activo = true
                }
            };

            foreach (Paciente p in pacientes)
            {
                context.Pacientes.Add(p);
            }
            context.SaveChanges();
        }
    }
}
