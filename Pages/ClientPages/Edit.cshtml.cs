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

namespace ISH_APP.Pages.ClientPages
{
    public class EditModel : BasePageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Client Client { get; set; } = default!;

        public SelectList AssuranceList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Client = await _context.Clients.FindAsync(id);

            if (Client == null)
            {
                return NotFound();
            }

            AssuranceList = new SelectList(await _context.Assurances.ToListAsync(), "AssuranceID", "Nom");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AssuranceList = new SelectList(await _context.Assurances.ToListAsync(), "AssuranceID", "Nom");
                return Page();
            }

            _context.Attach(Client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Client modifié avec succès.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clients.Any(c => c.ClientID == Client.ClientID))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }
    }
}