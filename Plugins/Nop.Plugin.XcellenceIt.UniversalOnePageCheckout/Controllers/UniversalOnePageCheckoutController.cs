// *************************************************************************
// *                                                                       *
// * Universal One Page Checkout Plugin for nopCommerce                    *
// * Copyright (c) Xcellence-IT. All Rights Reserved.                      *
// *                                                                       *
// *************************************************************************
// *                                                                       *
// * Email: info@nopaccelerate.com                                         *
// * Website: http://www.nopaccelerate.com                                 *
// *                                                                       *
// *************************************************************************
// *                                                                       *
// * This  software is furnished  under a license  and  may  be  used  and *
// * modified  only in  accordance with the terms of such license and with *
// * the  inclusion of the above  copyright notice.  This software or  any *
// * other copies thereof may not be provided or  otherwise made available *
// * to any  other  person.   No title to and ownership of the software is *
// * hereby transferred.                                                   *
// *                                                                       *
// * You may not reverse  engineer, decompile, defeat  license  encryption *
// * mechanisms  or  disassemble this software product or software product *
// * license.  Xcellence-IT may terminate this license if you don't comply *
// * with  any  of  the  terms and conditions set forth in  our  end  user *
// * license agreement (EULA).  In such event,  licensee  agrees to return *
// * licensor  or destroy  all copies of software  upon termination of the *
// * license.                                                              *
// *                                                                       *
// * Please see the  License file for the full End User License Agreement. *
// * The  complete license agreement is also available on  our  website at * 
// * http://www.nopaccelerate.com/enterprise-license                       *
// *                                                                       *
// *************************************************************************


using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.ActionFilters;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Models;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Services;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Utilities;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using XcellenceIt.Core;
using XcellenceIt.Core.Enums;

namespace XcellenceIt.Plugin.Misc.UniversalOnePageCheckout.Controllers
{
    [Area(AreaNames.Admin)]
    public partial class UniversalOnePageCheckoutController : BasePluginController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ICountryService _countryService;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly IUniversalOnePageCheckoutServices _universalOnePageCheckoutServices;

        #endregion

        #region Ctor

        public UniversalOnePageCheckoutController(
            ISettingService settingService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ICountryService countryService,
            IWebHelper webHelper,
            IStoreContext storeContext,
            INotificationService notificationService,
            IUniversalOnePageCheckoutServices universalOnePageCheckoutServices)

        {
            this._settingService = settingService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._countryService = countryService;
            this._webHelper = webHelper;
            this._storeContext = storeContext;
            this._notificationService = notificationService;
            this._universalOnePageCheckoutServices = universalOnePageCheckoutServices;
        }

        #endregion

        #region Methods

        #region Plugin Configure
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = new UniversalOnePageCheckoutConfigurationModel();

            //Check Authorization
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            #region License validation Form

            LicenseImplementer licenseImplementer = new LicenseImplementer();
            // ++License implementation
            var currentStoreId = (await _storeContext.GetCurrentStoreAsync()).Id;
            //load settings for a chosen store scope 
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<UniversalOnePageCheckoutSettings>(currentStoreId);

            model.IsLicenseActive =
            string.IsNullOrEmpty(settings.LicenseKey) ? false : true;
            if (model.IsLicenseActive)
            {
                var buildDate = _universalOnePageCheckoutServices.GetBuildDate(Assembly.GetExecutingAssembly());
                model.LicenseInformation =
                await licenseImplementer.GetLicenseInformationAsync(PluginUtility.PluginSystemName,
                settings.LicenseKey, buildDate, ProductName.UniversalOnePageCheckout);
            }
            else
            {
                string validationURL = string.Empty;
                validationURL = _webHelper.GetStoreLocation();
                if (!_webHelper.GetStoreLocation().EndsWith("/"))
                    validationURL = "/";
                validationURL += "Admin/UniversalOnePageCheckout/ValidateLicense";
                model.RegistrationForm =
                licenseImplementer.GetRegistrationForm(ProductName.UniversalOnePageCheckout,
                LicenseApiVersion.V1, NopVersion.CURRENT_VERSION, "", validationURL,
                "http://shop.xcellence-it.com/universal-onepagecheckout");
            }

            // --License implementation

