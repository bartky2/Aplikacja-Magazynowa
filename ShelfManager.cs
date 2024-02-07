using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace WarehouseApp
{
    public class ShelfManager
    {
        private Warehouse warehouse;
        private const string FileName = "dane.json";
        private const int MaxPlacesPerShelf = 10;

        public ShelfManager()
        {
            LoadWarehouseFromJson();
        }

        public void SaveWarehouseToJson()
        {
            string jsonData = JsonConvert.SerializeObject(warehouse, Formatting.Indented);
            File.WriteAllText(FileName, jsonData);
        }

        private void LoadWarehouseFromJson()
        {
            if (File.Exists(FileName))
            {
                string jsonData = File.ReadAllText(FileName);
                warehouse = JsonConvert.DeserializeObject<Warehouse>(jsonData);
            }
            else
            {
                warehouse = new Warehouse();
            }
        }

        public void AddOrUpdateProduct(string productName, int quantity = 1)
        {
            var existingProduct = warehouse.Products.Find(p => p.Name == productName);

            if (existingProduct != null)
            {
                existingProduct.Quantity += quantity;
            }
            else
            {
                Product newProduct = new Product
                {
                    Name = productName,
                    ShelfLocations = new List<string>(),
                    Quantity = quantity
                };

                warehouse.Products.Add(newProduct);
            }

            
            if (existingProduct == null || existingProduct.Quantity % MaxPlacesPerShelf == 0)
            {
                string newLocation = GenerateNextFreeLocation();
                warehouse.Products.Last().ShelfLocations.Add(newLocation);
            }

            SaveWarehouseToJson();

            Console.WriteLine($"Dodano/aktualizowano produkt '{productName}'.");
        }

        public List<Product> GetAllProducts()
        {
            return warehouse.Products;
        }

        public List<Product> SearchProductsByName(string productName)
        {
            return warehouse.Products.Where(p => p.Name.ToLower().Contains(productName.ToLower())).ToList();
        }

        private string GenerateNextFreeLocation()
        {
            for (char shelf = 'A'; shelf <= 'F'; shelf++)
            {
                for (int height = 0; height <= 5; height++)
                {
                    for (int place = 1; place <= MaxPlacesPerShelf; place++)
                    {
                        string location = $"A{shelf}-A{height}-{place:D2}";

                        
                        bool isLocationTaken = warehouse.Products.Any(p => p.ShelfLocations.Contains(location));

                        if (!isLocationTaken)
                        {
                            return location;
                        }
                    }
                }
            }

            return null;
        }
    }
}
