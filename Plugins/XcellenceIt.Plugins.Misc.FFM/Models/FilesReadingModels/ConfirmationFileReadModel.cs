using System.Collections.Generic;

namespace XcellenceIt.Plugins.Misc.FFM.Models.FilesReadingModels
{
    /// <summary>
    /// Represents an Confirmation FileRead Model
    /// </summary>
    public class ConfirmationFileReadModel
    {
        public string SalesOrder { get; set; }
        public string ShipToName { get; set; }
        public string ShipToAddress1 { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCityAndState { get; set; }
        public string ShipToZipCode { get; set; }
        public string CustomerPO { get; set; }
        public string ShipViaDescription { get; set; }
        public string SalesOrderDate { get; set; }
        public string SubTotalAmount { get; set; }
        public string FreightAmount { get; set; }
        public string MiscChargeAmount { get; set; }
        public string VolumeDiscountAmount { get; set; }
        public string ProjectedTotalAmount { get; set; }
        public List<ProductDetails> ProductDetails { get; set; }
    }

    public class ProductDetails
    {
        public string SNPartNumber { get; set; }
        public string UPCNumber { get; set; }
        public string PartDescription { get; set; }
        public string QuantityOrdered { get; set; }
        public string RetailPrice { get; set; }
        public string RegWholesale { get; set; }
        public string SaleWholesale { get; set; }
        public string ExtendedPrice { get; set; }
        public string UNFIPOLineNumber { get; set; }
        public string OnHandStatus { get; set; }
    }
}
