using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MadMedOmtankeApp.Pages.Employee
{
    public class IndexModel : PageModel
    {
        private readonly MadMedOmtankeApp.Data.MadMedOmtankeAppContext _context;

        public IndexModel(MadMedOmtankeApp.Data.MadMedOmtankeAppContext context)
        {
            _context = context;
        }

        public IList<EmployeeLibary.Models.Employee> Employee { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Employee != null)
            {
                Employee = await _context.Employee.ToListAsync();
            }
        }
    }
}
