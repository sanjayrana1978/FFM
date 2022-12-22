using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using System;

namespace XcellenceIt.Plugins.Misc.FFM.ActionFilters
{
    public class CustomerLoginCategoryPageActionFilter
    {
        ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();
        IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>(); 
        ISettingService _settingService = EngineContext.Current.Resolve<ISettingService>();
        ILogger _logger = EngineContext.Current.Resolve<ILogger>();
         

        public async void OnActionExecutingMethod(ActionExecutingContext filterContext)
        {
            try
            {

                //get registration model
                var controller = filterContext.Controller as Controller;
                var currentCustomer = await _workContext.GetCurrentCustomerAsync();
                var settings = await _settingService.LoadSettingAsync<FFMSettings>();
                if (settings.RedirectToCategoryId > 0 && !await _customerService.IsGuestAsync(currentCustomer))
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {{ "Controller", "PublicFFM" },
                    { "Action", "RedirectToCategory" }, { "categoryId", settings.RedirectToCategoryId} });
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("issue", ex);
            }
        }
    }
}
