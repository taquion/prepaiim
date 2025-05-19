using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class EntregaEvaluacion : BaseEntity
    {
        public Guid EvaluacionId { get; set; }
        public Guid InscripcionCursoId { get; set; } // Para ligar al alumno específico en ese curso
        public DateTime FechaEntrega { get; set; } = DateTime.UtcNow;
        public string? RutaArchivoBlob { get; set; } // Nombre/ruta en Azure Blob Storage (contenedor: archivosalumnos)
        public string? ComentariosAlumno { get; set; }
        public decimal? Calificacion { get; set; }
        public string? ComentariosProfesor { get; set; }
        public DateTime? FechaCalificacion { get; set; }

        // Propiedades de navegación
        public virtual Evaluacion? Evaluacion { get; set; }
        public virtual InscripcionCurso? InscripcionCurso { get; set; }
    }
}
