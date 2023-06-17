using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopPoC
{
    /// <summary>
    /// Represents a catalog of products in an online shop.
    /// </summary>
    public interface ICatalog
    {
        /// <summary>
        /// Gets all the products in the catalog.
        /// </summary>
        /// <returns>A concurrent dictionary containing the products.</returns>
        Task<List<Product>> GetProductsAsync(IClock clock);

        /// <summary>
        /// Gets a product from the catalog by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The product with the specified ID.</returns>
        Task<Product> GetProductByIdAsync(Guid productId, IClock clock);

        /// <summary>
        /// Adds a new product to the catalog.
        /// </summary>
        /// <param name="product">The product to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddProduct(Product product);

        /// <summary>
        /// Deletes a product from the catalog by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product to be deleted.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteProductById(Guid productId);

        /// <summary>
        /// Clears all products from the catalog.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ClearProducts();

        /// <summary>
        /// Updates a product in the catalog by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product to be updated.</param>
        /// <param name="newProduct">The updated product information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateProductById(Guid productId, Product newProduct);
    }
}