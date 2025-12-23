using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoesMarketDbLibrary.Contexts;
using ShoesMarketDbLibrary.Models;

namespace WebApp.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ShoesMarketDbLibrary.Contexts.ShoesMarketContext _context;

        public IndexModel(ShoesMarketDbLibrary.Contexts.ShoesMarketContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("Role")?.ToLower();
            var userId = HttpContext.Session.GetInt32("UserId");

            if (role == null)
            {
                return RedirectToPage("/Login");
            }

            var ordersQuery = _context.Orders
                .Include(o => o.Status)
                .Include(o => o.User)
                .Include(o => o.ShoeOrders)
                    .ThenInclude(so => so.Shoe)
                        .ThenInclude(s => s.Brand)
                .AsQueryable();

            if (role == "client")
            {
                ordersQuery = ordersQuery.Where(o => o.UserId == userId.Value);
            }

            Orders = await ordersQuery.ToListAsync();

            return Page();
        }
    }
}
