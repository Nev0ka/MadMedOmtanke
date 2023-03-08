using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeLibary.Models;
using System.Security.Cryptography;

namespace MadMedOmtankeApp.Data
{
    public class MadMedOmtankeContext : DbContext
    {
        public MadMedOmtankeContext (DbContextOptions<MadMedOmtankeContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeLibary.Models.Employee> Employee { get; set; } = default!;
        public DbSet<EmployeeLibary.Models.Department> Department { get; set; } = default!;
        public DbSet<EmployeeLibary.Models.Position> Position { get; set; } = default!;
        


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Position>().ToTable("Position");
        }
    }
}
