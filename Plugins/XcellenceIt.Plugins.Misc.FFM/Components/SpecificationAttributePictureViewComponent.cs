using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Factories;
using XcellenceIt.Plugins.Misc.FFM.Models.SpecificationAttributePicture;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.SpecificationAttributepictures;

namespace XcellenceIt.Plugins.Misc.FFM.Components
{
    [ViewComponent(Name = DefaultFFMStrings.SpecificationAttributePicture)]
    public class SpecificationAttributePictureViewComponent : NopViewComponent
    {
        private readonly ISpecificationAttributePictureService _specificationAttributePictureService;
        private readonly ISpecificAttributePictureFactory _specificAttributePictureFactory;
        private readonly IFFMServices _fFMServices;

        public SpecificationAttributePictureViewComponent(ISpecificationAttributePictureService specificationAttributePictureService,
            ISpecificAttributePictureFactory specificAttributePictureFactory,
            IFFMServices fFMServices)
        {
            _specificationAttributePictureService = specificationAttributePictureService;
            _specificAttributePictureFactory = specificAttributePictureFactory;
            _fFMServices = fFMServices;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!await _fFMServices.IsPluginEnable())
                return Content("");

            var modeldata = (SpecificationAttributeModel)additionalData;
            var specificationAttributePicture = await _specificationAttributePictureService.GetPicturesBySpecificationAttributeId(modeldata.Id);
            var model = _specificAttributePictureFactory.PreparePictureModelsAsync(new SpecificationAttributePictureModel(), specificationAttributePicture, modeldata.Id);

            return View(model);
        }

    }
}
