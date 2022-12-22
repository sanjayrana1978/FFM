using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Models.Company;
using XcellenceIt.Plugins.Misc.FFM.Services.Companies;

namespace XcellenceIt.Plugins.Misc.FFM.Factories
{
    public class CompanyModelFactory
    {
        #region Fields

        private readonly CompanyService _companyService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Ctor
        public CompanyModelFactory(CompanyService companyService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IEncryptionService encryptionService)
        {
            _companyService = companyService;
            _customerService = customerService;
            _localizationService = localizationService;
            _encryptionService = encryptionService;
        }
        #endregion

        #region Method
        public virtual Task<CompanySearchModel> PrepareCompanySearchModelAsync(CompanySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<CompanyListModel> PrepareCompanytListModelAsync(CompanySearchModel searchModel)
        {
            var companies = await _companyService.GetAllCompaniessAsync(pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);
            var model = await new CompanyListModel().PrepareToGridAsync(searchModel, companies, () =>
            {
                return companies.SelectAwait(async company =>
                {
                    return new CompanyModel
                    {
                        Id = company.Id,
                        Name = company.Name,
                        Point = company.Point,
                        CustomerRole = (await _customerService.GetCustomerRoleByIdAsync(company.CustomerRoleId))?.Name,
                        SubsidizedCode = company.SubsidizedCode
                    };
                });
            });

            return model;
        }

        public Task<CompanyModel> PrepareCompanyModelAsync(CompanyModel model, Company company, bool excludeProperties = false)
        {
            if (company != null)
            {
                if (model == null)
                {
                    model = new CompanyModel();
                    model.Id = company.Id;
                    model.Name = company.Name;
                    model.Point = company.Point;
                    model.SubsidizedCode = company.SubsidizedCode;
                }
                //prepare nested search model
                PrepareCompanyCustomerSearchModel(model.CompanyCustomerSearchModel, company);
            }
            return Task.FromResult(model);
        }

        protected virtual CompanyCustomerSearchModel PrepareCompanyCustomerSearchModel(CompanyCustomerSearchModel searchModel, Company company)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (company == null)
                throw new ArgumentNullException(nameof(company));

            searchModel.CompanyId = company.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual async Task<CustomerListModel> PrepareCompanyCustomerListModelAsync(CompanyCustomerSearchModel searchModel, Company company)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (company == null)
                throw new ArgumentNullException(nameof(company));

            var customerRoleIds = new[] { company.CustomerRoleId };
            var companyCustomers = (await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new CustomerListModel().PrepareToGridAsync(searchModel, companyCustomers, () =>
            {
                return companyCustomers.SelectAwait(async companyCustomer =>
                {
                    //fill in model values from the entity
                    return new CustomerModel {
                        Id = companyCustomer.Id,
                        Email = companyCustomer.Email,
                  //Email = (await _customerService.IsRegisteredAsync(companyCustomer))
                  //      ? _encryptionService.DecryptText(companyCustomer.Email, "4802407001667285")
                  //          : await _localizationService.GetResourceAsync("Admin.Customers.Guest"),
                  FullName = await _customerService.GetCustomerFullNameAsync(companyCustomer)
                    
                };
                });
            });

            return model;
        }

        #endregion
    }
}
