using Nop.Core.Configuration;

namespace XcellenceIt.Plugins.Misc.FFM
{
    public class FFMSettings : ISettings
    {
        public bool Enable { get; set; }
        public string Api { get; set; }
        public string RequestHeaderName { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public int PageSize { get; set; }
        public int UNFINumber { get; set; }
        public string FtpHost { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public int RedirectToCategoryId { get; set; }
    }
}
