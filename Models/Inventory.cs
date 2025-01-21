using System.ComponentModel.DataAnnotations;

namespace SkaftoBageriA.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; } // Primary Key

        [Required(ErrorMessage = "The Batch Number field is required.")]
        [MaxLength(50, ErrorMessage = "Batch number cannot exceed 50 characters.")]
        public string BatchNumber { get; set; }

        [Required(ErrorMessage = "Quantity in stock is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity in stock cannot be negative.")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Reorder point is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder point cannot be negative.")]
        public int ReorderPoint { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; } // Foreign Key

        public Product Product { get; set; } // Navigation property

        [Required(ErrorMessage = "Supplier is required.")]
        public int SupplierId { get; set; } // Foreign Key

        public Supplier Supplier { get; set; } // Navigation property
    }
}
