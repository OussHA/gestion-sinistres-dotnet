using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Models;

namespace ISH_APP.Pages.Statistiques
{
    public class IndexModel : PageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public IndexModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int? AnneeSelectionnee { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PrestataireSelectionne { get; set; }

        public SelectList ListeAnnees { get; set; }
        public SelectList ListePrestataires { get; set; }

        public Dictionary<string, int> DossiersParType { get; set; } = new();
        public Dictionary<string, int> DossiersParMois { get; set; } = new();
        public Dictionary<string, int> DossiersParVille { get; set; } = new();
        public Dictionary<string, int> DossiersParPrestataire { get; set; } = new();
        public Dictionary<string, int> DossiersParAssurance { get; set; } = new();
        public Dictionary<string, int> DossiersParUtilisateur { get; set; } = new();
        public Dictionary<string, int> DossiersEnRetardParPrestataire { get; set; } = new();

        public Dictionary<string, int> DossiersParSalarie { get; set; } = new();


        public async Task OnGetAsync()
        {
            var dossiersQuery = _context.Dossiers
                .Include(d => d.Prestataire)
                .Where(d => d.DateDeclaration.HasValue);

            // Préparation des années disponibles
            var anneesDisponibles = await dossiersQuery
                .Select(d => d.DateDeclaration!.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
            ListeAnnees = new SelectList(anneesDisponibles, AnneeSelectionnee);

            // Liste des prestataires
            var prestataires = await _context.Prestataires
                .OrderBy(p => p.Nom)
                .ToListAsync();
            ListePrestataires = new SelectList(prestataires, "PrestataireID", "Nom", PrestataireSelectionne);

            // Application des filtres
            if (AnneeSelectionnee.HasValue)
                dossiersQuery = dossiersQuery.Where(d => d.DateDeclaration!.Value.Year == AnneeSelectionnee);

            if (PrestataireSelectionne.HasValue)
                dossiersQuery = dossiersQuery.Where(d => d.PrestataireID == PrestataireSelectionne);

            // Dossiers par type
            DossiersParType = await dossiersQuery
                .GroupBy(d => d.TypeSinistre)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Type ?? "Inconnu", g => g.Count);

            // Dossiers par mois
            DossiersParMois = dossiersQuery
                .AsEnumerable()
                .GroupBy(d => new {
                    Year = d.DateDeclaration!.Value.Year,
                    Month = d.DateDeclaration!.Value.Month
                })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => $"{g.Key.Month:00}/{g.Key.Year}",
                    g => g.Count()
                );

            // Dossiers par ville
            DossiersParVille = await dossiersQuery
                .Include(d => d.Client)
                .Where(d => d.Client != null)
                .GroupBy(d => d.Client!.Ville)
                .Select(g => new { Ville = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Ville ?? "Inconnue", g => g.Count);


            // Dossiers par prestataire
            DossiersParPrestataire = await dossiersQuery
                .Where(d => d.Prestataire != null)
                .GroupBy(d => d.Prestataire!.Nom)
                .Select(g => new { Nom = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Nom ?? "Inconnu", g => g.Count);

            // Dossiers par assurance
            DossiersParAssurance = await dossiersQuery
                .Where(d => d.Assurance != null)
                .GroupBy(d => d.Assurance!.Nom)
                .Select(g => new { Nom = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Nom ?? "Inconnue", g => g.Count);

            // Dossiers par salarié (utilisateur)
            DossiersParUtilisateur = await dossiersQuery
                .Where(d => d.Utilisateur != null)
                .GroupBy(d => d.Utilisateur!.Nom + " " + d.Utilisateur!.Prenom)
                .Select(g => new { NomComplet = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.NomComplet ?? "Sans utilisateur", g => g.Count);

            // Dossiers en retard par prestataire
            var dateLimite = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15));

            DossiersEnRetardParPrestataire = await dossiersQuery
                .Where(d =>
                    d.DateDeclaration.HasValue &&
                    d.DateDeclaration.Value < dateLimite &&
                    d.Etat != "Validé" &&
                    d.Prestataire != null
                )
                .GroupBy(d => d.Prestataire!.Nom)
                .Select(g => new { Nom = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Nom ?? "Inconnu", g => g.Count);

        }
    }
}
