using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class Curso : BaseEntity
    {
        public Guid MateriaId { get; set; }
        public Guid TetramestreId { get; set; }
        public Guid ProfesorId { get; set; } // FK a Usuario.UsuarioID (Rol Profesor)
        public string? Grupo { get; set; } // e.g., "A", "B", "Mat-A", "Ves-B"
        public string Turno { get; set; } = string.Empty; // "Matutino", "Vespertino"
        public int? CupoMaximo { get; set; }
        public bool Activo { get; set; } = true;

        // Propiedades de navegación
        public virtual Materia? Materia { get; set; }
        public virtual Tetramestre? Tetramestre { get; set; }
        public virtual Usuario? Profesor { get; set; }
        public virtual ICollection<InscripcionCurso> Inscripciones { get; set; } = new List<InscripcionCurso>();
        public virtual ICollection<ContenidoCurso> ContenidosCurso { get; set; } = new List<ContenidoCurso>();
        public virtual ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
    }
}
