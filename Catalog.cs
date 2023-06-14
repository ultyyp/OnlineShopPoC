using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OnlineShopPoC
{
    /// <summary>
    /// Represents a catalog of products in an online shop.
    /// </summary>
    public class Catalog
    {
        private ConcurrentDictionary<Guid, Product> _products = GenerateProducts(20);

        /// <summary>
        /// Gets all the products in the catalog.
        /// </summary>
        /// <returns>A concurrent dictionary containing the products.</returns>
        public async Task<ConcurrentDictionary<Guid, Product>> GetProductsAsync()
        {
            return _products;
        }

        /// <summary>
        /// Gets a product from the catalog by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The product with the specified ID.</returns>
        public async Task<Product> GetProductByIdAsync(Guid productId)
        {
            if (!_products.TryGetValue(productId, out var product))
            {
                throw new KeyNotFoundException($"Key '{productId}' not found in the dictionary.");
            }
            else
            {
                return product;
            }
        }

        /// <summary>
        /// Adds a new product to the catalog.
        /// </summary>
        /// <param name="product">The product to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task AddProduct(Product product)
        {
            if (!_products.TryAdd(product.Id, product))
            {
                throw new ArgumentException("Product already exists in the dictionary.", nameof(product));
            };
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes a product from the catalog by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product to be deleted.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task DeleteProductById(Guid productId)
        {
            if (!_products.TryRemove(productId, out _))
            {
                throw new InvalidOperationException("Product not found in the dictionary.");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears all products from the catalog.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task ClearProducts()
        {
            _products.Clear();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates a product in the catalog by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product to be updated.</param>
        /// <param name="newProduct">The updated product information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task UpdateProductById(Guid productId, Product newProduct)
        {
            if (!_products.TryGetValue(productId, out var oldProductValue))
            {
                throw new KeyNotFoundException($"Key '{newProduct.Id}' not found in the dictionary.");
            }

            if (_products.TryUpdate(newProduct.Id, newProduct, oldProductValue))
            {
                throw new InvalidOperationException("Update operation failed.");
            }
            return Task.CompletedTask;
        }


        /// <summary>
        /// Generates products for the list.
        /// </summary>
        /// <param name="count">The ammount of products we want to generate.</param>
        /// /// <returns>A concurrent discitionnary containing the products.</returns>
        private static ConcurrentDictionary<Guid, Product> GenerateProducts(int count)
        {
            /// <summary> Array of product names. </summary>
            var productNames = new string[]
            {
                "Milk",
                "Bread",
                "Apples",
                "Bananas",
                "Cheese",
                "Eggs",
                "Tomatoes",
                "Potatoes",
                "Chicken",
                "Beef",
                "Pork",
                "Rice",
                "Pasta",
                "Biscuits",
                "Yogurt",
                "Oranges",
                "Cucumbers",
                "Onions",
                "Carrots",
                "Spinach",
                "Salmon",
                "Lettuce",
                "Strawberries",
                "Watermelon",
                "Coffee",
                "Tea",
                "Sugar",
                "Salt",
                "Pepper",
                "Olive Oil"
            };

            //Check that the count is less than the amount of available products
            if (count > productNames.Length)
            {
                throw new ArgumentOutOfRangeException($"Amount of generated products exceeded available product names.");
            }


            var random = new Random();
            var products = new ConcurrentDictionary<Guid, Product>();

            for (int i = 0; i < count; i++)
            {
                var name = productNames[i];
                var price = random.Next(50, 500);
                var producedAt = DateTime.Now.AddDays(-random.Next(1, 30));
                var expiredAt = producedAt.AddDays(random.Next(1, 365));
                var stock = random.NextDouble() * 100;

                var product = new Product(name, price);
                product.Description = "Description " + name;
                product.ProducedAt = producedAt;
                product.ExpiredAt = expiredAt;
                product.Stock = stock;
                products.TryAdd(product.Id, product);
            }

            return products;
        }
    }
}
