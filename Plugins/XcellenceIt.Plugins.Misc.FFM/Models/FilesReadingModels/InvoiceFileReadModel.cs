using System.Collections.Generic;

namespace XcellenceIt.Plugins.Misc.FFM.Models.FilesReadingModels
{
    /// <summary>
    /// Represents an Invoice FileRead Model
    /// </summary>
    public class InvoiceFileReadModel
    {
        public string Invoice { get; set; }
        public string ShipToName { get; set; }
        public string ShipToAddress1 { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCityAndState { get; set; }
        public string ShipToZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerPO { get; set; }
        public string ShipViaDescription { get; set; }
        public string SalesOrder { get; set; }
        public string ShippingDate { get; set; }
        public string Zero { get; set; }
        public string SubTotalAmount { get; set; }
        public string FreightAmount { get; set; }
        public string MiscChargeAmount { get; set; }
        public string VolumeDiscountAmount { get; set; }
        public string TaxAmount { get; set; }
        public string InvoiceAmount { get; set; }

        public List<InvoiceProductDetails> ProductDetails { get; set; }
    }
    public class InvoiceProductDetails
    {
        public string SNPartNumber { get; set; }
        public string UPCNumber { get; set; }
        public string PartDescription { get; set; }
        public string QuantityOrdered { get; set; }
        public string QuantityShipped { get; set; }
        public string UnitPrice { get; set; }
        public string RegWholesale { get; set; }
        public string SaleWholesale { get; set; }
        public string ExtendedPrice { get; set; }
        public string UNFIPOLineNumber { get; set; }
    }
}
