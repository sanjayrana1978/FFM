using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Models.ImportToCSV
{
    public record ImportToCSVModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Misc.FFM.ImportToCSV.ProductName")]
        public string ProductName { get; set; }
        
        [NopResourceDisplayName("Plugins.Misc.FFM.ImportToCSV.Sku")]
        public string Sku { get; set; }
    }
}
