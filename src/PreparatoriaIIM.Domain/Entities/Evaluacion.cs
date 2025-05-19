using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class Evaluacion
    {
        public int EvaluacionID { get; set; }
        public int CursoID { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string TipoEvaluacion { get; set; } = string.Empty; // "Tarea", "ExamenParcial", "ExamenFinal", "Proyecto", "Participacion"
        public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaEntregaLimite { get; set; }
        public decimal Ponderacion { get; set; } // e.g., 0.20 para 20% de la calificación del curso
        public bool EsCalificable { get; set; } = true;

        public virtual Curso? Curso { get; set; }
        public virtual ICollection<EntregaEvaluacion> Entregas { get; set; } = new List<EntregaEvaluacion>();
    }
}
