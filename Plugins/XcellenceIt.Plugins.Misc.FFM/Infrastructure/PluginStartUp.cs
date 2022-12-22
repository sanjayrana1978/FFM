using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Factories;
using XcellenceIt.Plugins.Misc.FFM.ActionFilters;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Controllers;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Factories;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Services;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.UnfiValidator;
using XcellenceIt.Plugins.Misc.FFM.Factories;
using XcellenceIt.Plugins.Misc.FFM.Models;
using XcellenceIt.Plugins.Misc.FFM.Models.Company;
using XcellenceIt.Plugins.Misc.FFM.Models.UNFI;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.Companies;
using XcellenceIt.Plugins.Misc.FFM.Services.FileServices;
using XcellenceIt.Plugins.Misc.FFM.Services.FTPServices;
using XcellenceIt.Plugins.Misc.FFM.Services.OverrideServices;
using XcellenceIt.Plugins.Misc.FFM.Services.ProductAdditionalInfomation;
using XcellenceIt.Plugins.Misc.FFM.Services.SpecificationAttributepictures;
using XcellenceIt.Plugins.Misc.FFM.Services.UnfiServices;
using XcellenceIt.Plugins.Misc.FFM.Validators;
using XcellenceIt.Plugins.Misc.FFM.ViewEngine;

namespace XcellenceIt.Plugins.Misc.FFM.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring services on application startup
    /// </summary>
    public class PluginStartUp : INopStartup
    {
        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {

        }

        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //override admin Controllers
            //services.AddScoped<CustomerController, OverrideCustomerController>();

            //override admin factories
            //services.AddScoped<ICustomerModelFactory, OverrideCustomerModelFactory>();
            //services.AddScoped<IOrderModelFactory, OverrrideOrderModelFactory>();
            //services.AddScoped<IReturnRequestModelFactory, OverrideReturnRequestModelFactory>();
            //services.AddScoped<IRecurringPaymentModelFactory, OverrideRecurringPaymentModelFactory>();
            //services.AddScoped<IShoppingCartModelFactory, OverrrideShoppingCartModelFactory>();
            //services.AddScoped<IActivityLogModelFactory, OverrideActivityLogModelFactory>();
            //services.AddScoped<INewsModelFactory, OverrideNewsModelFactory>();

            //override  factories
            //services.AddScoped<Nop.Web.Factories.ICustomerModelFactory, Factories.OverrideCustomerModelFactory>();
            //services.AddScoped<Nop.Web.Factories.IAddressModelFactory, Factories.OverrideAddressModelFactory>();
            //services.AddScoped<Nop.Web.Factories.IProductModelFactory, Factories.OverrrideProductModelFactory>();

            //Overrride Controller
            //services.AddScoped<Nop.Web.Controllers.CustomerController, Controllers.OverrideCustomerController>();

            //override service
            //services.AddScoped<IEncryptionService, OverrideEncryptionService>();
            //services.AddScoped<ICustomerRegistrationService, OverrideCustomerRegistrationService>();
            //services.AddScoped<IAddressAttributeFormatter, OverrideAddressAttributeFormatter>();
            //services.AddScoped<IOrderProcessingService, OverrideOrderProcessingService>();
            //services.AddScoped<IMessageTokenProvider, OverrideMessageTokenProvider>();

            //services.AddScoped(typeof(IRepository<>), typeof(CustomEntityRepository<>));
            //services.AddScoped<ICustomerService, CustomCustomerService>();
            services.AddScoped<IFFMServices, FFMServices>();
            services.AddScoped<ISchedulerTaskPerformServices, SchedulerTaskPerformServices>();
            services.AddScoped<ICustomProductServices, CustomProductsServices>();
            services.AddScoped<IRestAPICaller, RestAPICaller>();
            services.AddScoped<ICustomProductImagesUrlsService, CustomProductImagesUrlsService>();
            services.AddScoped<IOrderFileServices, OrderFileServices>();
            services.AddScoped<IFTPFileService, FTPFileService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<ISpecificationAttributePictureService, SpecificationAttributePictureService>();
            services.AddScoped<IProductAdditionalInfoService, ProductAdditionalInfoService>();
            services.AddScoped<UnfiCategoriesService>();
            services.AddScoped<CustomCustomerAttributeValueService>();
            services.AddScoped<ImportToCSVService>();
            services.AddScoped<CustomImportManager>();

            //Factory   
            services.AddScoped<Factories.CompanyModelFactory>();
            services.AddScoped<Nop.Web.Factories.IShoppingCartModelFactory, Factories.CustomShoppingCartModelFactory>();
            services.AddScoped<Factories.ISpecificAttributePictureFactory, Factories.SpecificAttributePictureFactory>();
            services.AddScoped<Factories.IProductAdditionalInfoFactory, Factories.ProductAdditionalInfoFactory>();
            services.AddScoped<UnfiCategoriesFactory>();
            services.AddScoped<ImportToCSVModelFactory>();
           

            //Action Filter
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new PasswordRecoveryConfirmPostActionFilter());
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new PasswordRecoveryConfirmPostActionFilter());
            });

            services.Configure<MvcOptions>(congig =>
            {
                congig.Filters.Add(new FFMFilterProvider());
            });
            
            services.Configure<MvcOptions>(congig =>
            {
                congig.Filters.Add(new CustomRegistrationActionFilter());
            });

            //View Engine
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new FFMViewEngine());
            });

            //validator
            services.AddTransient<IValidator<ConfigurationModel>, ConfigurationModelValidator>();
            services.AddTransient<IValidator<CompanyModel>, CompanyValidator>();
            services.AddTransient<IValidator<UnfiCategoriesModel>, UnfiValidator>();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => int.MaxValue;
    }
}
