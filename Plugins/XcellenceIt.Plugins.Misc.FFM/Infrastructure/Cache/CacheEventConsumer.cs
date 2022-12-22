namespace XcellenceIt.Plugins.Misc.FFM.Infrastructure.Cache
{
    public class CacheEventConsumer
    {

    }
    //    //languages
    //    IConsumer<EntityInsertedEvent<Customer>>,
    //    IConsumer<EntityUpdatedEvent<Customer>>,
    //    IConsumer<EntityDeletedEvent<Customer>>
    //{
    //    #region Fields

    //    private readonly IStaticCacheManager _staticCacheManager;

    //    #endregion

    //    #region Ctor

    //    public CacheEventConsumer(IStaticCacheManager staticCacheManager)
    //    {
    //        _staticCacheManager = staticCacheManager;
    //    }

    //    #endregion

    //    #region Methods

    //    #region Customers

    //    /// <returns>A task that represents the asynchronous operation</returns>
    //    public async Task HandleEventAsync(EntityInsertedEvent<Customer> eventMessage)
    //    {
    //        await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.DecryptCustomerPrefixCacheKeyById, eventMessage.Entity.CustomerGuid));
    //        await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.EncryptCustomerPrefixCacheKeyById, eventMessage.Entity.CustomerGuid));
    //    }

    //    /// <returns>A task that represents the asynchronous operation</returns>
    //    public async Task HandleEventAsync(EntityUpdatedEvent<Customer> eventMessage)
    //    {
    //        await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.DecryptCustomerPrefixCacheKeyById, eventMessage.Entity.CustomerGuid));
    //        await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.EncryptCustomerPrefixCacheKeyById, eventMessage.Entity.CustomerGuid));
    //    }

    //    /// <returns>A task that represents the asynchronous operation</returns>
    //    public async Task HandleEventAsync(EntityDeletedEvent<Customer> eventMessage)
    //    {
    //        await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.DecryptCustomerPrefixCacheKeyById, eventMessage.Entity.CustomerGuid));
    //        await _staticCacheManager.RemoveByPrefixAsync(string.Format(NopCustomerEncryption.EncryptCustomerPrefixCacheKeyById, eventMessage.Entity.CustomerGuid));
    //    }

    //    #endregion

    //    #endregion

    //}
}
