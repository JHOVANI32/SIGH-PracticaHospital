using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGH_PracticaHospital.Models
{
    public class Medico
    {
        [Key]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El apellido debe tener entre 3 y 100 caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La cédula profesional es requerida")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "La cédula debe tener entre 5 y 20 caracteres")]
        public string CedulaProfesional { get; set; }

        [Required(ErrorMessage = "La especialidad es requerida")]
        [ForeignKey("Especialidad")]
        public int EspecialidadId { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(20)]
        [Phone(ErrorMessage = "El teléfono no es válido")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "El número de consultorio es requerido")]
        [StringLength(20)]
        public string NumeroConsultorio { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual Especialidad Especialidad { get; set; }
        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public virtual ICollection<Diagnostico> Diagnosticos { get; set; } = new List<Diagnostico>();
    }
}
