using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class Pago : BaseEntity
    {
        public Guid AlumnoId { get; set; } // A quién beneficia el pago (FK a Alumno.Id)
        public Guid? PagadorId { get; set; } // Quién realizó el pago (FK a Usuario.Id, puede ser el mismo alumno, un padre, etc.)
        public decimal MontoTotalPagado { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.UtcNow;
        public string MetodoPago { get; set; } = string.Empty; // "Transferencia", "Efectivo", "TarjetaCredito", "TarjetaDebito"
        public string? ReferenciaPago { get; set; } // Folio, No. de autorización, etc.
        public Guid RegistradoPorId { get; set; } // FK a Usuario.Id (Staff/Admin que registró)
        public string? Notas { get; set; }
        public string Estado { get; set; } = string.Empty; // "Aplicado", "PendienteDeAplicar", "Reembolsado", "Cancelado"

        // Propiedades de navegación
        public virtual Alumno? Alumno { get; set; }
        public virtual Usuario? Pagador { get; set; }
        public virtual Usuario? RegistradoPor { get; set; }
        public virtual ICollection<RelacionPagoCargo> RelacionesPagoCargo { get; set; } = new List<RelacionPagoCargo>();
    }
}
