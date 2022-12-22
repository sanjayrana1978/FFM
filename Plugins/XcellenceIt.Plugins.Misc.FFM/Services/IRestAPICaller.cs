using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public interface IRestAPICaller
    {
        Task<string> RestAPICallString(string api, string requestHeader, string headerValue);
    }
}
