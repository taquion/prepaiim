using System;
using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class Alumno : Usuario
    {
        public string Matricula { get; set; } = string.Empty;
        public DateTime? FechaIngreso { get; set; }
        public string? Curp { get; set; }
        public string? Nss { get; set; }
        public string? Direccion { get; set; }
        public string? Colonia { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? TelefonoEmergencia { get; set; }
        public string? NombreTutor { get; set; }
        public string? ParentescoTutor { get; set; }
        public string? TelefonoTutor { get; set; }
        public string? EmailTutor { get; set; }
        public bool EsBecado { get; set; } = false;
        public decimal? PorcentajeBeca { get; set; }
        public string? Comentarios { get; set; }
        
        // Relaciones
        public ICollection<InscripcionCurso> Inscripciones { get; set; } = new List<InscripcionCurso>();
        public ICollection<CargoAlumno> Cargos { get; set; } = new List<CargoAlumno>();
        public ICollection<RelacionPadreAlumno> Padres { get; set; } = new List<RelacionPadreAlumno>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<BecaDescuentoAplicado> BecasAplicadas { get; set; } = new List<BecaDescuentoAplicado>();
    }
}
