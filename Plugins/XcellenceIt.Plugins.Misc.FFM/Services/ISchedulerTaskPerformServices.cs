using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public interface ISchedulerTaskPerformServices
    {
        /// <summary>
        /// Manage product manufacturer
        /// </summary>
        /// <param name="Name">Name</param>
        /// <param name="productId">productId</param>
        Task ManageProductManufacturer(string Name, int productId);

        /// <summary>
        /// Manage product images
        /// </summary>
        /// <param name="images">images</param>
        /// <param name="productId">productId</param>
        Task ManageProductImagesAsync(string[] images, int productId);

        /// <summary>
        /// Manage Category And SubCategory
        /// </summary>
        /// <param name="categoryName">categoryName</param>
        /// <param name="subCategoryName">subCategoryName</param>
        /// <param name="productId">productId</param>
        Task ManageProductCategoryAndSubCategoryAsync(string categoryName, string subCategoryName, int productId);

        /// <summary>
        ///  Manage products tags
        /// </summary>
        /// <param name="keywords">keywords</param>
        /// <param name="productId">productId</param>
        Task ManageProductTagsAsync(string keywords, int productId);

        /// <summary>
        /// Manage Inventory
        /// </summary>
        /// <param name="inventory">inventory</param>
        /// <param name="productId">productId</param>
        /// <param name="update">update</param>
        Task ManageProductInventoryAsync(Dictionary<string, int> inventory, int productId, bool update);

        /// <summary>
        /// Manage product specification attributes
        /// </summary>
        /// <param name="claims">claims</param>
        /// <param name="productId">productId</param>
        Task ManageProductSpecificationAttributesAsync(Dictionary<string, string> claims, int productId);

        /// <summary>
        /// Manage product attributes
        /// </summary>
        /// <param name="size">size</param>
        /// <param name="productId">productId</param>
        Task ManageProductAttributesAsync(string size, int productId);

        /// <summary>
        /// Is product updated
        /// </summary>
        /// <param name="product"></param>
        /// <param name="apiProduct"></param>
        /// <returns>true or false</returns>
        bool IsProductUpdated(Product product, ApiProduct apiProduct);

        /// <summary>
        /// Insert ingredients
        /// </summary>
        /// <param name="productId">productId</param>
        /// <param name="ingredients">ingredients</param>
        /// <returns></returns>
        Task InsertProductIngredients(int productId, string ingredients);

        /// <summary>
        /// Update ingredients
        /// </summary>
        /// <param name="productId">productId</param>
        /// <param name="ingredients">ingredients</param>
        /// <returns></returns>
        Task UpdateProductIngredients(int productId, string ingredients);
    }
}
