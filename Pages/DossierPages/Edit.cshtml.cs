using Azure;
using ISH_APP.Data;
using ISH_APP.Models;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
// ... (usings inchangés)
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ISH_APP.Pages.DossierPages
{
    public class EditModel : BasePageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Dossier Dossier { get; set; } = default!;

        [BindProperty]
        public List<IFormFile> PiecesJointes { get; set; } = new();

        [BindProperty]
        public List<int> UtilisateursSelectionnes { get; set; } = new();

        public List<SelectListItem> ListeUtilisateurs { get; set; } = new();
        public List<PieceJointe> PiecesDejaJointes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Dossier = await _context.Dossiers
                .Include(d => d.Client)               // charge le client
                .Include(d => d.DossierUtilisateurs)  // charge les co-utilisateurs
                .FirstOrDefaultAsync(d => d.DossierID == id);


            if (Dossier == null)
                return NotFound();

            UtilisateursSelectionnes = Dossier.DossierUtilisateurs.Select(du => du.UtilisateurID).ToList();

            ListeUtilisateurs = await _context.Utilisateurs
                .Where(u => u.UtilisateurID != Dossier.UtilisateurID)
                .Select(u => new SelectListItem
                {
                    Value = u.UtilisateurID.ToString(),
                    Text = $"{u.Nom} {u.Prenom} ({u.Email})"
                }).ToListAsync();

            PiecesDejaJointes = await _context.PiecesJointes
                .Where(p => p.DossierID == Dossier.DossierID)
                .ToListAsync();

            // Liste Clients
            ViewData["ClientID"] = new SelectList(
                _context.Clients.Select(c => new {
                    c.ClientID,
                    NomComplet = c.Nom + " " + c.Prenom
                }),
                "ClientID", "NomComplet", Dossier.ClientID
            );

            // Liste Utilisateurs avec rôle Assurance
            ViewData["AssuranceID"] = new SelectList(
                _context.Utilisateurs
                    .Where(u => u.Role == "Assurance")
                    .Select(u => new {
                        u.UtilisateurID,
                        NomComplet = u.Nom + " " + u.Prenom
                    }),
                "UtilisateurID", "NomComplet", Dossier.AssuranceID
            );

            // Liste Utilisateurs avec rôle Prestataire
            ViewData["PrestataireID"] = new SelectList(
                _context.Utilisateurs
                    .Where(u => u.Role == "Prestataire")
                    .Select(u => new {
                        u.UtilisateurID,
                        NomComplet = u.Nom + " " + u.Prenom
                    }),
                "UtilisateurID", "NomComplet", Dossier.PrestataireID
            );


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // ===== DÉBOGAGE 1 : Valeur reçue du formulaire =====
            System.Diagnostics.Debug.WriteLine($">>> DÉBOGAGE : Etat reçu = '{Dossier.Etat}'");
            System.Diagnostics.Debug.WriteLine($">>> DÉBOGAGE : DossierID = {Dossier.DossierID}");

            // ===== DÉBOGAGE 2 : Vérifier ModelState =====
            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine(">>> DÉBOGAGE : ModelState INVALIDE");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        foreach (var error in state.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($">>> Erreur sur {key}: {error.ErrorMessage}");
                        }
                    }
                }

                // Recharge les dropdowns
                ViewData["ClientID"] = new SelectList(
                    _context.Clients.Select(c => new {
                        c.ClientID,
                        NomComplet = c.Nom + " " + c.Prenom
                    }),
                    "ClientID", "NomComplet", Dossier.ClientID
                );

                ViewData["AssuranceID"] = new SelectList(
                    _context.Utilisateurs
                        .Where(u => u.Role == "Assurance")
                        .Select(u => new {
                            u.UtilisateurID,
                            NomComplet = u.Nom + " " + u.Prenom
                        }),
                    "UtilisateurID", "NomComplet", Dossier.AssuranceID
                );

                ViewData["PrestataireID"] = new SelectList(
                    _context.Utilisateurs
                        .Where(u => u.Role == "Prestataire")
                        .Select(u => new {
                            u.UtilisateurID,
                            NomComplet = u.Nom + " " + u.Prenom
                        }),
                    "UtilisateurID", "NomComplet", Dossier.PrestataireID
                );

                ListeUtilisateurs = await _context.Utilisateurs
                    .Where(u => u.UtilisateurID != Dossier.UtilisateurID)
                    .Select(u => new SelectListItem
                    {
                        Value = u.UtilisateurID.ToString(),
                        Text = $"{u.Nom} {u.Prenom} ({u.Email})"
                    }).ToListAsync();

                return Page();
            }

            var dossierFromDb = await _context.Dossiers.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DossierID == Dossier.DossierID);

            if (dossierFromDb == null)
                return NotFound();

            // ===== DÉBOGAGE 3 : Comparaison avant/après =====
            System.Diagnostics.Debug.WriteLine($">>> DÉBOGAGE : Ancien Etat DB = '{dossierFromDb.Etat}'");
            System.Diagnostics.Debug.WriteLine($">>> DÉBOGAGE : Nouvel Etat formulaire = '{Dossier.Etat}'");

            var utilisateurId = HttpContext.Session.GetInt32("UtilisateurID");
            string utilisateurNom = "Utilisateur inconnu";

            if (utilisateurId != null)
            {
                var utilisateur = await _context.Utilisateurs.FindAsync(utilisateurId.Value);
                if (utilisateur != null)
                {
                    utilisateurNom = $"{utilisateur.Nom} {utilisateur.Prenom}";
                }
            }

            var modifications = new List<HistoriqueModification>();

            void LogChange(string champ, string? ancienne, string? nouvelle)
            {
                if (ancienne != nouvelle)
                {
                    System.Diagnostics.Debug.WriteLine($">>> CHANGEMENT : {champ} | '{ancienne}' -> '{nouvelle}'");
                    modifications.Add(new HistoriqueModification
                    {
                        DossierID = Dossier.DossierID,
                        ChampModifie = champ,
                        AncienneValeur = ancienne,
                        NouvelleValeur = nouvelle,
                        DateModification = DateTime.UtcNow,
                        ModifiePar = utilisateurNom
                    });
                }
            }

            LogChange("TypeSinistre", dossierFromDb.TypeSinistre, Dossier.TypeSinistre);
            LogChange("ClientID", dossierFromDb.ClientID.ToString(), Dossier.ClientID.ToString());
            LogChange("AssuranceID", dossierFromDb.AssuranceID?.ToString(), Dossier.AssuranceID?.ToString());
            LogChange("PrestataireID", dossierFromDb.PrestataireID?.ToString(), Dossier.PrestataireID?.ToString());
            LogChange("Priorite", dossierFromDb.Priorite, Dossier.Priorite);
            LogChange("Etat", dossierFromDb.Etat, Dossier.Etat);
            LogChange("CommentaireISH", dossierFromDb.CommentaireISH, Dossier.CommentaireISH);
            LogChange("CommentaireAssurance", dossierFromDb.CommentaireAssurance, Dossier.CommentaireAssurance);
            LogChange("CommentairePrestataire", dossierFromDb.CommentairePrestataire, Dossier.CommentairePrestataire);
            LogChange("Description", dossierFromDb.Description, Dossier.Description);

            // Mise à jour réelle
            var dossierToUpdate = await _context.Dossiers.FindAsync(Dossier.DossierID);
            if (dossierToUpdate == null) return NotFound();

            dossierToUpdate.TypeSinistre = Dossier.TypeSinistre;
            dossierToUpdate.ClientID = Dossier.ClientID;
            dossierToUpdate.AssuranceID = Dossier.AssuranceID;
            dossierToUpdate.PrestataireID = Dossier.PrestataireID;
            dossierToUpdate.Priorite = Dossier.Priorite;
            dossierToUpdate.Etat = Dossier.Etat;
            dossierToUpdate.DateDeclaration = Dossier.DateDeclaration;
            dossierToUpdate.DateDebutTravaux = Dossier.DateDebutTravaux;
            dossierToUpdate.DateFinTravaux = Dossier.DateFinTravaux;
            dossierToUpdate.CommentaireISH = Dossier.CommentaireISH;
            dossierToUpdate.CommentaireAssurance = Dossier.CommentaireAssurance;
            dossierToUpdate.CommentairePrestataire = Dossier.CommentairePrestataire;
            dossierToUpdate.Description = Dossier.Description;

            // ===== DÉBOGAGE 4 : Valeur juste avant SaveChanges =====
            System.Diagnostics.Debug.WriteLine($">>> DÉBOGAGE : Etat à sauvegarder = '{dossierToUpdate.Etat}'");

            _context.HistoriqueModifications.AddRange(modifications);

            try
            {
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine(">>> DÉBOGAGE : SaveChanges RÉUSSI");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                System.Diagnostics.Debug.WriteLine($">>> ERREUR : {ex.Message}");
                if (!_context.Dossiers.Any(e => e.DossierID == Dossier.DossierID))
                    return NotFound();
                else
                    throw;
            }

            // ... reste du code (pièces jointes et co-utilisateurs)

            return RedirectToPage("./Details", new { id = Dossier.DossierID });
        }
  }
}


