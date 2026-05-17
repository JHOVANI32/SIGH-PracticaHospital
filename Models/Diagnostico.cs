using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_PracticaHospital.Models
{
    public class Diagnostico
    {
        [Key]
        public int DiagnosticoId { get; set; }

        [Required(ErrorMessage = "El paciente es requerido")]
        [ForeignKey("Paciente")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El médico es requerido")]
        [ForeignKey("Medico")]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "La descripción del diagnóstico es requerida")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El código CIE-10 es requerido")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 10 caracteres")]
        public string CodigoCIE10 { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        [Required(ErrorMessage = "La severidad es requerida")]
        [StringLength(20)]
        public string Severidad { get; set; } // Leve, Moderada, Severa

        [DataType(DataType.DateTime)]
        public DateTime FechaDiagnostico { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual Paciente Paciente { get; set; }
        public virtual Medico Medico { get; set; }
    }
}
