using Nop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;

namespace XcellenceIt.Plugins.Misc.FFM.Services.ProductAdditionalInfomation
{
    public class ProductAdditionalInfoService : IProductAdditionalInfoService
    {
        private readonly IRepository<ProductAdditionalInfo> _productAdditionalInforepository;

        public ProductAdditionalInfoService(IRepository<ProductAdditionalInfo> productAdditionalInforepository)
        {
            _productAdditionalInforepository = productAdditionalInforepository;
        }

        /// <summary>
        /// Inserts a productAdditionalInfo  
        /// </summary>
        /// <param name="productAdditionalInfo"> productAdditionalInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductAdditionalInfoAsync(ProductAdditionalInfo productAdditionalInfo)
        {
            await _productAdditionalInforepository.InsertAsync(productAdditionalInfo);
        }

        /// <summary>
        /// Updates a productAdditionalInfo 
        /// </summary>
        /// <param name="productAdditionalInfo">productAdditionalInfo </param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductAdditionalInfoAsync(ProductAdditionalInfo productAdditionalInfo)
        {
            await _productAdditionalInforepository.UpdateAsync(productAdditionalInfo);
        }

        /// <summary>
        /// Gets a productAdditionalInfo  by productId identifier
        /// </summary>
        /// <param name="productId">The productId identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the productAdditionalInfo
        /// </returns>
        public virtual async Task<ProductAdditionalInfo> GetProductAdditionalInfoByIdAsync(int productId)
        {
            if (productId == 0)
                return null;

            var query = from pa in _productAdditionalInforepository.Table
                        where pa.ProductId == productId
                        select pa;

            return await query.FirstOrDefaultAsync();

        }

        /// <summary>
        /// get a productAdditionalInfo
        /// </summary>
        /// <param name="productId"> productAdditionalInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public ProductAdditionalInfo GetProductAdditionalInfoByProductIdAsync(int productId)
        {
            return _productAdditionalInforepository.Table.FirstOrDefault(x => x.ProductId == productId);

        }

        /// <summary>
        /// Deletes a productAdditionalInfo
        /// </summary>
        /// <param name="productAdditionalInfo"> productAdditionalInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductAdditionalInfoAsync(ProductAdditionalInfo productAdditionalInfo)
        {
            if (productAdditionalInfo == null)
                throw new ArgumentNullException(nameof(productAdditionalInfo));

            await _productAdditionalInforepository.DeleteAsync(productAdditionalInfo);
        }
    }
}
