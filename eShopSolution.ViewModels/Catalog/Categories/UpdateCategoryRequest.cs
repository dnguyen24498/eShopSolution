using eShopSolution.Data.Enums;

namespace eShopSolution.ViewModels.Catalog.Categories
{
    public class UpdateCategoryRequest
    {
        public int SortOrder { set; get; }
        public bool IsShowOnHome { set; get; }
        public int? ParentId { set; get; }
        public Status Status { set; get; }
        public string Name { set; get; }
        public string SeoDescription { set; get; }
        public string SeoTitle { set; get; }
        public string SeoAlias { set; get; }
    }
}