using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class InscripcionCurso : BaseEntity
    {
        public Guid AlumnoId { get; set; }
        public Guid CursoId { get; set; }
        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
        public decimal? CalificacionPeriodo1 { get; set; }
        public decimal? CalificacionPeriodo2 { get; set; }
        public decimal? CalificacionPeriodo3 { get; set; }
        public decimal? CalificacionFinal { get; set; }
        public string Estado { get; set; } = string.Empty; // "Cursando", "Aprobado", "Reprobado", "Baja"

        // Propiedades de navegación
        public virtual Alumno? Alumno { get; set; }
        public virtual Curso? Curso { get; set; }
        public virtual ICollection<EntregaEvaluacion> EntregasEvaluaciones { get; set; } = new List<EntregaEvaluacion>();
    }
}
