using Nop.Web.Framework.Models;

namespace XcellenceIt.Plugins.Misc.FFM.Models.Company
{
    public partial record CompanyCustomerSearchModel : BaseSearchModel
    {
        #region Properties

        public int CompanyId { get; set; }

        #endregion
    }
}
