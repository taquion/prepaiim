using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class Lead : BaseEntity
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? NombreAlumnoProspecto { get; set; } // Si el contacto es un padre/tutor
        public string? NivelInteres { get; set; } // e.g., "Preparatoria"
        public string? TurnoInteres { get; set; } // "Matutino", "Vespertino", "Indiferente"
        public string? ComoNosConocio { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public string EstadoSeguimiento { get; set; } = "Nuevo"; // "Nuevo", "Contactado", "EnProceso", "Inscrito", "Descartado"
        public string? Notas { get; set; }
        public Guid? AsignadoAId { get; set; } // FK a Usuario.Id (Staff asignado para seguimiento)

        // Propiedades de navegación
        public virtual Usuario? AsignadoA { get; set; }
    }
}
