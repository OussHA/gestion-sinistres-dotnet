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
    public class IndexModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public IndexModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Prestataire> Prestataire { get; set; } = new List<Prestataire>();

        [BindProperty(SupportsGet = true)]
        public string? NomSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ServiceSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? VilleSearch { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Prestataires.AsQueryable();

            if (!string.IsNullOrWhiteSpace(NomSearch))
            {
                string nom = NomSearch.ToLower();
                query = query.Where(p => p.Nom.ToLower().Contains(nom));
            }

            if (!string.IsNullOrWhiteSpace(ServiceSearch))
            {
                string service = ServiceSearch.ToLower();
                query = query.Where(p => p.Services.ToLower().Contains(service));
            }

            if (!string.IsNullOrWhiteSpace(VilleSearch))
            {
                string ville = VilleSearch.ToLower();
                query = query.Where(p => p.VilleSiege.ToLower().Contains(ville));
            }

            Prestataire = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var prestataire = await _context.Prestataires.FindAsync(id);

            if (prestataire == null)
            {
                TempData["ErrorMessage"] = "Prestataire non trouvé.";
                return RedirectToPage();
            }

            try
            {
                _context.Prestataires.Remove(prestataire);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Prestataire supprimé avec succès.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression : " + ex.Message;
            }

            return RedirectToPage();
        }

    }
}
