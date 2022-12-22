﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Factories
{
    public class OverrideAddressModelFactory : AddressModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ICountryService _countryService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IWorkContext _workContext;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Ctor

        public OverrideAddressModelFactory(AddressSettings addressSettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            ICountryService countryService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            IWorkContext workContext,
            IEncryptionService encryptionService) : base(addressSettings,
                addressAttributeFormatter,
                addressAttributeParser,
                addressAttributeService,
                countryService,
                genericAttributeService,
                localizationService,
                stateProvinceService,
                workContext)
        {
            _addressSettings = addressSettings;
            _addressAttributeFormatter = addressAttributeFormatter;
            _addressAttributeParser = addressAttributeParser;
            _addressAttributeService = addressAttributeService;
            _countryService = countryService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _stateProvinceService = stateProvinceService;
            _workContext = workContext;
            _encryptionService = encryptionService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address attributes
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address entity</param>
        /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of address entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task PrepareCustomAddressAttributesAsync(AddressModel model,
            Address address, string overrideAttributesXml = "")
        {
            var attributes = await _addressAttributeService.GetAllAddressAttributesAsync();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _addressAttributeService.GetAddressAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = _encryptionService.DecryptText(!string.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    address?.CustomAttributes, "4802407001667285");
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = await _addressAttributeParser.ParseAddressAttributeValuesAsync(selectedAddressAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                var enteredText = _addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address entity</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="addressSettings">Address settings</param>
        /// <param name="loadCountries">Countries loading function; pass null if countries do not need to load</param>
        /// <param name="prePopulateWithCustomerFields">Whether to populate model properties with the customer fields (used with the customer entity)</param>
        /// <param name="customer">Customer entity; required if prePopulateWithCustomerFields is true</param>
        /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of the address entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task PrepareAddressModelAsync(AddressModel model,
            Address address, bool excludeProperties,
            AddressSettings addressSettings,
            Func<Task<IList<Country>>> loadCountries = null,
            bool prePopulateWithCustomerFields = false,
            Customer customer = null,
            string overrideAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (addressSettings == null)
                throw new ArgumentNullException(nameof(addressSettings));

            if (!excludeProperties && address != null)
            {
                model.Id = address.Id;
                model.FirstName = _encryptionService.DecryptText(address.FirstName, "4802407001667285");
                model.LastName = _encryptionService.DecryptText(address.LastName, "4802407001667285");
                model.Email = _encryptionService.DecryptText(address.Email, "4802407001667285");
                model.Company = _encryptionService.DecryptText(address.Company, "4802407001667285");
                model.CountryId = address.CountryId;
                model.CountryName = await _countryService.GetCountryByAddressAsync(address) is Country country ? await _localizationService.GetLocalizedAsync(country, x => x.Name) : null;
                model.StateProvinceId = address.StateProvinceId;
                model.StateProvinceName = await _stateProvinceService.GetStateProvinceByAddressAsync(address) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name) : null;
                model.County = _encryptionService.DecryptText(address.County, "4802407001667285");
                model.City = _encryptionService.DecryptText(address.City, "4802407001667285");
                model.Address1 = _encryptionService.DecryptText(address.Address1, "4802407001667285");
                model.Address2 = _encryptionService.DecryptText(address.Address2, "4802407001667285");
                model.ZipPostalCode = _encryptionService.DecryptText(address.ZipPostalCode, "4802407001667285");
                model.PhoneNumber = _encryptionService.DecryptText(address.PhoneNumber, "4802407001667285");
                model.FaxNumber = _encryptionService.DecryptText(address.FaxNumber, "4802407001667285");
            }

            if (address == null && prePopulateWithCustomerFields)
            {
                if (customer == null)
                    throw new Exception("Customer cannot be null when prepopulating an address");
                model.Email = _encryptionService.DecryptText(customer.Email, "4802407001667285");
                model.FirstName = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute), "4802407001667285");
                model.LastName = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute), "4802407001667285");
                model.Company = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CompanyAttribute), "4802407001667285");
                model.Address1 = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddressAttribute), "4802407001667285");
                model.Address2 = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddress2Attribute), "4802407001667285");
                model.ZipPostalCode = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute), "4802407001667285");
                model.City = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CityAttribute), "4802407001667285");
                model.County = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CountyAttribute), "4802407001667285");
                model.PhoneNumber = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute), "4802407001667285");
                model.FaxNumber = _encryptionService.DecryptText(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FaxAttribute), "4802407001667285");
            }

            //countries and states
            if (addressSettings.CountryEnabled && loadCountries != null)
            {
                var countries = await loadCountries();

                if (_addressSettings.PreselectCountryIfOnlyOne && countries.Count == 1)
                {
                    model.CountryId = countries[0].Id;
                }
                else
                {
                    model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
                }

                foreach (var c in countries)
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (addressSettings.StateProvinceEnabled)
                {
                    var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
                    var states = (await _stateProvinceService
                        .GetStateProvincesByCountryIdAsync(model.CountryId ?? 0, languageId))
                        .ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem
                            {
                                Text = await _localizationService.GetLocalizedAsync(s, x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = (s.Id == model.StateProvinceId)
                            });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }
                }
            }

            //form fields
            model.CompanyEnabled = addressSettings.CompanyEnabled;
            model.CompanyRequired = addressSettings.CompanyRequired;
            model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
            model.CityEnabled = addressSettings.CityEnabled;
            model.CityRequired = addressSettings.CityRequired;
            model.CountyEnabled = addressSettings.CountyEnabled;
            model.CountyRequired = addressSettings.CountyRequired;
            model.CountryEnabled = addressSettings.CountryEnabled;
            model.StateProvinceEnabled = addressSettings.StateProvinceEnabled;
            model.PhoneEnabled = addressSettings.PhoneEnabled;
            model.PhoneRequired = addressSettings.PhoneRequired;
            model.FaxEnabled = addressSettings.FaxEnabled;
            model.FaxRequired = addressSettings.FaxRequired;

            //customer attribute services
            if (_addressAttributeService != null && _addressAttributeParser != null)
            {
                await PrepareCustomAddressAttributesAsync(model, address, overrideAttributesXml);
            }
            if (_addressAttributeFormatter != null && address != null)
            {
                model.FormattedCustomAddressAttributes = await _addressAttributeFormatter.FormatAttributesAsync(address.CustomAttributes);
            }
        }

        #endregion
    }
}
