using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ISH_APP.Data;
using ISH_APP.Models;

namespace ISH_APP.Pages.AssurancePages
{
    public class CreateModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public CreateModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Assurance Assurance { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Assurances.Add(Assurance);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
