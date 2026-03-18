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
    public class IndexModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;



        public IndexModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Ajout de la propriété NomClient pour le filtre
        [BindProperty(SupportsGet = true)]
        public string? EtatFiltre { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? NomClient { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Priorite { get; set; }



        public IList<Dossier> Dossier { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var utilisateurId = HttpContext.Session.GetInt32("UtilisateurID");

            var query = _context.Dossiers
    .Include(d => d.Client)
    .Include(d => d.Assurance)
    .Include(d => d.Prestataire)
    .Include(d => d.DossierUtilisateurs)
        .ThenInclude(du => du.Utilisateur)
    .AsQueryable();

            // Filtrer par utilisateur connecté via DossierUtilisateur
            if (utilisateurId != null)
            {
                query = query.Where(d =>
                    d.DossierUtilisateurs.Any(du => du.UtilisateurID == utilisateurId));
            }



            // Filtre par état
            if (!string.IsNullOrEmpty(EtatFiltre))
            {
                if (EtatFiltre == "En retard")
                {
                    var dateLimite = DateOnly.FromDateTime(DateTime.Now.AddDays(-15));
                    query = query.Where(d =>
                        d.Etat != "Validé" &&
                        d.DateDeclaration.HasValue &&
                        d.DateDeclaration.Value < dateLimite
                    );
                }
                else
                {
                    query = query.Where(d => d.Etat == EtatFiltre);
                }
            }
            // **Sinon, ne pas filtrer du tout sur l’état !**
            // Cela affichera tous les dossiers, validés inclus

            // Filtre par nom client
            if (!string.IsNullOrEmpty(NomClient))
            {
                query = query.Where(d => d.Client.Nom.Contains(NomClient));
            }

            // Filtre par priorité
            if (!string.IsNullOrEmpty(Priorite))
            {
                query = query.Where(d => d.Priorite == Priorite);
            }

            Dossier = await query.ToListAsync();
        }



    }
}
