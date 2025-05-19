using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class RelacionPadreAlumno : BaseEntity
    {
        public Guid PadreId { get; set; } // FK a Usuario.Id (Rol Padre/Tutor)
        public Guid AlumnoId { get; set; } // FK a Alumno.Id (que a su vez es Usuario.Id)
        public string TipoRelacion { get; set; } = string.Empty; // "Padre", "Madre", "TutorLegal"
        public bool Activa { get; set; } = true;
        public bool PuedeVerCalificaciones { get; set; } = true;
        public bool PuedeVerFinanzas { get; set; } = true;
        public bool EsContactoPrincipal { get; set; } = false;

        // Propiedades de navegación
        public virtual Usuario? Padre { get; set; }
        public virtual Alumno? Alumno { get; set; }
    }
}
