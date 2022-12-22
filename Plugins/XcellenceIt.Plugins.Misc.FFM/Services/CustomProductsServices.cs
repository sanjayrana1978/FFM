using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public class CustomProductsServices : ICustomProductServices
    {
        #region Fields

        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<ProductTag> _tagRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<ProductProductTagMapping> _productProductTagMappingRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeGroup> _specificationAttributeGroupRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository<ProductAttribute> _productAttributesRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;

        #endregion

        #region Ctor

        public CustomProductsServices(IRepository<Manufacturer> manufacturerRepository,
            IRepository<ProductTag> tagRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<Warehouse> warehouseRepository,
            IRepository<ProductProductTagMapping> productProductTagMappingRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeGroup> specificationAttributeGroupRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<ProductAttribute> productAttributesRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<Category> categoryRepository,
            IRepository<StateProvince> stateProvinceRepository)
        {
            _manufacturerRepository = manufacturerRepository;
            _tagRepository = tagRepository;
            _productTagRepository = productTagRepository;
            _warehouseRepository = warehouseRepository;
            _productProductTagMappingRepository = productProductTagMappingRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeGroupRepository = specificationAttributeGroupRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _productAttributesRepository = productAttributesRepository;
            _productCategoryRepository = productCategoryRepository;
            _categoryRepository = categoryRepository;
            _stateProvinceRepository = stateProvinceRepository;
        }

        #endregion

        #region Methods 

        /// <summary>
        ///  Get manufacturer by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>Manufacturer</returns>
        public async Task<Manufacturer> GetManufacturerByNameAsync(string name)
        {
            return await _manufacturerRepository.Table.Where(m => m.Name == name && !m.Deleted).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get tags by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>ProductTag</returns>
        public async Task<ProductTag> GetTagsByNameAsync(string name)
        {
            return await _tagRepository.Table.Where(t => t.Name == name).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Insert product tag
        /// </summary>
        /// <param name="productTag">productTag</param>
        public async Task InsertProductTagAsync(ProductTag productTag)
        {
            await _productTagRepository.InsertAsync(productTag);
        }

        /// <summary>
        ///  Get warehouses by name
        /// </summary>
        /// <param name="name">name</param>
        public async Task<Warehouse> GetWarehousesByName(string name)
        {
            return await _warehouseRepository.Table.Where(w => w.Name.Equals(name)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get product product tag mapping by product id
        /// </summary>
        /// <param name="productId">productId</param>
        /// <returns>ProductProductTagMapping lists</returns>
        public async Task<List<ProductProductTagMapping>> GetProductProductTagMappingByProductId(int productId)
        {
            return await _productProductTagMappingRepository.Table.Where(t => t.ProductId == productId).ToListAsync();
        }

        /// <summary>
        /// Delete product product tag mapping
        /// </summary>
        /// <param name="productProductTagMapping">productProductTagMapping</param>
        public async Task DeleteProductProductTagMapping(ProductProductTagMapping productProductTagMapping)
        {
            await _productProductTagMappingRepository.DeleteAsync(productProductTagMapping);
        }

        /// <summary>
        /// Get specification attribute by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>SpecificationAttribute</returns>
        public async Task<SpecificationAttribute> GetSpecificationAttributeByName(string name)
        {
            return await _specificationAttributeRepository.Table.Where(s => s.Name.Equals(name)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get specification attribute group by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>SpecificationAttributeGroup</returns>
        public async Task<SpecificationAttributeGroup> GetSpecificationAttributeGroupByName(string name)
        {
            return await _specificationAttributeGroupRepository.Table.Where(s => s.Name.Equals(name)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///  Get specification attribute options by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>SpecificationAttributeOption</returns>
        public async Task<SpecificationAttributeOption> GetSpecificationAttributeOptionByNameAndSpecificationAttributeId(string name, int specificationAttributeId)
        {
            return await _specificationAttributeOptionRepository.Table.Where(s => s.Name.Equals(name) && s.SpecificationAttributeId == specificationAttributeId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get product attribute by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ProductAttribute> GetProductAttributeByName(string name)
        {
            return await _productAttributesRepository.Table.Where(a => a.Name.Equals(name)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get Product Category By CategoryId and ProductId
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> GetProductCategoryByCategoryIdProductId(int categoryId, int productId)
        {
            bool isExits = false;
            var query = await _productCategoryRepository.Table.Where(x => x.CategoryId == categoryId && x.ProductId == productId).ToListAsync();
            if (query.Any())
                isExits = true;
            return isExits;
        }

        /// <summary>
        /// Get Category By Name
        /// </summary>
        /// <param name="categoryId">categoryId</param>
        /// <returns></returns>
        public async Task<Category> GetCategoryByName(string categoryName)
        {
            return await _categoryRepository.Table.Where(c => c.Name == categoryName).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get category by category name And suCategory name
        /// </summary>
        /// <param name="categoryId">categoryId</param>
        /// <param name="subCateroyId">subCateroyId</param>
        /// <returns></returns>
        public async Task<Category> GetCategoryByCategoryNameAndSuCategoryName(string categoryName, string subCateroyName)
        {
            var category = await _categoryRepository.Table.Where(c => c.Name == categoryName).FirstOrDefaultAsync();

            if (category == null)
                return null;

            return await _categoryRepository.Table.Where(c => c.Name == subCateroyName && c.ParentCategoryId == category.Id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// GetState province by state provinceName
        /// </summary>
        /// <param name="stateProvinceName">stateProvinceName</param>
        /// <returns>StateProvince</returns>
        public async Task<StateProvince> GetStateProvinceByStateProvinceName(string stateProvinceName)
        {
            if (string.IsNullOrEmpty(stateProvinceName))
                return null;

            return await _stateProvinceRepository.Table.FirstOrDefaultAsync(s => s.Name == stateProvinceName);
        }

        #endregion

    }
}
