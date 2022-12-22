using Nop.Services.ScheduleTasks;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.Companies;

namespace XcellenceIt.Plugins.Misc.FFM.ScheduleTask
{
    public class AddRemoveRewardPointTask : IScheduleTask
    {
        #region Fields
        private readonly IFFMServices _fFMServices;
        private readonly CompanyService _companyService;
        #endregion

        #region Ctor
        public AddRemoveRewardPointTask(IFFMServices fFMServices,
            CompanyService companyService)
        {
            _fFMServices = fFMServices;
            _companyService = companyService;
        }
        #endregion

        #region Method
        public async Task ExecuteAsync()
        {
            if (!await _fFMServices.IsPluginEnable())
                return;

            await _companyService.RewardPointAddAsync();
        }
        #endregion
    }
}
