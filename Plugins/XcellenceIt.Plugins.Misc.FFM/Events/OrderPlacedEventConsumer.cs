using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.FileServices;

namespace XcellenceIt.Plugins.Misc.FFM.Events
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        #region Fields

        private readonly IOrderFileServices _orderFileServices;
        private readonly IFFMServices _ffmService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;


        #endregion

        #region Ctor

        public OrderPlacedEventConsumer(IOrderFileServices orderFileServices,
            IFFMServices ffmService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IRewardPointService rewardPointService)
        {
            _orderFileServices = orderFileServices;
            _ffmService = ffmService;
            _customerService = customerService;
            _localizationService = localizationService;
            _rewardPointService = rewardPointService;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage?.Order != null)
            {
                if (await _ffmService.IsPluginEnable() == false)
                    return;

                string unfiNumber = await _orderFileServices.GetUnfiNumberAsync();

                // Customer order text file
                await _orderFileServices.ManageCustomerOrderTextFile(eventMessage.Order, unfiNumber);
                var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Order.CustomerId);

                if (eventMessage.Order.RedeemedRewardPointsEntryId != null)
                {
                    var rewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, customer.RegisteredInStoreId);
                    if (rewardPointsBalance != 0)
                        await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, -rewardPointsBalance,
                        customer.RegisteredInStoreId, await _localizationService.GetResourceAsync("Plugins.Misc.FFM.OrderPlace.RemoveRewardPoint.After.OrderPlace.Message"));
                }
            }

            await Task.CompletedTask;
        }

        #endregion  
    }
}
