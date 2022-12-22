using System;
using System.Reflection;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Utilities;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using XcellenceIt.Core;
using static Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.AssemblyAttributes;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Services
{
    /// <summary>
    /// Universal One page checkout Carousel Service
    /// </summary>
    public class UniversalOnePageCheckoutServices : IUniversalOnePageCheckoutServices
    {
        #region Fields

        private readonly IPluginService _pluginService;
        private readonly ISettingService _settingService;
        private readonly UniversalOnePageCheckoutSettings _universalOnePageCheckoutSettings;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public UniversalOnePageCheckoutServices(
                        IPluginService pluginService,
                        ISettingService settingService,
                        UniversalOnePageCheckoutSettings universalOnePageCheckoutSettings,
                        IStoreContext storeContext)
        {
            this._pluginService = pluginService;
            this._settingService = settingService;
            this._universalOnePageCheckoutSettings = universalOnePageCheckoutSettings;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilty

        /// <summary>
        /// Get plugin status
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsValidPlugin()
        {
            //Check requested plugin install or not
            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(PluginUtility.PluginSystemName, LoadPluginsMode.All);
            if (pluginDescriptor != null && !pluginDescriptor.Installed)
                return false;

            //Check plugin enable or not
            if (!_universalOnePageCheckoutSettings.EnableUniversalOnePageCheckout)
                return false;
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var universalOnePageCheckoutSetting = await _settingService.LoadSettingAsync<UniversalOnePageCheckoutSettings>(storeId);

            //Check licence detail
            LicenseImplementer licenseImplementer = new LicenseImplementer();
            if (!await licenseImplementer.IsLicenseActiveAsync(PluginUtility.PluginSystemName, universalOnePageCheckoutSetting.LicenseKey, GetBuildDate(Assembly.GetExecutingAssembly())))
                return false;

            return true;
        }


        /// <summary>
        /// Get plugin status
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsValidPluginStoreWise(int storeId)
        {
            //Check requested plugin install or not
            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(PluginUtility.PluginSystemName, LoadPluginsMode.All);
            if (pluginDescriptor != null && !pluginDescriptor.Installed)
                return false;

            //Check plugin enable or not
            if (!_universalOnePageCheckoutSettings.EnableUniversalOnePageCheckout)
                return false;


            var universalOnePageCheckoutSetting = await _settingService.LoadSettingAsync<UniversalOnePageCheckoutSettings>(storeId);

            //Check licence detail
            LicenseImplementer licenseImplementer = new LicenseImplementer();
            if (!await licenseImplementer.IsLicenseActiveAsync(PluginUtility.PluginSystemName, universalOnePageCheckoutSetting.LicenseKey, GetBuildDate(Assembly.GetExecutingAssembly())))
                return false;

            return true;
        }

        /// <summary>
        /// Get Bbild date. Licence purpose 
        /// </summary>
        /// <returns></returns>
        public DateTime GetBuildDate(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();
            return attribute != null ? attribute.DateTime : default(DateTime);
        }

        #endregion
    }
}
