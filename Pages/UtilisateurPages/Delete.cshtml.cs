using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Data;
using ISH_APP.Models;

namespace ISH_APP.Pages.UtilisateurPages
{
    public class DeleteModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public DeleteModel(ISH_APP.Data.ApplicationDbContext context)
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

            var utilisateur = await _context.Utilisateurs.FirstOrDefaultAsync(m => m.UtilisateurID == id);

            if (utilisateur is not null)
            {
                Utilisateur = utilisateur;

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

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur != null)
            {
                Utilisateur = utilisateur;
                _context.Utilisateurs.Remove(Utilisateur);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
