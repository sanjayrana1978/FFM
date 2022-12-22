using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;

namespace XcellenceIt.Plugins.Misc.FFM.Services.Companies
{
    public class CompanyService
    {
        #region Fields

        private readonly IRepository<Company> _companyRepository;
        private readonly ICustomerService _customerService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public CompanyService(IRepository<Company> companyRepository,
            ICustomerService customerService,
            IRewardPointService rewardPointService,
            ILocalizationService localizationService)
        {
            _companyRepository = companyRepository;
            _customerService = customerService;
            _rewardPointService = rewardPointService;
            _localizationService = localizationService;
        }
        #endregion

        #region Method

        public async Task<IPagedList<Company>> GetAllCompaniessAsync(string keywords = null, int pageIndex = 0,
            int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var companies = await _companyRepository.GetAllPagedAsync(query =>
            {
                if (!string.IsNullOrEmpty(keywords))
                    query = query.Where(x => x.SubsidizedCode.Equals(keywords));

                    return query;

            }, pageIndex, pageSize, getOnlyTotalCount);

            return companies;
        }

        public async Task<Company> GetCompanyByIdAsync(int companyId)
        {
            return await _companyRepository.GetByIdAsync(companyId, cache => default);
        }
        
        public async Task<Company> GetCompanyBySubsidizedCodeAsync(string keywords)
        {
            var query = await (from c in _companyRepository.Table
                        where c.SubsidizedCode == keywords
                        select c).FirstOrDefaultAsync();
            return query;
        }

        public async Task InsertCompanyAsync(Company company)
        {
            await _companyRepository.InsertAsync(company);
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            await _companyRepository.UpdateAsync(company);
        }

        public async Task DeleteCompanyAsync(Company company)
        {
            await _companyRepository.DeleteAsync(company);
        }

        public virtual Customer FindCompanyCustomers(IList<Customer> source, int customerId)
        {
            foreach (var companyCustomer in source)
                if (companyCustomer.Id == customerId)
                    return companyCustomer;

            return null;
        }

        public virtual async Task<IList<Company>> GetCompanyCustomerRoleId(int[] customerRoleId)
        {
            var query = (from a in _companyRepository.Table
                        where customerRoleId.Contains(a.CustomerRoleId)
                        select a).ToList();

            return query;
        }

        public async Task RewardPointAddAsync()
        {
            DateTime currentDate = DateTime.UtcNow;

            var startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            if(currentDate.Day == startDate.Day)
            {
                var companies = await (await GetAllCompaniessAsync()).ToListAsync();
                foreach (var company in companies)
                {
                    var customerRoleIds = new[] { company.CustomerRoleId };
                    var customers = await (await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds)).ToListAsync();

                    foreach (var customer in customers)
                    {
                        await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, company.Point,
                        customer.RegisteredInStoreId, string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.SchedularTask.AddRewardPoint.Message"),company.Name), activatingDate: startDate, endDate: endDate);
                    }
                }
            }
            else if (currentDate.Day == endDate.Day)
            {
                var companies = await (await GetAllCompaniessAsync()).ToListAsync();
                foreach (var company in companies)
                {
                    var customerRoleIds = new[] { company.CustomerRoleId };
                    var customers = await (await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds)).ToListAsync();

                    foreach (var customer in customers)
                    {
                        var rewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, customer.RegisteredInStoreId);

                        if(rewardPointsBalance != 0)
                            await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, -rewardPointsBalance,
                            customer.RegisteredInStoreId, string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.SchedularTask.RemoveRewardPoint.Message"), company.Name));
                    }
                }
            }
        }
        
        #endregion
    }
}
