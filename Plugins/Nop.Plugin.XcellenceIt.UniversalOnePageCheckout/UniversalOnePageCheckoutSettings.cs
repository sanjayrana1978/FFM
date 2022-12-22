using Nop.Core.Configuration;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout
{
    public class UniversalOnePageCheckoutSettings : ISettings
    {
        #region Properties

        /// <summary>
        /// Enable UniversalOnePageCheckout Plugin
        /// </summary>
        public bool EnableUniversalOnePageCheckout { get; set; }

        /// <summary>
        /// Title when displaying UniversalOnePageCheckout to user
        /// </summary>
        public string UniversalOnePageCheckoutTitle { get; set; }

        /// <summary>
        /// Enable Drop on Plugin uninstall
        /// </summary>
        public bool EnableDropPluginUninstall { get; set; }

        /// <summary>
        /// Disable Shopping Cart
        /// </summary>
        public bool DisableShoppingCart { get; set; }

        /// <summary>
        /// Enable Ship To Same Address by default
        /// </summary>
        public bool ShipToSameAddress { get; set; }

        /// <summary>
        /// Themes Colour
        /// </summary>
        public string ThemeColour { get; set; }

        /// <summary>
        /// Show estimate shipping
        /// </summary>
        public bool EnableEstimateShippng { get; set; }

        /// <summary>
        /// Show discount box
        /// </summary>
        public bool EnableDiscountBox { get; set; }

        /// <summary>
        /// Show gift card box
        /// </summary>
        public bool EnableGiftCardBox { get; set; }

        /// <summary>
        /// Show OrderNoteMessage box
        /// </summary>
        public bool EnableOrderNoteMessage { get; set; }

        /// <summary>
        /// DefaultBillingCountry
        /// </summary>
        public int DefaultBillingCountryId { get; set; }

        /// <summary>
        /// DefaultShippingCountry
        /// </summary>
        public int DefaultShippingCountryId { get; set; }

        /// <summary>
        /// License Setting Variables
        /// </summary>
        public string LicenseKey { get; set; }
        public string OtherLicenseSettings { get; set; }

        #endregion
    }
}
