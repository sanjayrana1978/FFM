using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace XcellenceIt.Plugins.Misc.FFM.Models.SpecificationAttributePicture
{
    public record SpecificationAttributePictureModel : BaseNopEntityModel
    {
        public int SpecificationAttributeId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.FFM.Admin.FFM.Fields.Picture")]
        [UIHint("Picture")]
        public int PictureId { get; set; }
    }
}
