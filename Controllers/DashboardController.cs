using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkaftoBageriA.Data;
using SkaftoBageriA.Models;
using SkaftoBageriA.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SkaftoBageriA.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard Index
        public IActionResult Index()
        {
            return View();
        }

        // Search functionality
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("SearchResults", new SearchResultsViewModel
                {
                    Query = query,
                    Products = new List<Product>(),
                    Suppliers = new List<Supplier>(),
                    Inventory = new List<Inventory>()
                });
            }

            // Fetch results from Products, Suppliers, and Inventory
            var productResults = await _context.Products
                .Where(p => EF.Functions.Like(p.ProductName, $"%{query}%"))
                .Include(p => p.Supplier)
                .ToListAsync();

            var supplierResults = await _context.Suppliers
                .Where(s => EF.Functions.Like(s.Name, $"%{query}%"))
                .ToListAsync();

            var inventoryResults = await _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Supplier)
                .Where(i => EF.Functions.Like(i.BatchNumber, $"%{query}%") ||
                            EF.Functions.Like(i.Product.ProductName, $"%{query}%") ||
                            EF.Functions.Like(i.Supplier.Name, $"%{query}%"))
                .ToListAsync();

            var model = new SearchResultsViewModel
            {
                Query = query,
                Products = productResults,
                Suppliers = supplierResults,
                Inventory = inventoryResults
            };

            return View("SearchResults", model);
        }
    }
}
