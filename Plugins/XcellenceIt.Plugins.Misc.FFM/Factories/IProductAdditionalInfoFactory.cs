using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Models.ProducstAdditinalInfo;

namespace XcellenceIt.Plugins.Misc.FFM.Factories
{
    public partial interface IProductAdditionalInfoFactory
    {
        /// <summary>
        /// Prepare the picture model
        /// </summary>
        /// <param name="picture">picture</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        ProductAdditionalInfoModel PrepareProductAdditionalInfoModelAsync(ProductAdditionalInfoModel model,
            ProductAdditionalInfo productAdditionalInfo,
            int ProductId,
            bool excludeProperties = false);
    }
}
