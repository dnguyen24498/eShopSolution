using eShopSolution.Data.Enums;

namespace eShopSolution.ViewModels.Catalog.Categories
{
    public class UpdateCategoryRequest
    {
        public int CategoryId { get; set; }
        public int SortOrder { set; get; }
        public bool IsShowOnHome { set; get; }
        public int? ParentId { set; get; }
        public Status Status { set; get; }
    }
}