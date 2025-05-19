using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class BecaDescuento
    {
        public int BecaDescuentoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Tipo { get; set; } = string.Empty; // "Porcentaje", "MontoFijo"
        public decimal Valor { get; set; } // Si Porcentaje, 0.10 para 10%. Si MontoFijo, el importe.
        public bool EsPermanente { get; set; } // Aplica a todos los cargos elegibles o solo una vez
        public DateTime? FechaInicioVigencia { get; set; }
        public DateTime? FechaFinVigencia { get; set; }
        public bool Activa { get; set; } = true;
        // Podría haber condiciones para su aplicación, ej. "Solo colegiaturas", "Promedio > 9.0"
        public string? CondicionesAplicacion { get; set; }

        public virtual ICollection<BecaDescuentoAplicado> Aplicaciones { get; set; } = new List<BecaDescuentoAplicado>();
    }
}
