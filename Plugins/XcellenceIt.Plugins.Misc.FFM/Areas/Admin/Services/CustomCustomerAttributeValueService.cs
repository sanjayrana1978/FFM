using Nop.Core.Domain.Customers;
using Nop.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Services
{
    public class CustomCustomerAttributeValueService
    {
        private readonly IRepository<CustomerAttributeValue> _customerAttributeValueRepository;

        public CustomCustomerAttributeValueService(IRepository<CustomerAttributeValue> customerAttributeValueRepository)
        {
            _customerAttributeValueRepository = customerAttributeValueRepository;
        }

        public virtual async Task<IList<CustomerAttributeValue>> GetCustomCustomerAttributeValue(int attributeId)
        {
            //var query = (from a in _customerAttributeValueRepository.Table
            //            where a.Id == attributeId
            //            select a).ToList();
            var query = from cav in _customerAttributeValueRepository.Table
                        orderby cav.DisplayOrder, cav.Id
                        where cav.CustomerAttributeId == attributeId
                        select cav;
            return query.ToList();
        }
    }
}
