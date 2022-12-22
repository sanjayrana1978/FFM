using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Services
{
    /// <summary>
    /// Universal One page checkout Service interface
    /// </summary>
    public interface IUniversalOnePageCheckoutServices
    {
        #region Utility

        /// <summary>
        /// Get plugin status
        /// </summary>
        /// <returns></returns>
        Task<bool> IsValidPlugin();

        /// <summary>
        /// Get plugin status
        /// </summary>
        /// <returns></returns>
        Task<bool> IsValidPluginStoreWise(int storeId);

        /// <summary>
        /// Get Bbild date. Licence purpose 
        /// </summary>
        /// <returns></returns>
        DateTime GetBuildDate(Assembly assembly);

        #endregion
    }
}
