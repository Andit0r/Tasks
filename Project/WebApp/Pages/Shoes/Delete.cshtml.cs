using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoesMarketDbLibrary.Contexts;
using ShoesMarketDbLibrary.Models;

namespace WebApp.Pages.Shoes
{
    public class DeleteModel : PageModel
    {
        private readonly ShoesMarketDbLibrary.Contexts.ShoesMarketContext _context;

        public DeleteModel(ShoesMarketDbLibrary.Contexts.ShoesMarketContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Shoe Shoe { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoe = await _context.Shoes.FirstOrDefaultAsync(m => m.ShoeId == id);

            if (shoe is not null)
            {
                Shoe = shoe;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoe = await _context.Shoes.FindAsync(id);
            if (shoe != null)
            {
                Shoe = shoe;
                _context.Shoes.Remove(Shoe);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
