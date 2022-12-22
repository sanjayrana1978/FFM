using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Logging;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public class CustomEntityRepository<TEntity> : EntityRepository<TEntity> where TEntity : BaseEntity
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly INopDataProvider _dataProvider;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public CustomEntityRepository(IEventPublisher eventPublisher,
            INopDataProvider dataProvider,
            IStaticCacheManager staticCacheManager)
        : base(eventPublisher,
            dataProvider,
            staticCacheManager)
        {
            _eventPublisher = eventPublisher;
            _dataProvider = dataProvider;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Utilities

        protected bool IsBase64String(string base64, bool isEncrypting = false)
        {
            Task.Run(async () => await EngineContext.Current.Resolve<ILogger>().InsertLogAsync(LogLevel.Information, base64));
            if (string.IsNullOrEmpty(base64))
                return false;
            base64 = base64.Trim();
            if (isEncrypting && int.TryParse(base64, out _))
            {
                return false;
            }
            if (base64.Length % 4 == 0)
            {
                var baseStringChecked = false;
                if (!baseStringChecked && !Regex.IsMatch(base64, @"^(?=(.*\d){1,}).{12,}$", RegexOptions.Singleline))
                {
                    if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*\d){0,})(?=(.*[a-z]){3,})(?=(.*[A-Z]){3,})(?=(.*[+/=]?)).{12,}$", RegexOptions.Singleline))
                        baseStringChecked = true;
                }
                if (!baseStringChecked && !Regex.IsMatch(base64, @"^(?=(.*\d){2,}).{12,}$", RegexOptions.Singleline))
                {
                    if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*\d){1,})(?=(.*[a-z]){3,})(?=(.*[A-Z]){3,})(?=(.*[+/=]){1,}).{12,}$", RegexOptions.Singleline))
                        baseStringChecked = true;
                }
                if (!baseStringChecked && !Regex.IsMatch(base64, @"^(?=(.*[+/=]){1,})$", RegexOptions.Singleline))
                {
                    if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*\d){2,})(?=(.*[a-z]){2,})(?=(.*[A-Z]){3,})(?=(.*[+/=]?)).{12,}$", RegexOptions.Singleline))
                        baseStringChecked = true;
                }
                if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*[+/=]){1,})$", RegexOptions.Singleline))
                {
                    if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*\d){2,})(?=(.*[a-z]){3,})(?=(.*[A-Z]){3,})(?=(.*[+/=]){1,}).{12,}$", RegexOptions.Singleline))
                        baseStringChecked = true;
                }
                return baseStringChecked;
            }
            return false;
        }

        /// <summary>
        /// Encrypt string
        /// </summary>
        /// <param name="plainText">plainText</param>
        /// <returns>string</returns>
        public virtual string EncryptString(string plainText)
        {
            return EngineContext.Current.Resolve<IEncryptionService>().EncryptText(plainText, "4802407001667285");
        }

        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns>string</returns>
        public virtual string DecryptString(string cipherText)
        {
            try
            {
                string decryptText = EngineContext.Current.Resolve<IEncryptionService>().DecryptText(cipherText, "4802407001667285");
                return string.IsNullOrEmpty(decryptText) ? null : decryptText;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Encrypted customer
        /// </summary>
        /// <param name="customer">customer</param>
        /// <returns>Encrypted customer</returns>
        public virtual Customer EncryptedCustomer(Customer customer)
        {
            customer.Username = EncryptString(customer.Username) ?? customer.Username;
            customer.Email = EncryptString(customer.Email) ?? customer.Email;
            //customer.EmailToRevalidate = EncryptString(customer.EmailToRevalidate) ?? customer.EmailToRevalidate;
            //customer.AdminComment = EncryptString(customer.AdminComment) ?? customer.AdminComment;
            //customer.SystemName = EncryptString(customer.SystemName) ?? customer.SystemName;
            //customer.LastIpAddress = EncryptString(customer.LastIpAddress) ?? customer.LastIpAddress;
            return customer;
        }

        /// <summary>
        /// Decrypted customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Decrypted customer</returns>
        public virtual Customer DecryptedCustomer(Customer customer)
        {
            customer.Username = DecryptString(customer.Username) ?? customer.Username;
            customer.Email = DecryptString(customer.Email) ?? customer.Email;
            //customer.EmailToRevalidate = DecryptString(customer.EmailToRevalidate) ?? customer.EmailToRevalidate;
            //customer.AdminComment = DecryptString(customer.AdminComment) ?? customer.AdminComment;
            //customer.SystemName = DecryptString(customer.SystemName) ?? customer.SystemName;
            //customer.LastIpAddress = DecryptString(customer.LastIpAddress) ?? customer.LastIpAddress;
            return customer;
        }

        /// <summary>
        /// Encrypted address
        /// </summary>
        /// <param name="address">address</param>
        /// <returns>Encrypted address</returns>  
        public virtual Address EncryptedAddress(Address address)
        {
            address.FirstName = EncryptString(address.FirstName) ?? address.FirstName;
            address.LastName = EncryptString(address.LastName) ?? address.LastName;
            address.Email = EncryptString(address.Email) ?? address.Email;
            address.Company = EncryptString(address.Company) ?? address.Company;
            address.County = EncryptString(address.County) ?? address.County;
            address.City = EncryptString(address.City) ?? address.City;
            address.Address1 = EncryptString(address.Address1) ?? address.Address1;
            address.Address2 = EncryptString(address.Address2) ?? address.Address2;
            address.ZipPostalCode = EncryptString(address.ZipPostalCode) ?? address.ZipPostalCode;
            address.PhoneNumber = EncryptString(address.PhoneNumber) ?? address.PhoneNumber;
            address.FaxNumber = EncryptString(address.FaxNumber) ?? address.FaxNumber;
            address.CustomAttributes = EncryptString(address.CustomAttributes) ?? address.CustomAttributes;
            return address;
        }

        /// <summary>
        /// Decrypted address
        /// </summary>
        /// <param name="address">address</param>
        /// <returns>Decrypted address</returns>
        public virtual Address DecryptedAddress(Address address)
        {
            address.FirstName = DecryptString(address.FirstName) ?? address.FirstName;
            address.LastName = DecryptString(address.LastName) ?? address.LastName;
            address.Email = DecryptString(address.Email) ?? address.Email;
            address.Company = DecryptString(address.Company) ?? address.Company;
            address.County = DecryptString(address.County) ?? address.County;
            address.City = DecryptString(address.City) ?? address.City;
            address.Address1 = DecryptString(address.Address1) ?? address.Address1;
            address.Address2 = DecryptString(address.Address2) ?? address.Address2;
            address.ZipPostalCode = DecryptString(address.ZipPostalCode) ?? address.ZipPostalCode;
            address.PhoneNumber = DecryptString(address.PhoneNumber) ?? address.PhoneNumber;
            address.FaxNumber = DecryptString(address.FaxNumber) ?? address.FaxNumber;
            address.CustomAttributes = DecryptString(address.CustomAttributes) ?? address.CustomAttributes;
            return address;
        }

        /// <summary>
        /// Manage cryptography entity insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>entity</returns>
        public async Task<TEntity> ManageCryptographyEntityInsert(TEntity entity)
        {
            if (entity is Customer customer && customer.Email != null)
            {
                var encryptEntity = entity;
                EncryptedCustomer(encryptEntity as Customer);
                await _dataProvider.InsertEntityAsync(encryptEntity);
                return entity;
            }

            if (entity is Address)
            {
                var encryptEntity = entity;
                EncryptedAddress(encryptEntity as Address);
                await _dataProvider.InsertEntityAsync(encryptEntity);
                return entity;
            }

            await _dataProvider.InsertEntityAsync(entity);
            return entity;
        }

        /// <summary>
        /// Manage cryptography entity update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>entity</returns>
        public async Task<TEntity> ManageCryptographyEntityUpdate(TEntity entity)
        {
            if (entity is Customer)
            {
                var encryptEntity = entity;
                EncryptedCustomer(encryptEntity as Customer);
                await _dataProvider.UpdateEntityAsync(encryptEntity);
                Customer customer = entity as Customer;
                return customer.Email == null && IsBase64String(customer.Email) ? entity : encryptEntity;
            }

            if (entity is Address)
            {
                var encryptEntity = entity;
                EncryptedAddress(encryptEntity as Address);
                await _dataProvider.UpdateEntityAsync(encryptEntity);
                return entity;
            }

            await _dataProvider.UpdateEntityAsync(entity);
            return entity;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Insert the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <param name="publishEvent">Whether to publish event notification</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InsertAsync(TEntity entity, bool publishEvent = true)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity = await ManageCryptographyEntityInsert(entity);

            //event notification
            if (publishEvent)
                await _eventPublisher.EntityInsertedAsync(entity);
        }

        /// <summary>
        /// Update the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <param name="publishEvent">Whether to publish event notification</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UpdateAsync(TEntity entity, bool publishEvent = true)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity = await ManageCryptographyEntityUpdate(entity);

            //event notification
            if (publishEvent)
                await _eventPublisher.EntityUpdatedAsync(entity);
        }

        /// <summary>
        /// Get paged list of all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of entity entries
        /// </returns>
        public override async Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool includeDeleted = true)
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            var entity = _dataProvider.GetTable<TEntity>();

            if (entity.ToList() is List<Customer>)
            {
                //var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCustomerEncryption.AllCustomersDecryptKey);
                //query = _staticCacheManager.Get(cacheKey, () =>
                //{
                var customers = entity.ToList() as List<Customer>;
                var customerList = new List<Customer>();

                foreach (var customer in customers)
                {
                    var customerCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCustomerEncryption.CustomCustomerCacheKey, customer.Id);
                    var getCustomer = _staticCacheManager.Get(customerCacheKey, () =>
                    {
                        var cust = new Customer()
                        {
                            Id = customer.Id,
                            CustomerGuid = customer.CustomerGuid,
                            Username = DecryptString(customer.Username) ?? customer.Username,
                            Email = DecryptString(customer.Email) ?? customer.Email,
                            EmailToRevalidate = DecryptString(customer.EmailToRevalidate) ?? customer.EmailToRevalidate,
                            AdminComment = DecryptString(customer.AdminComment) ?? customer.AdminComment,
                            IsTaxExempt = customer.IsTaxExempt,
                            AffiliateId = customer.AffiliateId,
                            VendorId = customer.VendorId,
                            HasShoppingCartItems = customer.HasShoppingCartItems,
                            RequireReLogin = customer.RequireReLogin,
                            FailedLoginAttempts = customer.FailedLoginAttempts,
                            CannotLoginUntilDateUtc = customer.CannotLoginUntilDateUtc,
                            Active = customer.Active,
                            Deleted = customer.Deleted,
                            IsSystemAccount = customer.IsSystemAccount,
                            SystemName = DecryptString(customer.SystemName) ?? customer.SystemName,
                            LastIpAddress = DecryptString(customer.LastIpAddress) ?? customer.LastIpAddress,
                            CreatedOnUtc = customer.CreatedOnUtc,
                            LastLoginDateUtc = customer.LastLoginDateUtc,
                            LastActivityDateUtc = customer.LastActivityDateUtc,
                            RegisteredInStoreId = customer.RegisteredInStoreId,
                            BillingAddressId = customer.BillingAddressId,
                            ShippingAddressId = customer.ShippingAddressId
                        };
                        return cust;
                    });
                    customerList.Add(getCustomer);
                }
                query = (IQueryable<TEntity>)customerList.AsQueryable();
                //});
            }

            //if (entity.ToList() is List<Address> addresses)
            //{
            //    var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CustomAddressesCacheKey, entity);
            //    query = _staticCacheManager.Get(cacheKey, () =>
            //    {
            //        var addresses = entity.ToList() as List<Address>;
            //        var addressList = new List<Address>();

            //        foreach (var address in addresses)
            //        {
            //            var addressCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CustomAddressCacheKey, address.Id);
            //            var getAddress = _staticCacheManager.Get(addressCacheKey, () =>
            //            {
            //                var cust = new Address()
            //                {
            //                    Id = address.Id,
            //                    FirstName = DecryptString(address.FirstName) ?? address.FirstName,
            //                    LastName = DecryptString(address.LastName) ?? address.LastName,
            //                    Email = DecryptString(address.Email) ?? address.Email,
            //                    Company = DecryptString(address.Company) ?? address.Company,
            //                    CountryId = address.CountryId,
            //                    StateProvinceId = address.StateProvinceId,
            //                    County = DecryptString(address.County) ?? address.County,
            //                    City = DecryptString(address.City) ?? address.City,
            //                    Address1 = DecryptString(address.Address1) ?? address.Address1,
            //                    Address2 = DecryptString(address.Address2) ?? address.Address2,
            //                    ZipPostalCode = DecryptString(address.ZipPostalCode) ?? address.ZipPostalCode,
            //                    PhoneNumber = DecryptString(address.PhoneNumber) ?? address.PhoneNumber,
            //                    FaxNumber = DecryptString(address.FaxNumber) ?? address.FaxNumber,
            //                    CustomAttributes = DecryptString(address.CustomAttributes) ?? address.CustomAttributes,
            //                    CreatedOnUtc = address.CreatedOnUtc
            //                };
            //                return cust;
            //            });
            //            addressList.Add(getAddress);
            //        }
            //        return (IQueryable<TEntity>)addressList.AsQueryable();
            //    });
            //}

            query = func != null ? func(query) : query;

            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Get the entity entry
        /// </summary>
        /// <param name="id">Entity entry identifier</param>
        /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
        /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="ISoftDeletedEntity"/> entities)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        public override async Task<TEntity> GetByIdAsync(int? id, Func<IStaticCacheManager, CacheKey> getCacheKey = null, bool includeDeleted = true)
        {
            if (!id.HasValue || id == 0)
                return null;

            async Task<TEntity> getEntityAsync()
            {
                var entity = await AddDeletedFilter(Table, includeDeleted).FirstOrDefaultAsync(entity => entity.Id == Convert.ToInt32(id));
                if (entity is Address address)
                {
                    DecryptedAddress(entity as Address);
                }
                if (entity is Customer customer)
                {
                    DecryptedCustomer(entity as Customer);
                }
                return entity;

            }

            if (getCacheKey == null)
                return await getEntityAsync();

            //caching
            var cacheKey = getCacheKey(_staticCacheManager)
                ?? _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.ByIdCacheKey, id);

            return await _staticCacheManager.GetAsync(cacheKey, getEntityAsync);
        }

        #endregion

    }
}