            #endregion
            var universalOnePageCheckoutSetting = await _settingService.LoadSettingAsync<UniversalOnePageCheckoutSettings>(storeScope);
            //Prepare configure model
            model.ActiveStoreScopeConfiguration = storeScope;
            model.EnableUniversalOnePageCheckout = universalOnePageCheckoutSetting.EnableUniversalOnePageCheckout;
            model.UniversalOnePageCheckoutTitle = universalOnePageCheckoutSetting.UniversalOnePageCheckoutTitle;
            model.EnableDropPluginUninstall = universalOnePageCheckoutSetting.EnableDropPluginUninstall;

            model.DisableShoppingCart = universalOnePageCheckoutSetting.DisableShoppingCart;
            model.ShipToSameAddress = universalOnePageCheckoutSetting.ShipToSameAddress;
            model.ThemeColour = universalOnePageCheckoutSetting.ThemeColour;
            model.EnableEstimateShippng = universalOnePageCheckoutSetting.EnableEstimateShippng;
            model.EnableDiscountBox = universalOnePageCheckoutSetting.EnableDiscountBox;
            model.EnableGiftCardBox = universalOnePageCheckoutSetting.EnableGiftCardBox;
            model.EnableOrderNoteMessage = universalOnePageCheckoutSetting.EnableOrderNoteMessage;

            model.DefaultBillingCountryId = universalOnePageCheckoutSetting.DefaultBillingCountryId;
            model.DefaultShippingCountryId = universalOnePageCheckoutSetting.DefaultShippingCountryId;

