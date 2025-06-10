using Core.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositery;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositery
{
    public class ProductsRepository : IProductsRepositery
    {
        private readonly StoreContext _context;
        private readonly ILogger<ProductsRepository> _logger;

        public ProductsRepository(StoreContext context,ILogger<ProductsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all products from the database.");
                var products = await _context.Products.OrderByDescending(c => c.Id).ToListAsync();
                _logger.LogInformation("Fetched {Count} products.", products.Count);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all products.");
                throw;
            }
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching product with ID: {ProductId}", id);
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", id);
                }
                else
                {
                    _logger.LogInformation("Product with ID {ProductId} retrieved.", id);
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task AddProductAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Adding a new product: {@Product}", product);
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product added with ID: {ProductId}", product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new product.");
                throw;
            }
        }

        //public async Task UpdateProductAsync(Product product)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Updating product with ID: {ProductId}", product.Id);
        //        _context.Entry(product).State = EntityState.Modified;
        //        await _context.SaveChangesAsync();
        //        _logger.LogInformation("Product with ID {ProductId} updated successfully.", product.Id);
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        _logger.LogError(ex, "Concurrency error updating product with ID: {ProductId}", product.Id);
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error updating product with ID: {ProductId}", product.Id);
        //        throw;
        //    }
        //}

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", product.Id);

                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update.", product.Id);
                    return;
                }

                // Update properties manually or use a mapping tool like AutoMapper
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.PictureUrl = product.PictureUrl;
                existingProduct.Brand = product.Brand;
                existingProduct.Type = product.Type;
                existingProduct.QuantityInStock = product.QuantityInStock;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product with ID {ProductId} updated successfully.", product.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating product with ID: {ProductId}", product.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating product with ID: {ProductId}", product.Id);
                throw;
            }
        }


        public async Task<string> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);
                var product = await _context.Products.FindAsync(id);

                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Product with ID {ProductId} deleted.", id);
                    return "Product Deleted Successfully Id:"+ id;
                }
                else
                {
                    _logger.LogWarning("Attempted to delete non-existing product with ID: {ProductId}", id);
                    return "Attempted to delete non-existing product";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product with ID: {ProductId}", id);
                throw;
            }
        }
    }
}
