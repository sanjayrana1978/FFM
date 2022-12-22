using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace XcellenceIt.Plugins.Misc.FFM.Models.Company
{
    public partial record AddCustomerToCompanyModel : BaseNopModel
    {
        #region Ctor

        public AddCustomerToCompanyModel()
        {
            SelectedCustomerIds = new List<int>();
        }
        #endregion

        #region Properties

        public int CompanyId { get; set; }

        public IList<int> SelectedCustomerIds { get; set; }

        #endregion

    }
}
