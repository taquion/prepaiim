using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class RelacionPagoCargo
    {
        public int RelacionPagoCargoID { get; set; }
        public int PagoID { get; set; }
        public int CargoAlumnoID { get; set; }
        public decimal MontoAplicado { get; set; } // Cuánto de este pago se aplicó a este cargo
        public DateTime FechaAplicacion { get; set; } = DateTime.UtcNow;

        public virtual Pago? Pago { get; set; }
        public virtual CargoAlumno? CargoAlumno { get; set; }
    }
}
