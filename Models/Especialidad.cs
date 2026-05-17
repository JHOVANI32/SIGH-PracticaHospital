using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGH_PracticaHospital.Models
{
    public class Especialidad
    {
        [Key]
        public int EspecialidadId { get; set; }

        [Required(ErrorMessage = "El nombre de la especialidad es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activa { get; set; } = true;

        // Navegación
        public virtual ICollection<Medico> Medicos { get; set; } = new List<Medico>();
    }
}
