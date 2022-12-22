using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models.Extensions;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Factories
{
    public class OverrideRecurringPaymentModelFactory : RecurringPaymentModelFactory
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IWorkContext _workContext;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Ctor

        public OverrideRecurringPaymentModelFactory(IDateTimeHelper dateTimeHelper,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IWorkContext workContext,
            IEncryptionService encryptionService) : base(dateTimeHelper,
                customerService,
                localizationService,
                orderProcessingService,
                orderService,
                paymentService,
                workContext)
        {
            _dateTimeHelper = dateTimeHelper;
            _customerService = customerService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentService = paymentService;
            _workContext = workContext;
            _encryptionService = encryptionService;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Prepare paged recurring payment list model
        /// </summary>
        /// <param name="searchModel">Recurring payment search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payment list model
        /// </returns>
        public override async Task<RecurringPaymentListModel> PrepareRecurringPaymentListModelAsync(RecurringPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get recurringPayments
            var recurringPayments = await _orderService.SearchRecurringPaymentsAsync(showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new RecurringPaymentListModel().PrepareToGridAsync(searchModel, recurringPayments, () =>
            {
                return recurringPayments.SelectAwait(async recurringPayment =>
                {
                    //fill in model values from the entity
                    var recurringPaymentModel = recurringPayment.ToModel<RecurringPaymentModel>();

                    var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId);
                    var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

                    //convert dates to the user time
                    if ((await _orderProcessingService.GetNextPaymentDateAsync(recurringPayment)) is DateTime nextPaymentDate)
                    {
                        recurringPaymentModel.NextPaymentDate = (await _dateTimeHelper
                            .ConvertToUserTimeAsync(nextPaymentDate, DateTimeKind.Utc)).ToString(CultureInfo.InvariantCulture);
                        recurringPaymentModel.CyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(recurringPayment);
                    }

                    recurringPaymentModel.StartDate = (await _dateTimeHelper
                        .ConvertToUserTimeAsync(recurringPayment.StartDateUtc, DateTimeKind.Utc)).ToString(CultureInfo.InvariantCulture);

                    //fill in additional values (not existing in the entity)
                    recurringPaymentModel.CustomerId = customer.Id;
                    recurringPaymentModel.InitialOrderId = order.Id;

                    recurringPaymentModel.CyclePeriodStr = await _localizationService.GetLocalizedEnumAsync(recurringPayment.CyclePeriod);
                    recurringPaymentModel.CustomerEmail = (await _customerService.IsRegisteredAsync(customer))
                        ? _encryptionService.DecryptText(customer.Email, "4802407001667285")
                        : await _localizationService.GetResourceAsync("Admin.Customers.Guest");

                    return recurringPaymentModel;
                });
            });

            return model;
        }


        #endregion
    }
}
