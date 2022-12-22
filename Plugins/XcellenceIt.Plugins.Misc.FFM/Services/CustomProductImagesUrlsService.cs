using Nop.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public class CustomProductImagesUrlsService : ICustomProductImagesUrlsService
    {
        #region Fields

        private readonly IRepository<ProductImagesUrls> _productSkuImagesUrlsrepository;

        #endregion

        #region Ctor

        public CustomProductImagesUrlsService(IRepository<ProductImagesUrls> productSkuImagesUrlsrepository)
        {
            _productSkuImagesUrlsrepository = productSkuImagesUrlsrepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a productSkuImagesUrls
        /// </summary>
        /// <param name="productSkuImagesUrls">ProductSkuImagesUrls</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InsertProductImagesUrlsAsync(ProductImagesUrls productSkuImagesUrls)
        {
            await _productSkuImagesUrlsrepository.InsertAsync(productSkuImagesUrls);
        }

        /// <summary>
        /// Check is already exists product sku with image urls
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public async Task<List<ProductImagesUrls>> GetProductImageUrlsById(int productId)
        {
            return await _productSkuImagesUrlsrepository.Table.Where(p => p.ProductId == productId).ToListAsync();
        }

        /// <summary>
        /// Deletes a productSkuImagesUrls
        /// </summary>
        /// <param name="productSkuImagesUrls"></param>
        /// <returns></returns>
        public async Task DeleteProductImagesUrlsAsync(ProductImagesUrls productSkuImagesUrls)
        {
            await _productSkuImagesUrlsrepository.DeleteAsync(productSkuImagesUrls);
        }

        #endregion
    }
}
