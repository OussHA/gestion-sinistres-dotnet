using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ISH_APP.Data;
using ISH_APP.Models;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Pages.ClientPages
{
    public class CreateModel : BasePageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Client Client { get; set; } = default!;

        public SelectList AssuranceList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            AssuranceList = new SelectList(await _context.Assurances.ToListAsync(), "AssuranceID", "Nom");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AssuranceList = new SelectList(await _context.Assurances.ToListAsync(), "AssuranceID", "Nom");
                return Page();
            }

            _context.Clients.Add(Client);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}