using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Factories
{
    internal class OverrrideShoppingCartModelFactory : ShoppingCartModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Ctor

        public OverrrideShoppingCartModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStoreService storeService,
            ITaxService taxService,
            IEncryptionService encryptionService) : base(catalogSettings,
                baseAdminModelFactory,
                countryService,
                customerService,
                dateTimeHelper,
                localizationService,
                priceFormatter,
                productAttributeFormatter,
                productService,
                shoppingCartService,
                storeService,
                taxService)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _storeService = storeService;
            _taxService = taxService;
            _encryptionService = encryptionService;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Prepare paged shopping cart list model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart list model
        /// </returns>
        public override async Task<ShoppingCartListModel> PrepareShoppingCartListModelAsync(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customers with shopping carts
            var customers = await _customerService.GetCustomersWithShoppingCartsAsync(searchModel.ShoppingCartType,
                storeId: searchModel.StoreId,
                productId: searchModel.ProductId,
                createdFromUtc: searchModel.StartDate,
                createdToUtc: searchModel.EndDate,
                countryId: searchModel.BillingCountryId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new ShoppingCartListModel().PrepareToGridAsync(searchModel, customers, () =>
            {
                return customers.SelectAwait(async customer =>
                {
                    //fill in model values from the entity
                    var shoppingCartModel = new ShoppingCartModel
                    {
                        CustomerId = customer.Id
                    };

                    //fill in additional values (not existing in the entity)
                    shoppingCartModel.CustomerEmail = (await _customerService.IsRegisteredAsync(customer))
                        ? _encryptionService.DecryptText(customer.Email, "4802407001667285")
                        : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                    shoppingCartModel.TotalItems = (await _shoppingCartService
                        .GetShoppingCartAsync(customer, searchModel.ShoppingCartType,
                            searchModel.StoreId, searchModel.ProductId, searchModel.StartDate, searchModel.EndDate))
                        .Sum(item => item.Quantity);

                    return shoppingCartModel;
                });
            });

            return model;
        }

        #endregion
    }
}
