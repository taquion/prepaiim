using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class RelacionPadreAlumno
    {
        public int RelacionID { get; set; }
        public Guid PadreUsuarioID { get; set; } // FK a Usuario.UsuarioID (Rol Padre/Tutor)
        public Guid AlumnoUsuarioID { get; set; } // FK a Alumno.AlumnoID (que a su vez es UsuarioID)
        public string TipoRelacion { get; set; } = string.Empty; // "Padre", "Madre", "TutorLegal"
        public bool Activa { get; set; } = true;
        public bool PuedeVerCalificaciones { get; set; } = true;
        public bool PuedeVerFinanzas { get; set; } = true;
        public bool EsContactoPrincipal { get; set; } = false;

        public virtual Usuario? Padre { get; set; }
        public virtual Alumno? Alumno { get; set; }
    }
}
