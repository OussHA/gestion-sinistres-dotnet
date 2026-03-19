using ISH_APP.Data;
using ISH_APP.Models;
using ISH_APP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ISH_APP.Pages.DossierPages
{

    public class CreateModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> UploadedFiles { get; set; } = new();

        private readonly ISH_APP.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailSender _emailSender;

        public CreateModel(ISH_APP.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager, EmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }


        public IActionResult OnGet()
        {
            Dossier = new Dossier
            {
                DateDeclaration = DateOnly.FromDateTime(DateTime.Today),
                Etat = "Recherche prestataire"
            };

            ViewData["ClientID"] = new SelectList(
                _context.Clients.Select(c => new {
                    c.ClientID,
                    NomComplet = c.Nom + " " + c.Prenom
                }),
                "ClientID",
                "NomComplet"
            );

            // Utilisateurs avec rôle "Assurance"
            ViewData["AssuranceID"] = new SelectList(
                _context.Utilisateurs
                    .Where(u => u.Role == "Assurance")
                    .Select(u => new {
                        u.UtilisateurID,
                        NomComplet = u.Nom + " " + u.Prenom
                    }),
                "UtilisateurID",
                "NomComplet"
            );

            // Utilisateurs avec rôle "Prestataire"
            ViewData["PrestataireID"] = new SelectList(
                _context.Utilisateurs
                    .Where(u => u.Role == "Prestataire")
                    .Select(u => new {
                        u.UtilisateurID,
                        NomComplet = u.Nom + " " + u.Prenom
                    }),
                "UtilisateurID",
                "NomComplet"
            );


            return Page();
        }


        [BindProperty]
        public Dossier Dossier { get; set; } = default!;

        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var utilisateurID = HttpContext.Session.GetInt32("UtilisateurID");

            if (!utilisateurID.HasValue)
            {
                // Rediriger vers login ou afficher message
                return RedirectToPage("/AuthPages/Login");
            }

            
            Dossier.UtilisateurID = utilisateurID.Value;


            _context.Dossiers.Add(Dossier);
            await _context.SaveChangesAsync();


            if (UploadedFiles != null && UploadedFiles.Count > 0)
            {
                foreach (var file in UploadedFiles)
                {
                    if (file.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);

                        var piece = new PieceJointe
                        {
                            DossierID = Dossier.DossierID,
                            NomFichier = Path.GetFileName(file.FileName),
                            TypeFichier = file.ContentType,
                            Contenu = memoryStream.ToArray(),
                            DateAjout = DateTime.UtcNow
                        };

                        _context.PieceJointes.Add(piece);
                    }
                }

                await _context.SaveChangesAsync(); // sauvegarde toutes les PJ d'un coup
            }

            var utilisateurId = HttpContext.Session.GetInt32("UtilisateurID");
            string utilisateurNom = "Utilisateur inconnu";

           

            // --- AJOUTER LES LIENS UTILISATEURS ---
            var utilisateursAffectes = new List<int>();

            // Créateur (interne)
            if (utilisateurID != null)
                utilisateursAffectes.Add(utilisateurID.Value);

            // 2Assurance
            if (Dossier.AssuranceID != 0)
            {
                var assuranceUtilisateur = await _context.Utilisateurs
                    .Where(u => u.Role == "Assurance" && u.UtilisateurID == Dossier.AssuranceID)
                    .Select(u => u.UtilisateurID)
                    .FirstOrDefaultAsync();

                if (assuranceUtilisateur != 0)
                    utilisateursAffectes.Add(assuranceUtilisateur);
            }

            // 3Prestataire
            if (Dossier.PrestataireID != 0)
            {
                var prestataireUtilisateur = await _context.Utilisateurs
                    .Where(u => u.Role == "Prestataire" && u.UtilisateurID == Dossier.PrestataireID)
                    .Select(u => u.UtilisateurID)
                    .FirstOrDefaultAsync();

                if (prestataireUtilisateur != 0)
                    utilisateursAffectes.Add(prestataireUtilisateur);
            }

            // Créer les entrées dans DossierUtilisateur
            foreach (var uid in utilisateursAffectes.Distinct())
            {
                _context.DossierUtilisateurs.Add(new DossierUtilisateur
                {
                    DossierID = Dossier.DossierID,
                    UtilisateurID = uid
                });
            }

            await _context.SaveChangesAsync();


            var nouvelHistorique = new HistoriqueModification
            {
                DossierID = Dossier.DossierID,
                ChampModifie = "Création",
                AncienneValeur = null,
                NouvelleValeur = "Dossier créé",
                ModifiePar = utilisateurNom,
                DateModification = DateTime.UtcNow
            };

            _context.HistoriqueModifications.Add(nouvelHistorique);
            await _context.SaveChangesAsync();


            // Récupère le prestataire
            var prestataire = await _context.Prestataires.FindAsync(Dossier.PrestataireID);

            // Vérifie qu’il existe, puis envoie le mail
            if (prestataire != null)
            {
                string body = $@"
    <!DOCTYPE html>
    <html lang='fr'>
    <head>
        <meta charset='UTF-8'>
        <style>
            body {{
                font-family: Arial, sans-serif;
                color: #333;
            }}
            .container {{
                padding: 20px;
                background-color: #f9f9f9;
                border-radius: 10px;
            }}
            .footer {{
                margin-top: 40px;
                font-size: 12px;
                color: #888;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h2 style='color: #0d6efd;'>Nouvelle demande de dossier</h2>
            <p>Bonjour <strong>{prestataire.Nom}</strong>,</p>
            <p>Un nouveau dossier vous a été attribué. Merci de vous connecter à la plateforme ISH afin de compléter les informations nécessaires.</p>

            <p>
                👉 <a href='https://ish-app.fr/prestataire/dossiers' style='color: #0d6efd; text-decoration: none;'>Accéder au dossier</a>
            </p>

            <p>Cordialement,</p>

            <div class='footer'>
                <strong>ISH - Intermédiaire Sinistre Habitation</strong><br/>
                support@ish-app.fr<br/>
                +33 1 23 45 67 89
            </div>
        </div>
    </body>
    </html>";

                await _emailSender.SendEmailAsync(
                    toEmail: prestataire.Mail,
                    subject: "Nouvelle demande de dossier",
                    body: body
                );
            }

            return RedirectToPage("./Details", new { id = Dossier.DossierID });


        }

    }
}
