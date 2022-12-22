using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Customer;
using System.Threading.Tasks;

namespace XcellenceIt.Plugin.Misc.UniversalOnePageCheckout.Controllers
{
    public class ShippingAddressController : BasePluginController
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;

        public ShippingAddressController(
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IAddressModelFactory addressModelFactory,
            ICustomerService customerService,
            ICountryService countryService,
            AddressSettings addressSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressService addressService)
        {
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _storeContext = storeContext;
            _addressModelFactory = addressModelFactory;
            _customerService = customerService;
            _countryService = countryService;
            _addressSettings = addressSettings;
            _addressAttributeParser = addressAttributeParser;
            _addressService = addressService;
        }

        public virtual async Task<IActionResult> EditAddress(int selectedId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var address = await _customerService.GetCustomerAddressAsync(customer.Id, selectedId);

            var model = new CustomerAddressEditModel();
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));
            var productHtml = await RenderPartialViewToStringAsync("AddressEditPopup", model);
            return Json(new
            {
                Result = true,
                htmldata = productHtml
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddressEdit(CustomerAddressEditModel model, int addressId, IFormCollection form)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            //find address (ensure that it belongs to the current customer)
            var address = await _customerService.GetCustomerAddressAsync(customer.Id, model.Address.Id);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address.FirstName = model.Address.FirstName;
                address.LastName = model.Address.LastName;
                address.Email = model.Address.Email;
                address.Company = model.Address.Company;
                address.CountryId = model.Address.CountryId;
                address.StateProvinceId = model.Address.StateProvinceId;
                address.City = model.Address.City;
                address.Address1 = model.Address.Address1;
                address.Address2 = model.Address.Address2;
                address.ZipPostalCode = model.Address.ZipPostalCode;
                address.PhoneNumber = model.Address.PhoneNumber;
                address.FaxNumber = model.Address.FaxNumber;


                address.CustomAttributes = customAttributes;
                await _addressService.UpdateAddressAsync(address);

                var shippingAddressModel = new CustomerAddressEditModel();
                await _addressModelFactory.PrepareAddressModelAsync(shippingAddressModel.Address,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));

                return Json(new
                {
                    Results = true,
                    html = await RenderPartialViewToStringAsync("UpdateShippingAddress", shippingAddressModel),
                    addressId = shippingAddressModel.Address.Id
                });
            }

            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));

            return Json(new
            {
                Results = false,
                html = await RenderPartialViewToStringAsync("AddressEditPopup", model),
            });

        }
    }
}
