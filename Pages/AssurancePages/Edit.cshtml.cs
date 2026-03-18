using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Data;
using ISH_APP.Models;

namespace ISH_APP.Pages.AssurancePages
{
    public class EditModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public EditModel(ISH_APP.Data.ApplicationDbContext context)
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

            var assurance =  await _context.Assurances.FirstOrDefaultAsync(m => m.AssuranceID == id);
            if (assurance == null)
            {
                return NotFound();
            }
            Assurance = assurance;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Assurance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssuranceExists(Assurance.AssuranceID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AssuranceExists(int id)
        {
            return _context.Assurances.Any(e => e.AssuranceID == id);
        }
    }
}
