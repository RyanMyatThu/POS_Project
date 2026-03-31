using Microsoft.AspNetCore.Mvc;
using POSSampleOWN.DTOs;
using POSSampleOWN.Services;
using System.Threading.Tasks;

namespace POSSampleOWN.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories/allCategories
        [HttpGet("allCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/categories/categoriesById/{id}
        [HttpGet("categoriesById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (!result.IsSuccess)
            {
                return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
            }
            return Ok(result);
        }

        // POST: api/categories/categoryCreate
        [HttpPost("categoryCreate")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO request)
        {
            var result = await _categoryService.CreateAsync(request);
            if (!result.IsSuccess)
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

        // PATCH: api/categories/updateCategory/{id}
        [HttpPatch("updateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO request)
        {
            var result = await _categoryService.UpdateAsync(id, request);
            if (!result.IsSuccess)
            {
                return result.Message.Contains("not found") ? NotFound(result) : StatusCode(500, result);
            }
            return Ok(result);
        }

        // DELETE: api/categories/deleteCategory/{id}
        [HttpDelete("deleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
            }
            return Ok(result);
        }
    }
}
