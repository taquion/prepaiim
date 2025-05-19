using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class BecaDescuentoAplicado : BaseEntity
    {
        public Guid CargoAlumnoId { get; set; }
        public Guid BecaDescuentoId { get; set; }
        public decimal MontoDescontado { get; set; }
        public DateTime FechaAplicacion { get; set; } = DateTime.UtcNow;
        public Guid? AprobadoPorUsuarioId { get; set; } // FK a Usuario.Id (Admin que aprobó, si requiere)
        public string? Notas { get; set; }

        // Propiedades de navegación
        public virtual CargoAlumno? CargoAlumno { get; set; }
        public virtual BecaDescuento? BecaDescuento { get; set; }
        public virtual Usuario? AprobadoPor { get; set; }
    }
}
