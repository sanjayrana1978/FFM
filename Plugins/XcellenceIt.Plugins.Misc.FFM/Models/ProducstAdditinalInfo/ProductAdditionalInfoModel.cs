using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace XcellenceIt.Plugins.Misc.FFM.Models.ProducstAdditinalInfo
{
    public record ProductAdditionalInfoModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Admin.Fields.IngredientsInfomation")]
        public string IngredientsInfomation { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Admin.Fields.NutritionalInfomation")]
        public string NutritionalInfomation { get; set; }
    }
}
