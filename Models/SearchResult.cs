namespace SkaftoBageriA.Models
{
    public class SearchResult
    {
        public string Type { get; set; } // Product, Supplier, or Inventory
        public string Name { get; set; } // Name of the entity
        public int Id { get; set; } // Identifier (ProductId, SupplierId, InventoryId)
    }
}
