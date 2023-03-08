using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MadMedOmtankeApp.Data
{
    public class MadMedOmtankeAppContext : DbContext
    {
        public MadMedOmtankeAppContext (DbContextOptions<MadMedOmtankeAppContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeLibary.Models.Employee> Employee { get; set; } = default!;
    }
}
