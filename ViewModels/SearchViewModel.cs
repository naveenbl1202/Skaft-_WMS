using System.Collections.Generic;
using SkaftoBageriA.Models;

namespace SkaftoBageriA.ViewModels
{
    public class SearchResultsViewModel
    {
        public string Query { get; set; } // The search query
        public List<Product> Products { get; set; } = new();
        public List<Supplier> Suppliers { get; set; } = new();
        public List<Inventory> Inventory { get; set; } = new();
    }
}
