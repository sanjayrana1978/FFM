using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Authentication;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services.OverrideServices
{
    public class OverrideCustomerRegistrationService : CustomerRegistrationService
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IEncryptionService _encryptionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly INotificationService _notificationService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

        #region Ctor

        public OverrideCustomerRegistrationService(CustomerSettings customerSettings,
            IAuthenticationService authenticationService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IEncryptionService encryptionService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INotificationService notificationService,
            IRewardPointService rewardPointService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            RewardPointsSettings rewardPointsSettings) : base(customerSettings,
                authenticationService,
                customerActivityService,
                customerService,
                encryptionService,
                eventPublisher,
                genericAttributeService,
                localizationService,
                multiFactorAuthenticationPluginManager,
                newsLetterSubscriptionService,
                notificationService,
                rewardPointService,
                shoppingCartService,
                storeContext,
                storeService,
                workContext,
                workflowMessageService,
                rewardPointsSettings)
        {
            _customerSettings = customerSettings;
            _authenticationService = authenticationService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _encryptionService = encryptionService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _notificationService = notificationService;
            _rewardPointService = rewardPointService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _storeService = storeService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public override async Task<CustomerRegistrationResult> RegisterCustomerAsync(CustomerRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Customer == null)
                throw new ArgumentException("Can't load current customer");

            var result = new CustomerRegistrationResult();
            if (request.Customer.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }

            if (request.Customer.IsBackgroundTaskAccount())
            {
                result.AddError("Background task account can't be registered");
                return result;
            }

            if (await _customerService.IsRegisteredAsync(request.Customer))
            {
                result.AddError("Current customer is already registered");
                return result;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.EmailIsNotProvided"));
                return result;
            }

            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError(await _localizationService.GetResourceAsync("Common.WrongEmail"));
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.PasswordIsNotProvided"));
                return result;
            }

            if (_customerSettings.UsernamesEnabled && string.IsNullOrEmpty(request.Username))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UsernameIsNotProvided"));
                return result;
            }
            request.Username = _encryptionService.EncryptText(request.Username, "4802407001667285");
            request.Email = _encryptionService.EncryptText(request.Email, "4802407001667285");

            //validate unique user
            if (await _customerService.GetCustomerByEmailAsync(request.Email) != null)
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.EmailAlreadyExists"));
                return result;
            }

            if (_customerSettings.UsernamesEnabled && await _customerService.GetCustomerByUsernameAsync(request.Username) != null)
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UsernameAlreadyExists"));
                return result;
            }

            //at this point request is valid
            request.Customer.Username = request.Username;
            request.Customer.Email = request.Email;

            var customerPassword = new CustomerPassword
            {
                CustomerId = request.Customer.Id,
                PasswordFormat = request.PasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    customerPassword.Password = request.Password;
                    break;
                case PasswordFormat.Encrypted:
                    customerPassword.Password = _encryptionService.EncryptText(request.Password);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = _encryptionService.CreateSaltKey(NopCustomerServicesDefaults.PasswordSaltKeySize);
                    customerPassword.PasswordSalt = saltKey;
                    customerPassword.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _customerSettings.HashedPasswordFormat);
                    break;
            }

            await _customerService.InsertCustomerPasswordAsync(customerPassword);

            request.Customer.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
            if (registeredRole == null)
                throw new NopException("'Registered' role could not be loaded");

            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = request.Customer.Id, CustomerRoleId = registeredRole.Id });

            //remove from 'Guests' role            
            if (await _customerService.IsGuestAsync(request.Customer))
            {
                var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
                await _customerService.RemoveCustomerRoleMappingAsync(request.Customer, guestRole);
            }

            //add reward points for customer registration (if enabled)
            if (_rewardPointsSettings.Enabled && _rewardPointsSettings.PointsForRegistration > 0)
            {
                var endDate = _rewardPointsSettings.RegistrationPointsValidity > 0
                    ? (DateTime?)DateTime.UtcNow.AddDays(_rewardPointsSettings.RegistrationPointsValidity.Value) : null;
                await _rewardPointService.AddRewardPointsHistoryEntryAsync(request.Customer, _rewardPointsSettings.PointsForRegistration,
                    request.StoreId, await _localizationService.GetResourceAsync("RewardPoints.Message.EarnedForRegistration"), endDate: endDate);
            }

            await _customerService.UpdateCustomerAsync(request.Customer);

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newEmail">New email</param>
        /// <param name="requireValidation">Require validation of new email address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task SetEmailAsync(Customer customer, string newEmail, bool requireValidation)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (newEmail == null)
                throw new NopException("Email cannot be null");

            newEmail = newEmail.Trim();
            var oldEmail = customer.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.NewEmailIsNotValid"));

            if (newEmail.Length > 100)
                throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.EmailTooLong"));

            newEmail = _encryptionService.EncryptText(newEmail, "4802407001667285");
            var customer2 = await _customerService.GetCustomerByEmailAsync(newEmail);
            if (customer2 != null && customer.Id != customer2.Id)
                throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.EmailAlreadyExists"));

            if (requireValidation)
            {
                //re-validate email
                customer.EmailToRevalidate = newEmail;
                await _customerService.UpdateCustomerAsync(customer);

                //email re-validation message
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute, Guid.NewGuid().ToString());
                await _workflowMessageService.SendCustomerEmailRevalidationMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);
            }
            else
            {
                customer.Email = newEmail;
                await _customerService.UpdateCustomerAsync(customer);

                if (string.IsNullOrEmpty(oldEmail) || oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
                    return;

                //update newsletter subscription (if required)
                foreach (var store in await _storeService.GetAllStoresAsync())
                {
                    var subscriptionOld = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(oldEmail, store.Id);

                    if (subscriptionOld == null)
                        continue;

                    subscriptionOld.Email = _encryptionService.DecryptText(newEmail, "4802407001667285");
                    await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscriptionOld);
                }
            }
        }

        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newUsername">New Username</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task SetUsernameAsync(Customer customer, string newUsername)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (!_customerSettings.UsernamesEnabled)
                throw new NopException("Usernames are disabled");

            newUsername = newUsername.Trim();

            if (newUsername.Length > NopCustomerServicesDefaults.CustomerUsernameLength)
                throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.UsernameTooLong"));

            newUsername = _encryptionService.EncryptText(newUsername, "4802407001667285");
            var user2 = await _customerService.GetCustomerByUsernameAsync(newUsername);
            if (user2 != null && customer.Id != user2.Id)
                throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.UsernameAlreadyExists"));

            customer.Username = newUsername;
            await _customerService.UpdateCustomerAsync(customer);
        }

        #endregion
    }
}
