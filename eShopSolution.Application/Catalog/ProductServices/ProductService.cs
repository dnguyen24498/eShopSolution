using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using EShopSolution.Ultilities.Exceptions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using eShopSolution.Application.Category.Common;
using eShopSolution.ViewModels.Catalog.ProductImages;

namespace eShopSolution.Application.Category.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly EShopSolutionDbContext context;
        private readonly IFileStorageService storageService;

        public ProductService(EShopSolutionDbContext ctx, IFileStorageService storageSer)
        {
            context = ctx;
            storageService = storageSer;
        }

        public async Task<ProductImageViewModel> AddProductImageAsync(CreateProductImageRequest createNewRequest)
        {
            try
            {
                var productImage = new ProductImage()
                {
                    Caption = createNewRequest.Caption,
                    DateCreated = DateTime.Now,
                    IsDefault = createNewRequest.IsDefault,
                    ProductId = createNewRequest.ProductId,
                    SortOrder = createNewRequest.SortOrder,
                };

                if (createNewRequest.ImageFile != null)
                {
                    productImage.FileSize = createNewRequest.ImageFile.Length;
                    productImage.ImagePath = await this.SaveFile(createNewRequest.ImageFile);
                }

                await context.ProductImages.AddAsync(productImage);
                await context.SaveChangesAsync();
                return  new ProductImageViewModel()
                {
                    Caption = productImage.Caption,
                    Id = productImage.Id,
                    DateCreated = productImage.DateCreated,
                    FileSize = productImage.FileSize,
                    ImagePath = productImage.ImagePath,
                    IsDefault = productImage.IsDefault,
                    ProductId = productImage.ProductId,
                    SortOrder = productImage.SortOrder
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task AddProductViewCountAsync(int productId)
        {
            var product = await context.Products.FindAsync(productId);
            product.ViewCount += 1;
            await context.SaveChangesAsync();
        }

        public async Task<ProductViewModel> CreateProductAsync(CreateProductRequest createNewRequest)
        {
            try
            {
                var newProduct = new Product()
                {
                    Price = createNewRequest.Price,
                    OriginalPrice = createNewRequest.OriginalPrice,
                    Stock = createNewRequest.Stock,
                    DateCreated = DateTime.Now.Date,
                    ViewCount = 0,
                    ProductTranslations = new List<ProductTranslation>()
                    {
                        new ProductTranslation()
                        {
                            Name = createNewRequest.Name,
                            Description = createNewRequest.Description,
                            Details = createNewRequest.Details,
                            LanguageId = createNewRequest.LanguageId,
                            SeoAlias = createNewRequest.SeoAlias,
                            SeoDescription = createNewRequest.SeoDescription,
                            SeoTitle = createNewRequest.SeoTitle
                        }
                    }
                };
                if (createNewRequest.ThumbnailImage != null)
                {
                    newProduct.ProductImages = new List<ProductImage>()
                    {
                        new ProductImage()
                        {
                            Caption = "Thumbnail image",
                            DateCreated = DateTime.Now.Date,
                            FileSize = createNewRequest.ThumbnailImage.Length,
                            ImagePath = await this.SaveFile(createNewRequest.ThumbnailImage),
                            IsDefault = true,
                            SortOrder = 1
                        }
                    };
                }

                await context.Products.AddAsync(newProduct);
                await context.SaveChangesAsync();
                return new ProductViewModel()
                {
                    Description = createNewRequest.Description,
                    Details = createNewRequest.Details,
                    Name = createNewRequest.Name,
                    Price = createNewRequest.Price,
                    Stock = createNewRequest.Stock,
                    DateCreated = newProduct.DateCreated,
                    LanguageId = createNewRequest.LanguageId,
                    OriginalPrice = createNewRequest.OriginalPrice,
                    ProductId = newProduct.Id,
                    SeoAlias = createNewRequest.SeoAlias,
                    SeoDescription = createNewRequest.SeoDescription,
                    SeoTitle = createNewRequest.SeoTitle,
                    ViewCount = newProduct.ViewCount
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await context.Products.FindAsync(productId);
                if (product == null) throw new EShopException($"Cannot find a product where ID: {productId}");
                var images = context.ProductImages.Where(x => x.ProductId == productId);
                foreach (var image in images)
                {
                    await storageService.DeleteFileAsync(image.ImagePath);
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return true;

            }
            catch (EShopException e)
            {
                return false;
            }
        }

        public Task<List<ProductViewModel>> GetAllProductImageByProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductViewModel>> GetAllProduct(string languageId)
        {
            var products = from p in context.Products
                join pt in context.ProductTranslations on p.Id equals pt.ProductId
                select new ProductViewModel()
                {
                    Description = pt.Description,
                    Details = pt.Details,
                    Name = pt.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    SeoDescription = pt.SeoDescription,
                    DateCreated = p.DateCreated,
                    LanguageId = pt.LanguageId,
                    OriginalPrice = p.OriginalPrice,
                    ProductId = p.Id,
                    SeoAlias = pt.SeoAlias,
                    SeoTitle = pt.SeoTitle,
                    ViewCount = p.ViewCount
                };
            return products.ToListAsync();
        }

        public async Task<PagedResult<ProductViewModel>> GetAllProductPagingByCategoriesAsync(string languageId,GetProductsPagingByCategoriesRequest request)
        {
            var query = from p in context.Products
                        join pt in context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in context.ProductInCategories on p.Id equals pic.ProductId
                        join c in context.Categories on pic.CategoryId equals c.Id
                        where (pt.LanguageId == languageId)
                        select new { p, pt, pic };
            if (!string.IsNullOrEmpty(request.Keyword)) query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            if (request.CategoryIds.Count > 0) query = query.Where(p => request.CategoryIds.Contains(p.pic.CategoryId));
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).Select(x => new ProductViewModel()
            {
                DateCreated=x.p.DateCreated,
                Description=x.pt.Description,
                Details=x.pt.Details,
                Name=x.pt.Name,
                OriginalPrice=x.p.OriginalPrice,
                Price=x.p.Price,
                SeoAlias=x.pt.SeoAlias,
                SeoDescription=x.pt.SeoDescription,
                SeoTitle=x.pt.SeoTitle,
                Stock=x.p.Stock,
                ViewCount=x.p.ViewCount,
                ProductId=x.p.Id,
                LanguageId=x.pt.LanguageId
            }).ToListAsync();

            var pagedResult = new PagedResult<ProductViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };

            return pagedResult;

        }

        public async Task<PagedResult<ProductViewModel>> GetAllProductPagingByCategoryAsync(string languageId,GetProductsPagingByCategoryRequest request)
        {
            var query = from p in context.Products
                        join pt in context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in context.ProductInCategories on p.Id equals pic.ProductId
                        join c in context.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId==languageId
                        select new { p, pt, pic };
            if(request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(x => x.pic.CategoryId == request.CategoryId);
            }
            int totalRow =await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).Select(x=>new ProductViewModel() {
                DateCreated = x.p.DateCreated,
                Description = x.pt.Description,
                Details = x.pt.Details,
                Name = x.pt.Name,
                OriginalPrice = x.p.OriginalPrice,
                Price = x.p.Price,
                SeoAlias = x.pt.SeoAlias,
                SeoDescription = x.pt.SeoDescription,
                SeoTitle = x.pt.SeoTitle,
                Stock = x.p.Stock,
                ViewCount = x.p.ViewCount,
                ProductId = x.p.Id,
                LanguageId = x.pt.LanguageId
            }).ToListAsync();
            var pagedResult = new PagedResult<ProductViewModel>()
            {
                Items = data,
                TotalRecord=totalRow
            };

            return pagedResult;
        }

        public async Task<ProductViewModel> GetProductByIdAsync(int productId, string languageId)
        {
            var product = await context.Products.FindAsync(productId);
            var productLanguage = await context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId && x.LanguageId == languageId);

            var productResult = new ProductViewModel()
            {
                DateCreated = product.DateCreated,
                Description = productLanguage.Description,
                Details = productLanguage.Details,
                Name = productLanguage.Name,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                SeoAlias = productLanguage.SeoAlias,
                SeoDescription = productLanguage.SeoDescription,
                SeoTitle = productLanguage.SeoTitle,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                ProductId = product.Id,
                LanguageId = productLanguage.LanguageId
            };
            return productResult;
        }

        public async Task<ProductImageViewModel> GetProductImageByIdAsync(int imageId)
        {
            var image = await context.ProductImages.FindAsync(imageId);
            if (image == null) throw new EShopException($"Cannot find image with id {imageId}");

            var productImageResult = new ProductImageViewModel()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                ImagePath = image.ImagePath,
                IsDefault = image.IsDefault,
                SortOrder = image.SortOrder,
                Id = image.Id,
                ProductId = image.ProductId
            };
            return productImageResult;
        }

        public async Task<bool> RemoveProductImageAsync(int imageId)
        {
            var image = await context.ProductImages.FindAsync(imageId);
            if (image == null) throw new EShopException($"Cannot find image with id {imageId}");
            else context.ProductImages.Remove(image);
            
            return await context.SaveChangesAsync()>0;
        }

        public async Task<ProductViewModel> UpdateProductAsync(UpdateProductRequest updateRequest)
        {
            try
            {
                var product = await context.Products.FindAsync(updateRequest.ProductId);
                var productTranslation = await context.ProductTranslations.FirstOrDefaultAsync(x =>
                    x.ProductId == updateRequest.ProductId && x.LanguageId == updateRequest.LanguageId);
                if (product == null || productTranslation == null)
                    throw new EShopException($"Cannot find product with id: {updateRequest.ProductId}");

                productTranslation.Name = updateRequest.Name;
                productTranslation.SeoAlias = updateRequest.SeoAlias;
                productTranslation.SeoTitle = updateRequest.SeoTitle;
                productTranslation.SeoDescription = updateRequest.SeoDescription;
                productTranslation.Description = updateRequest.Description;
                productTranslation.Details = updateRequest.Details;

                if (updateRequest.ThumbnailImage != null)
                {
                    var imageThumbnail = await context.ProductImages.FirstOrDefaultAsync(x =>
                        x.IsDefault == true && x.ProductId == updateRequest.ProductId);
                    if (imageThumbnail != null)
                    {
                        imageThumbnail.FileSize = updateRequest.ThumbnailImage.Length;
                        imageThumbnail.ImagePath = await this.SaveFile(updateRequest.ThumbnailImage);
                        context.ProductImages.Update(imageThumbnail);
                    }
                }
                await context.SaveChangesAsync();
                return new ProductViewModel()
                {
                    Description = updateRequest.Description,
                    Details = updateRequest.Details,
                    Name = updateRequest.Name,
                    Price = product.Price,
                    Stock = product.Stock,
                    DateCreated = product.DateCreated,
                    LanguageId = updateRequest.LanguageId,
                    OriginalPrice = product.OriginalPrice,
                    ProductId = product.Id,
                    SeoAlias = updateRequest.SeoAlias,
                    SeoDescription = updateRequest.SeoDescription,
                    SeoTitle = updateRequest.SeoTitle,
                    ViewCount = product.ViewCount
                };
            }
            catch(EShopException e)
            {
                return null;
            }
        }

        public async Task<ProductImageViewModel> UpdateProductImageAsync(UpdateProductImageRequest request)
        {
            try
            {
                var image = await context.ProductImages.FindAsync(request.Id);
                if (image == null) throw new EShopException($"Cannot find image with id:{request.Id}");
                if (request.ImageFile != null)
                {
                    image.Caption = request.Caption;
                    image.DateCreated = DateTime.Now;
                    image.FileSize = request.ImageFile.Length;
                    image.ImagePath = await this.SaveFile(request.ImageFile);
                    image.SortOrder = request.SortOrder;
                    image.IsDefault = request.IsDefault;
                }

                context.ProductImages.Update(image); 
                await context.SaveChangesAsync();
                return  new ProductImageViewModel()
                {
                    Caption = request.Caption,
                    Id = image.Id,
                    DateCreated = image.DateCreated,
                    FileSize = image.FileSize,
                    ImagePath = image.ImagePath,
                    IsDefault = image.IsDefault,
                    ProductId = image.ProductId,
                    SortOrder = image.SortOrder
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateProductPriceAsync(int productId, decimal newPrice)
        {
            var product = await context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find product with id: {productId}");
            product.Price = newPrice;
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductStockAsync(int productId, int addedQuantity)
        {
            var product = await context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find product with id: {productId}");
            product.Stock += addedQuantity;
            return await context.SaveChangesAsync() > 0;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }
    }
}
