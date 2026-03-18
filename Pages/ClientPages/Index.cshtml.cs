using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Data;
using ISH_APP.Models;
using ISH_APP.Filtres;

namespace ISH_APP.Pages.ClientPages
{
    [RoleAuthorize("Interne", "Assurance")]
    public class IndexModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public IndexModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }


        public IList<Client> Clients { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? SearchTerm, string? sortOrder, int? pageNumber)
        {
            CurrentFilter = SearchTerm;
            CurrentSort = sortOrder;
            PageNumber = pageNumber ?? 1;
            int pageSize = 10;

            var query = _context.Clients.Include(c => c.Assurance).AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(c =>
                    c.Nom.Contains(SearchTerm) || c.Telephone.Contains(SearchTerm));
            }

            // Tri simple
            switch (sortOrder)
            {
                case "nom_desc":
                    query = query.OrderByDescending(c => c.Nom);
                    break;
                case "prenom":
                    query = query.OrderBy(c => c.Prenom);
                    break;
                case "prenom_desc":
                    query = query.OrderByDescending(c => c.Prenom);
                    break;
                default:
                    query = query.OrderBy(c => c.Nom);
                    break;
            }

            int totalClients = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalClients / (double)pageSize);

            Clients = await query
                .Skip((PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var hasDossier = await _context.Dossiers.AnyAsync(d => d.ClientID == id);

            if (hasDossier)
            {
                TempData["ErrorMessage"] = "Ce client a des dossiers associés et ne peut pas être supprimé.";
                return RedirectToPage();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }


        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public string? CurrentSort { get; set; }
        public string? CurrentFilter { get; set; }

    }
}
