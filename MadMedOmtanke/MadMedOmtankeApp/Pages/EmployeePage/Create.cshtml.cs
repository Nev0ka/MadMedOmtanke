using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadMedOmtankeApp.Pages.Employee
{
    public class CreateModel : PageModel
    {
        private readonly MadMedOmtankeApp.Data.MadMedOmtankeAppContext _context;

        public CreateModel(MadMedOmtankeApp.Data.MadMedOmtankeAppContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public EmployeeLibary.Models.Employee Employee { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Employee.Add(Employee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
