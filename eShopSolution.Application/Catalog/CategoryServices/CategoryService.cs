using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using EShopSolution.Ultilities.Exceptions;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.ProductInCategories;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.EntityFrameworkCore;

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

        public async Task<CategoryViewModel> UpdateCategory(UpdateCategoryRequest request)
        {
            try
            {
                var category = await context.Categories.FindAsync(request.CategoryId);
                if (category == null) throw new EShopException($"Cannot find category with id: {request.CategoryId}");
                category.Status = request.Status;
                category.ParentId = request.ParentId;
                category.SortOrder = request.SortOrder;
                category.IsShowOnHome = request.IsShowOnHome;
                await context.SaveChangesAsync();
                return new CategoryViewModel()
                {
                    Status = category.Status,
                    ParentId = category.ParentId,
                    SortOrder = category.SortOrder,
                    IsShowOnHome = category.IsShowOnHome
                };
            }
            catch (EShopException ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            try
            {
                var category = await context.Categories.FindAsync(categoryId);
                if (category == null) return false;
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CategoryViewModel>>GetCategory(string languageId)
        {
            var query = from c in context.Categories
                join ct in context.CategoryTranslations on c.Id equals ct.CategoryId
                where (ct.LanguageId == languageId)
                select (new CategoryViewModel()
                {    
                    Id = c.Id,
                    Name = ct.Name,
                    Status = c.Status,
                    LanguageId = ct.LanguageId,
                    ParentId = c.ParentId,
                    SeoAlias = ct.SeoAlias,
                    SeoDescription = ct.SeoDescription,
                    SeoTitle = ct.SeoTitle,
                    SortOrder = c.SortOrder,
                    IsShowOnHome = c.IsShowOnHome
                });
            return await query.ToListAsync();
        }

        public async Task<List<ProductViewModel>> GetProductsInCategory(int categoryId, string languageId)
        {
            var products = from p in context.Products
                join pic in context.ProductInCategories on p.Id equals pic.ProductId
                join c in context.Categories on pic.CategoryId equals c.Id
                join pt in context.ProductTranslations on p.Id equals pt.ProductId
                where (c.Id == categoryId && pt.LanguageId == languageId)
                select (new ProductViewModel()
                {
                    Description = pt.Description,
                    Details = pt.Details,
                    Name = pt.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    DateCreated = p.DateCreated,
                    LanguageId = pt.LanguageId,
                    OriginalPrice = p.OriginalPrice,
                    ProductId = p.Id,
                    SeoAlias = pt.SeoAlias,
                    SeoDescription = pt.SeoDescription,
                    SeoTitle = pt.SeoTitle,
                    ViewCount = p.ViewCount
                });
            return await products.ToListAsync();
        }

        public async Task<int> AddProductToCategories(CreateProductInCategoriesRequest request)
        {
            var product = await context.Products.FindAsync(request.ProductId);
            if(product==null) throw  new EShopException($"Cannot find product with id: {request.ProductId}");
            foreach (var c in request.CategoryIds)
            {
                var checkCategory = await context.Categories.FindAsync(c);
                if (checkCategory == null) break;
                var checkCategoryAndProduct =
                    await context.ProductInCategories.SingleOrDefaultAsync(x =>
                        x.CategoryId == c && x.ProductId == request.ProductId);
                if (checkCategoryAndProduct == null)
                    await context.ProductInCategories.AddAsync(new ProductInCategory()
                    {
                        CategoryId = c,
                        ProductId = request.ProductId
                    });
            }

            await context.SaveChangesAsync();
            return request.ProductId;
        }
    }
}