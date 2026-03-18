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
    public class DetailsModel : BasePageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Dossier Dossier { get; set; } = default!;
        public IList<PieceJointe> PiecesJointes { get; set; } = new List<PieceJointe>();
        public List<HistoriqueModification> Historiques { get; set; } = new List<HistoriqueModification>();

        // Utilisateurs liés par rôle
        public Utilisateur? UtilisateurAssurance { get; set; }
        public Utilisateur? UtilisateurPrestataire { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            // Charger le dossier et les co-utilisateurs
            Dossier = await _context.Dossiers
                .Include(d => d.Client)
                .Include(d => d.DossierUtilisateurs)
                    .ThenInclude(du => du.Utilisateur)
                .FirstOrDefaultAsync(d => d.DossierID == id);

            if (Dossier == null)
                return NotFound();

            // Vérifier si un prestataire est assigné via les co-utilisateurs
            var prestataireAssigné = Dossier.DossierUtilisateurs
                .FirstOrDefault(du => du.Utilisateur != null && du.Utilisateur.Role == "Prestataire");

            if (prestataireAssigné != null && Dossier.Etat != "En cours")
            {
                Dossier.Etat = "En cours";                // Modifier l'état
                _context.Dossiers.Update(Dossier);        // Marquer comme modifié
                await _context.SaveChangesAsync();        // Sauvegarder dans la BDD
            }

            // Récupérer l'assurance liée au dossier
            UtilisateurAssurance = await _context.Utilisateurs
                .Where(u => u.Role == "Assurance" && u.UtilisateurID == Dossier.AssuranceID)
                .FirstOrDefaultAsync();

            // Récupérer le prestataire lié au dossier
            UtilisateurPrestataire = await _context.Utilisateurs
                .Where(u => u.Role == "Prestataire" && u.UtilisateurID == Dossier.PrestataireID)
                .FirstOrDefaultAsync();

            // Récupérer les pièces jointes
            PiecesJointes = await _context.PiecesJointes
                .Where(pj => pj.DossierID == id)
                .ToListAsync();

            // Récupérer l'historique des modifications
            Historiques = await _context.HistoriqueModifications
                .Where(h => h.DossierID == id)
                .OrderByDescending(h => h.DateModification)
                .ToListAsync();

            return Page();
        }


        public async Task<IActionResult> OnPostDeletePieceJointeAsync(int pieceId)
        {
            var piece = await _context.PiecesJointes.FindAsync(pieceId);
            if (piece != null)
            {
                var dossierId = piece.DossierID;
                _context.PiecesJointes.Remove(piece);
                await _context.SaveChangesAsync();
                return RedirectToPage("/DossierPages/Details", new { id = dossierId });
            }
            return NotFound();
        }
    }
}
