using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Factories;
using XcellenceIt.Plugins.Misc.FFM.Models.Company;
using XcellenceIt.Plugins.Misc.FFM.Services.Companies;

namespace XcellenceIt.Plugins.Misc.FFM.Controllers
{
    public class CompanyController : BaseAdminController
    {
        #region Fields
        private readonly CompanyModelFactory _companyModelFactory;
        private readonly CompanyService _companyService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerModelFactory _customerModelFactory;
        #endregion

        #region Ctor
        public CompanyController(CompanyModelFactory companyModelFactory,
            CompanyService companyService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ICustomerService customerService, ICustomerModelFactory customerModelFactory)
        {
            _companyModelFactory = companyModelFactory;
            _companyService = companyService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _customerService = customerService;
            _customerModelFactory = customerModelFactory;
        }
        #endregion

        #region Method
        public async Task<IActionResult> List()
        {
            //prepare model
            var model = await _companyModelFactory.PrepareCompanySearchModelAsync(new CompanySearchModel());

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> List(CompanySearchModel searchModel)
        {
            //prepare model
            var model = await _companyModelFactory.PrepareCompanytListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {

            //prepare model
            var model = await _companyModelFactory.PrepareCompanyModelAsync(new CompanyModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CompanyModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var customerRole = new CustomerRole();
                customerRole.Name = model.Name;
                customerRole.SystemName = model.Name;
                customerRole.Active = true;
                await _customerService.InsertCustomerRoleAsync(customerRole);

                var company = new Company();
                company.Name = model.Name;
                company.Point = model.Point;
                company.CustomerRoleId = customerRole.Id;
                company.SubsidizedCode = model.SubsidizedCode;
                await _companyService.InsertCompanyAsync(company);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = company.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _companyModelFactory.PrepareCompanyModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get a store with the specified id
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _companyModelFactory.PrepareCompanyModelAsync(null, company);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(CompanyModel model, bool continueEditing)
        {
            //try to get a store with the specified id
            var company = await _companyService.GetCompanyByIdAsync(model.Id);
            if (company == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                company.Point = model.Point;
                company.SubsidizedCode = model.SubsidizedCode;
                await _companyService.UpdateCompanyAsync(company);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = company.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _companyModelFactory.PrepareCompanyModelAsync(model, company, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a manufacturer with the specified id
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
                return RedirectToAction("List");

            var customerRole = await _customerService.GetCustomerRoleByIdAsync(company.CustomerRoleId);
            if (customerRole != null)
                await _customerService.DeleteCustomerRoleAsync(customerRole);

            await _companyService.DeleteCompanyAsync(company);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Deleted"));

            return RedirectToAction("List");
        }


        [HttpPost]
        public virtual async Task<IActionResult> CompanyCustomerList(CompanyCustomerSearchModel searchModel)
        {
            //try to get a product with the specified id
            var company = await _companyService.GetCompanyByIdAsync(searchModel.CompanyId)
                ?? throw new ArgumentException("No company found with the specified id");

            //prepare model
            var model = await _companyModelFactory.PrepareCompanyCustomerListModelAsync(searchModel, company);

            return Json(model);
        }

        public virtual async Task<IActionResult> CustomerAddPopup(int companyId)
        {
            //prepare model
            var model = await _customerModelFactory.PrepareCustomerSearchModelAsync(new CustomerSearchModel());

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> CustomerAddPopup(AddCustomerToCompanyModel model)
        {
            //try to get a manufacturer with the specified id
            var company = await _companyService.GetCompanyByIdAsync(model.CompanyId);
            if (company == null)
                return Content(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Errors.CompanyNotFound"));

            var customerRole = await _customerService.GetCustomerRoleByIdAsync(company.CustomerRoleId); 
            if (customerRole == null)
                return Content(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Errors.CustomerRoleNotFound"));

            var selectedCustomers = await _customerService.GetCustomersByIdsAsync(model.SelectedCustomerIds.ToArray());
            if (selectedCustomers.Any())
            {
                var customerRoleIds = new[] { company.CustomerRoleId };
                var existingCompanyCustomers = await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds);
                foreach (var customer in selectedCustomers)
                {
                    //whether product manufacturer with such parameters already exists
                    if (_companyService.FindCompanyCustomers(existingCompanyCustomers, customer.Id) != null)
                        continue;

                    await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                }
            }

            ViewBag.RefreshPage = true;
            return View(new CustomerSearchModel());
        }

        public virtual async Task<IActionResult> DeleteCustomerRole(int customerId, int companyId)
        {
            try
            {
                //try to get a manufacturer with the specified id
                var company = await _companyService.GetCompanyByIdAsync(companyId);
                if (company == null)
                    return Json(new { Result = false, Message = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Errors.CompanyNotFound") });

                var customerRole = await _customerService.GetCustomerRoleByIdAsync(company.CustomerRoleId);
                if (customerRole == null)
                    return Json(new { Result = false, Message = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Errors.CustomerRoleNotFound") });

                var customer = await _customerService.GetCustomerByIdAsync(customerId);
                if (customer == null)
                    return Json(new { Result = false, Message = await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Errors.CustomerNotFound") });

                await _customerService.RemoveCustomerRoleMappingAsync(customer, customerRole);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
            
        }

        #endregion
    }
}
