using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class RelacionPagoCargo : BaseEntity
    {
        public Guid PagoId { get; set; }
        public Guid CargoAlumnoId { get; set; }
        public decimal MontoAplicado { get; set; } // Cuánto de este pago se aplicó a este cargo
        public DateTime FechaAplicacion { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        public virtual Pago? Pago { get; set; }
        public virtual CargoAlumno? CargoAlumno { get; set; }
    }
}
