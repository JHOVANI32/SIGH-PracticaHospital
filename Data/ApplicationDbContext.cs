using Microsoft.EntityFrameworkCore;
using SIGH_PracticaHospital.Models;

namespace SIGH_PracticaHospital.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Diagnostico> Diagnosticos { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales de relaciones y restricciones

            // Paciente - Cita (1:N)
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Medico - Cita (1:N)
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Medico)
                .WithMany(m => m.Citas)
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Especialidad - Medico (1:N)
            modelBuilder.Entity<Medico>()
                .HasOne(m => m.Especialidad)
                .WithMany(e => e.Medicos)
                .HasForeignKey(m => m.EspecialidadId)
                .OnDelete(DeleteBehavior.Restrict);

            // Paciente - Diagnostico (1:N)
            modelBuilder.Entity<Diagnostico>()
                .HasOne(d => d.Paciente)
                .WithMany(p => p.Diagnosticos)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Medico - Diagnostico (1:N)
            modelBuilder.Entity<Diagnostico>()
                .HasOne(d => d.Medico)
                .WithMany(m => m.Diagnosticos)
                .HasForeignKey(d => d.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Diagnostico - Tratamiento (1:N)
            modelBuilder.Entity<Tratamiento>()
                .HasOne(t => t.Diagnostico)
                .WithMany()
                .HasForeignKey(t => t.DiagnosticoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para mejorar rendimiento
            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.Cedula)
                .IsUnique();

            modelBuilder.Entity<Medico>()
                .HasIndex(m => m.CedulaProfesional)
                .IsUnique();

            modelBuilder.Entity<Cita>()
                .HasIndex(c => c.FechaCita);
        }
    }
}
