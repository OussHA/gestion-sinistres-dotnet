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
    public class DetailsModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public DetailsModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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

        public async Task<IActionResult> OnPostChangerMotDePasseAsync(int UtilisateurID, string AncienMotDePasse, string NouveauMotDePasse, string ConfirmationMotDePasse)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(UtilisateurID);
            if (utilisateur == null)
            {
                return NotFound();
            }

            // Vérifie si l'ancien mot de passe est correct (en clair)
            if (utilisateur.MotDePasseHash != AncienMotDePasse)
            {
                ModelState.AddModelError(string.Empty, "L'ancien mot de passe est incorrect.");
                Utilisateur = utilisateur;
                return Page();
            }

            if (NouveauMotDePasse != ConfirmationMotDePasse)
            {
                ModelState.AddModelError(string.Empty, "Le nouveau mot de passe et la confirmation ne correspondent pas.");
                Utilisateur = utilisateur;
                return Page();
            }

            // Mise à jour du mot de passe en clair
            utilisateur.MotDePasseHash = NouveauMotDePasse;
            await _context.SaveChangesAsync();

            TempData["Message"] = "Mot de passe mis à jour avec succès.";
            return RedirectToPage(new { id = UtilisateurID });
        }

    }
}
