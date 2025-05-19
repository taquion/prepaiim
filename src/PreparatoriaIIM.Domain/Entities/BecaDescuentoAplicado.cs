using System;

namespace PreparatoriaIIM.Domain.Entities
{
    public class BecaDescuentoAplicado
    {
        public int BecaDescuentoAplicadoID { get; set; }
        public int CargoAlumnoID { get; set; }
        public int BecaDescuentoID { get; set; }
        public decimal MontoDescontado { get; set; }
        public DateTime FechaAplicacion { get; set; } = DateTime.UtcNow;
        public Guid? AprobadoPorUsuarioID { get; set; } // FK a Usuario.UsuarioID (Admin que aprobó, si requiere)
        public string? Notas { get; set; }

        public virtual CargoAlumno? CargoAlumno { get; set; }
        public virtual BecaDescuento? BecaDescuento { get; set; }
        public virtual Usuario? AprobadoPor { get; set; }
    }
}
