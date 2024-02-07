using System.Collections.Generic;


namespace WarehouseApp
{
    public class Warehouse
    {
        public List<Product> Products { get; set; }

        public Warehouse()
        {
            Products = new List<Product>();
        }
    }
}
