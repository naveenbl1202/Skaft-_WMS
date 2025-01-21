using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkaftoBageriA.Models
{
    public class Product
    {
        public int ProductId { get; set; } // Primary Key

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Stock is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int ProductStock { get; set; }

        [Required(ErrorMessage = "Reorder point is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder point cannot be negative.")]
        public int ReorderPoint { get; set; }

        [Required(ErrorMessage = "Supplier is required.")]
        public int SupplierId { get; set; } // Foreign Key

        public Supplier Supplier { get; set; } // Navigation Property

        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
