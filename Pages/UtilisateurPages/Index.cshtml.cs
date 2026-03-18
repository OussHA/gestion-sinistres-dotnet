using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Data;
using ISH_APP.Models;

namespace ISH_APP.Pages.UtilisateurPages
{
    public class IndexModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public IndexModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Utilisateur> Utilisateur { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Utilisateur = await _context.Utilisateurs.ToListAsync();
        }
    }
}
