using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.FileServices;

namespace XcellenceIt.Plugins.Misc.FFM
{
    public class FFMPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly INopFileProvider _fileProvider;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IOrderFileServices _orderFileServices;
        private readonly IFFMServices _fFMServices;
        private readonly WidgetSettings _widgetSettings;
        private readonly ICategoryService _categoryService;

        #endregion

        #region Ctor

        public FFMPlugin(IRepository<Language> languageRepository,
            INopFileProvider fileProvider,
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            IOrderFileServices orderFileServices,
            IFFMServices fFMServices,
            WidgetSettings widgetSettings,
            ICategoryService categoryService)
        {
            _languageRepository = languageRepository;
            _fileProvider = fileProvider;
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _orderFileServices = orderFileServices;
            _fFMServices = fFMServices;
            _widgetSettings = widgetSettings;
            _categoryService = categoryService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Install locale resources
        /// </summary>
        public virtual async Task InstallLocaleResources()
        {
            var language = _languageRepository.Table.Single(l => l.Name == "EN");
            foreach (var filepath in Directory.EnumerateFiles(_fileProvider.MapPath("~/Plugins/Misc.FFM/Localization"),
                "defaultResources.xml", SearchOption.TopDirectoryOnly))
            {
                using var sr = new StreamReader(filepath);
                // var localxml = File.ReadAllText(filepath);
                await _localizationService.ImportResourcesFromXmlAsync(language, sr);
            }
        }

        /// <summary>
        ///  UnInstall locale resources
        /// </summary>
        public virtual async Task UnInstallLocaleResources()
        {
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.FFM");
        }

        /// <summary>
        /// ManageSiteMapAsync
        /// </summary>
        /// <param name="rootNode">rootNode</param>
        /// <returns></returns>
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var xcellenceItMenu = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.XcellenceIT.Menu"),
                Visible = true,
                IconClass = "nav-icon fas fa-book"
            };

            var ffmMenu = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.FFM"),
                Visible = true,
                IconClass = "nav-icon fas fa-book"
            };

            var menuItem = new SiteMapNode()
            {
                SystemName = "Plugins.Misc.FFM.Configure",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Configure"),
                ControllerName = "FFM",
                ActionName = "Configure",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", AreaNames.Admin } },
            };

            var companymenuItem = new SiteMapNode()
            {
                SystemName = "Plugins.Misc.FFM.Company",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company"),
                ControllerName = "Company",
                ActionName = "List",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", AreaNames.Admin } },
            };

            var UNFImenuItem = new SiteMapNode()
            {
                SystemName = "Plugins.Misc.FFM.UnfiCategories",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.UnfiCategories"),
                ControllerName = "UnfiCategories",
                ActionName = "List",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", AreaNames.Admin } },
            };

            var importToCsvItem = new SiteMapNode()
            {
                SystemName = "Plugins.Misc.FFM.ImportToCsv",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.ImportToCsv"),
                ControllerName = "ImportToCSV",
                ActionName = "List",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", AreaNames.Admin } },
            };

            var xcellenceItMenuExists = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.XcellenceIT.Menu");
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.Title == xcellenceItMenuExists);

            if (pluginNode != null)
            {
                pluginNode.ChildNodes.Add(ffmMenu);

            }
            else
            {
                rootNode.ChildNodes.Add(xcellenceItMenu);
                xcellenceItMenu.ChildNodes.Add(ffmMenu);
            }

            ffmMenu.ChildNodes.Add(menuItem);
            ffmMenu.ChildNodes.Add(companymenuItem);
            ffmMenu.ChildNodes.Add(UNFImenuItem);
            ffmMenu.ChildNodes.Add(importToCsvItem);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/FFM/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await InstallLocaleResources();
            await _fFMServices.AddDefaultFFMEmailMessageTemplate();
            await _orderFileServices.InsertScheduleTaskAsync();

            var fFMSettings = new FFMSettings()
            {
                Enable = true,
            };
            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(DefaultFFMStrings.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(DefaultFFMStrings.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }
            await _settingService.SaveSettingAsync(fFMSettings);
            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            await UnInstallLocaleResources();
            await _orderFileServices.DeleteMessageTemplate();
            await _orderFileServices.DeleteScheduleTaskAsync();

            await base.UninstallAsync();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { AdminWidgetZones.SpecificationAttributeDetailsBlock, AdminWidgetZones.ProductDetailsBlock, PublicWidgetZones.ProductDetailsBeforeCollateral, PublicWidgetZones.ProductDetailsInsideOverviewButtonsAfter });
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == AdminWidgetZones.SpecificationAttributeDetailsBlock)
            {
                return DefaultFFMStrings.SpecificationAttributePicture;
            }
            if (widgetZone == AdminWidgetZones.ProductDetailsBlock)
            {
                return DefaultFFMStrings.ProductAdditinalInfo;
            }
            if (widgetZone == PublicWidgetZones.ProductDetailsBeforeCollateral)
            {
                return DefaultFFMStrings.ProductAdditinalInfoDiaplayFrontSide;
            }
            if (widgetZone == PublicWidgetZones.ProductDetailsInsideOverviewButtonsAfter)
            {
                return DefaultFFMStrings.DisplyFrontSideIngredientInfo;
            }
            return null;
        }

        public bool HideInWidgetList => false;

        #endregion
    }
}
