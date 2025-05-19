using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class Lead
    {
        public int LeadID { get; set; }
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
        public Guid? AsignadoAUsuarioID { get; set; } // FK a Usuario.UsuarioID (Staff asignado para seguimiento)

        public virtual Usuario? AsignadoA { get; set; }
    }
}
