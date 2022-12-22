using System.Collections.Generic;

namespace XcellenceIt.Plugins.Misc.FFM.Models
{
    public class ApiResponseProductModel
    {
        public Pageinfo PageInfo { get; set; }
        public ApiProduct[] Products { get; set; }
    }
}

public class Pageinfo
{
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int ProductCountInThisResult { get; set; }
}

public class ApiProduct
{
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string Brand { get; set; }
    public string MinimumOrderQty { get; set; }
    public string ProductNumber { get; set; }
    public object DateUpdated { get; set; }
    public string ProductName { get; set; }
    public string Pack { get; set; }
    public string Size { get; set; }
    public string SellingUnit { get; set; }
    public object MapPolicyPrice { get; set; }
    public object Discountable { get; set; }
    public string Description { get; set; }
    public string Ingredients { get; set; }
    public string UPC { get; set; }
    public float Length { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Weight { get; set; }
    public object ShipsWithColdPack { get; set; }
    public string CountryOfOrigin { get; set; }
    public object ProductStateRestrictions { get; set; }
    public bool AmazonRestricted { get; set; }
    public string Keywords { get; set; }
    public bool BrandPreapprovalNeeded { get; set; }
    public object DateAdded { get; set; }
    public bool ShippingMeltRisk { get; set; }
    public Dictionary<string, string> Claims { get; set; }
    public Dictionary<string, int> Inventory { get; set; }
    public string[] Images { get; set; }
    public Pricinginfo[] PricingInfo { get; set; }
}
public class Pricinginfo
{
    public string ChainCode { get; set; }
    public string SuggestedRetailPrice { get; set; }
    public string NetCost { get; set; }
    public float NetMarginAtSRP { get; set; }
    public object PromotionalDiscountPercent { get; set; }
    public string UndiscountedContractPrice { get; set; }
}