using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public interface ICustomProductServices
    {
        /// <summary>
        ///  Get manufacturer by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>Manufacturer</returns>
        Task<Manufacturer> GetManufacturerByNameAsync(string name);

        /// <summary>
        /// Get tags by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>ProductTag</returns>
        Task<ProductTag> GetTagsByNameAsync(string name);

        /// <summary>
        /// Insert product tag
        /// </summary>
        /// <param name="productTag">productTag</param>
        Task InsertProductTagAsync(ProductTag productTag);

        /// <summary>
        ///  Get warehouses by name
        /// </summary>
        /// <param name="name">name</param>
        Task<Warehouse> GetWarehousesByName(string name);

        /// <summary>
        /// Get product product tag mapping by product id
        /// </summary>
        /// <param name="productId">productId</param>
        /// <returns>ProductProductTagMapping lists</returns>
        Task<List<ProductProductTagMapping>> GetProductProductTagMappingByProductId(int productId);

        /// <summary>
        /// Delete product product tag mapping
        /// </summary>
        /// <param name="productProductTagMapping">productProductTagMapping</param>
        Task DeleteProductProductTagMapping(ProductProductTagMapping productProductTagMapping);


        /// <summary>
        /// Get specification attribute by name
        /// </summary>
        /// <param name="name">name</param>
        Task<SpecificationAttribute> GetSpecificationAttributeByName(string name);

        /// <summary>
        /// Get specification attribute group by name
        /// </summary>
        /// <param name="name">name</param>
        Task<SpecificationAttributeGroup> GetSpecificationAttributeGroupByName(string name);

        /// <summary>
        ///  Get specification attribute options by name
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="specificationAttributeId">specificationAttributeId</param>
        /// <returns>SpecificationAttributeOption</returns>
        Task<SpecificationAttributeOption> GetSpecificationAttributeOptionByNameAndSpecificationAttributeId(string name, int specificationAttributeId);

        /// <summary>
        /// Get product attribute by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<ProductAttribute> GetProductAttributeByName(string name);

        /// <summary>
        /// Get Product Category By CategoryId and ProductId
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> GetProductCategoryByCategoryIdProductId(int categoryId, int productId);

        /// <summary>
        /// Get Category By Name
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<Category> GetCategoryByName(string categoryName);

        /// <summary>
        /// Get category by category name And suCategory name
        /// </summary>
        /// <param name="categoryName">categoryName</param>
        /// <param name="subCateroyName">subCateroyName</param>
        /// <returns></returns>
        Task<Category> GetCategoryByCategoryNameAndSuCategoryName(string categoryName, string subCateroyName);

        /// <summary>
        /// GetState province by state provinceName
        /// </summary>
        /// <param name="stateProvinceName">stateProvinceName</param>
        /// <returns>StateProvince</returns>
        Task<StateProvince> GetStateProvinceByStateProvinceName(string stateProvinceName);
    }
}
