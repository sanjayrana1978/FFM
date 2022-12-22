using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Services;
using Nop.Web.Controllers;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout
{
    public class CheckoutActionFilter : IActionFilter
    {
        #region Fields
        IUniversalOnePageCheckoutServices _universalOnePageCheckoutServices = EngineContext.Current.Resolve<IUniversalOnePageCheckoutServices>();
        #endregion


        #region Uitility

        public async void CheckOutValidPluginStoreWise(ActionExecutingContext context)
        {
            var _storeContext = EngineContext.Current.Resolve<IStoreContext>();
            if (await _universalOnePageCheckoutServices.IsValidPluginStoreWise((await _storeContext.GetCurrentStoreAsync()).Id))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "OnePageCheckout",
                    controller = "OPCCheckout",
                }));
            }

        }

        public async void ShoppingCartValidPluginStoreWise(ActionExecutingContext context)
        {
            var _storeContext = EngineContext.Current.Resolve<IStoreContext>();
            if (await _universalOnePageCheckoutServices.IsValidPluginStoreWise((await _storeContext.GetCurrentStoreAsync()).Id))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "Cart",
                    controller = "OPCShoppingCart",
                }));
            }

        }

        #endregion

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // decalrre to get model and its controller 
            var controller = (ControllerActionDescriptor)context.ActionDescriptor;

            // Web / Front side 
            if (controller.ControllerTypeInfo == typeof(CheckoutController) && context.HttpContext.Request.Method.ToString() == "GET" &&
                    controller.ActionName.Equals("OnePageCheckout"))
            {

                CheckOutValidPluginStoreWise(context);

            }

            if (controller.ControllerTypeInfo == typeof(ShoppingCartController) && context.HttpContext.Request.Method.ToString() == "GET" &&
                   controller.ActionName.Equals("Cart"))
            {
                ShoppingCartValidPluginStoreWise(context);
            }
        }

    }
}
