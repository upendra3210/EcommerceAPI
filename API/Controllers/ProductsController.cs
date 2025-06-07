using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext _context;

        public ProductsController(StoreContext Context)
        {
            _context = Context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Product>> UpdateProduct(Product product, int id)
        {
            var Data = _context.Products.FindAsync(id);

            if (Data == null)
            {
                return NotFound();
            }

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return product;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var Data = await _context.Products.FindAsync(id);

            if (Data == null)
            {
                return NotFound();
            }

            _context.Products.Remove(Data);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
