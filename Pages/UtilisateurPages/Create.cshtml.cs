using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ISH_APP.Data;
using ISH_APP.Models;
using Microsoft.AspNetCore.Http; // nécessaire pour IFormFile

namespace ISH_APP.Pages.UtilisateurPages
{
    public class CreateModel : BasePageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public CreateModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Utilisateur Utilisateur { get; set; } = default!;

        [BindProperty]
        public IFormFile? LogoFile { get; set; } // <- ajout du fichier upload


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (LogoFile != null && LogoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await LogoFile.CopyToAsync(ms);
                Utilisateur.Logo = ms.ToArray(); // convertit le fichier en byte[]
            }
            else
            {
                Utilisateur.Logo = null; // si pas de fichier, on met à null
            }

            // Vérification de l'unicité de l'email
            _context.Utilisateurs.Add(Utilisateur);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
