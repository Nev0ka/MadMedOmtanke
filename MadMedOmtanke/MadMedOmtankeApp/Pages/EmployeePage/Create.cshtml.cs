using EmployeeLibary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MadMedOmtankeApp.Pages.EmployeePage
{
    public class CreateModel : EmployeeSelectlist
    {
        private readonly Data.MadMedOmtankeContext _context;
        internal static SelectList SelectedPosition { get; set; }
        internal static SelectList SelectedDepartment { get; set; }

        public CreateModel(Data.MadMedOmtankeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; }
        public Position Position { get; set; }
        public Department Department { get; set; }

        public IActionResult OnGet()
        {
            PopulatePositionDropdownList(_context);
            PopulateDepartmentDropdownList(_context);
            SelectedPosition = PositionSL;
            SelectedDepartment = DepartmentSL;
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var position in SelectedPosition)
            {
                if(Convert.ToInt32(position.Value)== Employee.PositionID)
                {
                    Position.PositionName = position.Text;
                    break;
                }
            }

            foreach (var department in SelectedDepartment)
            {
                if (Convert.ToInt32(department.Value) == Employee.DepartmentID)
                {
                    Department.Location = department.Text;
                    break;
                }
            }

            _context.Employee.Add(Employee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
