using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoesMarketDbLibrary.Contexts;
using ShoesMarketDbLibrary.Models;

namespace WebApp.Pages.Shoes
{
    public class EditModel : PageModel
    {
        private readonly ShoesMarketDbLibrary.Contexts.ShoesMarketContext _context;

        public EditModel(ShoesMarketDbLibrary.Contexts.ShoesMarketContext context)
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

            var shoe =  await _context.Shoes.FirstOrDefaultAsync(m => m.ShoeId == id);
            if (shoe == null)
            {
                return NotFound();
            }
            Shoe = shoe;
           ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "Name");
           ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
           ViewData["VendorId"] = new SelectList(_context.Vendors, "VendorId", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Shoe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShoeExists(Shoe.ShoeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ShoeExists(int id)
        {
            return _context.Shoes.Any(e => e.ShoeId == id);
        }
    }
}
