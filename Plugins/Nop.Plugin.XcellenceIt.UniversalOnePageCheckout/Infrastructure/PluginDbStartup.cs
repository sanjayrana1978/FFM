using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.ActionFilters;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Services;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.ViewEngines;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout
{
    /// <summary>
    /// Represents object for the configuring plugin DB context on application startup
    /// </summary>
    public class PluginDbStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //licence
            string coreDll = "Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.XcellenceIt.Core.dll";
            EmbeddedAssembly.Load(coreDll, "XcellenceIt.Core.dll");

            //register services
            //builder.RegisterType<UniversalOnePageCheckoutServices>().As<IUniversalOnePageCheckoutServices>().InstancePerLifetimeScope();
            services.AddScoped<IUniversalOnePageCheckoutServices, UniversalOnePageCheckoutServices>();

            services.Configure<MvcOptions>(congig => {
                congig.Filters.Add(new CustomErrorHandlerAttribute());
                congig.Filters.Add(new CheckoutActionFilter());
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewEngine());
            });
        }
        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 11;

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
