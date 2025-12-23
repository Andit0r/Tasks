using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesMarketDbLibrary.Contexts;
using ShoesMarketDbLibrary.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ShoesMarketContext _context;

        public OrdersController(ShoesMarketContext context)
        {
            _context = context;
        }

        // GET: api/Orders/user/{login}
        [Authorize]
        [HttpGet("user/{login}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser(string login)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Status)
                .Include(o => o.ShoeOrders)
                    .ThenInclude(so => so.Shoe)
                .Where(o => o.User.Login == login)
                .ToListAsync();

            return Ok(orders);
        }

        // PUT: api/Orders/{orderId}
        [Authorize(Roles = "admin,manager")]
        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] Order updatedOrder)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order is null)
                return NotFound();

            if (updatedOrder.StatusId.HasValue && updatedOrder.StatusId != order.StatusId)
                order.StatusId = updatedOrder.StatusId;

            if (updatedOrder.DeliveryDate != default && updatedOrder.DeliveryDate != order.DeliveryDate)
                order.DeliveryDate = updatedOrder.DeliveryDate;

            _context.Update(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
