using Core.Entities;

namespace Infrastructure.IRepositery
{
    public interface IProductsRepositery
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);  
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<string> DeleteProductAsync(int id);
    }
}
