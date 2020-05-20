using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Category.ProductServices
{
    public interface IProductService
    {
        Task<ProductViewModel> CreateProductAsync(CreateProductRequest createNewRequest);

        Task<ProductViewModel> UpdateProductAsync(UpdateProductRequest updateRequest);

        Task<bool> DeleteProductAsync(int productId);

        Task<ProductViewModel> GetProductByIdAsync(int productId, string languageId);

        Task<bool> UpdateProductPriceAsync(int productId, decimal newPrice);

        Task<bool> UpdateProductStockAsync(int productId, int addedQuantity);

        Task AddProductViewCountAsync(int productId);

        Task<PagedResult<ProductViewModel>> GetAllProductPagingByCategoriesAsync(string languageId,GetProductsPagingByCategoriesRequest request);

        Task<PagedResult<ProductViewModel>> GetAllProductPagingByCategoryAsync(string languageId,GetProductsPagingByCategoryRequest request);

        Task<ProductImageViewModel> AddProductImageAsync(CreateProductImageRequest createNewRequest);

        Task<bool> RemoveProductImageAsync(int imageId);

        Task<ProductImageViewModel> UpdateProductImageAsync(UpdateProductImageRequest request);

        Task<ProductImageViewModel> GetProductImageByIdAsync(int imageId);

        Task<List<ProductViewModel>> GetAllProductImageByProductAsync(int productId);

        Task<List<ProductViewModel>> GetAllProduct(string languageId);

    }
}
