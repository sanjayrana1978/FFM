using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace XcellenceIt.Plugins.Misc.FFM.Models.UNFI
{
    public record UnfiCategoriesModel : BaseNopEntityModel
    {
        public UnfiCategoriesModel()
        {
            AllCategories = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Misc.FFM.UnfiCategories.Fields.Categories")]
        public int Categories { get; set; }

        public string UnfiCategoryName { get; set; }

        public string CategoryName { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.UnfiCategories")]
        public IList<int> UnfiCategory { get; set; }
        public IList<SelectListItem> AllCategories { get; set; }
    }
}
