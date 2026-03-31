using Microsoft.AspNetCore.Mvc;
using POSSampleOWN.DTOs;
using POSSampleOWN.Services;
using System.Threading.Tasks;

namespace POSSampleOWN.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products/allProducts
        [HttpGet("allProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/products/availableProductsById/{id}
        [HttpGet("availableProductsById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.IsSuccess)
            {
                return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
            }
            return Ok(result);
        }

        // GET: api/products/availableProducts
        [HttpGet("availableProducts")]
        public async Task<IActionResult> GetAvailableProducts()
        {
            var result = await _productService.GetAvailableProductsAsync();
            return Ok(result);
        }

        // POST: api/products/productCreate
        [HttpPost("productCreate")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO createRequest)
        {
            var result = await _productService.CreateAsync(createRequest);
            if (!result.IsSuccess)
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

        // PATCH: api/products/productsUpdate/{id}
        [HttpPatch("productsUpdate/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO updateRequest)
        {
            var result = await _productService.UpdateAsync(id, updateRequest);
            if (!result.IsSuccess)
            {
                return result.Message.Contains("not found") ? NotFound(result) : StatusCode(500, result);
            }
            return Ok(result);
        }

        // DELETE: api/products/productSoftDelete/{id}
        [HttpDelete("productSoftDelete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return result.Message.Contains("not found") ? NotFound(result) : StatusCode(500, result);
            }
            return Ok(result);
        }
    }
}
