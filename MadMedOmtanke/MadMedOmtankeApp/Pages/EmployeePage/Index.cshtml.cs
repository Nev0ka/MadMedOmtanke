using EmployeeLibary.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MadMedOmtankeApp.Pages.EmployeePage
{
    public class IndexModel : PageModel
    {
        private readonly Data.MadMedOmtankeContext _context;

        public IndexModel(Data.MadMedOmtankeContext context)
        {
            _context = context;
        }

        public string InitialSort { get; set; }
        public string NameSort { get; set; }
        public Employee showEmployee { get; set; }
        public Position showPosition { get; set; }

        public IList<Employee> Employee { get; set; }

        public async Task OnGetAsync(string sortOrder)
        {
            InitialSort = String.IsNullOrEmpty(sortOrder) ? "employeeId_desc" : "";
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            IQueryable<Employee> employeeIQ = from e in _context.Employee
                                              select e;

            switch (sortOrder)
            {
                case "employeeId_desc":
                    employeeIQ = employeeIQ.OrderByDescending(e => e.Initials);
                    break;
                case "name_desc":
                    employeeIQ = employeeIQ.OrderByDescending(e => e.Name);
                    break;
                default:
                    employeeIQ = employeeIQ.OrderBy(e => e.Initials);
                    break;
            }

            Employee = await employeeIQ.AsNoTracking().ToListAsync();

            //if(showEmployee.PositionID == showPosition.ID)
            //{
            //    showEmployee.PositionID = showPosition.PositionName;
            //}
        }
    }
}
