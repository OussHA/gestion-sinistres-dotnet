using ISH_APP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ISH_APP.Pages.AuthPages
{
    public class LogoutModel : BasePageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear(); // vide la session
            return RedirectToPage("/AuthPages/Login");
        }
    }
}

