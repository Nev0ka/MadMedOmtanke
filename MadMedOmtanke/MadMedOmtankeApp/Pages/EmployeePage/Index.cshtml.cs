using EmployeeLibary.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MadMedOmtankeApp.Pages.EmployeePage
{
    public class IndexModel : PageModel
    {
        private readonly Data.MadMedOmtankeContext _context;
        public readonly Position position;
        public readonly Department department;

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
        public IList<Department> Departments { get; set; }
        public IList<Position> Positions { get; set; }


        public async Task OnGetAsync(string sortOrder, string searchString)
        {
            InitialSort = String.IsNullOrEmpty(sortOrder) ? "employeeId_desc" : "";
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            PositionSort = String.IsNullOrEmpty(sortOrder) ? "position_desc" : "";
            DepartmentSort = String.IsNullOrEmpty(sortOrder) ? "department_desc" : "";

            CurrentFilter = searchString;

            IQueryable<Employee> employeeIQ = from e in _context.Employee
                                              select e;

            IQueryable<Department> departmentIQ = from d in _context.Department
                                                  select d;

            IQueryable<Position> positionIQ = from p in _context.Position
                                              select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                employeeIQ = employeeIQ.Where(e => e.Name.Contains(searchString)  
                                                || e.ClosetManager.Contains(searchString));

                departmentIQ = departmentIQ.Where(d => d.Location.Contains(searchString));
                positionIQ = positionIQ.Where(p => p.PositionName.Contains(searchString));
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
                    positionIQ = positionIQ.OrderByDescending(p => p.PositionName);
                    break;
                case "department_desc":
                    departmentIQ = departmentIQ.OrderBy(d => d.Location);
                    break;
                default:
                    employeeIQ = employeeIQ.OrderBy(e => e.Initials);
                    break;
            }

            Employee = await employeeIQ.AsNoTracking().ToListAsync();
            Departments = await departmentIQ.AsNoTracking().ToListAsync();
            Positions = await positionIQ.AsNoTracking().ToListAsync();
        }
    }
}
