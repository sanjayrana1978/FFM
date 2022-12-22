using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Events;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Events
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        //Address
        IConsumer<EntityInsertedEvent<Address>>,
        IConsumer<EntityUpdatedEvent<Address>>,
        //customer
        IConsumer<EntityInsertedEvent<Customer>>,
        IConsumer<EntityUpdatedEvent<Customer>>
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public EventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region Address

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Address> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.CustomAddressNumberPrefix, eventMessage.Entity.Id));
            await _staticCacheManager.RemoveByPrefixAsync(NopCustomerEncryption.EncryptCustomerPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopCustomerEncryption.DecryptCustomerPrefixCacheKey);
        }

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Address> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.CustomAddressNumberPrefix, eventMessage.Entity.Id));
            await _staticCacheManager.RemoveByPrefixAsync(NopCustomerEncryption.EncryptCustomerPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopCustomerEncryption.DecryptCustomerPrefixCacheKey);
        }

        #endregion

        #region Customer

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Customer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.CustomCustomerNumberPrefix, eventMessage.Entity.Id));
            await _staticCacheManager.RemoveByPrefixAsync(NopCustomerEncryption.EncryptCustomerPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopCustomerEncryption.DecryptCustomerPrefixCacheKey);
        }

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Customer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.CustomCustomerNumberPrefix, eventMessage.Entity.Id));
            await _staticCacheManager.RemoveByPrefixAsync(NopCustomerEncryption.EncryptCustomerPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopCustomerEncryption.DecryptCustomerPrefixCacheKey);
        }

        #endregion

        #endregion
    }
}
