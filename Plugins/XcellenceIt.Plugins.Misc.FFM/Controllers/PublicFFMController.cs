using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Services.Companies;

namespace XcellenceIt.Plugins.Misc.FFM.Controllers
{
    public class PublicFFMController : BasePublicController
    {
        #region Fields

        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IStoreContext _storeContext;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IProductService _productService;
        private readonly IPermissionService _permissionService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IMeasureService _measureService;
        private readonly ICategoryService _categoryService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWebHelper _webHelper;
        private readonly IUrlRecordService _urlRecordService;
        private readonly CompanyService _companyService;
        private readonly ILocalizationService _localizationService;

        public PublicFFMController(IShoppingCartService shoppingCartService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IStoreContext storeContext,
            ICheckoutAttributeService checkoutAttributeService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            IProductService productService,
            IPermissionService permissionService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IMeasureService measureService,
            ICategoryService categoryService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IWebHelper webHelper,
            IUrlRecordService urlRecordService,
            CompanyService companyService,
            ILocalizationService localizationService)
        {
            _shoppingCartService = shoppingCartService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _storeContext = storeContext;
            _checkoutAttributeService = checkoutAttributeService;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _productService = productService;
            _permissionService = permissionService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _measureService = measureService;
            _categoryService = categoryService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _webHelper = webHelper;
            _urlRecordService = urlRecordService;
            _companyService = companyService;
            _localizationService = localizationService;
        }

        #endregion

        protected virtual async Task ParseAndSaveCheckoutAttributesAsync(IList<ShoppingCartItem> cart, IFormCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var excludeShippableAttributes = !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var store = await _storeContext.GetCurrentStoreAsync();
            var checkoutAttributes = await _checkoutAttributeService.GetAllCheckoutAttributesAsync(store.Id, excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var controlId = $"checkout_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = await _checkoutAttributeService.GetCheckoutAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }

                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
                            }
                            catch
                            {
                                // ignored
                            }

                            if (selectedDate.HasValue)
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                        }

                        break;
                    case AttributeControlType.FileUpload:
                        {
                            _ = Guid.TryParse(form[controlId], out var downloadGuid);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            //validate conditional attributes (if specified)
            foreach (var attribute in checkoutAttributes)
            {
                var conditionMet = await _checkoutAttributeParser.IsConditionMetAsync(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                    attributesXml = _checkoutAttributeParser.RemoveCheckoutAttribute(attributesXml, attribute);
            }

            //save checkout attributes
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.CheckoutAttributes, attributesXml, store.Id);
        }

        /// <summary>
        /// Render component to string
        /// </summary>
        /// <param name="componentName">Component name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns>Result</returns>

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UpdateShoppingCart(IFormCollection form, bool isEditable)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("Homepage");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //get identifiers of items to remove
            var itemIdsToRemove = form["removefromcart"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            var products = (await _productService.GetProductsByIdsAsync(cart.Select(item => item.ProductId).Distinct().ToArray()))
                .ToDictionary(item => item.Id, item => item);

            //get order items with changed quantity
            var itemsWithNewQuantity = cart.Select(item => new
            {
                //try to get a new quantity for the item, set 0 for items to remove
                NewQuantity = itemIdsToRemove.Contains(item.Id) ? 0 : int.TryParse(form[$"itemquantity{item.Id}"], out var quantity) ? quantity : item.Quantity,
                Item = item,
                Product = products.ContainsKey(item.ProductId) ? products[item.ProductId] : null
            }).Where(item => item.NewQuantity != item.Item.Quantity);

            //order cart items
            //first should be items with a reduced quantity and that require other products; or items with an increased quantity and are required for other products
            var orderedCart = await itemsWithNewQuantity
                .OrderByDescendingAwait(async cartItem =>
                    (cartItem.NewQuantity < cartItem.Item.Quantity &&
                     (cartItem.Product?.RequireOtherProducts ?? false)) ||
                    (cartItem.NewQuantity > cartItem.Item.Quantity && cartItem.Product != null && (await _shoppingCartService
                         .GetProductsRequiringProductAsync(cart, cartItem.Product)).Any()))
                .ToListAsync();

            //try to update cart items with new quantities and get warnings
            var warnings = await orderedCart.SelectAwait(async cartItem => new
            {
                ItemId = cartItem.Item.Id,
                Warnings = await _shoppingCartService.UpdateShoppingCartItemAsync(customer,
                    cartItem.Item.Id, cartItem.Item.AttributesXml, cartItem.Item.CustomerEnteredPrice,
                    cartItem.Item.RentalStartDateUtc, cartItem.Item.RentalEndDateUtc, cartItem.NewQuantity, true)
            }).ToListAsync();

            //updated cart
            cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            //prepare model
            var model = new ShoppingCartModel();
            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

            //update current warnings
            foreach (var warningItem in warnings.Where(warningItem => warningItem.Warnings.Any()))
            {
                //find shopping cart item model to display appropriate warnings
                var itemModel = model.Items.FirstOrDefault(item => item.Id == warningItem.ItemId);
                if (itemModel != null)
                    itemModel.Warnings = warningItem.Warnings.Concat(itemModel.Warnings).Distinct().ToList();
            }
            //update blocks
            var ordetotalssectionhtml = await RenderViewComponentToStringAsync("OrderTotals", new { isEditable });

            var shoppingcarthtml = await RenderPartialViewToStringAsync("UpdateCart", model);
            var cartEmpty = false;
            if (model.Items.Count() == 0)
            {
                cartEmpty = true;
            }
            return Json(new
            {
                ordetotalssectionhtml,
                shoppingcarthtml,
                cartEmpty
            });

        }

        [HttpsRequirement]
        ////available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        // This method render OTP page when user click on link provided in email
        public virtual async Task<IActionResult> RedirectToCategory(int categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            var seName = await _urlRecordService.GetSeNameAsync(category);
            return View("~/Plugins/Misc.FFM/Themes/Emporium/Views/Category/RedirectToCategory.cshtml", seName);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SubsidizedCode(string refferaltext)
        {
            if (string.IsNullOrWhiteSpace(refferaltext))
                return Content("");

            var success = false;

            refferaltext = refferaltext.Trim();

            var company = await _companyService.GetCompanyBySubsidizedCodeAsync(refferaltext);
            var statusText = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.SubsidizedCode.Unavailable");
            if (company != null)
            {
                statusText = string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.SubsidizedCode.Available"), company.Name);
                success = true;
            }
            return Json(new
            {
                Text = statusText,
                Success = success
            });
        }
    }
}
