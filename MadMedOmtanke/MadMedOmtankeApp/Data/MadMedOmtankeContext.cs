using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Employee.Models;

namespace MadMedOmtankeApp.Data
{
    public class MadMedOmtankeContext : DbContext
    {
        public MadMedOmtankeContext (DbContextOptions<MadMedOmtankeContext> options)
            : base(options)
        {
        }

        public DbSet<Employee.Models.Employee> Employee { get; set; } = default!;
    }
}
