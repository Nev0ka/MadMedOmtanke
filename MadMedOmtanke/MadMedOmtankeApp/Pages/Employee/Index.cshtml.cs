using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Employee.Models;
using MadMedOmtankeApp.Data;

namespace MadMedOmtankeApp.Pages.Employee
{
    public class IndexModel : PageModel
    {
        private readonly MadMedOmtankeApp.Data.MadMedOmtankeAppContext _context;

        public IndexModel(MadMedOmtankeApp.Data.MadMedOmtankeAppContext context)
        {
            _context = context;
        }

        public IList<Employee> Employee { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Employee != null)
            {
                Employee = await _context.Employee.ToListAsync();
            }
        }
    }
}
