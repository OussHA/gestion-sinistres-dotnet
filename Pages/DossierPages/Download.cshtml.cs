using ISH_APP.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Models;

namespace ISH_APP.Pages.DossierPages
{
    public class DownloadModel : BasePageModel
    {
        private readonly ApplicationDbContext _context;

        public DownloadModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PieceJointe PieceJointe { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var piece = await _context.PieceJointes.FirstOrDefaultAsync(p => p.PieceJointeID == id);


            if (piece == null || piece.Contenu == null)
            {
                return NotFound();
            }

            return File(piece.Contenu, piece.TypeFichier, piece.NomFichier);
        }
    }
}
