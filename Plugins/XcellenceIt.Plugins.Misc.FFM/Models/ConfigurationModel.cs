using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace XcellenceIt.Plugins.Misc.FFM.Models
{
    /// <summary>
    /// Represents an Configuration Model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Enable")]
        public bool Enable { get; set; }

        public bool Enable_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.Api")]
        public string Api { get; set; }
        public bool Api_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.RequestHeaderName")]
        public string RequestHeaderName { get; set; }
        public bool RequestHeaderName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.PrimaryKey")]
        public string PrimaryKey { get; set; }
        public bool PrimaryKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.SecondaryKey")]
        public string SecondaryKey { get; set; }
        public bool SecondaryKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.PageSize")]
        public int PageSize { get; set; }
        public bool PageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.UNFINumber")]
        public int UNFINumber { get; set; }
        public bool UNFINumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.FtpHost")]
        public string FtpHost { get; set; }
        public bool FtpHost_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.FtpUserName")]
        public string FtpUserName { get; set; }
        public bool FtpUserName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.FtpPassword")]
        public string FtpPassword { get; set; }
        public bool FtpPassword_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Configuration.RedirectToCategory")]
        public int RedirectToCategoryId { get; set; }
        public bool RedirectToCategoryId_OverrideForStore { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

    }
}
