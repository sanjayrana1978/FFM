using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.ScheduleTasks;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Models;
using XcellenceIt.Plugins.Misc.FFM.Services;

namespace XcellenceIt.Plugins.Misc.FFM.ScheduleTask
{
    public class UpdateProductsFromApiTask : IScheduleTask
    {
        #region Fields

        private readonly IFFMServices _fFMServices;
        private readonly IRestAPICaller _restAPICaller;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public UpdateProductsFromApiTask(IFFMServices fFMServices,
            IRestAPICaller restAPICaller,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _fFMServices = fFMServices;
            _restAPICaller = restAPICaller;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            if (!await _fFMServices.IsPluginEnable())
                return;

            var ffmSettings = await _fFMServices.GetAllConfiguration();

            var apiParams = new ApiParamsModel()
            {
                Api = ffmSettings.Api,
                PrimaryKey = ffmSettings.PrimaryKey,
                SecondaryKey = ffmSettings.SecondaryKey,
                RequestHeader = ffmSettings.RequestHeaderName,
            };
            var api = string.Empty;
            if (ffmSettings.Api.Contains('?'))
            {
                api = ffmSettings.Api.Split("?")[0];
                api = api + "?page=" + 1 + "&limit=" + ffmSettings.PageSize;
            }
            else
            {
                api = ffmSettings.Api + "?page=" + 1 + "&limit=" + ffmSettings.PageSize;
            }

            var data = await _restAPICaller.RestAPICallString(api, apiParams.RequestHeader, headerValue: apiParams.PrimaryKey);
            await _fFMServices.UpdateProductsFromApi(JsonConvert.DeserializeObject<ApiResponseProductModel>(data), apiParams);
        }

        #endregion
    }
}



