using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Services.Companies;

namespace XcellenceIt.Plugins.Misc.FFM.ActionFilters
{
    public class CustomRegistrationActionFilter : IActionFilter
    {
        ILocalizationService _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();
        IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
        CompanyService _companyService = EngineContext.Current.Resolve<CompanyService>();
        IStoreContext _storeContext = EngineContext.Current.Resolve<IStoreContext>();
        IRewardPointService _rewardPointService = EngineContext.Current.Resolve<IRewardPointService>();

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            await RestrictRegistration(context);
        }

        public async Task RestrictRegistration(ActionExecutingContext context)
        {
            if (((ControllerActionDescriptor)context.ActionDescriptor).ControllerTypeInfo == typeof(CustomerController)
            && ((ControllerActionDescriptor)context.ActionDescriptor).ActionName.Equals("Register") && context.HttpContext.Request.Method.ToString() == "POST")
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var requestForm = context.HttpContext.Request.Form;

                string subsidizedCode = requestForm.ContainsKey("subsidizedCodeText") ? requestForm["subsidizedCodeText"].ToString() : string.Empty;
                var company = await _companyService.GetCompanyBySubsidizedCodeAsync(subsidizedCode);
                var customerRole = await _customerService.GetCustomerRoleByIdAsync(company.CustomerRoleId);
                //new role
                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                //add reward points
                await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, company.Point,
                        store.Id, await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.AddRewardPoints"));
            }
        }
    }
}
