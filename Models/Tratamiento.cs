using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_PracticaHospital.Models
{
    public class Tratamiento
    {
        [Key]
        public int TratamientoId { get; set; }

        [Required(ErrorMessage = "El diagnóstico es requerido")]
        [ForeignKey("Diagnostico")]
        public int DiagnosticoId { get; set; }

        [Required(ErrorMessage = "El nombre del tratamiento es requerido")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 200 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }

        [StringLength(500)]
        public string Medicamentos { get; set; }

        [StringLength(500)]
        public string Recomendaciones { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20)]
        public string Estado { get; set; } = "En Proceso"; // En Proceso, Completado, Suspendido

        [StringLength(500)]
        public string Observaciones { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual Diagnostico Diagnostico { get; set; }
    }
}
