using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class ConceptoPago
    {
        public int ConceptoPagoID { get; set; }
        public string Nombre { get; set; } = string.Empty; // e.g., "Inscripción", "Colegiatura Tetra 1", "Examen Extraordinario Mat."
        public string? Descripcion { get; set; }
        public decimal MontoDefault { get; set; }
        public bool EsRecurrente { get; set; } // e.g., colegiatura es recurrente por tetra
        public bool Activo { get; set; } = true;
        // Podría tener un campo para identificar si es por inscripción, colegiatura, extraordinario, certificado, etc.
        public string Categoria { get; set; } = string.Empty; // "Inscripcion", "Colegiatura", "Extraordinario", "Certificado", "Otro"

        public virtual ICollection<CargoAlumno> CargosAlumno { get; set; } = new List<CargoAlumno>();
    }
}
