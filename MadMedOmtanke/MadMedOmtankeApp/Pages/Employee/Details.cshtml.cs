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
    public class DetailsModel : PageModel
    {
        private readonly MadMedOmtankeApp.Data.MadMedOmtankeAppContext _context;

        public DetailsModel(MadMedOmtankeApp.Data.MadMedOmtankeAppContext context)
        {
            _context = context;
        }

      public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FirstOrDefaultAsync(m => m.ID == id);
            if (employee == null)
            {
                return NotFound();
            }
            else 
            {
                Employee = employee;
            }
            return Page();
        }
    }
}
