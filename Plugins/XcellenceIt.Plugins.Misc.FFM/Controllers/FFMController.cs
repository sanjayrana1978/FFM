using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Services;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Models;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.ProductAdditionalInfomation;
using XcellenceIt.Plugins.Misc.FFM.Services.SpecificationAttributepictures;

namespace XcellenceIt.Plugins.Misc.FFM.Controllers
{
    public class FFMController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IFFMServices _fFMServices;
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly ISpecificationAttributePictureService _specificationAttributePictureService;
        private readonly IProductAdditionalInfoService _productAdditionalInfoService;
        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly CustomCustomerAttributeValueService _customCustomerAttributeValueService;

        #endregion

        #region Ctor

        public FFMController(IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IFFMServices fFMServices,
            ICustomerService customerService,
            IAddressService addressService,
            ISpecificationAttributePictureService specificationAttributePictureService,
            IProductAdditionalInfoService productAdditionalInfoService,
            ILogger logger,
            ICategoryService categoryService,
            ICustomerAttributeService customerAttributeService,
            CustomCustomerAttributeValueService customCustomerAttributeValueService)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _fFMServices = fFMServices;
            _customerService = customerService;
            _addressService = addressService;
            _specificationAttributePictureService = specificationAttributePictureService;
            _productAdditionalInfoService = productAdditionalInfoService;
            _logger = logger;
            _categoryService = categoryService;
            _customerAttributeService = customerAttributeService;
            _customCustomerAttributeValueService = customCustomerAttributeValueService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var ffmSettings = await _settingService.LoadSettingAsync<FFMSettings>(storeScope);

            var model = new ConfigurationModel
            {
                Enable = ffmSettings.Enable,
                Api = ffmSettings.Api,
                RequestHeaderName = ffmSettings.RequestHeaderName,
                PrimaryKey = ffmSettings.PrimaryKey,
                SecondaryKey = ffmSettings.SecondaryKey,
                ActiveStoreScopeConfiguration = storeScope,
                PageSize = ffmSettings.PageSize,
                UNFINumber = ffmSettings.UNFINumber,
                FtpHost = ffmSettings.FtpHost,
                FtpUserName = ffmSettings.FtpUserName,
                FtpPassword = ffmSettings.FtpPassword
            };

            if (storeScope > 0)
            {
                model.Enable_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.Enable, storeScope);
                model.Api_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.Api, storeScope);
                model.RequestHeaderName_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.RequestHeaderName, storeScope);
                model.PrimaryKey_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.PrimaryKey, storeScope);
                model.SecondaryKey_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.SecondaryKey, storeScope);
                model.PageSize_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.PageSize, storeScope);
                model.UNFINumber_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.UNFINumber, storeScope);
                model.FtpHost_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.FtpHost, storeScope);
                model.FtpUserName_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.FtpUserName, storeScope);
                model.FtpPassword_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.FtpPassword, storeScope);
                model.RedirectToCategoryId_OverrideForStore = await _settingService.SettingExistsAsync(ffmSettings, x => x.RedirectToCategoryId, storeScope);
            }

            //prepare categories
            var categories = await _categoryService.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                model.AvailableCategories.Add(new SelectListItem() { Text = category.Name, Value = category.Id.ToString() });
                
            }
            
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var ffmSettings = await _settingService.LoadSettingAsync<FFMSettings>(storeScope);
            if (ModelState.IsValid)
            {
                ffmSettings.Enable = model.Enable;
                ffmSettings.Api = model.Api;
                ffmSettings.RequestHeaderName = model.RequestHeaderName;
                ffmSettings.PrimaryKey = model.PrimaryKey;
                ffmSettings.SecondaryKey = model.SecondaryKey;
                ffmSettings.PageSize = model.PageSize;
                ffmSettings.UNFINumber = model.UNFINumber;
                ffmSettings.FtpHost = model.FtpHost;
                ffmSettings.FtpUserName = model.FtpUserName;
                ffmSettings.FtpPassword = model.FtpPassword;
                ffmSettings.RedirectToCategoryId = model.RedirectToCategoryId;

                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.Enable, model.Enable_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.Api, model.Api_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.RequestHeaderName, model.RequestHeaderName_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.PrimaryKey, model.PrimaryKey_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.SecondaryKey, model.SecondaryKey_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.PageSize, model.PageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.UNFINumber, model.UNFINumber_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.FtpHost, model.FtpHost_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.FtpUserName, model.FtpUserName_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.FtpPassword, model.FtpPassword_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(ffmSettings, x => x.RedirectToCategoryId, model.RedirectToCategoryId_OverrideForStore, storeScope, false);

                await _settingService.ClearCacheAsync();

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.UNFINumber.Required"));
            }
            return await Configure();
        }

        public async Task<IActionResult> FFMResetCustomerPassword()
        {
            try
            {
                await _fFMServices.FFMPasswordRecoverySend();
                return Json(new { result = true });
            }
            catch (Exception exception)
            {
                return Json(new { result = false, message = exception.Message });
            }
        }

        public async Task<IActionResult> EncryptCryptography()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();

                foreach (var customer in customers)
                {
                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        await _logger.InsertLogAsync(LogLevel.Information, customer.Id.ToString());
                        await _customerService.UpdateCustomerAsync(customer);
                    }

                    //address
                    var customeaddress = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
                    foreach (var address in customeaddress)
                    {
                        if (!string.IsNullOrEmpty(address.Email))
                        {
                            await _logger.InsertLogAsync(LogLevel.Information, address.Id.ToString());
                            await _addressService.UpdateAddressAsync(address);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                return Json(new
                {
                    result = false,
                    message = exception.Message
                });
            }

            return Json(new
            {
                result = true,
                // message = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.EncryptCryptography.Success")
            });
        }


        [HttpPost]
        public virtual async Task<IActionResult> SpecificationAttributePictureTab(int specificationAttributeId, int pictureId)
        {
            var model = new SpecificationAttributePictures();

            if (ModelState.IsValid)
            {
                var picture = await _specificationAttributePictureService.GetPicturesBySpecificationAttributeId(specificationAttributeId);

                if (picture == null)
                {
                    if (pictureId <= 0)
                    {
                        return Json(new { result = false });
                    }
                    else
                    {
                        model.SpecificationAttributeId = specificationAttributeId;
                        model.PictureId = pictureId;
                        await _specificationAttributePictureService.InsertPictureAsync(model);
                    }
                }
                else
                {
                    picture.SpecificationAttributeId = specificationAttributeId;
                    picture.PictureId = pictureId;
                    await _specificationAttributePictureService.UpdatePictureAsync(picture);
                }

            }
            return Json(new { result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAdditionalInfoTab(int productId, string ingredientsInfomation, string nutritionalInfomation)
        {

            var model = new ProductAdditionalInfo();

            if (ModelState.IsValid)
            {
                var product = await _productAdditionalInfoService.GetProductAdditionalInfoByIdAsync(productId);

                if (product == null)
                {
                    model.ProductId = productId;
                    model.IngredientsInfomation = ingredientsInfomation;
                    model.NutritionalInfomation = nutritionalInfomation;
                    await _productAdditionalInfoService.InsertProductAdditionalInfoAsync(model);
                }
                else
                {
                    product.ProductId = productId;
                    product.IngredientsInfomation = ingredientsInfomation;
                    product.NutritionalInfomation = nutritionalInfomation;
                    await _productAdditionalInfoService.UpdateProductAdditionalInfoAsync(product);
                }
            }

            return Json(new { result = true });
        }

        #endregion
    }
}