using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkaftoBageriA.Data;
using SkaftoBageriA.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SkaftoBageriA.Controllers
{
    [Authorize]
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            var suppliers = await _context.Suppliers.ToListAsync();
            return View(suppliers);
        }

        // GET: Suppliers/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        // GET: Suppliers/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct the errors and try again.");
                return View(supplier);
            }

            try
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while adding the supplier.");
                return View(supplier);
            }
        }

        // GET: Suppliers/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.SupplierId) return NotFound();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct the errors and try again.");
                return View(supplier);
            }

            try
            {
                _context.Update(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating supplier: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the supplier.");
                return View(supplier);
            }
        }

        // GET: Suppliers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Remove related products and inventories
                var relatedProducts = await _context.Products.Where(p => p.SupplierId == id).ToListAsync();
                if (relatedProducts.Any())
                {
                    var relatedInventories = await _context.Inventories
                        .Where(i => relatedProducts.Select(p => p.ProductId).Contains(i.ProductId))
                        .ToListAsync();

                    _context.Inventories.RemoveRange(relatedInventories);
                    _context.Products.RemoveRange(relatedProducts);
                }

                var supplier = await _context.Suppliers.FindAsync(id);
                if (supplier == null) return NotFound();

                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting supplier: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while deleting the supplier.");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
