using System.Threading.Tasks;
using eShopSolution.Application.Catalog.CategoryServices;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.ProductInCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("languages/{languageId}")]
        public async Task<IActionResult> GetCategory(string languageId)
        {
            var categories = await _categoryService.GetCategory(languageId);
            if (categories != null) return Ok(categories);
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid) return BadRequest();
            var newCategory = await _categoryService.CreateCategory(request);
            if (newCategory != null) return Ok(newCategory);
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid) return BadRequest();
            var updatedCategory = await _categoryService.UpdateCategory(request);
            if (updatedCategory != null) return Ok(updatedCategory);
            return BadRequest();
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var isSuccess = await _categoryService.DeleteCategory(categoryId);
            if (isSuccess) return Ok();
            return BadRequest();
        }

        [HttpGet("{categoryId}/products/languages/{languageId}")]
        public async Task<IActionResult> GetProductsInCategory(int categoryId,string languageId)
        {
            var products = await _categoryService.GetProductsInCategory(categoryId, languageId);
            if (products != null) return Ok(products);
            return BadRequest();
        }

        [HttpPost("products")]
        public async Task<IActionResult> AddProductIntoCategories([FromForm] CreateProductInCategoriesRequest request)
        {
            var productId = await _categoryService.AddProductToCategories(request);
            if (productId > 0) return Ok();
            return BadRequest();
        }
    }
}