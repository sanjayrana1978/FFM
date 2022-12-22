using Nop.Core.Caching;

namespace XcellenceIt.Plugins.Misc.FFM
{
    public class DefaultFFMStrings
    {
        public const string OrderTextFile = "Order";
        public const string ConfirmationTextFile = "Confirmation";
        public const string InvoiceTextFile = "Invoice";
        public const string TrackingTextFile = "Tracking";
        public const string OrderFolderName = "Orders";
        public const string ConfirmationsFolderName = "Confirmations";
        public const string ShippingFolderName = "Shipping";
        public const string TrackingFolderName = "Tracking";
        public const string UpdateProductsFromApiTask = "Add products from api";
        public const string ReadFilesFromFTP = "Downloads files from ftp server";
        public const string OrderPlaceConfirmationMessageTemplate = "SentOrderPlacedConfirmationMail";
        public const string OrderCancelMessageTemplate = "SentOrderCancelMail";
        public const string CustomProperyProductBasePricePAngV = "ProductBasePricePAngV";


        public static string AddRemoveRewardPointTaskName => "Update RewardPoint (FFM plugin)";

        public static string AddRemoveRewardPointTaskType => "XcellenceIt.Plugins.Misc.FFM.ScheduleTask.AddRemoveRewardPointTask";

        public const string SpecificationAttributePicture = "SpecificationAttributePicture";

        public const string ProductAdditinalInfo = "ProductAdditinalInfo";
        public const string SystemName = "Misc.FFM";

        public const string ProductAdditinalInfoDiaplayFrontSide = "ProductAdditinalInfoDiaplayFrontSide";
        public const string DisplyFrontSideIngredientInfo = "DisplyFrontSideIngredientInfo";

        public static string CategoryName  = "free from market";
    }

    public static partial class NopCustomerEncryption
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : cipher text
        /// </remarks>
        public static CacheKey DecryptCustomerByGuidCacheKey => new("Xit.customer.byguid.decrypt-{0}", DecryptCustomerPrefixCacheKey);
      
        public static string DecryptCustomerPrefixCacheKey => "Xit.customer.byguid.decrypt";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : plain text
        /// </remarks>
        public static CacheKey EncryptCustomerByGuidCacheKey => new("Xit.customer.byguid.encrypt-{0}", EncryptCustomerPrefixCacheKey);
        public static string EncryptCustomerPrefixCacheKey => "Xit.customer.byguid.encrypt";

        //public static CacheKey CustomCustomersCacheKey => new("Nop.customCustomer.number.{0}", CustomCustomersNumberPrefix);
        //public static string CustomCustomersNumberPrefix => "Nop.customCustomer.number.{0}";
        public static CacheKey CustomCustomerCacheKey => new("Nop.customCustomerEntity.number.{0}", CustomCustomerNumberPrefix);
        public static string CustomCustomerNumberPrefix => "Nop.customCustomerEntity.number.{0}";
        //public static CacheKey CustomAddressesCacheKey => new("Nop.customAddresses.number.{0}", CustomAddressesNumberPrefix);
        //public static string CustomAddressesNumberPrefix => "Nop.customAddresses.number.{0}";
        public static CacheKey CustomAddressCacheKey => new("Nop.customAddressEntity.number.{0}", CustomAddressNumberPrefix);
        public static string CustomAddressNumberPrefix => "Nop.customAddressEntity.number.{0}";

        ///// <summary>
        ///// Key for VendorNavigationModel caching
        ///// </summary>
        //public static CacheKey AllCustomersDecryptKey => new("Nop.AllCustomer.Decrypt");

    }
}
