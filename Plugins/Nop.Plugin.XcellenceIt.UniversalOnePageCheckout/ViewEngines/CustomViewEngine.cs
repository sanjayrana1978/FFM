using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Themes;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.ViewEngines
{
    public class CustomViewEngine : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";
        private readonly UniversalOnePageCheckoutSettings _universalOnePageCheckoutSetting = EngineContext.Current.Resolve<UniversalOnePageCheckoutSettings>();

        /// <summary>
        /// Invoked by a Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine to determine the
        /// values that would be consumed by this instance of Microsoft.AspNetCore.Mvc.Razor.IViewLocationExpander.
        /// The calculated values are used to determine if the view location has changed since the last time it was located.
        /// </summary>
        /// <param name="context">Context</param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //no need to add the themeable view locations at all as the administration should not be themeable anyway
            if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
                return;

            //var themeContext = (IThemeContext)context.ActionContext.HttpContext.RequestServices.GetService(typeof(IThemeContext));
            context.Values[THEME_KEY] = EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync().Result;
        }

        /// <summary>
        /// Invoked by a Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine to determine potential locations for a view.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="viewLocations">View locations</param>
        /// <returns>iew locations</returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (_universalOnePageCheckoutSetting.EnableUniversalOnePageCheckout)
            {
                if (context.Values.TryGetValue(THEME_KEY, out string theme))
                {
                    viewLocations = new[] {
                         //themes
                         $"~/Plugins/XcellenceIt.UniversalOnePageCheckout/Themes/{theme}/Views/{{1}}/{{0}}.cshtml" ,
                         $"~/Plugins/XcellenceIt.UniversalOnePageCheckout/Themes/{theme}/Views/Shared/{{0}}.cshtml" ,

                         // default
                         "~/Plugins/XcellenceIt.UniversalOnePageCheckout/Views/{1}/{0}.cshtml",
                          "~/Plugins/XcellenceIt.UniversalOnePageCheckout/Views/Shared/{0}.cshtml"
                    }
                      .Concat(viewLocations);
                }

                if (context.ViewName == "AddressEditPopup")
                    viewLocations = new[] {
                    $"~/Plugins/XcellenceIt.UniversalOnePageCheckout/Views/OPCShoppingCart/AddressEditPopup.cshtml",
                }
                   .Concat(viewLocations);

                if (context.ViewName == "UpdateShippingAddress")
                    viewLocations = new[] {
                    $"~/Plugins/XcellenceIt.UniversalOnePageCheckout/Views/OPCShoppingCart/UpdateShippingAddress.cshtml",
                }
                   .Concat(viewLocations);
            }
            return viewLocations;
        }
    }
}
