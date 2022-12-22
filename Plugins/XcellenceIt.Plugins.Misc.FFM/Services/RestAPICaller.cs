using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services
{
    public class RestAPICaller : IRestAPICaller
    {
        public async Task<string> RestAPICallString(string api = null, string requestHeader = null, string headerValue = null)
        {
            if (string.IsNullOrEmpty(api))
                return null;

            #pragma warning disable SYSLIB0014 // Type or member is obsolete
            var insertCustomerWebRequest = (HttpWebRequest)WebRequest.Create(api);
            #pragma warning restore SYSLIB0014 // Type or member is obsolete

            if (requestHeader != null && headerValue != null)
                insertCustomerWebRequest.Headers.Add(requestHeader, headerValue);

            var content = string.Empty;
            using (var response = (HttpWebResponse)insertCustomerWebRequest.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }
            return content;
        }
    }
}
