using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Web.Controllers;

namespace XcellenceIt.Plugins.Misc.FFM.ActionFilters
{
    public class FFMFilterProvider : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            if (((ControllerActionDescriptor)context.ActionDescriptor).ControllerTypeInfo == typeof(HomeController)
               && ((ControllerActionDescriptor)context.ActionDescriptor).ActionName.Equals("Index"))
            {
                CustomerLoginCategoryPageActionFilter obj = new CustomerLoginCategoryPageActionFilter();
                obj.OnActionExecutingMethod(context);
            }
        }
    }
}
