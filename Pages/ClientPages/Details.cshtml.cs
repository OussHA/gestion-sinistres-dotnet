using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Data;
using ISH_APP.Models;

namespace ISH_APP.Pages.ClientPages
{
    public class DetailsModel : BasePageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Client Client { get; set; } = null!;

        public List<Dossier> DossiersClient { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Client = await _context.Clients
                .Include(c => c.Assurance)
                .FirstOrDefaultAsync(c => c.ClientID == id);

            if (Client == null)
            {
                return NotFound();
            }

            // Charger les dossiers liés à ce client
            DossiersClient = await _context.Dossiers
                .Where(d => d.ClientID == id)
                .Include(d => d.Assurance)
                .Include(d => d.Prestataire)
                .ToListAsync();

            return Page();
        }
    }
}