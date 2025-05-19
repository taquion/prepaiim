using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class InscripcionCurso
    {
        public int InscripcionCursoID { get; set; }
        public Guid AlumnoUsuarioID { get; set; } // FK a Alumno.AlumnoID
        public int CursoID { get; set; }
        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
        public decimal? CalificacionPeriodo1 { get; set; }
        public decimal? CalificacionPeriodo2 { get; set; }
        public decimal? CalificacionPeriodo3 { get; set; }
        public decimal? CalificacionFinal { get; set; }
        public string Estado { get; set; } = string.Empty; // "Cursando", "Aprobado", "Reprobado", "Baja"

        public virtual Alumno? Alumno { get; set; }
        public virtual Curso? Curso { get; set; }
        public virtual ICollection<EntregaEvaluacion> EntregasEvaluaciones { get; set; } = new List<EntregaEvaluacion>();
    }
}
