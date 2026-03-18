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
    public class DetailsModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public DetailsModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Prestataire Prestataire { get; set; } = default!;

        public List<Dossier> DossiersAssocies { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestataire = await _context.Prestataires.FirstOrDefaultAsync(m => m.PrestataireID == id);

            if (prestataire is not null)
            {
                Prestataire = prestataire;
                DossiersAssocies = await _context.Dossiers
       .Include(d => d.Client)
       .Include(d => d.Assurance)
       .Where(d => d.PrestataireID == id)
       .ToListAsync();

                return Page();
            }

            return NotFound();
        }

    }
}
