using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class CargoAlumno : BaseEntity
    {
        public Guid AlumnoId { get; set; } // FK a Alumno.Id
        public Guid ConceptoPagoId { get; set; }
        public Guid? TetramestreId { get; set; } // FK a Tetramestre, nulo si no aplica (e.g. inscripción inicial)
        public decimal MontoOriginal { get; set; } // Monto antes de becas/descuentos
        public decimal MontoAPagar { get; set; } // Monto final después de becas/descuentos
        public decimal SaldoPendiente { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string EstadoPago { get; set; } = string.Empty; // "Pendiente", "PagadoParcialmente", "Pagado", "Vencido", "Cancelado"
        public string? Notas { get; set; }

        // Propiedades de navegación
        public virtual Alumno? Alumno { get; set; }
        public virtual ConceptoPago? ConceptoPago { get; set; }
        public virtual Tetramestre? Tetramestre { get; set; }
        public virtual ICollection<RelacionPagoCargo> RelacionesPagoCargo { get; set; } = new List<RelacionPagoCargo>();
        public virtual ICollection<BecaDescuentoAplicado> BecasDescuentosAplicados { get; set; } = new List<BecaDescuentoAplicado>();
    }
}
