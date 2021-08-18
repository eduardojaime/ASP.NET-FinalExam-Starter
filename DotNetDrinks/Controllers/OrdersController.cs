using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetDrinks.Data;
using DotNetDrinks.Models;
using Microsoft.AspNetCore.Authorization;

namespace DotNetDrinks.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders > List of orders page
        public async Task<IActionResult> Index()
        {
            // Admins should be able to se everything
            if (User.IsInRole("Administrator"))
            {
                return View(await _context.Orders.OrderByDescending(o => o.OrderDate).ToListAsync());
            }
            // Customers must only see their own orders
            else
            {
                return View(await _context.Orders
                                            .Where(o => o.CustomerId == User.Identity.Name)
                                            .OrderByDescending(o => o.OrderDate)
                                            .ToListAsync());
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include products and order details records
            var order = await _context.Orders
                                .Include(o => o.OrderDetails)
                                .ThenInclude(o => o.Product)
                                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            // add security if not admin and not owner of this order, return unauthorized message
            if (!User.IsInRole("Administrator") && User.Identity.Name != order.CustomerId)
            {
                return Unauthorized();
            }

            return View(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
