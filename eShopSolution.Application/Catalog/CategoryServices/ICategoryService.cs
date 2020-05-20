using System.Threading.Tasks;
using eShopSolution.ViewModels.Catalog.Categories;

namespace eShopSolution.Application.Catalog.CategoryServices
{
    public interface ICategoryService
    {
        Task<CategoryViewModel> CreateCategory(CreateCategoryRequest request);
        Task<CategoryViewModel> UpdateCategory(UpdateCategoryRequest request);
        Task<bool> DeleteCategory(int categoryId);
    }
}