using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Data;
using ISH_APP.Models;

namespace ISH_APP.Pages.AssurancePages
{
    public class DeleteModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public DeleteModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Assurance Assurance { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assurance = await _context.Assurances.FirstOrDefaultAsync(m => m.AssuranceID == id);

            if (assurance is not null)
            {
                Assurance = assurance;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assurance = await _context.Assurances.FindAsync(id);
            if (assurance != null)
            {
                Assurance = assurance;
                _context.Assurances.Remove(Assurance);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
