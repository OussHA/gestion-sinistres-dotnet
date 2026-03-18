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

namespace ISH_APP.Pages.UtilisateurPages
{
    public class EditModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public EditModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Utilisateur Utilisateur { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur =  await _context.Utilisateurs.FirstOrDefaultAsync(m => m.UtilisateurID == id);
            if (utilisateur == null)
            {
                return NotFound();
            }
            Utilisateur = utilisateur;
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

            _context.Attach(Utilisateur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UtilisateurExists(Utilisateur.UtilisateurID))
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

        private bool UtilisateurExists(int id)
        {
            return _context.Utilisateurs.Any(e => e.UtilisateurID == id);
        }
    }
}
