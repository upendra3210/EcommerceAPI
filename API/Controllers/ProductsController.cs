using Core.Entities;
using Infrastructure.IRepositery;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepositery _productsRepository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductsRepositery productsRepository, ILogger<ProductsController> logger)
        {
            _productsRepository = productsRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            _logger.LogInformation("Fetching all products.");
            var products = await _productsRepository.GetAllProductsAsync();
            _logger.LogInformation("Fetched {Count} products.", products?.Count() ?? 0);
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            _logger.LogInformation("Fetching product with ID {Id}.", id);
            var product = await _productsRepository.GetProductByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Product with ID {Id} retrieved successfully.", id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (product == null)
            {
                _logger.LogWarning("Attempted to create a null product.");
                return BadRequest("Product cannot be null");
            }

            _logger.LogInformation("Creating a new product");
            await _productsRepository.AddProductAsync(product);
            _logger.LogInformation("Product created with ID {Id}.", product.Id);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Product>> UpdateProduct(Product product, int id)
        {
            _logger.LogInformation("Updating product with ID {Id}.", id);
            var existingProduct = await _productsRepository.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                _logger.LogWarning("Cannot update product. Product with ID {Id} not found.", id);
                return NotFound();
            }

            product.Id = id; 
            await _productsRepository.UpdateProductAsync(product);
            _logger.LogInformation("Product with ID {Id} updated successfully.", id);

            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Deleting product with ID {Id}.", id);
            var existingProduct = await _productsRepository.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                _logger.LogWarning("Cannot delete product. Product with ID {Id} not found.", id);
                return NotFound();
            }

            string Data = await _productsRepository.DeleteProductAsync(id);
            _logger.LogInformation("Product with ID {Id} deleted successfully.", id);
            return Content(Data);
        }
    }
}
