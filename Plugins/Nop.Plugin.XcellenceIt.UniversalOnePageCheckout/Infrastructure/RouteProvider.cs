// *************************************************************************
// *                                                                       *
// * Universal One Page Checkout Plugin for nopCommerce                    *
// * Copyright (c) Xcellence-IT. All Rights Reserved.                      *
// *                                                                       *
// *************************************************************************
// *                                                                       *
// * Email: info@nopaccelerate.com                                         *
// * Website: http://www.nopaccelerate.com                                 *
// *                                                                       *
// *************************************************************************
// *                                                                       *
// * This  software is furnished  under a license  and  may  be  used  and *
// * modified  only in  accordance with the terms of such license and with *
// * the  inclusion of the above  copyright notice.  This software or  any *
// * other copies thereof may not be provided or  otherwise made available *
// * to any  other  person.   No title to and ownership of the software is *
// * hereby transferred.                                                   *
// *                                                                       *
// * You may not reverse  engineer, decompile, defeat  license  encryption *
// * mechanisms  or  disassemble this software product or software product *
// * license.  Xcellence-IT may terminate this license if you don't comply *
// * with  any  of  the  terms and conditions set forth in  our  end  user *
// * license agreement (EULA).  In such event,  licensee  agrees to return *
// * licensor  or destroy  all copies of software  upon termination of the *
// * license.                                                              *
// *                                                                       *
// * Please see the  License file for the full End User License Agreement. *
// * The  complete license agreement is also available on  our  website at * 
// * http://www.nopaccelerate.com/enterprise-license                       *
// *                                                                       *
// *************************************************************************

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Infrastructure;
using Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Utilities;
using Nop.Services.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        private readonly UniversalOnePageCheckoutSettings _universalOnePageCheckoutSettings = EngineContext.Current.Resolve<UniversalOnePageCheckoutSettings>();
        private readonly IPluginService _pluginService = EngineContext.Current.Resolve<IPluginService>();
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public async void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //Route to configure page of plugin
            endpointRouteBuilder.MapControllerRoute("Plugin.XcellenceIt.UniversalOnePageCheckout.Configure", "Admin/UniversalOnePageCheckout/Configure",
                new { controller = "UniversalOnePageCheckout", action = "Configure", area = AreaNames.Admin });

            //Check requested plugin install or not
            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(PluginUtility.PluginSystemName, LoadPluginsMode.All);
            if (pluginDescriptor != null && !pluginDescriptor.Installed)
                return;

            if (_universalOnePageCheckoutSettings.EnableUniversalOnePageCheckout)
            {
                //add product to cart (without any attributes and options). used on catalog pages.
                endpointRouteBuilder.MapControllerRoute("UPCAddProductToCart-Catalog", "addproducttocart/catalog/{productId:min(0)}/{shoppingCartTypeId:min(0)}/{quantity:min(0)}",
                    new { controller = "OPCShoppingCart", action = "AddProductToCart_Catalog" });

                //add product to cart (with attributes and options). used on the product details pages.
                endpointRouteBuilder.MapControllerRoute("UPCAddProductToCart-Details", "addproducttocart/details/{productId:min(0)}/{shoppingCartTypeId:min(0)}",
                    new { controller = "OPCShoppingCart", action = "AddProductToCart_Details" });
                ////shopping cart
                //add product to cart(with attributes and options). used on the product details pages.
                endpointRouteBuilder.MapControllerRoute("UPCShoppingCart", "shoppingcart/",
                    new { controller = "OPCShoppingCart", action = "Cart" });

                //checkout pages
                endpointRouteBuilder.MapControllerRoute("UopcCheckoutOnePage",
                                "opconepagecheckout/",
                                new { controller = "OPCCheckout", action = "OnePageCheckout" });

                endpointRouteBuilder.MapControllerRoute("UopcUpdateCart",
                "cart/uopcuopcupdatecart/",
                new { controller = "OPCShoppingCart", action = "UopcUpdateCart" });

                endpointRouteBuilder.MapControllerRoute("UopcApplyDiscountCoupon",
                    "cart/uopcapplydiscountcoupon/",
                    new { controller = "OPCShoppingCart", action = "UopcApplyDiscountCoupon" });
                endpointRouteBuilder.MapControllerRoute("UopcRemoveDiscountCoupon",
                    "cart/uopcremovediscountcoupon/",
                    new { controller = "OPCShoppingCart", action = "UopcRemoveDiscountCoupon" });

                endpointRouteBuilder.MapControllerRoute("UopcApplyGiftCard",
                    "cart/uopcapplygiftcard/",
                    new { controller = "OPCShoppingCart", action = "UopcApplyGiftCard" });

                endpointRouteBuilder.MapControllerRoute("UopcRemoveGiftCardCode",
                    "cart/uopcremovegiftcardcode/",
                    new { controller = "OPCShoppingCart", action = "UopcRemoveGiftCardCode" });


                ////shopping cart Method
                endpointRouteBuilder.MapControllerRoute("UopcShoppingCartOrderSummary",
                                "cart/",
                                new { controller = "OPCShoppingCart", action = "OrderSummary" });
                endpointRouteBuilder.MapControllerRoute("UopcShoppingCartOrderTotals",
                                "cart/",
                                new { controller = "OPCShoppingCart", action = "OrderTotals" });
                //checkout pages
                //endpointRouteBuilder.MapControllerRoute("UopcCheckoutOnePage",
                //                "onepagecheckout/",
                //                new { controller = "OPCCheckout", action = "OnePageCheckout" });
                endpointRouteBuilder.MapControllerRoute("UopcCheckoutShippingAddress",
                                "checkout/shippingaddress",
                                new { controller = "OPCCheckout", action = "OpcShippingForm" });
                endpointRouteBuilder.MapControllerRoute("UopcCheckoutSelectShippingAddress",
                                "onepagecheckout/selectshippingaddress",
                                new { controller = "OPCCheckout", action = "SelectShippingAddress" });
                endpointRouteBuilder.MapControllerRoute("UopcCheckoutBillingAddress",
                                "onepagecheckout/",
                                new { controller = "OPCCheckout", action = "OpcBillingForm" });
                endpointRouteBuilder.MapControllerRoute("UopcCheckoutSelectBillingAddress",
                                "onepagecheckout/selectbillingaddress",
                                new { controller = "OPCCheckout", action = "SelectBillingAddress" });

                endpointRouteBuilder.MapControllerRoute("UopcCheckoutShippingMethod",
                                "onepagecheckout/",
                                new { controller = "OPCCheckout", action = "OpcShippingMethodsForm" });
                endpointRouteBuilder.MapControllerRoute("UopcCheckoutPaymentMethod",
                                "checkout/paymentmethod",
                                new { controller = "onepagecheckout", action = "PaymentMethod" });
                endpointRouteBuilder.MapControllerRoute("UopcCheckoutPaymentInfo",
                                "checkout/paymentinfo",
                                new { controller = "onepagecheckout", action = "PaymentInfo" });

                //// One page Method
                endpointRouteBuilder.MapControllerRoute("OpcShippingForm",
                                "onepagecheckout/",
                                new { controller = "onepagecheckout", action = "OpcShippingForm" });
                endpointRouteBuilder.MapControllerRoute("OpcShippingMethodsForm",
                                "onepagecheckout/",
                                new { controller = "onepagecheckout", action = "OpcShippingMethodsForm" });

                endpointRouteBuilder.MapControllerRoute("OpcPaymentMethodsForm", "onepagecheckout/",
                                new { controller = "onepagecheckout", action = "OpcPaymentMethodsForm" });

                endpointRouteBuilder.MapControllerRoute("OpcPaymentInfoForm",
                                "onepagecheckout/",
                                new { controller = "onepagecheckout", action = "OpcPaymentInfoForm" });
                endpointRouteBuilder.MapControllerRoute("OpcConfirmOrderForm",
                                "onepagecheckout/opcconfirmorderform/",
                                new { controller = "onepagecheckout", action = "OpcConfirmOrderForm" });

                endpointRouteBuilder.MapControllerRoute("UpdateOnePageCheckout",
                               "onepagecheckout/updateonepagecheckout/",
                               new { controller = "onepagecheckout", action = "UpdateOnePageCheckout" });

                endpointRouteBuilder.MapControllerRoute("UpdateOrderShippingMethods",
                               "onepagecheckout/updateordershippingmethods/",
                               new { controller = "onepagecheckout", action = "UpdateOrderShippingMethods" });

                endpointRouteBuilder.MapControllerRoute("UpdateOrderPaymentMethods",
                               "onepagecheckout/updateorderpaymentmethods/",
                               new { controller = "onepagecheckout", action = "UpdateOrderPaymentMethods" });

                endpointRouteBuilder.MapControllerRoute("UpdateOrderAttributes",
                               "onepagecheckout/updateorderattributes/",
                               new { controller = "onepagecheckout", action = "UpdateOrderAttributes" });

                endpointRouteBuilder.MapControllerRoute("UpdateOrderTotalSummary",
                               "onepagecheckout/updateordertotalsummary/",
                               new { controller = "onepagecheckout", action = "UpdateOrderTotalSummary" });

                endpointRouteBuilder.MapControllerRoute("UopcCheckoutCompleted",
                                "checkout/completed/{orderId?}",
                                new { controller = "onepagecheckout", action = "Completed" },
                                new { orderId = @"\d+" });
                endpointRouteBuilder.MapControllerRoute("UopcConfirmOrder",
                                "onepagecheckout/OpcConfirmOrder/",
                                new { controller = "onepagecheckout", action = "OpcConfirmOrder" });
            }
        }

        public int Priority
        {
            get
            {
                return int.MaxValue;
            }
        }
    }
}