            model.AvailableBillingCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
            foreach (var c in await _countryService.GetAllCountriesAsync(_workContext.GetWorkingLanguageAsync().Id))
            {
                model.AvailableBillingCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, entity => entity.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.DefaultBillingCountryId
                });
            }

            model.AvailableShippingCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
            foreach (var c in await _countryService.GetAllCountriesAsync(_workContext.GetWorkingLanguageAsync().Id))
            {
                model.AvailableShippingCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, entity => entity.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.DefaultShippingCountryId
                });
            }


            if (storeScope > 0)
            {
                //model.EnableUniversalOnePageCheckout_OverrideForStore = _settingService.SettingExists(universalOnePageCheckoutSetting, x => x.EnableUniversalOnePageCheckout, storeScope);
                model.UniversalOnePageCheckoutTitle_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.UniversalOnePageCheckoutTitle, storeScope);
                model.EnableDropPluginUninstall_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.EnableDropPluginUninstall, storeScope);

                model.DisableShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.DisableShoppingCart, storeScope);
                model.ShipToSameAddress_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.ShipToSameAddress, storeScope);
                model.ThemeColour_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.ThemeColour, storeScope);
                model.EnableEstimateShippng_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.EnableEstimateShippng, storeScope);
                model.EnableDiscountBox_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.EnableDiscountBox, storeScope);
                model.EnableGiftCardBox_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.EnableGiftCardBox, storeScope);
                model.EnableOrderNoteMessage_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.EnableOrderNoteMessage, storeScope);
                model.DefaultBillingCountryId_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.DefaultBillingCountryId, storeScope);
                model.DefaultShippingCountryId_OverrideForStore = await _settingService.SettingExistsAsync(universalOnePageCheckoutSetting, x => x.DefaultShippingCountryId, storeScope);
            }

            return View("~/Plugins/XcellenceIt.UniversalOnePageCheckout/Views/UniversalOnePageCheckout/Configure.cshtml", model);
        }

        [Area(AreaNames.Admin)]
        [HttpPost]
        public async Task<IActionResult> Configure(UniversalOnePageCheckoutConfigurationModel model)
        {
            //Check authorization
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            //Check validity
            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var universalOnePageCheckoutSetting = await _settingService.LoadSettingAsync<UniversalOnePageCheckoutSettings>(storeScope);
            bool appRestart = false;
            //for rout mapping application restart
            if (universalOnePageCheckoutSetting.EnableUniversalOnePageCheckout != model.EnableUniversalOnePageCheckout)
                appRestart = true;

            universalOnePageCheckoutSetting.EnableUniversalOnePageCheckout = model.EnableUniversalOnePageCheckout;
            universalOnePageCheckoutSetting.UniversalOnePageCheckoutTitle = model.UniversalOnePageCheckoutTitle;
            universalOnePageCheckoutSetting.EnableDropPluginUninstall = model.EnableDropPluginUninstall;
            //
            universalOnePageCheckoutSetting.DisableShoppingCart = model.DisableShoppingCart;
            universalOnePageCheckoutSetting.ShipToSameAddress = model.ShipToSameAddress;
            universalOnePageCheckoutSetting.ThemeColour = model.ThemeColour;
            universalOnePageCheckoutSetting.EnableEstimateShippng = model.EnableEstimateShippng;
            universalOnePageCheckoutSetting.EnableDiscountBox = model.EnableDiscountBox;
            universalOnePageCheckoutSetting.EnableGiftCardBox = model.EnableGiftCardBox;
            universalOnePageCheckoutSetting.EnableOrderNoteMessage = model.EnableOrderNoteMessage;

            universalOnePageCheckoutSetting.DefaultBillingCountryId = model.DefaultBillingCountryId;
            universalOnePageCheckoutSetting.DefaultShippingCountryId = model.DefaultShippingCountryId;

            //if (storeScope == 0)
            await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.EnableUniversalOnePageCheckout, 0, false);

            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(universalOnePageCheckoutSetting, x => x.EnableUniversalOnePageCheckout, storeScope);

            if (model.UniversalOnePageCheckoutTitle_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.UniversalOnePageCheckoutTitle, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.UniversalOnePageCheckoutTitle, storeScope);

            if (model.EnableDropPluginUninstall_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.EnableDropPluginUninstall, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.EnableDropPluginUninstall, storeScope);

            //
            if (model.DisableShoppingCart_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.DisableShoppingCart, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.DisableShoppingCart, storeScope);

            if (model.ShipToSameAddress_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.ShipToSameAddress, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.ShipToSameAddress, storeScope);

            if (model.ThemeColour_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.ThemeColour, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.ThemeColour, storeScope);

            if (model.EnableEstimateShippng_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.EnableEstimateShippng, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.EnableEstimateShippng, storeScope);

            if (model.EnableDiscountBox_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.EnableDiscountBox, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.EnableDiscountBox, storeScope);

            if (model.EnableGiftCardBox_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.EnableGiftCardBox, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.EnableGiftCardBox, storeScope);

            if (model.EnableOrderNoteMessage_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.EnableOrderNoteMessage, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.EnableOrderNoteMessage, storeScope);

            if (model.DefaultBillingCountryId_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.DefaultBillingCountryId, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.DefaultBillingCountryId, storeScope);

            if (model.DefaultShippingCountryId_OverrideForStore || storeScope == 0)
                await _settingService.SaveSettingAsync(universalOnePageCheckoutSetting, x => x.DefaultShippingCountryId, storeScope, false);
            else if (storeScope > 0)
                await _settingService.DeleteSettingAsync(universalOnePageCheckoutSetting, x => x.DefaultShippingCountryId, storeScope);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //Application Restart for Routmap Change
            if (appRestart)
                _webHelper.RestartAppDomain();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion

        #endregion

        #region License

        [HttpPost]
        [CustomErrorHandler]
        public async Task<JsonResult> ValidateLicense(string licenseKey)
        {
            try
            {
                var currentStoreId = (await _storeContext.GetCurrentStoreAsync()).Id;
                //var currentStoreId = _storeContext.ActiveStoreScopeConfiguration;
                var buildDate = _universalOnePageCheckoutServices.GetBuildDate(Assembly.GetExecutingAssembly());
                LicenseImplementer licenseImplementer = new LicenseImplementer();
                LicenseRegistrationStatus registrationStatus =
                await licenseImplementer.RegisterLicenseKeyAsync(PluginUtility.PluginSystemName,
                licenseKey, buildDate);
                if (registrationStatus.ActiveStatus)
                {
                    //load settings
                    var universalOnePageCheckoutSettings = await _settingService.LoadSettingAsync<UniversalOnePageCheckoutSettings>(currentStoreId);

                    universalOnePageCheckoutSettings.LicenseKey = registrationStatus.LicenseKey;
                    await _settingService.SaveSettingAsync(universalOnePageCheckoutSettings, x =>
                     x.LicenseKey, currentStoreId, true);
                    return Json(new
                    {
                        status = "success",
                        success =
                    registrationStatus.StatusMessage + Environment.NewLine + "<br/>STATUS: " +
                    registrationStatus.StatusDescription
                    });
                }
                else
                if (string.IsNullOrEmpty(registrationStatus.StatusDescription))
                    return Json(new
                    {
                        status = "error",
                        error =
                    registrationStatus.StatusMessage
                    });
                else
                    return Json(new
                    {
                        status = "error",
                        error = registrationStatus.StatusMessage +
                    Environment.NewLine + "<br/>STATUS: " +
                    registrationStatus.StatusDescription
                    });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
