using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoesMarketDbLibrary.Models;

namespace WebApp.Pages.Shoes
{
    public class IndexModel : PageModel
    {
        private readonly ShoesMarketDbLibrary.Contexts.ShoesMarketContext _context;

        [BindProperty(SupportsGet = true)]
        public string Description { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBrand { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool HasDiscount { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool InStock { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortShoe { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Price { get; set; }

        public IndexModel(ShoesMarketDbLibrary.Contexts.ShoesMarketContext context)
        {
            _context = context;
        }

        public IList<Shoe> Shoe { get; set; } = default!;

        public IList<Brand> Brands { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Фильтры
            var shoes = _context.Shoes
                .Include(s => s.Brand)
                .Include(s => s.Category)
                .Include(s => s.Vendor).AsQueryable();

            // По описанию
            if (!string.IsNullOrWhiteSpace(Description))
                shoes = shoes
                    .Where(s => s.Description.Contains(Description));

            // По бренду
            if (!string.IsNullOrWhiteSpace(SortBrand))
                shoes = shoes.Where(s => s.Brand.Name == SortBrand);

            // По максимальной цене
            if (!string.IsNullOrWhiteSpace(Price))
                shoes = shoes.Where(s => (s.Price - s.Price * s.Discount / 100) <= Convert.ToInt32(Price));

            // По наличию скидки
            if (HasDiscount)
                shoes = shoes.Where(s => s.Discount > 0);

            // По наличию самого товара
            if (InStock)
                shoes = shoes.Where(s => s.Quantity > 0);

            // Сортировка
            switch (SortShoe)
            {
                case "name":
                    shoes = shoes.OrderBy(s => s.Category.Name);
                    break;
                case "vendor":
                    shoes = shoes.OrderBy(s => s.Vendor.Name);
                    break;
                case "price":
                    shoes = shoes.OrderBy(s => s.Price - s.Price * s.Discount / 100);
                    break;
                case "price_desc":
                    shoes = shoes.OrderByDescending(s => s.Price - s.Price * s.Discount / 100);
                    break;
                default:
                    break;
            }

            Shoe = await shoes.ToListAsync();
            Brands = await _context.Brands.ToListAsync();
        }

        public async Task<IActionResult> OnPostOrderAsync(int shoeId)
        {
            var role = HttpContext.Session.GetString("Role")?.ToLower();
            var userId = HttpContext.Session.GetInt32("UserId");

            if (role is null)
                return RedirectToPage("/Login");

            var shoe = await _context.Shoes.FindAsync(shoeId);

            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                ReceiveCode = new Random().Next(100, 1000),
                StatusId = _context.Statuses.FirstOrDefault(s => s.Name == "Новый").StatusId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var shoeOrder = new ShoeOrder
            {
                OrderId = order.OrderId,
                ShoeId = shoeId,
                Quantity = 1
            };

            _context.ShoeOrders.Add(shoeOrder);

            // Уменьшение количества обуви при заказе
            shoe.Quantity -= 1;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Orders/Index");
        }
    }
}
