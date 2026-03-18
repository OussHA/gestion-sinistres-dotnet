using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ISH_APP.Models
{
    public class BasePageModel : PageModel
    {
        protected string? Role => HttpContext.Session.GetString("UtilisateurRole");

        protected bool EstAdmin => Role == "Admin";
        protected bool EstPrestataire => Role == "Prestataire";
        protected bool EstAssurance => Role == "Assurance";
    }

}
