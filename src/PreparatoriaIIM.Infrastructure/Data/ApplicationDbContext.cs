using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PreparatoriaIIM.Domain.Entities;

namespace PreparatoriaIIM.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para cada entidad
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<BecaDescuento> BecasDescuentos { get; set; }
        public DbSet<BecaDescuentoAplicado> BecasDescuentosAplicados { get; set; }
        public DbSet<CargoAlumno> CargosAlumno { get; set; }
        public DbSet<ConceptoPago> ConceptosPago { get; set; }
        public DbSet<ContenidoCurso> ContenidosCurso { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<EntregaEvaluacion> EntregasEvaluacion { get; set; }
        public DbSet<Evaluacion> Evaluaciones { get; set; }
        public DbSet<InscripcionCurso> InscripcionesCurso { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<RelacionPadreAlumno> RelacionesPadreAlumno { get; set; }
        public DbSet<RelacionPagoCargo> RelacionesPagoCargo { get; set; }
        public DbSet<IdentityRole<Guid>> Roles { get; set; }
        public DbSet<Tetramestre> Tetramestres { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aquí se pueden agregar configuraciones de entidades usando Fluent API
            // Por ejemplo, configuraciones de claves foráneas, índices, etc.
            
            // Ejemplo de configuración de clave primaria compuesta
            modelBuilder.Entity<RelacionPagoCargo>()
                .HasKey(rc => new { rc.PagoID, rc.CargoAlumnoID });

            modelBuilder.Entity<BecaDescuentoAplicado>()
                .HasKey(ba => ba.BecaDescuentoAplicadoID);

            modelBuilder.Entity<RelacionPadreAlumno>()
                .HasKey(pa => pa.RelacionID);

            // Configuraciones adicionales pueden ir aquí
        }
    }
}
