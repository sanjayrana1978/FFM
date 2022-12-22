using Nop.Core.Domain.Customers;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Models;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public interface IFFMServices
    {
        /// <summary>
        /// Update products from api
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="apiParams">apiParams</param>
        Task UpdateProductsFromApi(ApiResponseProductModel model, ApiParamsModel apiParams);

        /// <summary>
        /// FFM password recovery send
        /// </summary>
        Task FFMPasswordRecoverySend();

        /// <summary>
        /// Get all customer where admin comment is 1
        /// </summary>
        /// <returns>customers</returns>
        Task<IList<Customer>> GetAllCustomerWhereAdminCommentIsOne();

        /// <summary>
        /// Check is plugin is enable
        /// </summary>
        /// <returns>true or false</returns>
        Task<bool> IsPluginEnable();

        /// <summary>
        /// Get all configuration
        /// </summary>
        /// <returns>FFM Settings</returns>
        Task<FFMSettings> GetAllConfiguration();

        /// <summary>
        /// Add Default EmailMessageTemplate
        /// </summary>
        Task AddDefaultFFMEmailMessageTemplate();

        /// <summary>
        /// Read Orderconformation File
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <returns>Read file</returns>
        Task<string[]> ReadOrderconformationFile(string folderName, string fileName);
    }
}
