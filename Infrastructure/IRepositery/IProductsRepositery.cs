using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
