using System.Collections.Generic;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public interface ICustomProductImagesUrlsService
    {
        /// <summary>
        /// Inserts a productSkuImagesUrls
        /// </summary>
        /// <param name="productSkuImagesUrls">ProductSkuImagesUrls</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertProductImagesUrlsAsync(ProductImagesUrls productSkuImagesUrls);

        /// <summary>
        /// Deletes a productSkuImagesUrls
        /// </summary>
        /// <param name="productSkuImagesUrls"></param>
        /// <returns></returns>
        Task DeleteProductImagesUrlsAsync(ProductImagesUrls productSkuImagesUrls);

        /// <summary>
        /// Check is already exists product sku with image urls
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<List<ProductImagesUrls>> GetProductImageUrlsById(int productId);
    }
}
