using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services.OverrideServices
{
    public class CustomCustomerService : CustomerService
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly INopDataProvider _dataProvider;
        private readonly IRepository<Address> _customerAddressRepository;
        private readonly IRepository<BlogComment> _blogCommentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerAddressMapping> _customerAddressMappingRepository;
        private readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
        private readonly IRepository<CustomerPassword> _customerPasswordRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<ForumPost> _forumPostRepository;
        private readonly IRepository<ForumTopic> _forumTopicRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IRepository<NewsComment> _newsCommentRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IRepository<ProductReviewHelpfulness> _productReviewHelpfulnessRepository;
        private readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;
        private readonly IRepository<ShoppingCartItem> _shoppingCartRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Ctor

        public CustomCustomerService(CustomerSettings customerSettings,
             IGenericAttributeService genericAttributeService,
             INopDataProvider dataProvider,
             IRepository<Address> customerAddressRepository,
             IRepository<BlogComment> blogCommentRepository,
             IRepository<Customer> customerRepository,
             IRepository<CustomerAddressMapping> customerAddressMappingRepository,
             IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
             IRepository<CustomerPassword> customerPasswordRepository,
             IRepository<CustomerRole> customerRoleRepository,
             IRepository<ForumPost> forumPostRepository,
             IRepository<ForumTopic> forumTopicRepository,
             IRepository<GenericAttribute> gaRepository,
             IRepository<NewsComment> newsCommentRepository,
             IRepository<Order> orderRepository,
             IRepository<ProductReview> productReviewRepository,
             IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
             IRepository<PollVotingRecord> pollVotingRecordRepository,
             IRepository<ShoppingCartItem> shoppingCartRepository,
             IStaticCacheManager staticCacheManager,
             IStoreContext storeContext,
             ShoppingCartSettings shoppingCartSettings,
             IEncryptionService encryptionService) : base(customerSettings,
                genericAttributeService,
                dataProvider,
                customerAddressRepository,
                blogCommentRepository,
                customerRepository,
                customerAddressMappingRepository,
                customerCustomerRoleMappingRepository,
                customerPasswordRepository,
                customerRoleRepository,
                forumPostRepository,
                forumTopicRepository,
                gaRepository,
                newsCommentRepository,
                orderRepository,
                productReviewRepository,
                productReviewHelpfulnessRepository,
                pollVotingRecordRepository,
                shoppingCartRepository,
                staticCacheManager,
                storeContext,
                shoppingCartSettings)
        {
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _dataProvider = dataProvider;
            _customerAddressRepository = customerAddressRepository;
            _blogCommentRepository = blogCommentRepository;
            _customerRepository = customerRepository;
            _customerAddressMappingRepository = customerAddressMappingRepository;
            _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
            _customerPasswordRepository = customerPasswordRepository;
            _customerRoleRepository = customerRoleRepository;
            _forumPostRepository = forumPostRepository;
            _forumTopicRepository = forumTopicRepository;
            _gaRepository = gaRepository;
            _newsCommentRepository = newsCommentRepository;
            _orderRepository = orderRepository;
            _productReviewRepository = productReviewRepository;
            _productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
            _pollVotingRecordRepository = pollVotingRecordRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _shoppingCartSettings = shoppingCartSettings;
            _encryptionService = encryptionService;
        }

        #endregion

        #region Utilities

        //public IQueryable<Customer> 

        #endregion

        #region Methods

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all customers; </param>
        /// <param name="email">Email; null to load all customers</param>
        /// <param name="username">Username; null to load all customers</param>
        /// <param name="firstName">First name; null to load all customers</param>
        /// <param name="lastName">Last name; null to load all customers</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all customers</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all customers</param>
        /// <param name="company">Company; null to load all customers</param>
        /// <param name="phone">Phone; null to load all customers</param>
        /// <param name="zipPostalCode">Phone; null to load all customers</param>
        /// <param name="ipAddress">IP address; null to load all customers</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        public override async Task<IPagedList<Customer>> GetAllCustomersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int affiliateId = 0, int vendorId = 0, int[] customerRoleIds = null,
            string email = null, string username = null, string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var customers = await _customerRepository.GetAllPagedAsync(query =>
            {
                //query = query.ToList().AsQueryable();
                if (createdFromUtc.HasValue)
                    query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
                if (createdToUtc.HasValue)
                    query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
                if (affiliateId > 0)
                    query = query.Where(c => affiliateId == c.AffiliateId);
                if (vendorId > 0)
                    query = query.Where(c => vendorId == c.VendorId);

                query = query.Where(c => !c.Deleted);

                if (customerRoleIds != null && customerRoleIds.Length > 0)
                {
                    query = query.Join(_customerCustomerRoleMappingRepository.Table, x => x.Id, y => y.CustomerId,
                            (x, y) => new { Customer = x, Mapping = y })
                        .Where(z => customerRoleIds.Contains(z.Mapping.CustomerRoleId))
                        .Select(z => z.Customer)
                        .Distinct();
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    query = query.Where(c => c.Email != null);
                    query = query.Where(c => c.Email.Contains(email));
                }

                if (!string.IsNullOrWhiteSpace(username))
                {
                    query = query.Where(c => c.Username != null);
                    query = query.Where(c => c.Username.Contains(username));
                }

                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == NopCustomerDefaults.FirstNameAttribute &&
                                    z.Attribute.Value.Contains(firstName))
                        .Select(z => z.Customer);
                }

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == NopCustomerDefaults.LastNameAttribute &&
                                    z.Attribute.Value.Contains(lastName))
                        .Select(z => z.Customer);
                }

                //date of birth is stored as a string into database.
                //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
                //so let's search it as a string
                if (dayOfBirth > 0 && monthOfBirth > 0)
                {
                    //both are specified
                    var dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" +
                                         dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                    //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
                    //dateOfBirthStr.Length = 5
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == NopCustomerDefaults.DateOfBirthAttribute &&
                                    z.Attribute.Value.Substring(5, 5) == dateOfBirthStr)
                        .Select(z => z.Customer);
                }
                else if (dayOfBirth > 0)
                {
                    //only day is specified
                    var dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                    //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                    //dateOfBirthStr.Length = 2
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == NopCustomerDefaults.DateOfBirthAttribute &&
                                    z.Attribute.Value.Substring(8, 2) == dateOfBirthStr)
                        .Select(z => z.Customer);
                }
                else if (monthOfBirth > 0)
                {
                    //only month is specified
                    var dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == NopCustomerDefaults.DateOfBirthAttribute &&
                                    z.Attribute.Value.Contains(dateOfBirthStr))
                        .Select(z => z.Customer);
                }

                //search by company
                if (!string.IsNullOrWhiteSpace(company))
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == NopCustomerDefaults.CompanyAttribute &&
                                    z.Attribute.Value.Contains(company))
                        .Select(z => z.Customer);
                }

                //search by phone
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == NopCustomerDefaults.PhoneAttribute &&
                                    z.Attribute.Value.Contains(phone))
                        .Select(z => z.Customer);
                }

                //search by zip
                if (!string.IsNullOrWhiteSpace(zipPostalCode))
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == NopCustomerDefaults.ZipPostalCodeAttribute &&
                                    z.Attribute.Value.Contains(zipPostalCode))
                        .Select(z => z.Customer);
                }

                //search by IpAddress
                if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
                {
                    query = query.Where(w => w.LastIpAddress != null);
                    query = query.Where(w => w.LastIpAddress == ipAddress);
                }

                query = query.OrderByDescending(c => c.CreatedOnUtc);

                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);

            return customers;
        }

        #endregion
    }
}
