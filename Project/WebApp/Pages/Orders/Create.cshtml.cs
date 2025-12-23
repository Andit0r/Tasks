using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoesMarketDbLibrary.Contexts;
using ShoesMarketDbLibrary.Models;

namespace WebApp.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly ShoesMarketDbLibrary.Contexts.ShoesMarketContext _context;

        public CreateModel(ShoesMarketDbLibrary.Contexts.ShoesMarketContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "Name");
        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FirstName");
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
