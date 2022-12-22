using Nop.Web.Framework.Models;

namespace XcellenceIt.Plugins.Misc.FFM.Models.UNFI
{
    public record UnfiCategoriesSearchModel : BaseSearchModel
    {

        public UnfiCategoriesSearchModel()
        {
            unfiCategories = new UnfiCategoriesModel();
        }
        public UnfiCategoriesModel unfiCategories { get; set; }
    }
}
