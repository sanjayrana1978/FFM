using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Utilities;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class UniversalOnePageCheckoutPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IRepository<Language> _languageRepository;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly INopFileProvider _fileProvider;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public UniversalOnePageCheckoutPlugin(ISettingService settingService,
            IRepository<Language> languageRepository,
            ILocalizationService localizationService,
            ILanguageService languageService,
            INopFileProvider fileProvider,
            IWebHelper webHelper)
        {
            this._settingService = settingService;
            this._languageRepository = languageRepository;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._fileProvider = fileProvider;
            this._webHelper = webHelper;
        }

        #endregion


        #region Utility

        public const string SYSTEM_NAME = "XcellenceIt.Plugin.Misc.UniversalOnePageCheckout.Configure";
        public const string NOP_ACCELERATE = "XcellenceIt.Plugin.Misc.UniversalOnePageCheckout.Navigation.Root.nopAccelerate";
        public const string ONE_PAGE_CHECKOUT = "Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Navigation.Root.UniversalOnePageCheckout";
        public const string CONFIGURATION = "XcellenceIt.Plugin.Misc.UniversalOnePageCheckout.Navigation.child.Configuration";

        #endregion


        #region Methods

        #region Install / Uninstall

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            // add local resouces 
            await InstallLocaleResources(_localizationService, _languageService);

            await base.InstallAsync();
            // return Task.CompletedTask;
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<UniversalOnePageCheckoutSettings>();

            // remove local resouces
            await DeleteLocalResources();

            await base.UninstallAsync();
        }

        /// <summary>
        /// Install Resource String
        /// </summary>
        protected virtual async Task InstallLocaleResources(ILocalizationService localizationService, ILanguageService languageService)
        {
            var langauges = _languageRepository.Table.Where(l => l.Published).ToList();
            foreach (var language in langauges)
            {
                foreach (var filePath in Directory.EnumerateFiles(_fileProvider.MapPath("~/Plugins/XcellenceIt.UniversalOnePageCheckout/Localization/ResourceString"),
                    "ResourceString.xml",
                    SearchOption.TopDirectoryOnly))
                {
                    using (var streamReader = new StreamReader(filePath))
                    {
                        await _localizationService.ImportResourcesFromXmlAsync(language, streamReader);
                    }
                }
            }
        }

        ///<summry>
		///Delete Resource String
		///</summry>
        protected virtual async Task DeleteLocalResources()
        {
            var file = Path.Combine(_fileProvider.MapPath("~/Plugins/XcellenceIt.UniversalOnePageCheckout/Localization/ResourceString"), "ResourceString.xml");
            var languageResourceNames = from name in XDocument.Load(file).Document.Descendants("LocaleResource")
                                        select name.Attribute("Name").Value;
            foreach (var item in languageResourceNames)
            {
                await _localizationService.DeleteLocaleResourcesAsync(item);
            }
        }

        #endregion

        /// <summary>
		/// Configuration
		/// </summary>
		/// <returns></returns>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/UniversalOnePageCheckout/Configure";
        }

        /// <summary>
		/// Add menu item
		/// </summary>
		/// <param name="rootNode"></param>
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            //nopaccelarete menu
            var mainMenuItem = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync(NOP_ACCELERATE),
                Visible = true,
                IconClass = "nav-icon fab fa-buysellads"
            };

            //plugin menu
            var pluginMenuItem = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync(ONE_PAGE_CHECKOUT),
                Visible = true,
                IconClass = "nav-icon far fa-dot-circle"
            };

            //plugin sub menu -> configure
            var configurationMenuItem = new SiteMapNode()
            {
                SystemName = SYSTEM_NAME,
                Title = await _localizationService.GetResourceAsync(CONFIGURATION),
                ControllerName = "UniversalOnePageCheckout",
                ActionName = "Configure",
                Visible = true,
                IconClass = "nav-icon far fa-circle",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            var HelpDocument = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync("XcellenceIt.Plugin.Misc.UniversalOnePageCheckout.Navigation.child.Help"),
                Url = PluginUtility.HelpDocument,
                OpenUrlInNewTab = true,
                Visible = true,
                IconClass = "nav-icon far fa-circle",
            };

            mainMenuItem.ChildNodes.Add(pluginMenuItem);
            pluginMenuItem.ChildNodes.Add(configurationMenuItem);
            pluginMenuItem.ChildNodes.Add(HelpDocument);

            var targetMenu = await rootNode.ChildNodes.FirstOrDefaultAwaitAsync(async x => x.Title == (await _localizationService.GetResourceAsync(NOP_ACCELERATE)));
            if (targetMenu != null)
            {
                targetMenu.ChildNodes.Add(pluginMenuItem);
            }
            else
            {
                rootNode.ChildNodes.Add(mainMenuItem);
            }
        }

        #endregion
    }
}
