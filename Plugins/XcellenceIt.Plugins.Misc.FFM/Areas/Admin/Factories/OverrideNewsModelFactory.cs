using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Factories
{
    public class OverrideNewsModelFactory : NewsModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsService _newsService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Ctor

        public OverrideNewsModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IHtmlFormatter htmlFormatter,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INewsService newsService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IEncryptionService encryptionService) : base(catalogSettings,
                baseAdminModelFactory,
                customerService,
                dateTimeHelper,
                htmlFormatter,
                languageService,
                localizationService,
                newsService,
                storeMappingSupportedModelFactory,
                storeService,
                urlRecordService)
        {
            _catalogSettings = catalogSettings;
            _customerService = customerService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _htmlFormatter = htmlFormatter;
            _languageService = languageService;
            _localizationService = localizationService;
            _newsService = newsService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _encryptionService = encryptionService;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Prepare paged news comment list model
        /// </summary>
        /// <param name="searchModel">News comment search model</param>
        /// <param name="newsItemId">News item Id; pass null to prepare comment models for all news items</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news comment list model
        /// </returns>
        public override async Task<NewsCommentListModel> PrepareNewsCommentListModelAsync(NewsCommentSearchModel searchModel, int? newsItemId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter comments
            var createdOnFromValue = searchModel.CreatedOnFrom == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var createdOnToValue = searchModel.CreatedOnTo == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;

            //get comments
            var comments = (await _newsService.GetAllCommentsAsync(newsItemId: newsItemId,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdOnToValue,
                commentText: searchModel.SearchText)).ToPagedList(searchModel);

            //prepare store names (to avoid loading for each comment)
            var storeNames = (await _storeService.GetAllStoresAsync())
                .ToDictionary(store => store.Id, store => store.Name);

            //prepare list model
            var model = await new NewsCommentListModel().PrepareToGridAsync(searchModel, comments, () =>
            {
                return comments.SelectAwait(async newsComment =>
                {
                    //fill in model values from the entity
                    var commentModel = newsComment.ToModel<NewsCommentModel>();

                    //convert dates to the user time
                    commentModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsComment.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    commentModel.NewsItemTitle = (await _newsService.GetNewsByIdAsync(newsComment.NewsItemId))?.Title;

                    if ((await _customerService.GetCustomerByIdAsync(newsComment.CustomerId)) is Customer customer)
                    {
                        commentModel.CustomerInfo = (await _customerService.IsRegisteredAsync(customer))
                            ? _encryptionService.DecryptText(customer.Email, "4802407001667285")
                            : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                    }

                    commentModel.CommentText = _htmlFormatter.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                    commentModel.StoreName = storeNames.ContainsKey(newsComment.StoreId) ? storeNames[newsComment.StoreId] : "Deleted";

                    return commentModel;
                });
            });

            return model;
        }


        #endregion
    }
}
