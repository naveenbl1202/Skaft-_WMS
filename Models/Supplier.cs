using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkaftoBageriA.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; } // Primary Key

        [Required(ErrorMessage = "The Name field is required.")]
        [MaxLength(100, ErrorMessage = "Supplier name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Address field is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "The Contact Person field is required.")]
        public string ContactPerson { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string ContactPersonPhone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string ContactPersonEmail { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        public string OrderDays { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
