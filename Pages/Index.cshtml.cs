using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ISH_APP.Data;
using System.Linq;
using ISH_APP.Models;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Pages
{

    public class IndexModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;             // Logger pour tracer les événements
        private readonly ApplicationDbContext _context;            // Contexte EF Core pour accéder à la BDD

        // Constructeur injectant logger et contexte BDD via DI (Dependency Injection)
        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Propriétés qui contiendront les comptes de dossiers par état pour l'affichage
        public int EnCours { get; set; }
        public int Recherche { get; set; }
        public int AttenteAssurance { get; set; }
        public int Valide { get; set; }
        public int ARefaire { get; set; }
        public int EnRetard { get; set; }

        // Propriétés liées aux filtres sur la page, bindées en GET (via query string)
        [BindProperty(SupportsGet = true)]
        public string? NomClient { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? IdDossier { get; set; }


        [BindProperty(SupportsGet = true)]
        public string? Priorite { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? EtatFiltre { get; set; }  // Etat filtré sélectionné

        // Liste des dossiers récupérés à afficher
        public List<Dossier> DossiersUtilisateur { get; set; } = new List<Dossier>();

        // Méthode appelée lors d'une requête GET sur la page
        public IActionResult OnGet()
        {
            // Récupération de l'ID utilisateur dans la session
            var utilisateurID = HttpContext.Session.GetInt32("UtilisateurID");

            if (utilisateurID == null)
            {
                // Si jamais connecté auparavant, éviter boucle
                if (!HttpContext.Session.Keys.Contains("DejaConnecte"))
                    HttpContext.Session.SetString("DejaConnecte", "1");

                return RedirectToPage("/AuthPages/Login");
            }

            // Construction de la requête avec toutes les inclusions nécessaires
            var query = _context.Dossiers
                .Include(d => d.Client)
                .Include(d => d.DossierUtilisateurs)
                    .ThenInclude(du => du.Utilisateur) // <- indispensable pour récupérer les utilisateurs liés
                .AsQueryable();

            // Filtrage sur l'état
            if (string.IsNullOrEmpty(EtatFiltre))
                query = query.Where(d => d.Etat != "Validé"); // exclut les validés par défaut
            else
                query = query.Where(d => d.Etat == EtatFiltre);

            // Filtrage sur l'ID du dossier
            if (IdDossier.HasValue)
                query = query.Where(d => d.DossierID == IdDossier.Value);

            // Filtrage sur le nom du client et la priorité
            if (!string.IsNullOrEmpty(NomClient))
                query = query.Where(d => d.Client.Nom.Contains(NomClient));

            if (!string.IsNullOrEmpty(Priorite))
                query = query.Where(d => d.Priorite == Priorite);

            // Exécution de la requête et stockage
            DossiersUtilisateur = query.ToList();

            // Compteurs pour les cartes statistiques
            EnCours = DossiersUtilisateur.Count(d => d.Etat == "En cours");
            Recherche = DossiersUtilisateur.Count(d => d.Etat == "Recherche Prestataire");
            AttenteAssurance = DossiersUtilisateur.Count(d => d.Etat == "Attente Validation Assurance");
            Valide = _context.Dossiers.Count(d => d.UtilisateurID == utilisateurID && d.Etat == "Validé");
            ARefaire = DossiersUtilisateur.Count(d => d.Etat == "À refaire");

            var dateLimite = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15));

            EnRetard = DossiersUtilisateur.Count(d =>
                d.DateDeclaration.HasValue &&
                d.DateDeclaration.Value < dateLimite &&
                d.Etat != "Validé" &&
                d.Etat != "Attente Validation Assurance" &&
                d.Etat != "À refaire"
            );


            return Page();
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
