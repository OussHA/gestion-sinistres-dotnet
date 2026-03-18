using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Data;
using ISH_APP.Models;

namespace ISH_APP.Pages.PrestatairePages
{
    public class DeleteModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public DeleteModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Prestataire Prestataire { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestataire = await _context.Prestataires.FirstOrDefaultAsync(m => m.PrestataireID == id);

            if (prestataire is not null)
            {
                Prestataire = prestataire;

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

            var dossier = await _context.Dossiers
                .Include(d => d.DossierUtilisateurs)
                .Include(d => d.PieceJointes)
                .FirstOrDefaultAsync(m => m.DossierID == id);

            if (dossier == null)
            {
                return NotFound();
            }

            // Supprimer les pièces jointes liées au dossier
            if (dossier.PieceJointes != null)
                _context.PiecesJointes.RemoveRange(dossier.PieceJointes);

            // Supprimer les co-utilisateurs liés au dossier
            if (dossier.DossierUtilisateurs != null)
                _context.DossierUtilisateurs.RemoveRange(dossier.DossierUtilisateurs);

            // Enfin, supprimer le dossier lui-même
            _context.Dossiers.Remove(dossier);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
