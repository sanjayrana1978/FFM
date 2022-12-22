using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Themes;
using System.Collections.Generic;
using System.Linq;
using XcellenceIt.Plugins.Misc.FFM.Services;

namespace XcellenceIt.Plugins.Misc.FFM.ViewEngine
{
    public class FFMViewEngine : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

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

            context.Values[THEME_KEY] = EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync().Result;
        }

        IFFMServices _fFMServices = EngineContext.Current.Resolve<IFFMServices>();
        /// <summary>
        /// Invoked by a Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine to determine potential locations for a view.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="viewLocations">View locations</param>
        /// <returns>iew locations</returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            context.Values.TryGetValue(THEME_KEY, out string theme);
            {
                viewLocations = new[] {
                        $"/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                        $"~/Plugins/Misc.FFM/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Misc.FFM/Views/{{0}}.cshtml"
                    }
                    .Concat(viewLocations);

                if(_fFMServices.IsPluginEnable().Result)
                {
                    if (context.ViewName == "Components/HeaderLinks/Default")
                    {
                        viewLocations = new[]
                        {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/Components/HeaderLinks/Default.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/Components/HeaderLinks/Default.cshtml"
                        }.Concat(viewLocations);
                    }

                    if (context.ViewName == "_ProductBox")
                    {
                        viewLocations = new[]
                        {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/_ProductBox.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/_ProductBox.cshtml"
                        }.Concat(viewLocations);
                    }
                    if (context.ViewName == "_ProductSpecifications")
                    {
                        viewLocations = new[]
                        {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/SpecificationAttributePicture/_SpecificationAttributePicture.cshtml",
                         $"~/Plugins/Misc.FFM/Views/SpecificationAttributePicture/_SpecificationAttributePicture.cshtml"
                        }.Concat(viewLocations);
                    }
                    if (context.ViewName == "Components/OrderSummary/Default")
                    {
                        viewLocations = new[]
                        {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/Components/OrderSummary/Default.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/Components/OrderSummary/Default.cshtml"
                        }.Concat(viewLocations);
                    }
                    if (context.ViewName.Equals("UpdateCart"))
                    {
                        viewLocations = new[] {
                          $"~/Plugins/Misc.FFM/Themes/{theme}/Views/ShoppingCart/UpdateCart.cshtml",
                          $"~/Plugins/Misc.FFM/Views/ShoppingCart/UpdateCart.cshtml"
                    }
                        .Concat(viewLocations);
                    }
                    if (context.ViewName == "Components/OrderTotals/Default")
                    {
                        viewLocations = new[]
                        {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/Components/OrderTotals/Default.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/Components/OrderTotals/Default.cshtml"
                        }.Concat(viewLocations);
                    }
                    if (context.ViewName == "Components/CategoryNavigation/Default")
                    {
                        viewLocations = new[]
                        {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/Components/CategoryNavigation/Default.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/Components/CategoryNavigation/Default.cshtml"
                        }.Concat(viewLocations);
                    }
                }
              
                if (context.ViewName == "Components/SpecificationAttributePicture/Default")
                {
                    viewLocations = new[]
                    {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/Components/SpecificationAttributePicture/Default.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/Components/SpecificationAttributePicture/Default.cshtml"
                        }.Concat(viewLocations);
                }
                if (context.ViewName == "Components/ProductAdditinalInfo/Default")
                {
                    viewLocations = new[]
                    {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/Components/ProductAdditionalInfo/Default.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/Components/ProductAdditionalInfo/Default.cshtml"
                        }.Concat(viewLocations);
                }
                if (context.ViewName == "Components/ProductAdditinalInfoDiaplayFrontSide/Default")
                {
                    viewLocations = new[]
                    {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/Components/ProductAdditinalInfoFrontSide/Default.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/Components/ProductAdditinalInfoFrontSide/Default.cshtml"
                        }.Concat(viewLocations);
                }
                if (context.ViewName == "Components/DisplyFrontSideIngredientInfo/Default")
                {
                    viewLocations = new[]
                    {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/Components/ProductIngredients/Default.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/Components/ProductIngredients/Default.cshtml"
                        }.Concat(viewLocations);
                }
                if (context.ViewName == "Register" && context.AreaName != AreaNames.Admin)
                {
                    viewLocations = new[]
                    {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/CustomRegister/Register.cshtml",
                         $"~/Plugins/Misc.FFM/Views/CustomRegister/Register.cshtml"
                        }.Concat(viewLocations);
                }
                if (context.ControllerName== "ImportToCSV" && context.ViewName == "List")
                {
                    viewLocations = new[]
                    {
                         $"~/Plugins/Misc.FFM/Areas/Admin/Views/ImportToCSV/List.cshtml"
                        }.Concat(viewLocations);
                }
                if (context.ViewName == "Components/FlyoutShoppingCart/Default")
                {
                    viewLocations = new[]
                    {
                         $"~/Plugins/Misc.FFM/Themes/{theme}/Views/Shared/Components/FlyoutShoppingCart/Default.cshtml",
                         $"~/Plugins/Misc.FFM/Views/Shared/Components/FlyoutShoppingCart/Default.cshtml"
                        }.Concat(viewLocations);
                }
            }
            return viewLocations;
        }
    }
}
