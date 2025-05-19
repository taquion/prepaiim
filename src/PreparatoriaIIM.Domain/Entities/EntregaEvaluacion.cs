using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class EntregaEvaluacion
    {
        public int EntregaEvaluacionID { get; set; }
        public int EvaluacionID { get; set; }
        public int InscripcionCursoID { get; set; } // Para ligar al alumno específico en ese curso
        public DateTime FechaEntrega { get; set; } = DateTime.UtcNow;
        public string? RutaArchivoBlob { get; set; } // Nombre/ruta en Azure Blob Storage (contenedor: archivosalumnos)
        public string? ComentariosAlumno { get; set; }
        public decimal? Calificacion { get; set; }
        public string? ComentariosProfesor { get; set; }
        public DateTime? FechaCalificacion { get; set; }

        public virtual Evaluacion? Evaluacion { get; set; }
        public virtual InscripcionCurso? InscripcionCurso { get; set; }
    }
}
