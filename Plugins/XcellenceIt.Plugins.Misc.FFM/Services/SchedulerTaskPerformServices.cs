using Microsoft.AspNetCore.StaticFiles;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Services.ProductAdditionalInfomation;
using XcellenceIt.Plugins.Misc.FFM.Services.UnfiServices;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public class SchedulerTaskPerformServices : ISchedulerTaskPerformServices
    {
        #region Fields

        private readonly IManufacturerService _manufacturerService;
        private readonly ICustomProductServices _ffmMProductServices;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductTagService _productTagService;
        private readonly IShippingService _shippingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IUrlRecordService _urlRecordService;
        protected readonly INopFileProvider _fileProvider;
        protected readonly ICustomProductImagesUrlsService _productSkuImagesUrlsService;
        private readonly IProductAdditionalInfoService _productAdditionalInfoService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly UnfiCategoriesService _unfiCategoriesService;

        #endregion

        #region Ctor
        public SchedulerTaskPerformServices(IManufacturerService manufacturerService,
            ICustomProductServices ffmMProductServices,
            IPictureService pictureService,
            IProductService productService,
            ICategoryService categoryService,
            IProductTagService productTagService,
            IShippingService shippingService,
            ISpecificationAttributeService specificationAttributeService,
            IProductAttributeService productAttributeService,
            IUrlRecordService urlRecordService,
            INopFileProvider fileProvider,
            ICustomProductImagesUrlsService productSkuImagesUrlsService,
            IProductAdditionalInfoService productAdditionalInfoService,
            ILogger logger,
            ILocalizationService localizationService,
            UnfiCategoriesService unfiCategoriesService)
        {
            _manufacturerService = manufacturerService;
            _ffmMProductServices = ffmMProductServices;
            _pictureService = pictureService;
            _productService = productService;
            _categoryService = categoryService;
            _productTagService = productTagService;
            _shippingService = shippingService;
            _specificationAttributeService = specificationAttributeService;
            _productAttributeService = productAttributeService;
            _urlRecordService = urlRecordService;
            _fileProvider = fileProvider;
            _productSkuImagesUrlsService = productSkuImagesUrlsService;
            _productAdditionalInfoService = productAdditionalInfoService;
            _logger = logger;
            _localizationService = localizationService;
            _unfiCategoriesService = unfiCategoriesService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get mime type from file path
        /// </summary>
        /// <param name="filePath">filePath</param>
        /// <returns>File mime type</returns>
        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

            //set to jpeg in case mime type cannot be found
            return mimeType ?? MimeTypes.ImageJpeg;
        }

        /// <summary>
        /// Get the unique name of the file (add -copy-(N) to the file name if there is already a file with that name in the directory)
        /// </summary>
        /// <param name="directoryPath">Path to the file directory</param>
        /// <param name="fileName">Original file name</param>
        /// <returns>Unique name of the file</returns>
        protected virtual string GetUniqueFileName(string directoryPath, string fileName)
        {
            var uniqueFileName = fileName;

            var i = 0;
            while (_fileProvider.FileExists(_fileProvider.Combine(directoryPath, uniqueFileName)))
            {
                uniqueFileName = $"{_fileProvider.GetFileNameWithoutExtension(fileName)}-Copy-{++i}{_fileProvider.GetFileExtension(fileName)}";
            }

            return uniqueFileName;
        }

        /// <summary>
        /// Insert product images from Url
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        protected virtual async Task<int> InsertProductImagesFromUrl(string imageUrl, int productId)
        {
            var webClient = new WebClient();

            byte[] pictureBinary = webClient.DownloadData(imageUrl);
            var mimeType = GetMimeTypeFromFilePath(imageUrl);

            var picture = await _pictureService.InsertPictureAsync(pictureBinary: pictureBinary, mimeType, seoFilename: null);

            var productPicture = new ProductPicture();

            productPicture.PictureId = picture.Id;
            productPicture.ProductId = productId;

            await _productService.InsertProductPictureAsync(productPicture);

            return picture.Id;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Manage product manufacturer
        /// </summary>
        /// <param name="Name">Name</param>
        /// <param name="productId">productId</param>
        /// <param name="update">update</param> 
        public async Task ManageProductManufacturer(string Name, int productId)
        {
            var manufacturer = await _ffmMProductServices.GetManufacturerByNameAsync(Name);
            var productManufacturerModel = new ProductManufacturer();
            var manufacturerForInsert = new Manufacturer();

            if (manufacturer != null)
            {
                if (!(await _manufacturerService.GetProductManufacturersByProductIdAsync(productId)).Where(x => x.ManufacturerId == manufacturer.Id).Any())
                {
                    productManufacturerModel = new ProductManufacturer();
                    productManufacturerModel.ProductId = productId;
                    productManufacturerModel.ManufacturerId = manufacturer.Id;
                    await _manufacturerService.InsertProductManufacturerAsync(productManufacturerModel);
                }
            }
            else
            {
                manufacturerForInsert = new Manufacturer();

                manufacturerForInsert.Name = Name;
                manufacturerForInsert.CreatedOnUtc = DateTime.UtcNow;
                manufacturerForInsert.UpdatedOnUtc = DateTime.UtcNow;
                manufacturerForInsert.Published = true;
                manufacturerForInsert.PageSize = 6;
                await _manufacturerService.InsertManufacturerAsync(manufacturerForInsert);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(manufacturerForInsert, null, manufacturerForInsert.Name, false);
                await _urlRecordService.SaveSlugAsync(manufacturerForInsert, seName, 0);

                if (!(await _manufacturerService.GetProductManufacturersByProductIdAsync(productId)).Where(x => x.ManufacturerId == manufacturerForInsert.Id).Any())
                {
                    productManufacturerModel = new ProductManufacturer();

                    productManufacturerModel.ProductId = productId;
                    productManufacturerModel.ManufacturerId = manufacturerForInsert.Id;
                    await _manufacturerService.InsertProductManufacturerAsync(productManufacturerModel);
                }
            }
        }

        /// <summary>
        /// Manage product images
        /// </summary>
        /// <param name="images">images</param>
        /// <param name="productId">productId</param>
        /// <param name="update">update</param> 
        public async Task ManageProductImagesAsync(string[] images, int productId)
        {
            foreach (var imageUrl in images)
            {
                await InsertProductImagesFromUrl(imageUrl, productId);
            }
        }

        /// <summary>
        /// Manage Category And SubCategory
        /// </summary>
        /// <param name="categoryName">categoryName</param>
        /// <param name="subCategoryName">subCategoryName</param>
        /// <param name="productId">productId</param>
        /// <param name="update">update</param>
        public async Task ManageProductCategoryAndSubCategoryAsync(string categoryName, string subCategoryName, int productId)
        {
            int categoryId = 0;
            var categoryInfo = new Category();
            categoryInfo.CreatedOnUtc = DateTime.UtcNow;
            categoryInfo.UpdatedOnUtc = DateTime.UtcNow;
            categoryInfo.IncludeInTopMenu = true;
            var productCategory = new ProductCategory();

            var unfiCategories = await _unfiCategoriesService.GetAllUnfiCategoriesAsync();
            // var unfiCategory = unfiCategories.Where(x => x.Categories == category.Id);
            var mainCategoryName = new Category();

            if (!categoryName.Equals("Undefined", StringComparison.OrdinalIgnoreCase))
            {
                var category = await _ffmMProductServices.GetCategoryByName(categoryName);
                if (category != null)
                {
                    categoryId = category.Id;
                    mainCategoryName = category;
                    var unfiCategorie = unfiCategories.Where(x => x.UnfiCategory == category.Id);
                    if (unfiCategorie.Any())
                    {
                        if (await _ffmMProductServices.GetProductCategoryByCategoryIdProductId(unfiCategorie.FirstOrDefault().Categories, productId) == false)
                        {
                            await _categoryService.InsertProductCategoryAsync(new ProductCategory
                            {
                                CategoryId = unfiCategorie.FirstOrDefault().Categories,
                                ProductId = productId
                            });
                        }
                    }
                }
            }

            if (!subCategoryName.Equals("Undefined", StringComparison.OrdinalIgnoreCase))
            {
                var subCategory = await _ffmMProductServices.GetCategoryByName(subCategoryName);

                if (subCategory != null)
                {
                    var unfiCategorie = unfiCategories.Where(x => x.UnfiCategory == subCategory.Id);

                    if (!categoryName.Equals("Undefined", StringComparison.OrdinalIgnoreCase))
                    {
                        var unfiCategorie1 = unfiCategories.Where(x => x.UnfiCategory == mainCategoryName.Id);
                        if (unfiCategorie.Any())
                        {
                            var getMainNopCategory = await _categoryService.GetCategoryByIdAsync(unfiCategorie.FirstOrDefault().Categories);
                            var getNopCategory = await _categoryService.GetCategoryByIdAsync(unfiCategorie.FirstOrDefault().Categories);
                            var subCategoryWithCategoryInfo = await _ffmMProductServices.GetCategoryByCategoryNameAndSuCategoryName(getMainNopCategory.Name, getNopCategory.Name);

                            if (subCategoryWithCategoryInfo != null)
                            {
                                if (unfiCategorie.Any())
                                {
                                    if (await _ffmMProductServices.GetProductCategoryByCategoryIdProductId(unfiCategorie.FirstOrDefault().Categories, productId) == false)
                                    {
                                        await _categoryService.InsertProductCategoryAsync(new ProductCategory
                                        {
                                            CategoryId = unfiCategorie.FirstOrDefault().Categories,
                                            ProductId = productId
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (categoryName.Equals("Undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!subCategoryName.Equals("Undefined", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (unfiCategorie.Any())
                                    {
                                        if (await _ffmMProductServices.GetProductCategoryByCategoryIdProductId(unfiCategorie.FirstOrDefault().Categories, productId) == false)
                                        {
                                            await _categoryService.InsertProductCategoryAsync(new ProductCategory
                                            {
                                                CategoryId = unfiCategorie.FirstOrDefault().Categories,
                                                ProductId = productId
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (unfiCategorie.Any())
                        {
                            if (await _ffmMProductServices.GetProductCategoryByCategoryIdProductId(unfiCategorie.FirstOrDefault().Categories, productId) == false)
                            {
                                await _categoryService.InsertProductCategoryAsync(new ProductCategory
                                {
                                    CategoryId = unfiCategorie.FirstOrDefault().Categories,
                                    ProductId = productId
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                if (categoryId > 0)
                {
                    if (await _ffmMProductServices.GetProductCategoryByCategoryIdProductId(categoryId, productId) == false)
                    {
                        var unfiCategorie = unfiCategories.Where(x => x.UnfiCategory == categoryId);
                        if (unfiCategorie.Any())
                        {
                            if (await _ffmMProductServices.GetProductCategoryByCategoryIdProductId(unfiCategorie.FirstOrDefault().Categories, productId) == false)
                            {
                                await _categoryService.InsertProductCategoryAsync(new ProductCategory
                                {
                                    CategoryId = unfiCategorie.FirstOrDefault().Categories,
                                    ProductId = productId
                                });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Manage products tags
        /// </summary>
        /// <param name="keywords">keywords</param>
        /// <param name="productId">productId</param>
        /// <param name="update">update</param>
        public async Task ManageProductTagsAsync(string keywords, int productId)
        {
            string[] tags;

            if (keywords.Contains(','))
                tags = keywords.Split(",");
            else
                tags = keywords.Split(";");

            var productTag = new ProductProductTagMapping();

            var tag = new ProductTag();

            foreach (var tagName in tags)
            {
                var tagExists = await _ffmMProductServices.GetTagsByNameAsync(tagName);

                if (tagExists == null)
                {
                    tag = new ProductTag();

                    tag.Name = tagName;
                    await _ffmMProductServices.InsertProductTagAsync(tag);

                    productTag = new ProductProductTagMapping();

                    if ((await _productTagService.GetAllProductTagsByProductIdAsync(productId)).Any())
                    {
                        if (!(await _productTagService.GetAllProductTagsByProductIdAsync(productId)).Where(x => x.Id == tag.Id).Any())
                        {
                            productTag.ProductId = productId;
                            productTag.ProductTagId = tag.Id;
                            await _productTagService.InsertProductProductTagMappingAsync(productTag);
                        }
                    }
                    else
                    {
                        productTag.ProductId = productId;
                        productTag.ProductTagId = tag.Id;
                        await _productTagService.InsertProductProductTagMappingAsync(productTag);
                    }
                }
                else
                {
                    productTag = new ProductProductTagMapping();
                    if ((await _productTagService.GetAllProductTagsByProductIdAsync(productId)).Any())
                    {
                        if (!(await _productTagService.GetAllProductTagsByProductIdAsync(productId)).Where(x => x.Id == tagExists.Id).Any())
                        {
                            productTag.ProductId = productId;
                            productTag.ProductTagId = tagExists.Id;
                            await _productTagService.InsertProductProductTagMappingAsync(productTag);
                        }
                    }
                    else
                    {
                        productTag.ProductId = productId;
                        productTag.ProductTagId = tagExists.Id;
                        await _productTagService.InsertProductProductTagMappingAsync(productTag);
                    }
                }
            }
        }

        /// <summary>
        /// Manage Inventory
        /// </summary>
        /// <param name="inventory">inventory</param>
        /// <param name="productId">productId</param>
        /// <param name="update">update</param>
        public async Task ManageProductInventoryAsync(Dictionary<string, int> inventory, int productId, bool update)
        {
            if (update)
            {
                var productWarehouses = await _productService.GetAllProductWarehouseInventoryRecordsAsync(productId);
                foreach (var productWarehouse in productWarehouses)
                {
                    await _productService.DeleteProductWarehouseInventoryAsync(productWarehouse);
                }
            }

            var warehouse = new Warehouse();
            var productWarehouseInventory = new ProductWarehouseInventory();

            foreach (var item in inventory)
            {
                var warehouseinfo = await _ffmMProductServices.GetWarehousesByName(item.Key);
                if (warehouseinfo == null)
                {
                    warehouse = new Warehouse();

                    warehouse.Name = item.Key;
                    await _shippingService.InsertWarehouseAsync(warehouse);

                    productWarehouseInventory = new ProductWarehouseInventory();

                    productWarehouseInventory.ProductId = productId;
                    productWarehouseInventory.WarehouseId = warehouse.Id;
                    productWarehouseInventory.StockQuantity = item.Value;
                    productWarehouseInventory.ReservedQuantity = 0;
                    await _productService.InsertProductWarehouseInventoryAsync(productWarehouseInventory);
                }
                else
                {
                    productWarehouseInventory = new ProductWarehouseInventory();

                    productWarehouseInventory.ProductId = productId;
                    productWarehouseInventory.WarehouseId = warehouseinfo.Id;
                    productWarehouseInventory.StockQuantity = item.Value;
                    productWarehouseInventory.ReservedQuantity = 0;
                    await _productService.InsertProductWarehouseInventoryAsync(productWarehouseInventory);
                }
            }
        }

        /// <summary>
        /// Manage product specification attributes
        /// </summary>
        /// <param name="claims">claims</param>
        /// <param name="productId">productId</param>
        /// <param name="update">update</param>
        public async Task ManageProductSpecificationAttributesAsync(Dictionary<string, string> claims, int productId)
        {
            var specificationAttributeGroup = new SpecificationAttributeGroup();
            var specificationAttribute = new SpecificationAttribute();
            var specificationAttributeOptions = new SpecificationAttributeOption();
            var productSpecificationAttribute = new ProductSpecificationAttribute();
            int specificationAttributeGroupId = 0;

            var cliamExists = await _ffmMProductServices.GetSpecificationAttributeGroupByName("Claims");

            if (cliamExists == null)
            {
                specificationAttributeGroup.Name = "Claims";
                await _specificationAttributeService.InsertSpecificationAttributeGroupAsync(specificationAttributeGroup);
                specificationAttributeGroupId = specificationAttributeGroup.Id;
            }
            else
            {
                specificationAttributeGroupId = cliamExists.Id;
            }

            foreach (var cliam in claims)
            {
                var specificationAttributes = await _ffmMProductServices.GetSpecificationAttributeByName(cliam.Key);
                int specificationAttributesId = 0;

                if (specificationAttributes == null)
                {
                    specificationAttribute = new SpecificationAttribute();

                    specificationAttribute.Name = cliam.Key;
                    specificationAttribute.SpecificationAttributeGroupId = specificationAttributeGroupId;

                    await _specificationAttributeService.InsertSpecificationAttributeAsync(specificationAttribute);

                    specificationAttributesId = specificationAttribute.Id;
                }
                else
                {
                    specificationAttributesId = specificationAttributes.Id;
                }

                var specificationAttributeOption = await _ffmMProductServices.GetSpecificationAttributeOptionByNameAndSpecificationAttributeId(cliam.Value, specificationAttributesId);
                int specificationAttributeOptionId = 0;

                if (specificationAttributeOption == null)
                {
                    specificationAttributeOptions = new SpecificationAttributeOption();

                    specificationAttributeOptions.Name = cliam.Value;
                    specificationAttributeOptions.SpecificationAttributeId = specificationAttributesId;

                    await _specificationAttributeService.InsertSpecificationAttributeOptionAsync(specificationAttributeOptions);

                    specificationAttributeOptionId = specificationAttributeOptions.Id;
                }
                else
                {
                    specificationAttributeOptionId = specificationAttributeOption.Id;
                }

                productSpecificationAttribute = new ProductSpecificationAttribute();

                productSpecificationAttribute.ProductId = productId;
                productSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOptionId;
                productSpecificationAttribute.ShowOnProductPage = true;
                productSpecificationAttribute.AllowFiltering = true;

                await _specificationAttributeService.InsertProductSpecificationAttributeAsync(productSpecificationAttribute);
            }
        }

        /// <summary>
        /// Manage product attributes
        /// </summary>
        /// <param name="size">size</param>
        /// <param name="productId">productId</param>
        /// <param name="update">update</param>
        public async Task ManageProductAttributesAsync(string size, int productId)
        {
            var attribute = await _ffmMProductServices.GetProductAttributeByName("Size");
            var productAttribute = new ProductAttribute();
            var productAttributeMapping = new ProductAttributeMapping();
            var productAttributeValue = new ProductAttributeValue();

            int attributeId = 0;

            if (attribute == null)
            {
                productAttribute = new ProductAttribute();

                productAttribute.Name = "Size";
                await _productAttributeService.InsertProductAttributeAsync(productAttribute);
                attributeId = productAttribute.Id;
            }
            else
            {
                attributeId = attribute.Id;
            }

            productAttributeMapping.ProductAttributeId = attributeId;
            productAttributeMapping.AttributeControlTypeId = 1;
            productAttributeMapping.IsRequired = true;
            productAttributeMapping.ProductId = productId;

            await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMapping);

            productAttributeValue.Name = size;
            productAttributeValue.ProductAttributeMappingId = productAttributeMapping.Id;

            await _productAttributeService.InsertProductAttributeValueAsync(productAttributeValue);
        }

        /// <summary>
        /// Is product updated
        /// </summary>
        /// <param name="product"></param>
        /// <param name="apiProduct"></param>
        /// <returns>true or false</returns>
        public bool IsProductUpdated(Product product, ApiProduct apiProduct)
        {
            bool isProductUpdated = false;

            if (!product.Name.Equals(apiProduct.ProductName, StringComparison.OrdinalIgnoreCase))
                isProductUpdated = true;

            if (!product.FullDescription.Equals(apiProduct.Description, StringComparison.OrdinalIgnoreCase))
                isProductUpdated = true;

            return isProductUpdated;
        }

        public async Task InsertProductIngredients(int productId, string ingredients)
        {
            if (productId == 0)
            {
                await _logger.ErrorAsync(string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.ProductIngredients"), productId));
            }
            var productInfomodel = new ProductAdditionalInfo();
            {
                productInfomodel.ProductId = productId;
                productInfomodel.IngredientsInfomation = ingredients;
            }
            await _productAdditionalInfoService.InsertProductAdditionalInfoAsync(productInfomodel);
        }

        public async Task UpdateProductIngredients(int productId, string ingredients)
        {
            if (productId == 0)
            {
                await _logger.ErrorAsync(string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.UpdateProductIngredients"), productId));
            }
            var productadditionalinfo = await _productAdditionalInfoService.GetProductAdditionalInfoByIdAsync(productId);
            if (productadditionalinfo == null)
            {
                var productInfomodel = new ProductAdditionalInfo();
                {
                    productInfomodel.ProductId = productId;
                    productInfomodel.IngredientsInfomation = ingredients;
                }
                await _productAdditionalInfoService.InsertProductAdditionalInfoAsync(productadditionalinfo);
            }
            else
            {
                productadditionalinfo.ProductId = productId;
                productadditionalinfo.IngredientsInfomation = ingredients;
                await _productAdditionalInfoService.UpdateProductAdditionalInfoAsync(productadditionalinfo);
            }
        }
    }
    #endregion

}
