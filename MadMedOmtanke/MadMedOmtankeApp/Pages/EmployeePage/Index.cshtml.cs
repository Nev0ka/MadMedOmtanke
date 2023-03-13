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
        public string PositionSort { get; set; }
        public string DepartmentSort { get; set; }
        public string CurrentFilter { get; set; }

        public IList<Employee> Employee { get; set; }

        public async Task OnGetAsync(string sortOrder, string searchString)
        {
            InitialSort = String.IsNullOrEmpty(sortOrder) ? "employeeId_desc" : "";
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            PositionSort = String.IsNullOrEmpty(sortOrder) ? "position_desc" : "";
            DepartmentSort = String.IsNullOrEmpty(sortOrder) ? "department_desc" : "";

            CurrentFilter = searchString;

            IQueryable<Employee> employeeIQ = from e in _context.Employee
                                              select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                employeeIQ = employeeIQ.Where(e => e.Name.Contains(searchString) 
                                                || e.DepartmentName.Contains(searchString) 
                                                || e.PositionName.Contains(searchString) 
                                                || e.ClosetManager.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "employeeId_desc":
                    employeeIQ = employeeIQ.OrderByDescending(e => e.Initials);
                    break;
                case "name_desc":
                    employeeIQ = employeeIQ.OrderByDescending(e => e.Name);
                    break;
                case "position_desc":
                    employeeIQ = employeeIQ.OrderByDescending(e => e.PositionName);
                    break;
                case "department_desc":
                    employeeIQ = employeeIQ.OrderBy(e => e.DepartmentName);
                    break;
                default:
                    employeeIQ = employeeIQ.OrderBy(e => e.Initials);
                    break;
            }

            Employee = await employeeIQ.AsNoTracking().ToListAsync();
        }
    }
}
