using EmployeeLibary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MadMedOmtankeApp.Pages.EmployeePage
{
    public class DetailsModel : PageModel
    {
        private readonly MadMedOmtankeApp.Data.MadMedOmtankeContext _context;

        public DetailsModel(MadMedOmtankeApp.Data.MadMedOmtankeContext context)
        {
            _context = context;
        }

        public Employee Employee { get; set; }
        public Department Department { get; set; }
        public Position Position { get; set; }

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
