using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;

namespace XcellenceIt.Plugins.Misc.FFM.Services.ProductAdditionalInfomation
{
    public interface IProductAdditionalInfoService
    {

        /// <summary>
        /// Inserts a productAdditionalInfo  
        /// </summary>
        /// <param name="productAdditionalInfo"> productAdditionalInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertProductAdditionalInfoAsync(ProductAdditionalInfo productAdditionalInfo);

        /// <summary>
        /// Updates a productAdditionalInfo 
        /// </summary>
        /// <param name="productAdditionalInfo">productAdditionalInfo </param>
        /// <returns>A task that represents the asynchronous operation</returns>
        /// 
        Task UpdateProductAdditionalInfoAsync(ProductAdditionalInfo productAdditionalInfo);

        /// <summary>
        /// Gets a productAdditionalInfo  by productId identifier
        /// </summary>
        /// <param name="productId">The productId identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the productAdditionalInfo
        /// </returns>
        Task<ProductAdditionalInfo> GetProductAdditionalInfoByIdAsync(int productId);

        /// <summary>
        /// get a productAdditionalInfo
        /// </summary>
        /// <param name="productId"> productAdditionalInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        ProductAdditionalInfo GetProductAdditionalInfoByProductIdAsync(int productId);

        /// <summary>
        /// Deletes a productAdditionalInfo
        /// </summary>
        /// <param name="productAdditionalInfo"> productAdditionalInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteProductAdditionalInfoAsync(ProductAdditionalInfo productAdditionalInfo);
    }
}
