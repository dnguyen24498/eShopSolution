using System.Collections.Generic;
using System.Threading.Tasks;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.ProductInCategories;
using eShopSolution.ViewModels.Catalog.Products;

namespace eShopSolution.Application.Catalog.CategoryServices
{
    public interface ICategoryService
    {
        Task<CategoryViewModel> CreateCategory(CreateCategoryRequest request);
        Task<CategoryViewModel> UpdateCategory(UpdateCategoryRequest request);
        Task<bool> DeleteCategory(int categoryId);
        Task<List<CategoryViewModel>> GetCategory(string languageId);
        Task<List<ProductViewModel>> GetProductsInCategory(int categoryId, string languageId);
        Task<int> AddProductToCategories(CreateProductInCategoriesRequest request);
    }
}