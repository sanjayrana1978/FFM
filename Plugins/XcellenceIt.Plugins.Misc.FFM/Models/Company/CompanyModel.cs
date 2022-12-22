using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace XcellenceIt.Plugins.Misc.FFM.Models.Company
{
    public record CompanyModel : BaseNopEntityModel
    {
        public CompanyModel()
        {
            CompanyCustomerSearchModel = new CompanyCustomerSearchModel();
        }
        [NopResourceDisplayName("Plugins.Misc.FFM.Company.Fields.Name")]
        public string Name { get; set; }


        [NopResourceDisplayName("Plugins.Misc.FFM.Company.Fields.Point")]
        public int Point { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Company.Fields.CustomerRole")]
        public string CustomerRole { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.SubsidizedCode")]
        public string SubsidizedCode { get; set; }

        public CompanyCustomerSearchModel CompanyCustomerSearchModel { get; set; }
    }
}
