using Microsoft.EntityFrameworkCore;

namespace PreparatoriaIIM.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add your DbSet properties here when you have entities
    // Example:
    // public DbSet<Alumno> Alumnos { get; set; }

    // public DbSet<Profesor> Profesores { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure your entity relationships and constraints here
    }
}
