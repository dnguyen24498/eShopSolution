using System.Collections.Generic;

namespace eShopSolution.ViewModels.Catalog.ProductInCategories
{
    public class CreateProductInCategoriesRequest
    {
        public List<int> CategoryIds { get; set; }
        public int ProductId { get; set; }
    }
}