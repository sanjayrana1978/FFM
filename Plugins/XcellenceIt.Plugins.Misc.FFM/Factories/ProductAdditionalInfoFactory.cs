using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Models.ProducstAdditinalInfo;

namespace XcellenceIt.Plugins.Misc.FFM.Factories
{
    public partial class ProductAdditionalInfoFactory : IProductAdditionalInfoFactory
    {
        /// <summary>
        /// Prepare the picture model
        /// </summary>
        /// <param name="picture">picture</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        public ProductAdditionalInfoModel PrepareProductAdditionalInfoModelAsync(ProductAdditionalInfoModel model,
            ProductAdditionalInfo productAdditionalInfo,
            int ProductId,
            bool excludeProperties = false)
        {
            if (productAdditionalInfo != null)
            {
                model.IngredientsInfomation = productAdditionalInfo.IngredientsInfomation;
                model.NutritionalInfomation = productAdditionalInfo.NutritionalInfomation;
            }

            model.ProductId = ProductId;

            return model;
        }

    }
}
