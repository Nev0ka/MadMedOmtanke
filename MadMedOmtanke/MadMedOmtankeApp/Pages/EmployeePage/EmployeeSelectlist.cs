using EmployeeLibary.Models;
using MadMedOmtankeApp.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MadMedOmtankeApp.Pages.EmployeePage
{
    public class EmployeeSelectlist : PageModel
    {
        public SelectList PositionSL { get; set; }
        public SelectList DepartmentSL { get; set; }

        public void PopulatePositionDropdownList(MadMedOmtankeContext _context, object selectedPosition = null)
        {
            var positionQuery = from p in _context.Position
                                orderby p.PositionName
                                select p;
            PositionSL = new SelectList(positionQuery.AsNoTracking(),
                nameof(Position.ID),
                nameof(Position.PositionName),
                selectedPosition);
        }

        public void PopulateDepartmentDropdownList(MadMedOmtankeContext _context, object selectedDepartment = null)
        {
            var departmentQuery = from d in _context.Department
                                  orderby d.Location
                                  select d;
            DepartmentSL = new SelectList(departmentQuery.AsNoTracking(),
                nameof(Department.ID),
                nameof(Department.Location),
                selectedDepartment);
        }
    }
}
