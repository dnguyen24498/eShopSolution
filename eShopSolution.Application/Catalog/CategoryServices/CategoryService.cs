using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Catalog.Categories;
namespace eShopSolution.Application.Catalog.CategoryServices
{
    public class CategoryService:ICategoryService
    {
        private readonly EShopSolutionDbContext context;

        public CategoryService(EShopSolutionDbContext ctx)
        {
            context = ctx;
        }
        public async Task<CategoryViewModel> CreateCategory(CreateCategoryRequest request)
        {
            try
            {
                var newCategory=new Data.Entities.Category()
                {
                    Status = request.Status,
                    ParentId = request.ParentId,
                    SortOrder = request.SortOrder,
                    IsShowOnHome = request.IsShowOnHome,
                    CategoryTranslations = new List<CategoryTranslation>()
                    {
                        new CategoryTranslation()
                        {
                            Name=request.Name,
                            LanguageId = request.LanguageId,
                            SeoAlias = request.SeoAlias,
                            SeoDescription = request.SeoDescription,
                            SeoTitle = request.SeoTitle
                        }
                    }
                };
                await context.Categories.AddAsync(newCategory);
                await context.SaveChangesAsync();
                return new CategoryViewModel()
                {
                    Id = newCategory.Id,
                    Name = request.Name,
                    LanguageId = request.LanguageId,
                    ParentId = request.ParentId,
                    SeoAlias = request.SeoAlias,
                    SeoDescription = request.SeoDescription,
                    SeoTitle = request.SeoTitle,
                    SortOrder = newCategory.SortOrder,
                    IsShowOnHome = newCategory.IsShowOnHome
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<CategoryViewModel> UpdateCategory(UpdateCategoryRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteCategory(int categoryId)
        {
            throw new System.NotImplementedException();
        }
    }
}