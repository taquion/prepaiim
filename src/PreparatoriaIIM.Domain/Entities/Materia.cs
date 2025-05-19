using System.Collections.Generic;

namespace PreparatoriaIIM.Domain.Entities
{
    public class Materia
    {
        public int MateriaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int? Creditos { get; set; }
        public int? TetramestreSugerido { get; set; } // e.g., 1, 2, 3, 4, 5, 6
        public bool Activa { get; set; } = true;

        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }
}
