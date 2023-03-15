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
        public IList<Department> Departments { get; set; }
        public IList<Position> Positions { get; set; }

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
                Positions = _context.Position.ToList();
                Departments = _context.Department.ToList();
            }
            return Page();
        }
    }
}
