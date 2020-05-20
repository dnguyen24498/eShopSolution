using System.Threading.Tasks;
using eShopSolution.Application.Category.ProductServices;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet("{productId}/language/{languageId}")]
        public async Task<IActionResult> GetProductById(int productId,string languageId)
        {
            var product = await productService.GetProductByIdAsync(productId, languageId);
            if (product == null) return BadRequest();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        {
            var product = await productService.CreateProductAsync(request);
            if (product != null) return Ok(product);
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductRequest request)
        {
            var product = await productService.UpdateProductAsync(request);
            if (product != null) return Ok(product);
            return BadRequest();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var isDelete = await productService.DeleteProductAsync(productId);
            if (isDelete) return Ok();
            return BadRequest();
        }

        [HttpPut("{productId}/newprice/{newPrice}")]
        public async Task<IActionResult> UpdatePrice(int productId, decimal newPrice)
        {
            var isSuccess = await productService.UpdateProductPriceAsync(productId, newPrice);
            if (isSuccess) return Ok();
            return BadRequest();
        }

        [HttpGet("language/{languageId}")]
        public async Task<IActionResult> GetProductPagingByCategories(string languageId,[FromForm] GetProductsPagingByCategoriesRequest request)
        {
            var products = await productService.GetAllProductPagingByCategoriesAsync(languageId,request);
            if (products != null) return Ok(products);
            return BadRequest();
        }

        [HttpPost("images")]
        public async Task<IActionResult> AddProductImage([FromForm] CreateProductImageRequest request)
        {
            var productImage = await productService.AddProductImageAsync(request);
            if (productImage != null) return Ok(productImage);
            return BadRequest();
        }

        [HttpPut("images")]
        public async Task<IActionResult> UpdateProductImage([FromForm] UpdateProductImageRequest request)
        {
            var productImage = await productService.UpdateProductImageAsync(request);
            if (productImage != null) return Ok(productImage);
            return BadRequest();
        }

        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> RemoveProductImage(int imageId)
        {
            var isDelete = await productService.RemoveProductImageAsync(imageId);
            if (isDelete) return Ok();
            return BadRequest();
        }
    }
}