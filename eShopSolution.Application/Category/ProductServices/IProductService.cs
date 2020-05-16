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
        Task<int> CreateProductAsync(CreateProductRequest createNewRequest);

        Task<int> UpdateProductAsync(UpdateProductRequest updateRequest);

        Task<int> DeleteProductAsync(int productId);

        Task<ProductViewModel> GetProductByIdAsync(int productId, string languageId);

        Task<bool> UpdateProductPriceAsync(int productId, decimal newPrice);

        Task<bool> UpdateProductStockAsync(int productId, int addedQuantity);

        Task AddProductViewCountAsync(int productId);

        Task<PagedResult<ProductViewModel>> GetAllProductPagingByCategoriesAsync(GetProductsPagingByCategoriesRequest request);

        Task<PagedResult<ProductViewModel>> GetAllProductPagingByCategoryAsync(GetProductsPagingByCategoryRequest request);

        Task<int> AddProductImageAsync(CreateProductImageRequest createNewRequest);

        Task<int> RemoveProductImageAsync(int imageId);

        Task<int> UpdateProductImageAsync(int imageId, UpdateProductImageRequest request);

        Task<ProductImageViewModel> GetProductImageByIdAsync(int imageId);

        Task<List<ProductViewModel>> GetAllProductImageByProductAsync(int productId);





    }
}
