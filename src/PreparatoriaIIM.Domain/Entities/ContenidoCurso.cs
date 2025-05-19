using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class ContenidoCurso : BaseEntity
    {
        public Guid CursoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string TipoContenido { get; set; } = string.Empty; // "Silabo", "Presentacion", "VideoLink", "Documento", "Tarea"
        public string? RutaArchivoBlob { get; set; } // Nombre/ruta en Azure Blob Storage (contenedor: contenidomaterias)
        public string? UrlExterna { get; set; }
        public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;
        public bool VisibleParaAlumnos { get; set; } = true;

        // Propiedades de navegación
        public virtual Curso? Curso { get; set; }
    }
}
