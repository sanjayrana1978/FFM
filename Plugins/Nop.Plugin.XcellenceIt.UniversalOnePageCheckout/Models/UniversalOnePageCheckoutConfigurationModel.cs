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

using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Models
{
    public record UniversalOnePageCheckoutConfigurationModel : BaseNopModel
    {
        public UniversalOnePageCheckoutConfigurationModel()
        {
            AvailableBillingCountries = new List<SelectListItem>();
            AvailableShippingCountries = new List<SelectListItem>();
        }

        /// <summary>
        /// Current Store
        /// </summary>
        public int ActiveStoreScopeConfiguration { get; set; }

        /// <summary>
        /// Override for stores
        /// </summary>
        public bool UniversalOnePageCheckout_OverrideForStore { get; set; }

        //Enable/Disable Plugin setting
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.EnableUniversalOnePageCheckout")]
        public bool EnableUniversalOnePageCheckout { get; set; }
        //public bool EnableUniversalOnePageCheckout_OverrideForStore { get; set; }

        //UniversalOnePageCheckout title
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.UniversalOnePageCheckoutTitle")]
        public string UniversalOnePageCheckoutTitle { get; set; }
        public bool UniversalOnePageCheckoutTitle_OverrideForStore { get; set; }

        //Enable drop tables on plugin uninstall setting
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.EnableDropPlugininUninstall")]
        public bool EnableDropPluginUninstall { get; set; }
        public bool EnableDropPluginUninstall_OverrideForStore { get; set; }

        // Disable Shopping Cart
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.DisableShoppingCart")]
        public bool DisableShoppingCart { get; set; }
        public bool DisableShoppingCart_OverrideForStore { get; set; }

        // Enable Ship To Same Address by default        
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.ShipToSameAddress")]
        public bool ShipToSameAddress { get; set; }
        public bool ShipToSameAddress_OverrideForStore { get; set; }

        // Themes Colour        
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.ThemeColour")]
        public string ThemeColour { get; set; }
        public bool ThemeColour_OverrideForStore { get; set; }

        // Show estimate shipping        
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.EnableEstimateShippng")]
        public bool EnableEstimateShippng { get; set; }
        public bool EnableEstimateShippng_OverrideForStore { get; set; }

        // Show discount box        
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.EnableDiscountBox")]
        public bool EnableDiscountBox { get; set; }
        public bool EnableDiscountBox_OverrideForStore { get; set; }

        // Show gift card box        
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.EnableGiftCardBox")]
        public bool EnableGiftCardBox { get; set; }
        public bool EnableGiftCardBox_OverrideForStore { get; set; }

        // Show OrderNoteMessage box
        [NopResourceDisplayName("Nop.Plugin.XcellenceIt.UniversalOnePageCheckout.Fields.EnableOrderNoteMessage")]
        public bool EnableOrderNoteMessage { get; set; }
        public bool EnableOrderNoteMessage_OverrideForStore { get; set; }

        // Show all Billing Country list
        public IList<SelectListItem> AvailableBillingCountries
        {
            get; set;
        }

        // Show all Shipping Country list
        public IList<SelectListItem> AvailableShippingCountries
        {
            get; set;
        }

        // Default Billing Country      
        [NopResourceDisplayName("XcellenceIt.Plugin.Misc.UniversalOnePageCheckout.Fields.DefaultPaymentMethodCountry")]
        public int DefaultBillingCountryId { get; set; }
        public bool DefaultBillingCountryId_OverrideForStore { get; set; }

        // Default Shipping Country       
        [NopResourceDisplayName("XcellenceIt.Plugin.Misc.UniversalOnePageCheckout.Fields.DefaultShippingMethodCountry")]
        public int DefaultShippingCountryId { get; set; }
        public bool DefaultShippingCountryId_OverrideForStore { get; set; }

        //License Variables
        public string RegistrationForm { get; set; }
        public string LicenseInformation { get; set; }
        public bool IsLicenseActive { get; set; }
    }
}
