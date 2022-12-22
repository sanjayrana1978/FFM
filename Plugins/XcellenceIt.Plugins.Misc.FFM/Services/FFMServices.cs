using DocumentFormat.OpenXml.Wordprocessing;
using LinqToDB;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Seo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Services;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Models;
using XcellenceIt.Plugins.Misc.FFM.Services.ProductAdditionalInfomation;
using XcellenceIt.Plugins.Misc.FFM.Services.UnfiServices;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public class FFMServices : IFFMServices
    {
        #region Fields

        private readonly IRestAPICaller _restAPICaller;
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        private readonly ISchedulerTaskPerformServices _schedulerTaskPerformServices;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly IPluginService _pluginService;
        private readonly Nop.Services.Messages.IMessageTemplateService _messageTemplateService;
        private readonly INopFileProvider _fileProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IProductAdditionalInfoService _productAdditionalInfoService;
        private readonly ICategoryService _categoryService;
        private readonly UnfiCategoriesService _unfiCategoriesService;
        private readonly ImportToCSVService _importToCSVService;

        #endregion

        #region Ctor

        public FFMServices(IRestAPICaller restAPICaller,
            IProductService productService,
            ISchedulerTaskPerformServices schedulerTaskPerformServices,
            ILogger logger,
            IUrlRecordService urlRecordService,
            ISettingService settingService,
            IStoreContext storeContext,
            IGenericAttributeService genericAttributeService,
            IRepository<Customer> customerRepository,
            IWorkflowMessageService workflowMessageService,
            IWorkContext workContext,
            IPluginService pluginService,
            IMessageTemplateService messageTemplateService,
            INopFileProvider fileProvider,
            ILocalizationService localizationService,
            IScheduleTaskService scheduleTaskService,
            IProductAdditionalInfoService productAdditionalInfoService,
            ICategoryService categoryService,
            UnfiCategoriesService unfiCategoriesService,
            ImportToCSVService importToCSVService)
        {
            _restAPICaller = restAPICaller;
            _productService = productService;
            _logger = logger;
            _schedulerTaskPerformServices = schedulerTaskPerformServices;
            _urlRecordService = urlRecordService;
            _settingService = settingService;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
            _customerRepository = customerRepository;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _pluginService = pluginService;
            _messageTemplateService = messageTemplateService;
            _fileProvider = fileProvider;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
            _productAdditionalInfoService = productAdditionalInfoService;
            _categoryService = categoryService;
            _unfiCategoriesService = unfiCategoriesService;
            _importToCSVService = importToCSVService;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Update products from api
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="apiParams">apiParams</param>
        public async Task UpdateProductsFromApi(ApiResponseProductModel model, ApiParamsModel apiParams)
        {
            if (model != null)
            {
                //Manage page and limit wise loop
                var totalPages = model.PageInfo.TotalPages;

                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var ffmSettings = await _settingService.LoadSettingAsync<FFMSettings>(storeScope);

                string api = apiParams.Api;
                for (int i = 1; i <= totalPages; i++)
                {
                    try
                    {
                        api = api.Split("?")[0];
                        api = api + "?page=" + i + "&limit=" + ffmSettings.PageSize;

                        var response = await _restAPICaller.RestAPICallString(api, apiParams.RequestHeader, headerValue: apiParams.PrimaryKey);
                        var data = JsonConvert.DeserializeObject<ApiResponseProductModel>(response);
                        var categories = await _categoryService.GetAllCategoriesAsync();
                        var unfiCategories = await _unfiCategoriesService.GetAllUnfiCategoriesAsync();
                        //var mappedCategory = (from a in categories
                        //                      join b in unfiCategories on a.Id equals b.Categories
                        //                      select a).Distinct();
                        //var samecategory = from a in mappedCategory
                        //                   join b in data.Products on a.Name equals b.Category
                        //                   where a.Name == b.Category
                        //                   select a;
                        foreach (var item in data.Products)
                        {
                            var check = await _importToCSVService.GetImportToCSVByNameAndSKU(item.ProductName, item.ProductNumber);
                            if (check != null)
                                continue;

                            var productinfo = await _productService.GetProductBySkuAsync(item.ProductNumber);
                            if (productinfo == null)
                            {
                                var product = new Product();
                                product.Sku = item.ProductNumber;
                                product.Price = Convert.ToDecimal(item.PricingInfo[0].SuggestedRetailPrice);
                                product.Name = item.ProductName;
                                product.FullDescription = item.Description;
                                product.Length = item.Length != 0.0 ? (decimal)item.Length : 0;
                                product.Width = item.Width != 0.0 ? (decimal)item.Width : 0;
                                product.Height = item.Height != 0.0 ? (decimal)item.Height : 0;
                                product.Weight = item.Weight != 0.0 ? (decimal)item.Weight : 0;
                                product.OrderMinimumQuantity = 1;
                                product.StockQuantity = int.Parse(item.Pack);
                                product.Published = true;
                                product.CreatedOnUtc = DateTime.UtcNow;
                                product.UpdatedOnUtc = DateTime.UtcNow;
                                product.UseMultipleWarehouses = true;
                                product.IsShipEnabled = true;
                                product.ManageInventoryMethodId = 1;
                                product.VisibleIndividually = true;
                                product.ProductTypeId = (int)ProductType.SimpleProduct;
                                product.LowStockActivityId = 1;
                                product.NotifyAdminForQuantityBelow = 2;
                                product.AllowCustomerReviews = true;
                                product.OrderMaximumQuantity = 10000;
                                product.DisplayStockQuantity = true;
                                product.DisplayStockAvailability = true;
                                product.AllowCustomerReviews = true;

                                await _productService.InsertProductAsync(product);

                                //var category = categories.Where(x => x.Name == item.Category).FirstOrDefault();
                                //if (category != null)
                                //{
                                //    var unfiCategory = unfiCategories.Where(x => x.Categories == category.Id);

                                //    foreach (var unficategory in unfiCategory)
                                //    {
                                //        //insert the new product category mapping
                                //        await _categoryService.InsertProductCategoryAsync(new ProductCategory
                                //        {
                                //            CategoryId = unficategory.UnfiCategory,
                                //            ProductId = product.Id,
                                //            IsFeaturedProduct = false,
                                //            DisplayOrder = 1
                                //        });
                                //    }
                                //}

                                //Manage Ingredients field Which Come From api.
                                await _schedulerTaskPerformServices.InsertProductIngredients(product.Id, item.Ingredients);



                                //search engine name
                                var productSeName = await _urlRecordService.ValidateSeNameAsync(product, null, product.Name, true);
                                await _urlRecordService.SaveSlugAsync(product, productSeName, 0);

                                //category or subcategory
                                if (!string.IsNullOrEmpty(item.Category) && !string.IsNullOrEmpty(item.Subcategory))
                                    await _schedulerTaskPerformServices.ManageProductCategoryAndSubCategoryAsync(categoryName: item.Category, subCategoryName: item.Subcategory, product.Id);

                                //Brand
                                if (!string.IsNullOrEmpty(item.Brand))
                                    await _schedulerTaskPerformServices.ManageProductManufacturer(item.Brand, product.Id);

                                //Images 
                                if (item.Images.Length > 0)
                                    await _schedulerTaskPerformServices.ManageProductImagesAsync(item.Images, product.Id);

                                //Tags
                                if (!string.IsNullOrEmpty(item.Keywords))
                                    await _schedulerTaskPerformServices.ManageProductTagsAsync(keywords: item.Keywords, product.Id);

                                //Inventory
                                if (item.Inventory != null)
                                    await _schedulerTaskPerformServices.ManageProductInventoryAsync(inventory: item.Inventory, product.Id, false);

                                //Claims
                                if (item.Claims.Count > 0)
                                    await _schedulerTaskPerformServices.ManageProductSpecificationAttributesAsync(item.Claims, product.Id);

                                //Product attributes (Size)
                                if (item.Size != null && !item.Size.Equals("Undefined"))
                                    await _schedulerTaskPerformServices.ManageProductAttributesAsync(item.Size, product.Id);
                            }
                            else
                            {
                                productinfo.Price = Convert.ToDecimal(item.PricingInfo[0].SuggestedRetailPrice);
                                productinfo.OrderMinimumQuantity = item.MinimumOrderQty != null ? int.Parse(item.MinimumOrderQty) : 1;
                                productinfo.StockQuantity = int.Parse(item.Pack);
                                productinfo.UpdatedOnUtc = DateTime.UtcNow;
                                productinfo.ProductTypeId = productinfo.ProductTypeId;
                                productinfo.LowStockActivityId = 1;
                                productinfo.NotifyAdminForQuantityBelow = 2;

                                //var category = categories.Where(x => x.Name == item.Category).FirstOrDefault();
                                //if (category != null)
                                //{
                                //    var unfiCategory = unfiCategories.Where(x => x.Categories == category.Id);

                                //    foreach (var unficategory in unfiCategory)
                                //    {
                                //        //insert the new product category mapping
                                //        await _categoryService.InsertProductCategoryAsync(new ProductCategory
                                //        {
                                //            CategoryId = unficategory.UnfiCategory,
                                //            ProductId = productinfo.Id,
                                //            IsFeaturedProduct = false,
                                //            DisplayOrder = 1
                                //        });
                                //    }
                                //}
                                await _productService.UpdateProductAsync(productinfo);

                                if (!string.IsNullOrEmpty(item.Ingredients))
                                    //Manage Ingredients field Which Come From api.
                                    await _schedulerTaskPerformServices.UpdateProductIngredients(productinfo.Id, item.Ingredients);

                                //Inventory
                                if (item.Inventory != null)
                                    await _schedulerTaskPerformServices.ManageProductInventoryAsync(inventory: item.Inventory, productinfo.Id, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await _logger.ErrorAsync("Error into cycle no: " + i, ex);
                    }
                }
            }
        }

        /// <summary>
        /// FFM password recovery send
        /// </summary>
        public async Task FFMPasswordRecoverySend()
        {
            var customers = await GetAllCustomerWhereAdminCommentIsOne();
            foreach (var customer in customers)
            {
                if (customer != null && customer.Active && !customer.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = customer.CustomerGuid;
                    await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute,
                        passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    await _genericAttributeService.SaveAttributeAsync(customer,
                        NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                    //send email
                    await _workflowMessageService.SendCustomerPasswordRecoveryMessageAsync(customer,
                        (await _workContext.GetWorkingLanguageAsync()).Id);
                }
            }

        }

        /// <summary>
        /// Get all customer where admin comment is 1
        /// </summary>
        /// <returns>customers</returns>
        public async Task<IList<Customer>> GetAllCustomerWhereAdminCommentIsOne()
        {
            return await _customerRepository.Table.Where(c => c.AdminComment == "1").ToListAsync();
        }

        /// <summary>
        /// Check is plugin is enable
        /// </summary>
        /// <returns>true or false</returns>
        public async Task<bool> IsPluginEnable()
        {
            bool isPluginEnable = false;

            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>("Misc.FFM", LoadPluginsMode.InstalledOnly);
            var pluginSettings = await GetAllConfiguration();

            if ((pluginDescriptor == null && pluginDescriptor != null && pluginDescriptor.Installed == true) || (pluginSettings.Enable))
                isPluginEnable = true;

            return isPluginEnable;
        }

        /// <summary>
        /// Get all configuration
        /// </summary>
        /// <returns>FFM Settings</returns>
        public async Task<FFMSettings> GetAllConfiguration()
        {
            return await _settingService.LoadSettingAsync<FFMSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
        }


        /// <summary>
        /// Add Default EmailMessageTemplate
        /// </summary>
        public async Task AddDefaultFFMEmailMessageTemplate()
        {
            var messageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(DefaultFFMStrings.OrderPlaceConfirmationMessageTemplate);
            if (!messageTemplates.Any())
            {
                // Add MessageTemplate 
                var messageTemplate = new MessageTemplate
                {
                    Name = DefaultFFMStrings.OrderPlaceConfirmationMessageTemplate,
                    Subject = "%Store.Name%. Your order Confirmed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order has been confirmed. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = 0
                };
                await _messageTemplateService.InsertMessageTemplateAsync(messageTemplate);
            }

            var ordercancleMessageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(DefaultFFMStrings.OrderCancelMessageTemplate);
            if (!ordercancleMessageTemplates.Any())
            {
                // Add MessageTemplate 
                var ordercancleMessageTemplate = new MessageTemplate
                {
                    Name = DefaultFFMStrings.OrderCancelMessageTemplate,
                    Subject = "%Store.Name%. Your order cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order has been cancelled. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = 0
                };
                await _messageTemplateService.InsertMessageTemplateAsync(ordercancleMessageTemplate);
            }
        }

        /// <summary>
        /// Read Orderconformation File
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <returns>Read file</returns>
        public async Task<string[]> ReadOrderconformationFile(string folderName, string fileName)
        {
            string path = _fileProvider.MapPath(string.Format("~/Plugins/Misc.FFM/FTPServerFiles/{0}/{1}", folderName, fileName));
            return await File.ReadAllLinesAsync(path, Encoding.UTF8);
        }


        #endregion
    }
}
