using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Data;
using ISH_APP.Models;

namespace ISH_APP.Pages.DossierPages
{
    public class DeleteModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public DeleteModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Dossier Dossier { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dossier = await _context.Dossiers.FirstOrDefaultAsync(m => m.DossierID == id);

            if (dossier is not null)
            {
                Dossier = dossier;

                return Page();
            }

            return NotFound();
        }

        // Gestion de la suppression d'un dossier via POST (handler nommé Delete)
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var dossier = await _context.Dossiers.FindAsync(id);

            if (dossier == null)
                return NotFound();

            // Charger et supprimer les co-utilisateurs
            var coUtilisateurs = await _context.DossierUtilisateurs
                .Where(du => du.DossierID == id)
                .ToListAsync();
            _context.DossierUtilisateurs.RemoveRange(coUtilisateurs);

            // Charger et supprimer les pièces jointes
            var pieces = await _context.PiecesJointes
                .Where(pj => pj.DossierID == id)
                .ToListAsync();
            _context.PiecesJointes.RemoveRange(pieces);

            // Supprimer le dossier
            _context.Dossiers.Remove(dossier);

            // Sauvegarder toutes les suppressions en une seule fois
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
