using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Web.Controllers;
using XcellenceIt.Plugins.Misc.FFM.Services;

namespace XcellenceIt.Plugins.Misc.FFM.ActionFilters
{
    public class PasswordRecoveryConfirmPostActionFilter : IActionFilter
    {
        #region Fields

        IFFMServices _ffmService = EngineContext.Current.Resolve<IFFMServices>();
        ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        #endregion

        public async void OnActionExecuting(ActionExecutingContext context)
        {
           
            var controller = (ControllerActionDescriptor)context.ActionDescriptor;

            if (controller.ControllerTypeInfo == typeof(CustomerController) && context.HttpContext.Request.Method.ToString() == "POST" &&
                  controller.ActionName.Equals("PasswordRecoveryConfirm"))
            {

                context.ActionArguments.TryGetValue("guid", out dynamic customerGuid);

                if (customerGuid == null)
                    return;

                var customer = await _customerService.GetCustomerByGuidAsync(customerGuid);

                if (customer.AdminComment != "1")
                    return;

                customer.AdminComment = string.Empty;
                await _customerService.UpdateCustomerAsync(customer);
            }
        }
    }
}
