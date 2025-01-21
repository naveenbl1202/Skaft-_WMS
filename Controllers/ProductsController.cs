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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Supplier)
                .ToListAsync();
            return View(products);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            PopulateSuppliersDropdown();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                if (!await ValidateSupplierExists(product.SupplierId))
                {
                    PopulateSuppliersDropdown();
                    return View(product);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while adding the product. Please try again.");
                PopulateSuppliersDropdown();
                return View(product);
            }
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            PopulateSuppliersDropdown();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId)
                return NotFound();

            try
            {
                if (!await ValidateSupplierExists(product.SupplierId))
                {
                    PopulateSuppliersDropdown();
                    return View(product);
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                    return NotFound();

                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the product. Please try again.");
                PopulateSuppliersDropdown();
                return View(product);
            }
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Check for related records in dependent entities
                var relatedInventories = _context.Inventories.Where(i => i.ProductId == id).ToList();
                if (relatedInventories.Any())
                {
                    _context.Inventories.RemoveRange(relatedInventories);
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound();

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while deleting the product. Please try again.");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        private async Task<bool> ValidateSupplierExists(int supplierId)
        {
            if (!await _context.Suppliers.AnyAsync(s => s.SupplierId == supplierId))
            {
                ModelState.AddModelError("SupplierId", "Invalid Supplier selected.");
                return false;
            }
            return true;
        }

        private void PopulateSuppliersDropdown()
        {
            try
            {
                var suppliers = _context.Suppliers.OrderBy(s => s.Name).ToList();
                ViewData["Suppliers"] = new SelectList(suppliers, "SupplierId", "Name");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching suppliers: {ex.Message}");
                ViewData["Suppliers"] = new SelectList(Enumerable.Empty<Supplier>(), "SupplierId", "Name");
            }
        }
    }
}
