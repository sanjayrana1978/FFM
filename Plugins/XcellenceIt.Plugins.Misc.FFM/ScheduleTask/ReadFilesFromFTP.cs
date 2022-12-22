using Nop.Services.ScheduleTasks;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.FileServices;

namespace XcellenceIt.Plugins.Misc.FFM.ScheduleTask
{
    public class ReadFilesFromFTP : IScheduleTask
    {
        private readonly IFFMServices _fFMServices;
        protected readonly IOrderFileServices _orderFileServices;

        public ReadFilesFromFTP(IFFMServices fFMServices,
            IOrderFileServices orderFileServices)
        {
            _fFMServices = fFMServices;
            _orderFileServices = orderFileServices;
        }

        public async Task ExecuteAsync()
        {
            if (!await _fFMServices.IsPluginEnable())
                return;

            ///scheduler task
            await _orderFileServices.ManageFileReadSchedulerTask();
        }
    }
}
