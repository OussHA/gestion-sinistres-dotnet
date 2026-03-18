using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ISH_APP.Data;
using ISH_APP.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ISH_APP.Pages
{
    public class LoginModel : BasePageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string MotDePasse { get; set; }

        public string Message { get; set; }

        public IActionResult OnPost(string returnUrl = null)
        {
            var utilisateur = _context.Utilisateurs
                .FirstOrDefault(u => u.Email == Email && u.MotDePasseHash == MotDePasse);

            if (utilisateur != null)
            {
                // Connexion réussie : stocker l'utilisateur en session
                HttpContext.Session.SetInt32("UtilisateurID", utilisateur.UtilisateurID);
                HttpContext.Session.SetString("UtilisateurNom", utilisateur.Nom);
                HttpContext.Session.SetString("UtilisateurRole", utilisateur.Role);

                // Si un returnUrl existe, rediriger vers celui-ci, sinon vers la page d'index
                return Redirect(returnUrl ?? "/Index");
            }

            Message = "Email ou mot de passe incorrect.";
            return Page();
        }
    }
}
