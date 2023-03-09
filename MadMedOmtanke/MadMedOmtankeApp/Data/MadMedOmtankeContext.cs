using EmployeeLibary.Models;
using Microsoft.EntityFrameworkCore;

namespace MadMedOmtankeApp.Data
{
    public class MadMedOmtankeContext : DbContext
    {
        public MadMedOmtankeContext(DbContextOptions<MadMedOmtankeContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; } = default!;
        public DbSet<Department> Department { get; set; } = default!;
        public DbSet<Position> Position { get; set; } = default!;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Position>().ToTable("Position");
        }
    }
}
