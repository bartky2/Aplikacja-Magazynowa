using System.Collections.Generic;

namespace WarehouseApp
{
    public class Product
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public List<string> ShelfLocations { get; set; } = new List<string>();
    }
}
