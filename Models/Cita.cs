using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_PracticaHospital.Models
{
    public class Cita
    {
        [Key]
        public int CitaId { get; set; }

        [Required(ErrorMessage = "El paciente es requerido")]
        [ForeignKey("Paciente")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El médico es requerido")]
        [ForeignKey("Medico")]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es requerida")]
        [DataType(DataType.DateTime)]
        public DateTime FechaCita { get; set; }

        [Required(ErrorMessage = "La hora es requerida")]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        [StringLength(500)]
        public string Motivo { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20)]
        public string Estado { get; set; } = "Programada"; // Programada, Realizada, Cancelada

        [StringLength(500)]
        public string Observaciones { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        public virtual Paciente Paciente { get; set; }
        public virtual Medico Medico { get; set; }
    }
}
