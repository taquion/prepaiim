using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class Tetramestre : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty; // e.g., "Primavera 2025", "Verano 2025", "Otoño 2025"
        public string Periodo { get; set; } = string.Empty; // Primavera, Verano, Otoño
        public int Anio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; } // Para indicar si es el actual, futuro, o pasado

        // Propiedades de navegación
        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
        public virtual ICollection<CargoAlumno> CargosAlumno { get; set; } = new List<CargoAlumno>();
    }
}
