using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkaftoBageriA.Data;
using SkaftoBageriA.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SkaftoBageriA.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory
        public async Task<IActionResult> Index()
        {
            var inventories = await _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Supplier)
                .ToListAsync();
            return View(inventories);
        }

        // GET: Inventory/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inventory inventory)
        {
            try
            {
                if (!await ValidateForeignKeys(inventory))
                {
                    PopulateDropdowns();
                    return View(inventory);
                }

                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating inventory: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while adding the inventory. Please try again.");
                PopulateDropdowns();
                return View(inventory);
            }
        }

        // GET: Inventory/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var inventory = await _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(i => i.InventoryId == id);

            if (inventory == null)
                return NotFound();

            PopulateDropdowns();
            return View(inventory);
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inventory inventory)
        {
            if (id != inventory.InventoryId)
                return NotFound();

            // Ensure the inventory exists before updating
            var existingInventory = await _context.Inventories.FindAsync(id);
            if (existingInventory == null)
                return NotFound();

            try
            {
                if (!await ValidateForeignKeys(inventory))
                {
                    PopulateDropdowns();
                    return View(inventory);
                }

                // Update inventory properties
                existingInventory.BatchNumber = inventory.BatchNumber;
                existingInventory.QuantityInStock = inventory.QuantityInStock;
                existingInventory.ReorderPoint = inventory.ReorderPoint;
                existingInventory.ProductId = inventory.ProductId;
                existingInventory.SupplierId = inventory.SupplierId;

                // Save changes
                _context.Inventories.Update(existingInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating inventory: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the inventory. Please try again.");
                PopulateDropdowns();
                return View(inventory);
            }
        }

        // GET: Inventory/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var inventory = await _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(i => i.InventoryId == id);

            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var inventory = await _context.Inventories.FindAsync(id);
                if (inventory == null)
                    return NotFound();

                _context.Inventories.Remove(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting inventory: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while deleting the inventory. Please try again.");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inventory/ReorderAlert
        [HttpGet]
        public async Task<IActionResult> CheckReorderAlerts()
        {
            var hasReorderItems = await _context.Inventories
                .AnyAsync(i => i.QuantityInStock <= i.ReorderPoint);

            return Json(new { hasReorderItems });
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ReorderAlert()
        {
            try
            {
                var reorderItems = await _context.Inventories
                    .Include(i => i.Product)
                    .Include(i => i.Supplier)
                    .Where(i => i.QuantityInStock <= i.ReorderPoint)
                    .ToListAsync();

                return View(reorderItems);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching reorder alerts: {ex.Message}");
                return View(new List<Inventory>());
            }
        }

        // Search Inventory
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { success = false, message = "Search query is empty." });

            var searchResults = await _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Supplier)
                .Where(i => EF.Functions.Like(i.BatchNumber, $"%{query}%") ||
                            EF.Functions.Like(i.Product.ProductName, $"%{query}%") ||
                            EF.Functions.Like(i.Supplier.Name, $"%{query}%"))
                .Select(i => new
                {
                    i.InventoryId,
                    i.BatchNumber,
                    ProductName = i.Product.ProductName,
                    SupplierName = i.Supplier.Name
                })
                .ToListAsync();

            if (!searchResults.Any())
                return Json(new { success = false, message = "No matching records found." });

            return Json(new { success = true, data = searchResults });
        }

        // Helper method to validate foreign keys
        private async Task<bool> ValidateForeignKeys(Inventory inventory)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == inventory.ProductId);
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierId == inventory.SupplierId);

            if (!productExists)
            {
                ModelState.AddModelError("ProductId", "Invalid Product selected.");
            }

            if (!supplierExists)
            {
                ModelState.AddModelError("SupplierId", "Invalid Supplier selected.");
            }

            return productExists && supplierExists;
        }

        // Helper method to populate dropdown lists for Products and Suppliers
        private void PopulateDropdowns()
        {
            ViewData["Products"] = new SelectList(_context.Products.OrderBy(p => p.ProductName), "ProductId", "ProductName");
            ViewData["Suppliers"] = new SelectList(_context.Suppliers.OrderBy(s => s.Name), "SupplierId", "Name");
        }
    }
}
